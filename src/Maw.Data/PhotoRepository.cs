using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Maw.Domain.Photos.ThreeD;
using Maw.Domain.Photos;


namespace Maw.Data
{
	public class PhotoRepository
        : Repository, IPhotoRepository
	{
        const int MAX_RESULTS = 600;

        const string CATEGORY_PROJECTION = @"
            id,
            name,
            year,
            (SELECT COUNT(1) 
               FROM photo.photo p 
              WHERE p.gps_latitude IS NOT NULL
                AND p.category_id = photo.category.id) > 0 AS has_gps_data,
            teaser_photo_path AS path,
            teaser_photo_width AS width,
            teaser_photo_height AS height";

        const string PHOTO_PROJECTION = @"
            id,
            category_id,
            CASE WHEN gps_latitude_ref_id = 'S' THEN -1.0 * gps_latitude
                 ELSE gps_latitude 
                  END AS latitude,
            CASE WHEN gps_longitude_ref_id = 'W' THEN -1.0 * gps_longitude
                 ELSE gps_longitude 
                  END AS longitude,
            xs_path AS path,
            xs_width AS width,
            xs_height AS height,
            sm_path AS path,
            sm_width AS width,
            sm_height AS height,
            md_path AS path,
            md_width AS width,
            md_height AS height,
            lg_path AS path,
            lg_width AS width,
            lg_height AS height,
            prt_path AS path,
            prt_width AS width,
            prt_height AS height";

        const string PHOTO_AND_CATEGORY_PROJECTION = @"
            p.id AS photo_id,
            p.category_id,
            p.xs_path,
            p.xs_width,
            p.xs_height,
            p.sm_path,
            p.sm_width,
            p.sm_height,
            p.md_path,
            p.md_width,
            p.md_height,
            p.lg_path,
            p.lg_width,
            p.lg_height,
            p.prt_path,
            p.prt_width,
            p.prt_height,
            CASE WHEN p.gps_latitude_ref_id = 'S' THEN -1.0 * p.gps_latitude
                 ELSE p.gps_latitude 
                  END AS latitude,
            CASE WHEN p.gps_longitude_ref_id = 'W' THEN -1.0 * p.gps_longitude
                 ELSE p.gps_longitude 
                  END AS longitude,
            c.name,
            c.year,
            (SELECT COUNT(1) 
               FROM photo.photo
              WHERE gps_latitude IS NOT NULL
                AND category_id = c.id) > 0 AS has_gps_data,
            c.teaser_photo_path,
            c.teaser_photo_width,
            c.teaser_photo_height";


        public PhotoRepository(string connectionString)
            : base(connectionString)
        {

        }


        public Task<PhotoAndCategory> GetRandomPhotoAsync(bool allowPrivate)
        {
            return RunAsync(async conn => {
                var result = await conn.QueryAsync(
                    $@"WITH random AS 
                       (
                            SELECT id
                              FROM photo.photo
                             WHERE (1 = @allowPrivate OR is_private = FALSE)
                             ORDER BY RANDOM()
                             LIMIT 1
                       )
                       SELECT {PHOTO_AND_CATEGORY_PROJECTION}
                         FROM random
                        INNER JOIN photo.photo p ON random.id = p.id
                        INNER JOIN photo.category c ON p.category_id = c.id;",
                    new { allowPrivate = allowPrivate ? 1 : 0 }
                ).ConfigureAwait(false);

                if(result == null || result.Count() == 0)
                {
                    return null;
                }

                // TODO: why is this cast needed?
                return (PhotoAndCategory)BuildPhotoAndCategory(result.First());
            });
        }


		public Task<IEnumerable<short>> GetYearsAsync()
        {
            return RunAsync(conn => {
                return conn.QueryAsync<short>(
                    @"SELECT DISTINCT year
                        FROM photo.category
                       ORDER BY year DESC;"
                );
            });
        }


        public Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate)
		{
            return RunAsync(conn => {
                return conn.QueryAsync<Category, PhotoInfo, Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                         FROM photo.category
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND year = @year
                        ORDER BY id;",
                    (category, photoInfo) => {
                        category.TeaserPhotoInfo = photoInfo;
                        return category;
                    },
                    new { 
                        allowPrivate = allowPrivate ? 1 : 0,
                        year = year
                    },
                    splitOn: "path"
                );
            });
		}


        public Task<short> GetCategoryCountAsync(bool allowPrivate)
        {
            return RunAsync(conn => {
                return conn.QuerySingleOrDefaultAsync<short>(
                    @"SELECT COUNT(1)
                        FROM photo.category
                       WHERE (1 = @allowPrivate OR is_private = FALSE);",
                    new { allowPrivate = allowPrivate ? 1 : 0 }
                );
            });
        }


        public Task<IEnumerable<Category>> GetRecentCategoriesAsync(short sinceId, bool allowPrivate)
        {
            return RunAsync(conn => {
                return conn.QueryAsync<Category, PhotoInfo, Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                        FROM photo.category
                       WHERE (1 = @allowPrivate OR is_private = FALSE)
                         AND id > @sinceId;",
                    (category, photoInfo) => {
                        category.TeaserPhotoInfo = photoInfo;
                        return category;
                    },
                    new { 
                        allowPrivate = allowPrivate ? 1 : 0,
                        sinceId = sinceId
                    },
                    splitOn: "path"
                );
            });
        }


		public Task<IEnumerable<Photo>> GetPhotosForCategoryAsync(short categoryId, bool allowPrivate)
		{
            return RunAsync(conn => {
                return conn.QueryAsync<Photo, PhotoInfo, PhotoInfo, PhotoInfo, PhotoInfo, PhotoInfo, Photo>(
                    $@"SELECT {PHOTO_PROJECTION}
                         FROM photo.photo
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND category_id = @categoryId
                        ORDER BY lg_path;",
                    (photo, xs, sm, md, lg, prt) => {
                        photo.XsInfo = xs;
                        photo.SmInfo = sm;
                        photo.MdInfo = md;
                        photo.LgInfo = lg;
                        photo.PrtInfo = prt;

                        return photo;
                    },
                    new { 
                        allowPrivate = allowPrivate ? 1 : 0,
                        categoryId = categoryId
                    },
                    splitOn: "path"
                );
            });
		}


		public Task<Category> GetCategoryAsync(short categoryId, bool allowPrivate)
        {
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<Category, PhotoInfo, Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                         FROM photo.category
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND id = @categoryId;",
                    (category, photoInfo) => {
                        category.TeaserPhotoInfo = photoInfo;
                        return category;
                    },
                    new { 
                        allowPrivate = allowPrivate ? 1 : 0,
                        categoryId = categoryId
                    },
                    splitOn: "path"
                ).ConfigureAwait(false);

                return result.SingleOrDefault();
            });
        }


		public Task<Photo> GetPhotoAsync(int photoId, bool allowPrivate)
		{
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<Photo, PhotoInfo, PhotoInfo, PhotoInfo, PhotoInfo, PhotoInfo, Photo>(
                    $@"SELECT {PHOTO_PROJECTION}
                         FROM photo.photo
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND id = @photoId;",
                    (photo, xs, sm, md, lg, prt) => {
                        photo.XsInfo = xs;
                        photo.SmInfo = sm;
                        photo.MdInfo = md;
                        photo.LgInfo = lg;
                        photo.PrtInfo = prt;

                        return photo;
                    },
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        photoId = photoId
                    },
                    splitOn: "path"
                );

                return result.SingleOrDefault();
            });
		}


		public Task<Detail> GetDetailForPhotoAsync(int photoId, bool allowPrivate)
		{
            return RunAsync(conn => {
                return conn.QuerySingleOrDefaultAsync<Detail>(
                    @"SELECT p.bits_per_sample,
                             cm.name AS compression,
                             cn.name AS contrast,
                             create_date,
                             digital_zoom_ratio,
                             exposure_compensation,
                             em.name AS exposure_mode,
                             ep.name AS exposure_program,
                             exposure_time,
                             f_number,
                             f.name AS flash,
                             focal_length,
                             focal_length_in_35_mm_format,
                             gc.name AS gain_control,
                             gps_altitude,
                             gpsar.name AS gps_alititude_ref,
                             gps_date_time_stamp AS gps_date_stamp,
                             gps_direction,
                             gpsdr.name AS gps_direction_ref,
                             gps_latitude,
                             gpslatr.name AS gps_latitude_ref,
                             gps_longitude,
                             gpslngr.name AS gps_longitude_ref,
                             gpsmm.name AS gps_measure_mode,
                             gps_satellites,
                             gpss.name AS gps_status,
                             gps_version_id,
                             iso,
                             ls.name AS light_source,
                             m.name AS make,
                             mm.name AS metering_mode,
                             md.name AS model,
                             o.name AS orientation,
                             s.name AS saturation,
                             sct.name AS scene_capture_type,
                             st.name AS scene_type,
                             sm.name AS sensing_method,
                             sh.name AS sharpness,
                             afam.name AS auto_focus_area_mode,
                             afp.name AS auto_focus_point,
                             adl.name AS active_d_lighting,
                             cs.name AS colorspace,
                             exposure_difference,
                             fcf.name AS flash_color_filter,
                             flash_compensation,
                             flash_control_mode,
                             flash_exposure_compensation,
                             flash_focal_length,
                             fm.name AS flash_mode,
                             fs.name AS flash_setting,
                             ft.name AS flash_type,
                             focus_distance,
                             foc.name AS focus_mode,
                             focus_position,
                             hinr.name AS high_iso_noise_reduction,
                             ha.name AS hue_adjustment,
                             nr.name AS noise_reduction,
                             pcn.name AS picture_control_name,
                             primary_af_point,
                             vrm.name AS vr_mode,
                             vr.name AS vibration_reduction,
                             vc.name AS vignette_control,
                             wb.name AS white_balance,
                             aperture,
                             af.name AS auto_focus,
                             depth_of_field,
                             field_of_view,
                             hyperfocal_distance,
                             l.name AS lens_id,
                             light_value,
                             scale_factor_35_efl,
                             shutter_speed,
                             rcm.name AS raw_conversion_mode,
                             sigmoidal_contrast_adjustment,
                             saturation_adjustment,
                             compression_quality
                        FROM photo.photo p
                        LEFT OUTER JOIN photo.active_d_lighting adl ON adl.id = p.active_d_lighting_id
                        LEFT OUTER JOIN photo.af_area_mode afam ON afam.id = p.af_area_mode_id
                        LEFT OUTER JOIN photo.af_point afp ON afp.id = p.af_point_id
                        LEFT OUTER JOIN photo.auto_focus af ON af.id = p.auto_focus_id
                        LEFT OUTER JOIN photo.colorspace cs ON cs.id = p.colorspace_id
                        LEFT OUTER JOIN photo.compression cm ON cm.id = p.compression_id
                        LEFT OUTER JOIN photo.contrast cn ON cn.id = p.contrast_id
                        LEFT OUTER JOIN photo.exposure_mode em ON em.id = p.exposure_mode_id
                        LEFT OUTER JOIN photo.exposure_program ep ON ep.id = p.exposure_program_id
                        LEFT OUTER JOIN photo.flash f ON f.id = p.flash_id
                        LEFT OUTER JOIN photo.flash_color_filter fcf ON fcf.id = p.flash_color_filter_id
                        LEFT OUTER JOIN photo.flash_mode fm ON fm.id = p.flash_mode_id
                        LEFT OUTER JOIN photo.flash_setting fs ON fs.id = p.flash_setting_id
                        LEFT OUTER JOIN photo.flash_type ft ON ft.id = p.flash_type_id
                        LEFT OUTER JOIN photo.focus_mode foc ON foc.id = p.focus_mode_id
                        LEFT OUTER JOIN photo.gain_control gc ON gc.id = p.gain_control_id
                        LEFT OUTER JOIN photo.gps_altitude_ref gpsar ON gpsar.id = p.gps_altitude_ref_id 
                        LEFT OUTER JOIN photo.gps_direction_ref gpsdr ON gpsdr.id = p.gps_direction_ref_id
                        LEFT OUTER JOIN photo.gps_latitude_ref gpslatr ON gpslatr.id = p.gps_latitude_ref_id
                        LEFT OUTER JOIN photo.gps_longitude_ref gpslngr ON gpslngr.id = p.gps_longitude_ref_id
                        LEFT OUTER JOIN photo.gps_measure_mode gpsmm ON gpsmm.id = p.gps_measure_mode_id
                        LEFT OUTER JOIN photo.gps_status gpss ON gpss.id = p.gps_status_id
                        LEFT OUTER JOIN photo.high_iso_noise_reduction hinr ON hinr.id = p.high_iso_noise_reduction_id
                        LEFT OUTER JOIN photo.hue_adjustment ha ON ha.id = p.hue_adjustment_id
                        LEFT OUTER JOIN photo.lens l ON l.id = p.lens_id
                        LEFT OUTER JOIN photo.light_source ls ON ls.id = p.light_source_id
                        LEFT OUTER JOIN photo.make m ON m.id = p.make_id
                        LEFT OUTER JOIN photo.metering_mode mm ON mm.id = p.metering_mode_id
                        LEFT OUTER JOIN photo.model md ON md.id = p.model_id
                        LEFT OUTER JOIN photo.noise_reduction nr ON nr.id = p.noise_reduction_id
                        LEFT OUTER JOIN photo.orientation o ON o.id = p.orientation_id
                        LEFT OUTER JOIN photo.picture_control_name pcn ON pcn.id = p.picture_control_name_id
                        LEFT OUTER JOIN photo.raw_conversion_mode rcm ON rcm.id = p.raw_conversion_mode_id
                        LEFT OUTER JOIN photo.saturation s ON s.id = p.saturation_id
                        LEFT OUTER JOIN photo.scene_capture_type sct ON sct.id = p.scene_capture_type_id
                        LEFT OUTER JOIN photo.scene_type st ON st.id = p.scene_type_id
                        LEFT OUTER JOIN photo.sensing_method sm ON sm.id = p.sensing_method_id
                        LEFT OUTER JOIN photo.sharpness sh ON sh.id = p.sharpness_id
                        LEFT OUTER JOIN photo.vibration_reduction vr ON vr.id = p.vibration_reduction_id
                        LEFT OUTER JOIN photo.vignette_control vc ON vc.id = p.vignette_control_id
                        LEFT OUTER JOIN photo.vr_mode vrm ON vrm.id = p.vr_mode_id
                        LEFT OUTER JOIN photo.white_balance wb ON wb.id = p.white_balance_id
                       WHERE (1 = @allowPrivate OR is_private = FALSE)
                         AND p.id = @photoId;",
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        photoId = photoId
                    }
                );
            });
		}


		public Task<IEnumerable<Comment>> GetCommentsForPhotoAsync(int photoId)
		{
            return RunAsync(conn => {
                return conn.QueryAsync<Comment>(
                    @"SELECT entry_date,
                             message AS comment_text,
                             u.username
                        FROM photo.comment c
                       INNER JOIN maw.user u ON c.user_id = u.id
                       WHERE c.photo_id = @photoId
                       ORDER by entry_date DESC;",
                    new { photoId = photoId }
                );
            });
		}


		public Task<Rating> GetRatingsAsync(int photoId, string username)
		{
            return RunAsync(conn => {
                return conn.QuerySingleOrDefaultAsync<Rating>(
                    @"SELECT (SELECT AVG(score) 
                                FROM photo.rating 
                               WHERE photo_id = @photoId
                             ) AS average_rating,
                             (SELECT AVG(score)
                                FROM photo.rating 
                               WHERE photo_id = @photoId
                                 AND user_id = (SELECT id 
                                                  FROM maw.user 
                                                 WHERE username = @username
                                               )
                             ) AS user_rating;",
                    new {
                        photoId = photoId,
                        username = username.ToLower()
                    }
                );
            });
		}


		public Task<int> InsertPhotoCommentAsync(int photoId, string username, string comment)
        {
            return RunAsync(conn => {
                return conn.ExecuteAsync(
                    @"INSERT INTO photo.comment
                           (
                             user_id,
                             photo_id,
                             message,
                             entry_date
                           )
                      VALUES
                           (
                             (SELECT id FROM maw.user WHERE username = @username),
                             @photoId,
                             @message,
                             @entryDate
                           );",
                    new {
                        username = username.ToLower(),
                        photoId = photoId,
                        message = comment,
                        entryDate = DateTime.Now
                    }
                );
            });
        }


		public Task<float?> SavePhotoRatingAsync(int photoId, string username, byte rating)
        {
            return RunAsync(async conn => {
                var result = await conn.ExecuteAsync(
                    @"INSERT INTO photo.rating 
                           (
                             photo_id, 
                             user_id,
                             score
                           )
                      VALUES 
                           (
                             @photoId, 
                             (SELECT id
                                FROM maw.user
                               WHERE username = @username
                             ),
                             @score
                           )
                        ON CONFLICT (photo_id, user_id) 
                        DO UPDATE
                       SET score = @score;",
                    new {
                        photoId = photoId,
                        username = username.ToLower(),
                        score = rating
                    }
                ).ConfigureAwait(false);

                return (await GetRatingsAsync(photoId, username).ConfigureAwait(false))?.AverageRating;
            });
        }
		
		
		public Task<float?> RemovePhotoRatingAsync(int photoId, string username)
		{
            return RunAsync(async conn => {
                var result = await conn.ExecuteAsync(
                    @"DELETE FROM photo.rating
                       WHERE photo_id = @photoId
                         AND user_id = (SELECT id
                                          FROM maw.user
                                         WHERE username = @username
                                       );",
                    new {
                        photoId = photoId,
                        username = username.ToLower()
                    }
                ).ConfigureAwait(false);

                return (await GetRatingsAsync(photoId, username).ConfigureAwait(false))?.AverageRating;
            });
		}
        

        public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
            return RunAsync(async conn => {
                var op = newestFirst ? "MAX" : "MIN";
                var sort = newestFirst ? "DESC" : "ASC";

                var result = await conn.QueryAsync(
                    $@"WITH comments AS 
                       (
                            SELECT photo_id,
                                   {op}(entry_date) AS entry_date
                              FROM photo.comment
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_AND_CATEGORY_PROJECTION}
                        FROM comments
                       INNER JOIN photo.photo p ON comments.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       INNER JOIN photo.category c ON p.category_id = c.id
                       ORDER BY entry_date {sort}
                       LIMIT @limit;",
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    }
                ).ConfigureAwait(false);

                // TODO: why is this cast needed?
                return result
                    .Select(x => BuildPhotoAndCategory(x))
                    .Cast<PhotoAndCategory>();
            });
        }


        public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserCommentDateAsync(string username, bool newestFirst, bool allowPrivate)
        {
            return RunAsync(async conn => {
                var op = newestFirst ? "MAX" : "MIN";
                var sort = newestFirst ? "DESC" : "ASC";

                var result = await conn.QueryAsync(
                    $@"WITH comments AS 
                       (
                            SELECT photo_id,
                                   {op}(entry_date) AS entry_date
                              FROM photo.comment c
                             INNER JOIN maw.user u ON c.user_id = u.id AND u.username = @username
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_AND_CATEGORY_PROJECTION}
                        FROM comments
                       INNER JOIN photo.photo p ON comments.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       INNER JOIN photo.category c ON p.category_id = c.id
                       ORDER BY entry_date {sort}
                       LIMIT @limit;",
                    new {
                        username = @username.ToLower(),
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    }
                ).ConfigureAwait(false);

                return result
                    .Select(x => BuildPhotoAndCategory(x))
                    .Cast<PhotoAndCategory>();
            });
        }


        public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
            return RunAsync(async conn => {
                var sort = greatestFirst ? "DESC" : "ASC";

                var result = await conn.QueryAsync(
                    $@"WITH comments AS 
                       (
                            SELECT photo_id,
                                   COUNT(1) AS comment_count
                              FROM photo.comment
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_AND_CATEGORY_PROJECTION}
                        FROM comments
                       INNER JOIN photo.photo p ON comments.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       INNER JOIN photo.category c ON p.category_id = c.id
                       ORDER BY comment_count {sort}
                       LIMIT @limit;",
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    }
                );

                return result
                    .Select(x => BuildPhotoAndCategory(x))
                    .Cast<PhotoAndCategory>();
            });
        }

		
        public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
        {
            return RunAsync(async conn => {
                var sort = highestFirst ? "DESC" : "ASC";

                var result = await conn.QueryAsync(
                    $@"WITH ratings AS 
                       (
                            SELECT photo_id,
                                   AVG(score) AS avg_score
                              FROM photo.rating
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_AND_CATEGORY_PROJECTION}
                        FROM ratings
                       INNER JOIN photo.photo p ON ratings.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       INNER JOIN photo.category c ON p.category_id = c.id
                       ORDER BY avg_score {sort}
                       LIMIT @limit;",
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    }
                );

                return result
                    .Select(x => BuildPhotoAndCategory(x))
                    .Cast<PhotoAndCategory>();
            });
        }

		
		public Task<IEnumerable<PhotoAndCategory>> GetPhotosAndCategoriesByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
		{
            return RunAsync(async conn => {
                var sort = highestFirst ? "DESC" : "ASC";

                var result = await conn.QueryAsync(
                    $@"WITH ratings AS 
                       (
                            SELECT photo_id,
                                   AVG(score) AS avg_score
                              FROM photo.rating r
                             INNER JOIN maw.user u ON r.user_id = u.id AND u.username = @username
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_AND_CATEGORY_PROJECTION}
                        FROM ratings
                       INNER JOIN photo.photo p ON ratings.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       INNER JOIN photo.category c ON p.category_id = c.id
                       ORDER BY avg_score {sort}
                       LIMIT @limit;",
                    new {
                        username = username.ToLower(),
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    }
                );

                return result
                    .Select(x => BuildPhotoAndCategory(x))
                    .Cast<PhotoAndCategory>();
            });
		}
        

        public Task<IEnumerable<CategoryPhotoCount>> GetStats(bool allowPrivate)
        {
            return RunAsync(conn => {
                return conn.QueryAsync<CategoryPhotoCount>(
                    @"SELECT year, 
                             id AS CategoryId, 
                             name AS CategoryName,
                             (SELECT COUNT(1)
                                FROM photo.photo
                               WHERE category_id = photo.category.id
                             ) AS photo_count
                       FROM photo.category
                      WHERE (1 = @allowPrivate OR is_private = FALSE)
                      ORDER BY year,
                               name;",
                    new { allowPrivate = allowPrivate ? 1 : 0 }
                );
            });
        }

        
        public Task<IEnumerable<Category3D>> GetAllCategories3D()
        {
            return RunAsync(conn => {
                return conn.QueryAsync<Category3D, Image3D, Category3D>(
                    @"SELECT id,
                             year,
                             name,
                             teaser_photo_path AS path,
                             teaser_photo_width AS width,
                             teaser_photo_height AS height
                        FROM photo.category
                       ORDER BY id;",
                    (cat, img) => {
                        cat.TeaserImage = img;
                        return cat;
                    },
                    splitOn: "path"
                );
            });
        }


        public Task<IEnumerable<Photo3D>> GetPhotos3D(int categoryId)
        {
            return RunAsync(conn => {
                return conn.QueryAsync<Photo3D, Image3D, Image3D, Image3D, Photo3D>(
                    @"SELECT id,
                             xs_path AS path,
                             xs_width AS width,
                             xs_height AS height,
                             md_path AS path,
                             md_width AS width,
                             md_height AS height,
                             lg_path AS path,
                             lg_width AS width,
                             lg_height AS height
                        FROM photo.photo
                       WHERE category_id = @categoryId
                       ORDER BY create_date;",
                    (photo, xs, md, lg) => {
                        photo.XsImage = xs;
                        photo.MdImage = md;
                        photo.LgImage = lg;

                        return photo;
                    },
                    new { categoryId = categoryId },
                    splitOn: "path"
                );
            });
        }
        

        PhotoAndCategory BuildPhotoAndCategory(dynamic item)
        {
            return new PhotoAndCategory {
                Photo = new Photo {
                    Id = item.photo_id,
                    CategoryId = item.category_id,
                    Latitude = (float?)item.latitude,
                    Longitude = (float?)item.longitude,
                    XsInfo = new PhotoInfo {
                        Path = item.xs_path,
                        Width = item.xs_width,
                        Height = item.xs_height
                    },
                    SmInfo = new PhotoInfo {
                        Path = item.sm_path,
                        Width = item.sm_width,
                        Height = item.sm_height
                    },
                    MdInfo = new PhotoInfo {
                        Path = item.md_path,
                        Width = item.md_width,
                        Height = item.md_height
                    },
                    LgInfo = new PhotoInfo {
                        Path = item.lg_path,
                        Width = item.lg_width,
                        Height = item.lg_height
                    },
                    PrtInfo = new PhotoInfo {
                        Path = item.prt_path,
                        Width = item.prt_width,
                        Height = item.prt_height
                    },
                },
                Category = new Category {
                    Id = item.category_id,
                    Name = item.name,
                    Year = item.year,
                    HasGpsData = item.has_gps_data,
                    TeaserPhotoInfo = new PhotoInfo {
                        Path = item.teaser_photo_path,
                        Width = item.teaser_photo_width,
                        Height = item.teaser_photo_height
                    }
                }
            };
        }
	}
}
