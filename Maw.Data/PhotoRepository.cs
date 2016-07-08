using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Maw.Data.EntityFramework.Photos;
using D = Maw.Domain.Photos;


namespace Maw.Data
{
	public class PhotoRepository
        : D.IPhotoRepository
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


        public async Task<D.PhotoAndCategory> GetRandomPhotoAsync(bool allowPrivate)
        {
            var rand = new Random();
			var count = await _ctx.Photo.CountAsync();
			var skip = (int)(rand.NextDouble() * count);

            // originally tried to incorporate all data in this sql call, but that took almost 10s to complete, by
            // separating the category pull, we are less than 15ms in sql!
			var photo = await _ctx.Photo.OrderBy(x => x.Id)
                .Skip(skip)
                .Take(1)
				.SingleAsync();

            return new D.PhotoAndCategory 
            {
                Photo = BuildPhoto(photo),
                Category = await GetCategoryAsync(photo.CategoryId, allowPrivate)
			};
        }


		public async Task<List<short>> GetYearsAsync()
        {
			return await _ctx.Category
                .Select(x => x.Year)
                .Distinct()
                .OrderByDescending(x => x)
				.ToListAsync();
        }


        public Task<List<D.Category>> GetCategoriesForYearAsync(short year, bool allowPrivate)
		{
            return _ctx.Category
                .Where(x => x.Year == year && (allowPrivate || !x.IsPrivate))
                .OrderBy(x => x.Id)
                .Select(x => new D.Category 
                {
                    Id = x.Id,
                    Name = x.Name,
                    Year = x.Year,
					HasGpsData = x.Photo.Any(i => i.GpsLatitude != null),
                    TeaserPhotoInfo = new D.PhotoInfo 
                    { 
                        Path = x.TeaserPhotoPath, 
						Width = (short)x.TeaserPhotoWidth,
						Height = (short)x.TeaserPhotoHeight
                    }
                })
				.ToListAsync();
		}


        public async Task<short> GetCategoryCountAsync(bool allowPrivate)
        {
            var count = await _ctx.Category
                .Where(x => allowPrivate || !x.IsPrivate)
                .CountAsync();

			return Convert.ToInt16(count);
        }


