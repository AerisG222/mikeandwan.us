using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Maw.Domain;
using Maw.Domain.Photos;


namespace Maw.Data
{
	public class PhotoRepository
        : Repository, IPhotoRepository
	{
        static readonly Type[] CATEGORY_PROJECTION_TYPES = new Type[] {
            typeof(Category),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo)
        };

        static readonly Type[] PHOTO_PROJECTION_TYPES = new Type[] {
            typeof(Photo),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo),
            typeof(MultimediaInfo),
        };

        const int MAX_RESULTS = 600;

        const string CATEGORY_PROJECTION = @"
            id,
            name,
            year,
            create_date,
            CASE WHEN gps_latitude_ref_id = 'S' THEN -1.0 * gps_latitude
                 ELSE gps_latitude
                  END AS latitude,
            CASE WHEN gps_longitude_ref_id = 'W' THEN -1.0 * gps_longitude
                 ELSE gps_longitude
                  END AS longitude,
            photo_count,
            total_size_xs,
            total_size_xs_sq,
            total_size_sm,
            total_size_md,
            total_size_lg,
            total_size_prt,
            total_size_src,
            COALESCE(total_size_xs, 0) +
                COALESCE(total_size_xs_sq, 0) +
                COALESCE(total_size_sm, 0) +
                COALESCE(total_size_md, 0) +
                COALESCE(total_size_lg, 0) +
                COALESCE(total_size_prt, 0) +
                COALESCE(total_size_src, 0) AS total_size,
            teaser_photo_path AS path,
            teaser_photo_height AS height,
            teaser_photo_width AS width,
            teaser_photo_size AS size,
            teaser_photo_sq_path AS path,
            teaser_photo_sq_height AS height,
            teaser_photo_sq_width AS width,
            teaser_photo_sq_size AS size";

        const string PHOTO_PROJECTION = @"
            id,
            category_id,
            create_date,
            CASE WHEN gps_latitude_ref_id = 'S' THEN -1.0 * gps_latitude
                 ELSE gps_latitude
                  END AS latitude,
            CASE WHEN gps_longitude_ref_id = 'W' THEN -1.0 * gps_longitude
                 ELSE gps_longitude
                  END AS longitude,
            xs_path AS path,
            xs_width AS width,
            xs_height AS height,
            xs_size AS size,
            xs_sq_path AS path,
            xs_sq_width AS width,
            xs_sq_height AS height,
            xs_sq_size AS size,
            sm_path AS path,
            sm_width AS width,
            sm_height AS height,
            sm_size AS size,
            md_path AS path,
            md_width AS width,
            md_height AS height,
            md_size AS size,
            lg_path AS path,
            lg_width AS width,
            lg_height AS height,
            lg_size AS size,
            prt_path AS path,
            prt_width AS width,
            prt_height AS height,
            prt_size AS size,
            src_path AS path,
            src_width AS width,
            src_height AS height,
            src_size AS size";


        public PhotoRepository(string connectionString)
            : base(connectionString)
        {

        }


        public Task<Photo> GetRandomAsync(bool allowPrivate)
        {
            return RunAsync(async conn => {
                var result = await conn.QueryAsync<Photo>(
                    $@"WITH random AS
                       (
                            SELECT id AS random_id
                              FROM photo.photo
                             WHERE (1 = @allowPrivate OR is_private = FALSE)
                             ORDER BY RANDOM()
                             LIMIT 1
                       )
                       SELECT {PHOTO_PROJECTION}
                         FROM random
                        INNER JOIN photo.photo p ON random.random_id = p.id;",
                    PHOTO_PROJECTION_TYPES,
                    (objects) => AssemblePhoto(objects),
                    new { allowPrivate = allowPrivate ? 1 : 0 },
                    splitOn: "path"
                ).ConfigureAwait(false);

                return result.First();
            });
        }


        public Task<IEnumerable<Photo>> GetRandomAsync(byte count, bool allowPrivate)
        {
            return RunAsync(conn => {
                return conn.QueryAsync<Photo>(
                    $@"WITH random AS
                       (
                            SELECT id AS random_id
                              FROM photo.photo
                             WHERE (1 = @allowPrivate OR is_private = FALSE)
                             ORDER BY RANDOM()
                             LIMIT {count}
                       )
                       SELECT {PHOTO_PROJECTION}
                         FROM random
                        INNER JOIN photo.photo p ON random.random_id = p.id;",
                    PHOTO_PROJECTION_TYPES,
                    (objects) => AssemblePhoto(objects),
                    new { allowPrivate = allowPrivate ? 1 : 0 },
                    splitOn: "path"
                );
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


        public Task<IEnumerable<Category>> GetAllCategoriesAsync(bool allowPrivate)
        {
            return RunAsync(conn => {
                return conn.QueryAsync<Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                         FROM photo.category
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                        ORDER BY id;",
                    CATEGORY_PROJECTION_TYPES,
                    (objects) => AssembleCategory(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0
                    },
                    splitOn: "path"
                );
            });
        }


        public Task<IEnumerable<Category>> GetCategoriesForYearAsync(short year, bool allowPrivate)
		{
            return RunAsync(conn => {
                return conn.QueryAsync<Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                         FROM photo.category
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND year = @year
                        ORDER BY id;",
                    CATEGORY_PROJECTION_TYPES,
                    (objects) => AssembleCategory(objects),
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
                return conn.QueryAsync<Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                        FROM photo.category
                       WHERE (1 = @allowPrivate OR is_private = FALSE)
                         AND id > @sinceId;",
                    CATEGORY_PROJECTION_TYPES,
                    (objects) => AssembleCategory(objects),
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
                return conn.QueryAsync<Photo>(
                    $@"SELECT {PHOTO_PROJECTION}
                         FROM photo.photo
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND category_id = @categoryId
                        ORDER BY lg_path;",
                    PHOTO_PROJECTION_TYPES,
                    (objects) => AssemblePhoto(objects),
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
                var result = await conn.QueryAsync<Category>(
                    $@"SELECT {CATEGORY_PROJECTION}
                         FROM photo.category
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND id = @categoryId;",
                    CATEGORY_PROJECTION_TYPES,
                    (objects) => AssembleCategory(objects),
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
                var result = await conn.QueryAsync<Photo>(
                    $@"SELECT {PHOTO_PROJECTION}
                         FROM photo.photo
                        WHERE (1 = @allowPrivate OR is_private = FALSE)
                          AND id = @photoId;",
                    PHOTO_PROJECTION_TYPES,
                    (objects) => AssemblePhoto(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        photoId = photoId
                    },
                    splitOn: "path"
                );

                return result.SingleOrDefault();
            });
		}


		public Task<Detail> GetDetailAsync(int photoId, bool allowPrivate)
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
                             shutter_speed
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


		public Task<IEnumerable<Comment>> GetCommentsAsync(int photoId)
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


		public Task<int> InsertCommentAsync(int photoId, string username, string comment)
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


		public Task<float?> SaveRatingAsync(int photoId, string username, byte rating)
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


		public Task<float?> RemoveRatingAsync(int photoId, string username)
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


        public Task<IEnumerable<Photo>> GetPhotosByCommentDateAsync(bool newestFirst, bool allowPrivate)
        {
            return RunAsync(conn => {
                var op = newestFirst ? "MAX" : "MIN";
                var sort = newestFirst ? "DESC" : "ASC";

                return conn.QueryAsync<Photo>(
                    $@"WITH comments AS
                       (
                            SELECT photo_id,
                                   {op}(entry_date) AS entry_date
                              FROM photo.comment
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_PROJECTION}
                        FROM comments
                       INNER JOIN photo.photo p ON comments.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       ORDER BY entry_date {sort}
                       LIMIT @limit;",
                    PHOTO_PROJECTION_TYPES,
                    (objects) => AssemblePhoto(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    },
                    splitOn: "path"
                );
            });
        }


        public Task<IEnumerable<Photo>> GetPhotosByUserCommentDateAsync(string username, bool newestFirst, bool allowPrivate)
        {
            return RunAsync(conn => {
                var op = newestFirst ? "MAX" : "MIN";
                var sort = newestFirst ? "DESC" : "ASC";

                return conn.QueryAsync<Photo>(
                    $@"WITH comments AS
                       (
                            SELECT photo_id,
                                   {op}(entry_date) AS entry_date
                              FROM photo.comment c
                             INNER JOIN maw.user u ON c.user_id = u.id AND u.username = @username
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_PROJECTION}
                        FROM comments
                       INNER JOIN photo.photo p ON comments.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       ORDER BY entry_date {sort}
                       LIMIT @limit;",
                    PHOTO_PROJECTION_TYPES,
                    (objects) => AssemblePhoto(objects),
                    new {
                        username = @username.ToLower(),
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    },
                    splitOn: "path"
                );
            });
        }


        public Task<IEnumerable<Photo>> GetPhotosByCommentCountAsync(bool greatestFirst, bool allowPrivate)
        {
            return RunAsync(conn => {
                var sort = greatestFirst ? "DESC" : "ASC";

                return conn.QueryAsync<Photo>(
                    $@"WITH comments AS
                       (
                            SELECT photo_id,
                                   COUNT(1) AS comment_count
                              FROM photo.comment
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_PROJECTION}
                        FROM comments
                       INNER JOIN photo.photo p ON comments.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       ORDER BY comment_count {sort}
                       LIMIT @limit;",
                    PHOTO_PROJECTION_TYPES,
                    (objects) => AssemblePhoto(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    },
                    splitOn: "path"
                );
            });
        }


        public Task<IEnumerable<Photo>> GetPhotosByAverageUserRatingAsync(bool highestFirst, bool allowPrivate)
        {
            return RunAsync(conn => {
                var sort = highestFirst ? "DESC" : "ASC";

                return conn.QueryAsync<Photo>(
                    $@"WITH ratings AS
                       (
                            SELECT photo_id,
                                   AVG(score) AS avg_score
                              FROM photo.rating
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_PROJECTION}
                        FROM ratings
                       INNER JOIN photo.photo p ON ratings.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       ORDER BY avg_score {sort}
                       LIMIT @limit;",
                    PHOTO_PROJECTION_TYPES,
                    (objects) => AssemblePhoto(objects),
                    new {
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    },
                    splitOn: "path"
                );
            });
        }


		public Task<IEnumerable<Photo>> GetPhotosByUserRatingAsync(string username, bool highestFirst, bool allowPrivate)
		{
            return RunAsync(conn => {
                var sort = highestFirst ? "DESC" : "ASC";

                return conn.QueryAsync<Photo>(
                    $@"WITH ratings AS
                       (
                            SELECT photo_id,
                                   AVG(score) AS avg_score
                              FROM photo.rating r
                             INNER JOIN maw.user u ON r.user_id = u.id AND u.username = @username
                             GROUP BY photo_id
                      )
                      SELECT {PHOTO_PROJECTION}
                        FROM ratings
                       INNER JOIN photo.photo p ON ratings.photo_id = p.id AND (1 = @allowPrivate OR is_private = FALSE)
                       ORDER BY avg_score {sort}
                       LIMIT @limit;",
                    PHOTO_PROJECTION_TYPES,
                    (objects) => AssemblePhoto(objects),
                    new {
                        username = username.ToLower(),
                        allowPrivate = allowPrivate ? 1 : 0,
                        limit = MAX_RESULTS
                    },
                    splitOn: "path"
                );
            });
		}


        Category AssembleCategory(object[] objects)
        {
            var category = (Category) objects[0];

            category.TeaserImage = (MultimediaInfo) objects[1];
            category.TeaserImageSq = (MultimediaInfo) objects[2];

            return category;
        }


        Photo AssemblePhoto(object[] objects)
        {
            var photo = (Photo) objects[0];

            photo.XsInfo = (MultimediaInfo) objects[1];
            photo.XsSqInfo = (MultimediaInfo) objects[2];
            photo.SmInfo = (MultimediaInfo) objects[3];
            photo.MdInfo = (MultimediaInfo) objects[4];
            photo.LgInfo = (MultimediaInfo) objects[5];
            photo.PrtInfo = (MultimediaInfo) objects[6];
            photo.SrcInfo = (MultimediaInfo) objects[7];

            return photo;
        }
	}
}
