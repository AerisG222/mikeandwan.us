using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Maw.Data.EntityFramework.Photos;
using Maw.Domain.Photos.ThreeD;
using D = Maw.Domain.Photos;



// TODO: try with an updated EF to see if we can run some of the commented out queries fully in the db.
//       as of 1.0.0, these queries fail
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
			var count = await _ctx.Photo.CountAsync().ConfigureAwait(false);
			var skip = (int)(rand.NextDouble() * count);

            // originally tried to incorporate all data in this sql call, but that took almost 10s to complete, by
            // separating the category pull, we are less than 15ms in sql!
			var photo = await _ctx.Photo.OrderBy(x => x.Id)
                .Skip(skip)
                .Take(1)
				.SingleAsync()
                .ConfigureAwait(false);

            return new D.PhotoAndCategory 
            {
                Photo = BuildPhoto(photo),
                Category = await GetCategoryAsync(photo.CategoryId, allowPrivate).ConfigureAwait(false)
			};
        }


		public async Task<List<short>> GetYearsAsync()
        {
			return await _ctx.Category
                .Select(x => x.Year)
                .Distinct()
                .OrderByDescending(x => x)
				.ToListAsync()
                .ConfigureAwait(false);
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
                .CountAsync()
                .ConfigureAwait(false);

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
				.ToListAsync()
                .ConfigureAwait(false);

			return photos
				.Select(x => BuildPhoto(x))
				.ToList();
		}


        // TODO: revert to true async processing once the following is fixed:
        //       https://github.com/aspnet/EntityFramework/issues/6534
		public Task<D.Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            var cat = _ctx.Category
				.Where(x => x.Id == categoryId && (allowPrivate || !x.IsPrivate))
				.Select(x => new 
                { 
                    Category = x, 
                    HasGps = x.Photo.Any(i => i.GpsLatitude != null) 
                })
				.Single();

			return Task.FromResult(BuildPhotoCategory(cat.Category, cat.HasGps));
        }


		public async Task<D.Photo> GetPhotoAsync(int photoId, bool allowPrivate)
		{
			var photo = await _ctx.Photo
				.SingleAsync(x => x.Id == photoId && (allowPrivate || !x.IsPrivate))
                .ConfigureAwait(false);

			return BuildPhoto(photo);
		}


		public async Task<D.Detail> GetDetailForPhotoAsync(int photoId, bool allowPrivate)
		{
            var photo = await _ctx.Photo
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
                .SingleAsync()
                .ConfigureAwait(false);

            return new D.Detail 
            {
                // exif
                BitsPerSample = (ushort?)photo.BitsPerSample,
                Compression = photo.Compression?.Name,
                Contrast = photo.Contrast?.Name,
                CreateDate = photo.CreateDate,
                DigitalZoomRatio = photo.DigitalZoomRatio,
                ExposureCompensation = photo.ExposureCompensation,
                ExposureMode = photo.ExposureMode?.Name,
                ExposureProgram = photo.ExposureProgram?.Name,
                ExposureTime = photo.ExposureTime,
                FNumber = (double?)photo.FNumber,
                Flash = photo.Flash?.Name,
                FocalLength = photo.FocalLength,
                FocalLengthIn35mmFormat = photo.FocalLengthIn35MmFormat,
                GainControl = photo.GainControl?.Name,
                GpsAltitude = photo.GpsAltitude,
                GpsAltitudeRef = photo.GpsAltitudeRef?.Name,
                GpsDateStamp = photo.GpsDateTimeStamp,
                GpsDirection = photo.GpsDirection,
                GpsDirectionRef = photo.GpsDirectionRef?.Name,
                GpsLatitude = photo.GpsLatitude,
                GpsLatitudeRef = photo.GpsLatitudeRef?.Name,
                GpsLongitude = photo.GpsLongitude,
                GpsLongitudeRef = photo.GpsLongitudeRef?.Name,
                GpsMeasureMode = photo.GpsMeasureMode?.Name,
                GpsSatellites = photo.GpsSatellites,
                GpsStatus = photo.GpsStatus?.Name,
                GpsVersionId = photo.GpsVersionId,
                Iso = photo.Iso,
                LightSource = photo.LightSource?.Name,
                Make = photo.Make?.Name,
                MeteringMode = photo.MeteringMode?.Name,
                Model = photo.Model?.Name,
                Orientation = photo.Orientation?.Name,
                Saturation = photo.Saturation?.Name,
                SceneCaptureType = photo.SceneCaptureType?.Name,
                SceneType = photo.SceneType?.Name,
                SensingMethod = photo.SensingMethod?.Name,
                Sharpness = photo.Sharpness?.Name,
                // nikon
                AutoFocusAreaMode = photo.AfAreaMode?.Name,
                AutoFocusPoint = photo.AfPoint?.Name,
                ActiveDLighting = photo.ActiveDLighting?.Name,
                Colorspace = photo.Colorspace?.Name,
                ExposureDifference = photo.ExposureDifference,
                FlashColorFilter = photo.FlashColorFilter?.Name,
                FlashCompensation = photo.FlashCompensation,
                FlashControlMode = photo.FlashControlMode,
                FlashExposureCompensation = photo.FlashExposureCompensation,
                FlashFocalLength = photo.FlashFocalLength,
                FlashMode = photo.FlashMode?.Name,
                FlashSetting = photo.FlashSetting?.Name,
                FlashType = photo.FlashType?.Name,
                FocusDistance = photo.FocusDistance,
                FocusMode = photo.FocusMode?.Name,
                FocusPosition = photo.FocusPosition,
                HighIsoNoiseReduction = photo.HighIsoNoiseReduction?.Name,
                HueAdjustment = photo.HueAdjustment?.Name,
                NoiseReduction = photo.NoiseReduction?.Name,
                PictureControlName = photo.PictureControlName?.Name,
                PrimaryAFPoint = photo.PrimaryAfPoint,
                VRMode = photo.VrMode?.Name,
                VibrationReduction = photo.VibrationReduction?.Name,
                VignetteControl = photo.VignetteControl?.Name,
                WhiteBalance = photo.WhiteBalance?.Name,
                // composite
                Aperture = (double?)photo.Aperture,
                AutoFocus = photo.AutoFocus?.Name,
                DepthOfField = photo.DepthOfField,
                FieldOfView = photo.FieldOfView,
                HyperfocalDistance = photo.HyperfocalDistance,
                LensId = photo.Lens?.Name,
                LightValue = photo.LightValue,
                ScaleFactor35Efl = photo.ScaleFactor35Efl,
                ShutterSpeed = photo.ShutterSpeed,
                // processing info
                RawConversionMode = photo.RawConversionMode?.Name,
                SigmoidalContrastAdjustment = photo.SigmoidalContrastAdjustment,
                SaturationAdjustment = photo.SaturationAdjustment,
                CompressionQuality = photo.CompressionQuality
            };
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

			var userId = await GetUserId(username).ConfigureAwait(false);
            var avg = await _ctx.Rating.Where(x => x.PhotoId == photoId).AverageAsync(x => (float?)x.Score).ConfigureAwait(false);
            var usr = await _ctx.Rating.Where(x => x.PhotoId == photoId && x.UserId == userId).MaxAsync(x => (byte?)x.Score).ConfigureAwait(false);

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
                var userId = await GetUserId(username).ConfigureAwait(false);
                    
                var ic = new Comment 
                {
                    UserId = userId,
    				PhotoId = photoId,
                    Message = comment,
                    EntryDate = DateTime.Now
                };
    
                _ctx.Comment.Add(ic);
    
                return await _ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            catch(DbUpdateException ex)
            {
                LogEntityFrameworkError(nameof(InsertPhotoCommentAsync), ex);
                throw;
            }
        }


		public async Task<float?> SavePhotoRatingAsync(int photoId, string username, byte rating)
        {
            var userId = await GetUserId(username).ConfigureAwait(false);
            var ir = await _ctx.Rating.SingleOrDefaultAsync(x => x.PhotoId == photoId && x.UserId == userId).ConfigureAwait(false);

            if(ir == null)
            {
                ir = new Rating 
                {
					PhotoId = photoId,
                    UserId = userId,
					Score = rating
                };

                _ctx.Rating.Add(ir);
                await _ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            else if(ir.Score != rating)
            {
                ir.Score = rating;

                await _ctx.SaveChangesAsync().ConfigureAwait(false);
            }

            return (float?) await _ctx.Rating.Where(x => x.PhotoId == photoId).AverageAsync(y => y.Score).ConfigureAwait(false);
        }
		
		
		public async Task<float?> RemovePhotoRatingAsync(int photoId, string username)
		{
            var userId = await GetUserId(username).ConfigureAwait(false);
            var rating = await _ctx.Rating.SingleAsync(x => x.PhotoId == photoId && x.UserId == userId).ConfigureAwait(false);

            _ctx.Rating.Remove(rating);

            await _ctx.SaveChangesAsync().ConfigureAwait(false);

            var avg = await _ctx.Rating
                .Where(x => x.PhotoId == photoId)
                .AverageAsync(y => (float?)y.Score)
                .ConfigureAwait(false);
            
            return avg ?? 0;
		}
        

/*
        // TODO: for the following methods, we fake out the hasGps value on Category to false as we currently do not
        //       use this when returning photos like this.  consider correcting this in the future
        public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
			var query = _ctx.Comment
                .Include(x => x.Photo)
                .ThenInclude(x => x.Category)
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
*/


        public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
			var query = await _ctx.Comment
                .Include(x => x.Photo)
                .ThenInclude(x => x.Category)
                .Where(x => allowPrivate || !x.Photo.IsPrivate)
				.GroupBy(x => x.Photo)
                .ToListAsync()
                .ConfigureAwait(false);

            var groups = query
				.Select(g => new 
                {
					Photo = g.Key,
                    Category = g.Key.Category,
					FirstPostDate = g.Min(x => x.EntryDate), 
					LastPostDate = g.Max(x => x.EntryDate)
				});

            if(newestFirst)
            {
				groups = groups.OrderByDescending(x => x.LastPostDate);
            }
            else
            {
				groups = groups.OrderBy(x => x.FirstPostDate);
            }

			return groups
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }


        public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDateAsync(string username, bool newestFirst, bool allowPrivate)
        {
			var query = await _ctx.Comment
                .Include(x => x.Photo)
                .ThenInclude(x => x.Category)
				.Where(x => x.User.Username == username)
                .Where(x => allowPrivate || !x.Photo.IsPrivate)
				.GroupBy(x => x.Photo)
                .ToListAsync()
                .ConfigureAwait(false);

            var groups = query
				.Select(g => new 
                {
					Photo = g.Key,
                    Category = g.Key.Category,
					FirstPostDate = g.Min(x => x.EntryDate), 
					LastPostDate = g.Max(x => x.EntryDate)
				});

            if(newestFirst)
            {
				groups = groups.OrderByDescending(x => x.LastPostDate);
            }
            else
            {
				groups = groups.OrderBy(x => x.FirstPostDate);
            }

			return groups
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }

        
/*        
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
*/


        public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
			var query = await _ctx.Comment
                .Include(x => x.Photo)
                .ThenInclude(x => x.Category)
                .Where(x => allowPrivate || !x.Photo.IsPrivate)
				.GroupBy(x => x.Photo)
                .ToListAsync()
                .ConfigureAwait(false);

            var groups = query
				.Select(g => new 
                {
					Photo = g.Key,
                    Category = g.Key.Category,
					CommentCount = g.Count()
				});

            if(greatestFirst)
            {
				groups = groups.OrderByDescending(x => x.CommentCount);
            }
            else
            {
				groups = groups.OrderBy(x => x.CommentCount);
            }

			return groups
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }


