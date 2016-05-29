using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Maw.Data.EntityFramework.Photos;
using Maw.Domain.Photos;


namespace Maw.Data
{
	public class PhotoRepository
        : IPhotoRepository
	{
        const int MAX_RESULTS = 600;


		readonly PhotoContext _ctx;
		readonly ILogger _log;


        public PhotoRepository(ILoggerFactory loggerFactory, PhotoContext context)
        {
			if(loggerFactory == null)
			{
				throw new ArgumentNullException(nameof(loggerFactory));
			}
			if(context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

            _ctx = context;
            _log = loggerFactory.CreateLogger<PhotoRepository>();
        }


        public async Task<PhotoAndCategory> GetRandomPhotoAsync(bool allowPrivate)
        {
            var rand = new Random();
			var count = await _ctx.photo.CountAsync();
			var skip = (int)(rand.NextDouble() * count);

            // originally tried to incorporate all data in this sql call, but that took almost 10s to complete, by
            // separating the category pull, we are less than 15ms in sql!
			var photo = await _ctx.photo.OrderBy(x => x.id)
                .Skip(skip)
                .Take(1)
				.SingleAsync();

            return new PhotoAndCategory {
                Photo = BuildPhoto(photo),
                Category = await GetCategoryAsync(photo.category_id, allowPrivate)
			};
        }


		public async Task<List<short>> GetYearsAsync()
        {
			return await _ctx.category
                .Select(x => x.year)
                .Distinct()
                .OrderByDescending(x => x)
				.ToListAsync();
        }


        public Task<List<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate)
		{
            return _ctx.category
                .Where(x => x.year == year && (allowPrivate || !x.is_private))
                .OrderBy(x => x.id)
                .Select(x => new Category {
                    Id = x.id,
                    Name = x.name,
                    Year = x.year,
					HasGpsData = x.photo.Any(i => i.gps_latitude != null),
                    TeaserPhotoInfo = new PhotoInfo { 
                        Path = x.teaser_photo_path, 
						Width = (short)x.teaser_photo_width,
						Height = (short)x.teaser_photo_height
                    }
                })
				.ToListAsync();
		}


        public async Task<short> GetCategoryCountAsync(bool allowPrivate)
        {
            var count = await _ctx.category
                .Where(x => allowPrivate || !x.is_private)
                .CountAsync();

			return Convert.ToInt16(count);
        }


        public Task<List<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate)
        {
            return _ctx.category
                .Where(x => x.id > sinceId && (allowPrivate || !x.is_private))
                .OrderBy(x => x.id)
                .Select(x => new Category {
                    Id = x.id,
                    Name = x.name,
                    Year = x.year,
					HasGpsData = x.photo.Any(i => i.gps_latitude != null),
                    TeaserPhotoInfo = new PhotoInfo { 
                        Path = x.teaser_photo_path, 
						Width = (short)x.teaser_photo_width,
						Height = (short)x.teaser_photo_height }
                })
                .ToListAsync();
        }