        public Task<List<D.Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate)
        {
            return _ctx.Category
                .Where(x => x.Id > sinceId && (allowPrivate || !x.IsPrivate))
                .OrderBy(x => x.Id)
                .Select(x => new D.Category 
                {
                    Id = x.Id,
                    Name = x.Name,
                    Year = x.Year,
					HasGpsData = x.Photo.Any(i => i.GpsLatitude != null),
                    TeaserPhotoInfo = new D.PhotoInfo 
                    { 
                        Path = x.TeaserPhotoPath, 
						Width = (short)x.TeaserPhotoWidth,
						Height = (short)x.TeaserPhotoHeight 
                    }
                })
                .ToListAsync();
        }


		public async Task<List<D.Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate)
		{
			var photos = await _ctx.Photo
                .Where(x => x.CategoryId == categoryId && (allowPrivate || !x.IsPrivate))
                .OrderBy(x => x.LgPath)
				.ToListAsync();

			return photos
				.Select(x => BuildPhoto(x))
				.ToList();
		}


		public async Task<List<D.Photo>> GetPhotosByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
			var query = _ctx.Comment
				.Where(x => allowPrivate || !x.Photo.IsPrivate)
				.GroupBy(x => x.Photo)
				.Select(x => new 
                {
					Photo = x.Key, 
					FirstPostDate = x.Min(d => d.EntryDate),
					LastPostDate = x.Max(d => d.EntryDate)
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


		public async Task<List<D.Photo>> GetPhotosByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate)
        {
			var query = _ctx.Comment
				.Include(c => c.User)
				.Where(x => x.User.Username == username)
				.GroupBy(x => x.Photo)
				.Select(g => new 
                {
					Photo = g.Key, 
					FirstPostDate = g.Min(x => x.EntryDate), 
					LastPostDate = g.Max(x => x.EntryDate)
				})
				.Where(x => allowPrivate || !x.Photo.IsPrivate);

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


		public async Task<List<D.Photo>> GetPhotosByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
			var query = _ctx.Comment
				.GroupBy(x => x.Photo)
				.Select(g => new 
                {
					Photo = g.Key, 
					CommentCount = g.Count()
				})
				.Where(x => allowPrivate || !x.Photo.IsPrivate);

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
		
		
		public async Task<List<D.Photo>> GetPhotosByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
		{
            var query = _ctx.Rating
                .GroupBy(x => x.Photo)
                .Select(g => new 
                {
                    Photo = g.Key, 
                    AverageRating = g.Average(x => x.Score)
                })
                .Where(x => allowPrivate || !x.Photo.IsPrivate);

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
		
		
		public async Task<List<D.Photo>> GetPhotosByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
		{
			var query = _ctx.Rating
				.Where(x => x.User.Username == username && (allowPrivate || !x.Photo.IsPrivate));

            if(highestFirst)
            {
				query.OrderByDescending(x => x.Score);
            }
            else
            {
				query.OrderBy(x => x.Score);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => BuildPhoto(x.Photo))
				.Take(MAX_RESULTS)
				.ToList();
		}


		public async Task<D.Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            var cat = await _ctx.Category
				.Where(x => x.Id == categoryId && (allowPrivate || !x.IsPrivate))
				.Select(x => new 
                { 
                    Category = x, 
                    HasGps = x.Photo.Any(i => i.GpsLatitude != null) 
                })
				.SingleAsync();

			return BuildPhotoCategory(cat.Category, cat.HasGps);
        }


		public async Task<D.Photo> GetPhotoAsync(int photoId, bool allowPrivate)
		{
			var photo = await _ctx.Photo
				.SingleAsync(x => x.Id == photoId && (allowPrivate || !x.IsPrivate));

			return BuildPhoto(photo);
		}


		public Task<D.Detail> GetDetailForPhotoAsync(int photoId, bool allowPrivate)
		{
            return _ctx.Photo
                .Include(x => x.ActiveDLighting)
                .Include(x => x.AfAreaMode)
                .Include(x => x.AfPoint)
                .Include(x => x.AutoFocus)
                .Include(x => x.Colorspace)
                .Include(x => x.Compression)
                .Include(x => x.Contrast)
                .Include(x => x.ExposureMode)
                .Include(x => x.ExposureProgram)
                .Include(x => x.Flash)
                .Include(x => x.FlashColorFilter)
                .Include(x => x.FlashMode)
                .Include(x => x.FlashSetting)
                .Include(x => x.FlashType)
                .Include(x => x.FocusMode)
                .Include(x => x.GainControl)
                .Include(x => x.GpsAltitudeRef)
                .Include(x => x.GpsDirectionRef)
                .Include(x => x.GpsLatitudeRef)
                .Include(x => x.GpsLongitudeRef)
                .Include(x => x.GpsMeasureMode)
                .Include(x => x.GpsStatus)
                .Include(x => x.HighIsoNoiseReduction)
                .Include(x => x.HueAdjustment)
                .Include(x => x.Lens)
                .Include(x => x.LightSource)
                .Include(x => x.Make)
                .Include(x => x.MeteringMode)
                .Include(x => x.Model)
                .Include(x => x.NoiseReduction)
                .Include(x => x.Orientation)
                .Include(x => x.PictureControlName)
                .Include(x => x.Saturation)
                .Include(x => x.SceneCaptureType)
                .Include(x => x.SceneType)
                .Include(x => x.SensingMethod)
                .Include(x => x.Sharpness)
                .Include(x => x.VibrationReduction)
                .Include(x => x.VignetteControl)
                .Include(x => x.VrMode)
                .Include(x => x.WhiteBalance)
                .Where(x => x.Id == photoId && (allowPrivate || !x.IsPrivate))
                .Select(x => new D.Detail 
                {
                    // exif
                    BitsPerSample = (ushort?)x.BitsPerSample,
                    Compression = x.Compression.Name, //?
                    Contrast = x.Contrast.Name, //?
                    CreateDate = x.CreateDate,
                    DigitalZoomRatio = x.DigitalZoomRatio,
                    ExposureCompensation = x.ExposureCompensation,
                    ExposureMode = x.ExposureMode.Name, //?
                    ExposureProgram = x.ExposureProgram.Name, //?
                    ExposureTime = x.ExposureTime,
                    FNumber = (double?)x.FNumber,
                    Flash = x.Flash.Name, //?
                    FocalLength = x.FocalLength,
                    FocalLengthIn35mmFormat = x.FocalLengthIn35MmFormat,
                    GainControl = x.GainControl.Name, //?
                    GpsAltitude = x.GpsAltitude,
                    GpsAltitudeRef = x.GpsAltitudeRef.Name, //?
                    GpsDateStamp = x.GpsDateTimeStamp,
                    GpsDirection = x.GpsDirection,
                    GpsDirectionRef = x.GpsDirectionRef.Name, //?
                    GpsLatitude = x.GpsLatitude,
                    GpsLatitudeRef = x.GpsLatitudeRef.Name, //?
                    GpsLongitude = x.GpsLongitude,
                    GpsLongitudeRef = x.GpsLongitudeRef.Name, //?
                    GpsMeasureMode = x.GpsMeasureMode.Name, //?
                    GpsSatellites = x.GpsSatellites,
                    GpsStatus = x.GpsStatus.Name, //?
                    GpsVersionId = x.GpsVersionId,
                    Iso = x.Iso,
                    LightSource = x.LightSource.Name, //?
                    Make = x.Make.Name, //?
                    MeteringMode = x.MeteringMode.Name, //?
                    Model = x.Model.Name, //?
                    Orientation = x.Orientation.Name, //?
                    Saturation = x.Saturation.Name, //?
                    SceneCaptureType = x.SceneCaptureType.Name, //?
                    SceneType = x.SceneType.Name, //?
                    SensingMethod = x.SensingMethod.Name, //?
                    Sharpness = x.Sharpness.Name, //?
                    // nikon
                    AutoFocusAreaMode = x.AfAreaMode.Name, //?
                    AutoFocusPoint = x.AfPoint.Name, //?
                    ActiveDLighting = x.ActiveDLighting.Name, //?
                    Colorspace = x.Colorspace.Name, //?
                    ExposureDifference = x.ExposureDifference,
                    FlashColorFilter = x.FlashColorFilter.Name, //?
                    FlashCompensation = x.FlashCompensation,
                    FlashControlMode = x.FlashControlMode,
                    FlashExposureCompensation = x.FlashExposureCompensation,
                    FlashFocalLength = x.FlashFocalLength,
                    FlashMode = x.FlashMode.Name, //?
                    FlashSetting = x.FlashSetting.Name, //?
                    FlashType = x.FlashType.Name, //?
                    FocusDistance = x.FocusDistance,
                    FocusMode = x.FocusMode.Name, //?
                    FocusPosition = x.FocusPosition,
                    HighIsoNoiseReduction = x.HighIsoNoiseReduction.Name, //?
                    HueAdjustment = x.HueAdjustment.Name, //?
                    NoiseReduction = x.NoiseReduction.Name, //?
                    PictureControlName = x.PictureControlName.Name, //?
                    PrimaryAFPoint = x.PrimaryAfPoint,
                    VRMode = x.VrMode.Name, //?
                    VibrationReduction = x.VibrationReduction.Name, //?
                    VignetteControl = x.VignetteControl.Name, //?
                    WhiteBalance = x.WhiteBalance.Name, //?
                    // composite
                    Aperture = (double?)x.Aperture,
                    AutoFocus = x.AutoFocus.Name, //?
                    DepthOfField = x.DepthOfField,
                    FieldOfView = x.FieldOfView,
                    HyperfocalDistance = x.HyperfocalDistance,
                    LensId = x.Lens.Name, //?
                    LightValue = x.LightValue,
                    ScaleFactor35Efl = x.ScaleFactor35Efl,
                    ShutterSpeed = x.ShutterSpeed,
                    // processing info
                    RawConversionMode = x.RawConversionMode.Name, //?
                    SigmoidalContrastAdjustment = x.SigmoidalContrastAdjustment,
                    SaturationAdjustment = x.SaturationAdjustment,
                    CompressionQuality = x.CompressionQuality
                })
                .SingleAsync();
		}


		public Task<List<D.GpsData>> GetGpsDataForCategoryAsync(short categoryId, bool allowPrivate)
		{
            return _ctx.Photo
                .Where(x => x.CategoryId == categoryId && x.GpsLatitude != null && (allowPrivate || !x.IsPrivate))
                .OrderBy(x => x.Id)
                .Select(x => new D.GpsData 
                { 
                    PhotoId = x.Id,
                    LongitudeDegrees = (float)x.GpsLongitude,
                    LatitudeDegrees = (float)x.GpsLatitude
                })
				.ToListAsync();
		}


		public Task<List<D.Comment>> GetCommentsForPhotoAsync(int photoId)
		{
            return _ctx.Comment
                .Include(c => c.User)
                .Where(x => x.PhotoId == photoId)
                .OrderBy(x => x.EntryDate)
                .Select(x => new D.Comment 
                {
                    CommentText = x.Message,
                    EntryDate = x.EntryDate,
                    Username = x.User.Username
                })
				.ToListAsync();
		}


		public async Task<D.Rating> GetRatingsAsync(int photoId, string username)
		{
            var rating = new D.Rating();

			var userId = await GetUserId(username);
            var avg = await _ctx.Rating.Where(x => x.PhotoId == photoId).AverageAsync(x => (float?)x.Score);
            var usr = await _ctx.Rating.Where(x => x.PhotoId == photoId && x.UserId == userId).MaxAsync(x => (byte?)x.Score);

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
                    
                var ic = new Comment 
                {
                    UserId = userId,
    				PhotoId = photoId,
                    Message = comment,
                    EntryDate = DateTime.Now
                };
    
                _ctx.Comment.Add(ic);
    
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
            var ir = await _ctx.Rating.SingleOrDefaultAsync(x => x.PhotoId == photoId && x.UserId == userId);

            if(ir == null)
            {
                ir = new Rating 
                {
					PhotoId = photoId,
                    UserId = userId,
					Score = rating
                };

                _ctx.Rating.Add(ir);
                await _ctx.SaveChangesAsync();
            }
            else if(ir.Score != rating)
            {
                ir.Score = rating;

                await _ctx.SaveChangesAsync();
            }

            return (float?) await _ctx.Rating.Where(x => x.PhotoId == photoId).AverageAsync(y => y.Score);
        }
		
		
		public async Task<float?> RemovePhotoRatingAsync(int photoId, string username)
		{
            var userId = await GetUserId(username);
            var rating = await _ctx.Rating.SingleAsync(x => x.PhotoId == photoId && x.UserId == userId);

            _ctx.Rating.Remove(rating);

            await _ctx.SaveChangesAsync();

            var avg = await _ctx.Rating
                .Where(x => x.PhotoId == photoId)
                .AverageAsync(y => (float?)y.Score);
            
            return avg ?? 0;
		}
        

        // TODO: for the following methods, we fake out the hasGps value on Category to false as we currently do not
        //       use this when returning photos like this.  consider correcting this in the future
        public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
			var query = _ctx.Comment
				.Where(x => allowPrivate || !x.Photo.IsPrivate)
				.GroupBy(x => x.Photo)
				.Select(x => new 
                {
					Photo = x.Key,
                    Category = x.Key.Category,
					FirstPostDate = x.Min(d => d.EntryDate),
					LastPostDate = x.Max(d => d.EntryDate)
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
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }


		public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDateAsync(string username, bool greatestFirst, bool allowPrivate)
        {
			var query = _ctx.Comment
				.Include(c => c.User)
				.Where(x => x.User.Username == username)
				.GroupBy(x => x.Photo)
				.Select(g => new 
                {
					Photo = g.Key,
                    Category = g.Key.Category,
					FirstPostDate = g.Min(x => x.EntryDate), 
					LastPostDate = g.Max(x => x.EntryDate)
				})
				.Where(x => allowPrivate || !x.Photo.IsPrivate);

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
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }


		public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
			var query = _ctx.Comment
				.GroupBy(x => x.Photo)
				.Select(g => new 
                {
					Photo = g.Key, 
                    Category = g.Key.Category,
					CommentCount = g.Count()
				})
				.Where(x => allowPrivate || !x.Photo.IsPrivate);

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
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }
		
		
		public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
		{
            var query = _ctx.Rating
                .GroupBy(x => x.Photo)
                .Select(g => new 
                {
                    Photo = g.Key, 
                    Category = g.Key.Category,
                    AverageRating = g.Average(x => x.Score)
                })
                .Where(x => allowPrivate || !x.Photo.IsPrivate);

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
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
                .Take(MAX_RESULTS)
                .ToList();
        }
		
		
		public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
		{
			var query = _ctx.Rating
				.Where(x => x.User.Username == username && (allowPrivate || !x.Photo.IsPrivate));

            if(highestFirst)
            {
				query.OrderByDescending(x => x.Score);
            }
            else
            {
				query.OrderBy(x => x.Score);
            }

			var photos = await query.ToListAsync();

			return photos
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Photo.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
		}
        
        
        async Task<short> GetUserId(string username)
        {
            return await _ctx.User
                .Where(u => u.Username == username)
                .Select(x => x.Id)
                .SingleAsync();
        }
        
        
		D.Photo BuildPhoto(Photo x)
        {
            return new D.Photo 
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                Latitude = x.GpsLatitude,
                Longitude = x.GpsLongitude,
                XsInfo = new D.PhotoInfo 
                {
                    Height = x.XsHeight,
                    Width = x.XsWidth,
                    Path = x.XsPath
                },
                SmInfo = new D.PhotoInfo 
                {
                    Height = x.SmHeight,
                    Width = x.SmWidth,
                    Path = x.SmPath
                },
                MdInfo = new D.PhotoInfo 
                {
                    Height = x.MdHeight,
                    Width = x.MdWidth,
                    Path = x.MdPath
                },
                LgInfo = new D.PhotoInfo 
                {
                    Height = x.LgHeight,
                    Width = x.LgWidth,
                    Path = x.LgPath
                },
                PrtInfo = new D.PhotoInfo 
                {
                    Height = x.PrtHeight,
                    Width = x.PrtWidth,
                    Path = x.PrtPath
                }
            };
        }


        D.Category BuildPhotoCategory(Category cat, bool hasGps)
		{
            return new D.Category
            { 	Id = cat.Id,
                Year = cat.Year,
                Name = cat.Name,
                TeaserPhotoInfo = new D.PhotoInfo 
				{	
                    Height = (short)cat.TeaserPhotoHeight,
					Width = (short)cat.TeaserPhotoWidth,
                    Path = cat.TeaserPhotoPath
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