/*
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
*/		
		
        public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
        {
			var query = await _ctx.Rating
                .Include(x => x.Photo)
                .ThenInclude(x => x.Category)
                .Where(x => allowPrivate || !x.Photo.IsPrivate)
				.GroupBy(x => x.Photo)
                .ToListAsync()
                .ConfigureAwait(false);

            var groups = query
				.Select(g => new 
                {
					Photo = g.Key,
                    Category = g.Key.Category,
					AverageRating = g.Average(x => x.Score)
				});

            if(highestFirst)
            {
				groups = groups.OrderByDescending(x => x.AverageRating);
            }
            else
            {
				groups = groups.OrderBy(x => x.AverageRating);
            }

			return groups
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
        }

/*
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
*/		
		
		public async Task<List<D.PhotoAndCategory>> GetPhotosAndCategoriesByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
		{
			var query = _ctx.Rating
                .Include(x => x.Photo)
                .ThenInclude(x => x.Category)
				.Where(x => x.User.Username == username && (allowPrivate || !x.Photo.IsPrivate));

            if(highestFirst)
            {
				query.OrderByDescending(x => x.Score);
            }
            else
            {
				query.OrderBy(x => x.Score);
            }

			var photos = await query.ToListAsync().ConfigureAwait(false);

			return photos
				.Select(x => new D.PhotoAndCategory 
                {
                    Photo = BuildPhoto(x.Photo),
                    Category = BuildPhotoCategory(x.Photo.Category, false)
                })
				.Take(MAX_RESULTS)
				.ToList();
		}
        

        public async Task<List<D.CategoryPhotoCount>> GetStats(bool allowPrivate)
        {
            return await _ctx.Category
                .Where(x => allowPrivate || !x.IsPrivate)
                .Select(x => new D.CategoryPhotoCount
                    {
                        Year = x.Year,
                        CategoryId = x.Id,
                        CategoryName = x.Name,
                        PhotoCount = _ctx.Photo.Count(p => p.CategoryId == x.Id)
                    })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.CategoryName)
                .ToListAsync()
                .ConfigureAwait(false);
        }

        
        public async Task<List<Category3D>> GetAllCategories3D()
        {
            return await _ctx.Category
                .OrderBy(x => x.Id)
                .Select(x => new Category3D {
                    Id = x.Id,
                    Name = x.Name,
                    Year = x.Year,
                    TeaserImage = new Image3D {
                        Path = x.TeaserPhotoPath,
                        Width = (short)x.TeaserPhotoWidth,
                        Height = (short)x.TeaserPhotoHeight
                    }
                })
                .ToListAsync();
        }


        public async Task<List<Photo3D>> GetPhotos3D(int categoryId)
        {
            return await _ctx.Photo
                .Where(x => x.CategoryId == categoryId)
                .OrderBy(x => x.CreateDate)
                .Select(x => new Photo3D {
                    Id = x.Id,
                    XsImage = new Image3D {
                        Path = x.XsPath,
                        Width = x.XsWidth,
                        Height = x.XsHeight
                    },
                    MdImage = new Image3D {
                        Path = x.MdPath,
                        Width = x.MdWidth,
                        Height = x.MdHeight
                    },
                    LgImage = new Image3D {
                        Path = x.LgPath,
                        Width = x.LgWidth,
                        Height = x.LgHeight
                    }
                })
                .ToListAsync();
        }


        async Task<short> GetUserId(string username)
        {
            return await _ctx.User
                .Where(u => u.Username == username)
                .Select(x => x.Id)
                .SingleAsync()
                .ConfigureAwait(false);
        }
        
        
		D.Photo BuildPhoto(Photo x)
        {
            return new D.Photo 
            {
                Id = x.Id,
                CategoryId = x.CategoryId,
                Latitude = string.Equals(x.GpsLatitudeRefId, "N", StringComparison.OrdinalIgnoreCase) ? x.GpsLatitude : -1.0f * x.GpsLatitude,
                Longitude = string.Equals(x.GpsLongitudeRefId, "E", StringComparison.OrdinalIgnoreCase) ? x.GpsLongitude : -1.0f * x.GpsLongitude,
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