		public async Task<List<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate)
		{
			var photos = await _ctx.photo
                .Where(x => x.category_id == categoryId && (allowPrivate || !x.is_private))
                .OrderBy(x => x.lg_path)
				.ToListAsync();

			return photos
				.Select(x => BuildPhoto(x))
				.ToList();
		}


		public async Task<List<Photo>> GetPhotosByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
			var query = _ctx.comment
				.Where(x => allowPrivate || !x.photo.is_private)
				.GroupBy(x => x.photo)
				.Select(x => new {
					Photo = x.Key, 
					FirstPostDate = x.Min(d => d.entry_date),
					LastPostDate = x.Max(d => d.entry_date)
				});

			if(newestFirst)
			{
				query.OrderByDescending(x => x.LastPostDate);
			}
			else
			{
				query.OrderByDescending(x => x.FirstPostDate);
			}

			var photos = await query.ToListAsync();

			return photos
				.Select(x => BuildPhoto(x.Photo))
				.Take(MAX_RESULTS)
				.ToList();
        }


		public async Task<List<Photo>> GetPhotosByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate)
        {
			var query = _ctx.comment
				.Include(c => c.user)
				.Where(x => x.user.username == username)
				.GroupBy(x => x.photo)
				.Select(g => new {
					Photo = g.Key, 
					FirstPostDate = g.Min(x => x.entry_date), 
					LastPostDate = g.Max(x => x.entry_date)
				})
				.Where(x => allowPrivate || !x.Photo.is_private);

            if(greatestFirst)
            {
				query.OrderByDescending(x => x.LastPostDate);
            }
            else
            {
				query.OrderBy(x => x.FirstPostDate);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => BuildPhoto(x.Photo))
				.Take(MAX_RESULTS)
				.ToList();
        }


		public async Task<List<Photo>> GetPhotosByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
			var query = _ctx.comment
				.GroupBy(x => x.photo)
				.Select(g => new {
					Photo = g.Key, 
					CommentCount = g.Count()
				})
				.Where(x => allowPrivate || !x.Photo.is_private);

            if(greatestFirst)
            {
				query.OrderByDescending(x => x.CommentCount);
            }
            else
            {
				query.OrderBy(x => x.CommentCount);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => BuildPhoto(x.Photo))
				.Take(MAX_RESULTS)
				.ToList();
        }
		
		
		public async Task<List<Photo>> GetPhotosByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
		{
            var query = _ctx.rating
                .GroupBy(x => x.photo)
                .Select(g => new {
                    Photo = g.Key, 
                    AverageRating = g.Average(x => x.score)
                })
                .Where(x => allowPrivate || !x.Photo.is_private);

            if(highestFirst) 
            {
                query.OrderByDescending(x => x.AverageRating);
            }
            else
            {
                query.OrderBy(x => x.AverageRating);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => BuildPhoto(x.Photo))
                .Take(MAX_RESULTS)
                .ToList();
        }
		
		
		public async Task<List<Photo>> GetPhotosByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
		{
			var query = _ctx.rating
				.Where(x => x.user.username == username && (allowPrivate || !x.photo.is_private));

            if(highestFirst)
            {
				query.OrderByDescending(x => x.score);
            }
            else
            {
				query.OrderBy(x => x.score);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => BuildPhoto(x.photo))
				.Take(MAX_RESULTS)
				.ToList();
		}


		public async Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            var cat = await _ctx.category
				.Where(x => x.id == categoryId && (allowPrivate || !x.is_private))
				.Select(x => new { Category = x, HasGps = x.photo.Any(i => i.gps_latitude != null) })
				.SingleAsync();

			return BuildPhotoCategory(cat.Category, cat.HasGps);
        }


		public async Task<Photo> GetPhotoAsync(int photoId, bool allowPrivate)
		{
			var photo = await _ctx.photo
				.SingleAsync(x => x.id == photoId && (allowPrivate || !x.is_private));

			return BuildPhoto(photo);
		}


		public Task<Detail> GetDetailForPhotoAsync(int photoId, bool allowPrivate)
		{
            return _ctx.photo
                .Include(x => x.active_d_lighting)
                .Include(x => x.af_area_mode)
                .Include(x => x.af_point)
                .Include(x => x.auto_focus)
                .Include(x => x.colorspace)
                .Include(x => x.compression)
                .Include(x => x.contrast)
                .Include(x => x.exposure_mode)
                .Include(x => x.exposure_program)
                .Include(x => x.flash)
                .Include(x => x.flash_color_filter)
                .Include(x => x.flash_mode)
                .Include(x => x.flash_setting)
                .Include(x => x.flash_type)
                .Include(x => x.focus_mode)
                .Include(x => x.gain_control)
                .Include(x => x.gps_altitude_ref)
                .Include(x => x.gps_direction_ref)
                .Include(x => x.gps_latitude_ref)
                .Include(x => x.gps_longitude_ref)
                .Include(x => x.gps_measure_mode)
                .Include(x => x.gps_status)
                .Include(x => x.high_iso_noise_reduction)
                .Include(x => x.hue_adjustment)
                .Include(x => x.lens)
                .Include(x => x.light_source)
                .Include(x => x.make)
                .Include(x => x.metering_mode)
                .Include(x => x.model)
                .Include(x => x.noise_reduction)
                .Include(x => x.orientation)
                .Include(x => x.picture_control_name)
                .Include(x => x.saturation)
                .Include(x => x.scene_capture_type)
                .Include(x => x.scene_type)
                .Include(x => x.sensing_method)
                .Include(x => x.sharpness)
                .Include(x => x.vibration_reduction)
                .Include(x => x.vignette_control)
                .Include(x => x.vr_mode)
                .Include(x => x.white_balance)
                .Where(x => x.id == photoId && (allowPrivate || !x.is_private))
                .Select(x => new Detail {
                    // exif
                    BitsPerSample = (ushort?)x.bits_per_sample,
                    Compression = x.compression.name, //?
                    Contrast = x.contrast.name, //?
                    CreateDate = x.create_date,
                    DigitalZoomRatio = x.digital_zoom_ratio,
                    ExposureCompensation = x.exposure_compensation,
                    ExposureMode = x.exposure_mode.name, //?
                    ExposureProgram = x.exposure_program.name, //?
                    ExposureTime = x.exposure_time,
                    FNumber = (double?)x.f_number,
                    Flash = x.flash.name, //?
                    FocalLength = x.focal_length,
                    FocalLengthIn35mmFormat = x.focal_length_in_35_mm_format,
                    GainControl = x.gain_control.name, //?
                    GpsAltitude = x.gps_altitude,
                    GpsAltitudeRef = x.gps_altitude_ref.name, //?
                    GpsDateStamp = x.gps_date_time_stamp,
                    GpsDirection = x.gps_direction,
                    GpsDirectionRef = x.gps_direction_ref.name, //?
                    GpsLatitude = x.gps_latitude,
                    GpsLatitudeRef = x.gps_latitude_ref.name, //?
                    GpsLongitude = x.gps_longitude,
                    GpsLongitudeRef = x.gps_longitude_ref.name, //?
                    GpsMeasureMode = x.gps_measure_mode.name, //?
                    GpsSatellites = x.gps_satellites,
                    GpsStatus = x.gps_status.name, //?
                    GpsVersionId = x.gps_version_id,
                    Iso = x.iso,
                    LightSource = x.light_source.name, //?
                    Make = x.make.name, //?
                    MeteringMode = x.metering_mode.name, //?
                    Model = x.model.name, //?
                    Orientation = x.orientation.name, //?
                    Saturation = x.saturation.name, //?
                    SceneCaptureType = x.scene_capture_type.name, //?
                    SceneType = x.scene_type.name, //?
                    SensingMethod = x.sensing_method.name, //?
                    Sharpness = x.sharpness.name, //?
                    // nikon
                    AutoFocusAreaMode = x.af_area_mode.name, //?
                    AutoFocusPoint = x.af_point.name, //?
                    ActiveDLighting = x.active_d_lighting.name, //?
                    Colorspace = x.colorspace.name, //?
                    ExposureDifference = x.exposure_difference,
                    FlashColorFilter = x.flash_color_filter.name, //?
                    FlashCompensation = x.flash_compensation,
                    FlashControlMode = x.flash_control_mode,
                    FlashExposureCompensation = x.flash_exposure_compensation,
                    FlashFocalLength = x.flash_focal_length,
                    FlashMode = x.flash_mode.name, //?
                    FlashSetting = x.flash_setting.name, //?
                    FlashType = x.flash_type.name, //?
                    FocusDistance = x.focus_distance,
                    FocusMode = x.focus_mode.name, //?
                    FocusPosition = x.focus_position,
                    HighIsoNoiseReduction = x.high_iso_noise_reduction.name, //?
                    HueAdjustment = x.hue_adjustment.name, //?
                    NoiseReduction = x.noise_reduction.name, //?
                    PictureControlName = x.picture_control_name.name, //?
                    PrimaryAFPoint = x.primary_af_point,
                    VRMode = x.vr_mode.name, //?
                    VibrationReduction = x.vibration_reduction.name, //?
                    VignetteControl = x.vignette_control.name, //?
                    WhiteBalance = x.white_balance.name, //?
                    // composite
                    Aperture = (double?)x.aperture,
                    AutoFocus = x.auto_focus.name, //?
                    DepthOfField = x.depth_of_field,
                    FieldOfView = x.field_of_view,
                    HyperfocalDistance = x.hyperfocal_distance,
                    LensId = x.lens.name, //?
                    LightValue = x.light_value,
                    ScaleFactor35Efl = x.scale_factor_35_efl,
                    ShutterSpeed = x.shutter_speed,
                    // processing info
                    RawConversionMode = x.raw_conversion_mode.name, //?
                    SigmoidalContrastAdjustment = x.sigmoidal_contrast_adjustment,
                    SaturationAdjustment = x.saturation_adjustment,
                    CompressionQuality = x.compression_quality
                })
                .SingleAsync();
		}


		public Task<List<GpsData>> GetGpsDataForCategoryAsync(short categoryId, bool allowPrivate)
		{
            return _ctx.photo
                .Where(x => x.category_id == categoryId && x.gps_latitude != null && (allowPrivate || !x.is_private))
                .OrderBy(x => x.id)
                .Select(x => new GpsData { 
                    PhotoId = x.id,
                    LongitudeDegrees = (float)x.gps_longitude,
                    LatitudeDegrees = (float)x.gps_latitude
                })
				.ToListAsync();
		}


		public Task<List<Comment>> GetCommentsForPhotoAsync(int photoId)
		{
            return _ctx.comment
                .Include(c => c.user)
                .Where(x => x.photo_id == photoId)
                .OrderBy(x => x.entry_date)
                .Select(x => new Comment {
                    CommentText = x.message,
                    EntryDate = x.entry_date,
                    Username = x.user.username
                })
				.ToListAsync();
		}


		public async Task<Rating> GetRatingsAsync(int photoId, string username)
		{
            var rating = new Rating();

			var userId = await GetUserId(username);
            var avg = await _ctx.rating.Where(x => x.photo_id == photoId).AverageAsync(x => (float?)x.score);
            var usr = await _ctx.rating.Where(x => x.photo_id == photoId && x.user_id == userId).MaxAsync(x => (byte?)x.score);

            if(avg != null)
            {
                rating.AverageRating = (float)avg;
                rating.UserRating = (byte)usr;
            }

            return rating;
		}


		public async Task<int> InsertPhotoCommentAsync(int photoId, string username, string comment)
        {
            try{
                var userId = await GetUserId(username);
                    
                var ic = new comment {
                    user_id = userId,
    				photo_id = photoId,
                    message = comment,
                    entry_date = DateTime.Now
                };
    
                _ctx.comment.Add(ic);
    
                return await _ctx.SaveChangesAsync();
            }
            catch(DbUpdateException ex)
            {
                LogEntityFrameworkError(nameof(InsertPhotoCommentAsync), ex);
                throw;
            }
        }


		public async Task<float?> SavePhotoRatingAsync(int photoId, string username, byte rating)
        {
            var userId = await GetUserId(username);
            var ir = await _ctx.rating.SingleOrDefaultAsync(x => x.photo_id == photoId && x.user_id == userId);

            if(ir == null)
            {
                ir = new rating {
					photo_id = photoId,
                    user_id = userId,
					score = rating
                };

                _ctx.rating.Add(ir);
                await _ctx.SaveChangesAsync();
            }
            else if(ir.score != rating)
            {
                ir.score = rating;

                await _ctx.SaveChangesAsync();
            }

            return (float?) await _ctx.rating.Where(x => x.photo_id == photoId).AverageAsync(y => y.score);
        }
		
		
		public async Task<float?> RemovePhotoRatingAsync(int photoId, string username)
		{
            var userId = await GetUserId(username);
            var rating = await _ctx.rating.SingleAsync(x => x.photo_id == photoId && x.user_id == userId);

            _ctx.rating.Remove(rating);

            await _ctx.SaveChangesAsync();

            var avg = await _ctx.rating
                .Where(x => x.photo_id == photoId)
                .AverageAsync(y => (float?)y.score);
            
            return avg ?? 0;
		}
        

        // TODO: for the following methods, we fake out the hasGps value on Category to false as we currently do not
        //       use this when returning photos like this.  consider correcting this in the future
        public async Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
			var query = _ctx.comment
				.Where(x => allowPrivate || !x.photo.is_private)
				.GroupBy(x => x.photo)
				.Select(x => new {
					Photo = x.Key,
                    Category = x.Key.category,
					FirstPostDate = x.Min(d => d.entry_date),
					LastPostDate = x.Max(d => d.entry_date)
				});

			if(newestFirst)
			{
				query.OrderByDescending(x => x.LastPostDate);
			}
			else
			{
				query.OrderByDescending(x => x.FirstPostDate);
			}

			var photos = await query.ToListAsync();

			return photos
				.Select(x => new PhotoAndCategory {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }


		public async Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate)
        {
			var query = _ctx.comment
				.Include(c => c.user)
				.Where(x => x.user.username == username)
				.GroupBy(x => x.photo)
				.Select(g => new {
					Photo = g.Key,
                    Category = g.Key.category,
					FirstPostDate = g.Min(x => x.entry_date), 
					LastPostDate = g.Max(x => x.entry_date)
				})
				.Where(x => allowPrivate || !x.Photo.is_private);

            if(greatestFirst)
            {
				query.OrderByDescending(x => x.LastPostDate);
            }
            else
            {
				query.OrderBy(x => x.FirstPostDate);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => new PhotoAndCategory {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }


		public async Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
			var query = _ctx.comment
				.GroupBy(x => x.photo)
				.Select(g => new {
					Photo = g.Key, 
                    Category = g.Key.category,
					CommentCount = g.Count()
				})
				.Where(x => allowPrivate || !x.Photo.is_private);

            if(greatestFirst)
            {
				query.OrderByDescending(x => x.CommentCount);
            }
            else
            {
				query.OrderBy(x => x.CommentCount);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => new PhotoAndCategory {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }
		
		
		public async Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
		{
            var query = _ctx.rating
                .GroupBy(x => x.photo)
                .Select(g => new {
                    Photo = g.Key, 
                    Category = g.Key.category,
                    AverageRating = g.Average(x => x.score)
                })
                .Where(x => allowPrivate || !x.Photo.is_private);

            if(highestFirst) 
            {
                query.OrderByDescending(x => x.AverageRating);
            }
            else
            {
                query.OrderBy(x => x.AverageRating);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => new PhotoAndCategory {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
                .Take(MAX_RESULTS)
                .ToList();
        }
		
		
		public async Task<List<PhotoAndCategory>> GetPhotosAndCategoriesByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
		{
			var query = _ctx.rating
				.Where(x => x.user.username == username && (allowPrivate || !x.photo.is_private));

            if(highestFirst)
            {
				query.OrderByDescending(x => x.score);
            }
            else
            {
				query.OrderBy(x => x.score);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => new PhotoAndCategory {
                    Photo = BuildPhoto(x.photo),
                    Category = BuildPhotoCategory(x.photo.category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
		}
        
        
        async Task<short> GetUserId(string username)
        {
            return await _ctx.user
                .Where(u => u.username == username)
                .Select(x => x.id)
                .SingleAsync();
        }
        
        
		Photo BuildPhoto(photo x)
        {
            return new Photo {
                Id = x.id,
                CategoryId = x.category_id,
                Latitude = x.gps_latitude,
                Longitude = x.gps_longitude,
                XsInfo = new PhotoInfo {
                    Height = x.xs_height,
                    Width = x.xs_width,
                    Path = x.xs_path
                },
                SmInfo = new PhotoInfo {
                    Height = x.sm_height,
                    Width = x.sm_width,
                    Path = x.sm_path
                },
                MdInfo = new PhotoInfo {
                    Height = x.md_height,
                    Width = x.md_width,
                    Path = x.md_path
                },
                LgInfo = new PhotoInfo {
                    Height = x.lg_height,
                    Width = x.lg_width,
                    Path = x.lg_path
                },
                PrtInfo = new PhotoInfo {
                    Height = x.prt_height,
                    Width = x.prt_width,
                    Path = x.prt_path
                }
            };
        }


        Category BuildPhotoCategory(category cat, bool hasGps)
		{
            return new Category
            { 	Id = cat.id,
                Year = cat.year,
                Name = cat.name,
                TeaserPhotoInfo = new PhotoInfo 
				{	Height = (short)cat.teaser_photo_height,
					Width = (short)cat.teaser_photo_width,
                    Path = cat.teaser_photo_path
				},
                HasGpsData = hasGps
			};
		}
        
        
        void LogEntityFrameworkError(string method, DbUpdateException ex)
		{
			_log.LogError(string.Format("Error calling {0}: {1}", method, ex.Message));

            /*
			foreach(var dbErr in ex.EntityValidationErrors)
			{
				foreach(var err in dbErr.ValidationErrors)
				{
					_log.LogError(string.Format("Error with property [{0}]: {1}", err.PropertyName, err.ErrorMessage));
				}
			}
            */
		}
	}
}

