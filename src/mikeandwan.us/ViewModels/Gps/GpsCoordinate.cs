using System;


namespace MawMvcApp.ViewModels.Gps
{
    public class GpsCoordinate
    {
        public float LatitudeDegrees { get; private set; }
		public float LongitudeDegrees { get; private set; }
		public float LatitudeMinutes { get; private set; }
		public float LongitudeMinutes { get; private set; }
        public float LatitudeSeconds { get; private set; }
        public float LongitudeSeconds { get; private set; }
		
		
        public LatitudeReference LatitudeRef
        {
            get
            {
                if(LatitudeDegrees < 0.0f)
                {
                    return LatitudeReference.South;
                }

                return LatitudeReference.North;
            }
        }


        public LongitudeReference LongitudeRef
        {
            get
            {
                if(LongitudeDegrees < 0.0f)
                {
                    return LongitudeReference.West;
                }

                return LongitudeReference.East;
            }
        }


        public GpsCoordinate(float latDegrees, float latMinutes, float latSeconds, float lngDegrees, float lngMinutes, float lngSeconds)
        {
            LatitudeDegrees = latDegrees;
            LatitudeMinutes = latMinutes;
            LatitudeSeconds = latSeconds;

            LongitudeDegrees = lngDegrees;
            LongitudeMinutes = lngMinutes;
            LongitudeSeconds = lngSeconds;
        }


        public GpsCoordinate(float latDegrees, float latMinutes, float lngDegrees, float lngMinutes)
        {
			float degrees;
			float minutes;
			float seconds;
			
            GetDegreesMinutesSeconds(latDegrees, latMinutes, out degrees, out minutes, out seconds);
			
			LatitudeDegrees = degrees;
			LatitudeMinutes = minutes;
			LatitudeSeconds = seconds;
			
            GetDegreesMinutesSeconds(lngDegrees, lngMinutes, out degrees, out minutes, out seconds);
			
			LongitudeDegrees = degrees;
			LongitudeMinutes = minutes;
			LongitudeSeconds = seconds;
        }


        public GpsCoordinate(float latDegrees, float lngDegrees)
        {
			float degrees;
			float minutes;
			float seconds;
			
            GetDegreesMinutesSeconds(latDegrees, out degrees, out minutes, out seconds);
			
			LatitudeDegrees = degrees;
			LatitudeMinutes = minutes;
			LatitudeSeconds = seconds;
			
            GetDegreesMinutesSeconds(lngDegrees, out degrees, out minutes, out seconds);
			
			LongitudeDegrees = degrees;
			LongitudeMinutes = minutes;
			LongitudeSeconds = seconds;
        }


        public static float GetDegrees(float degrees, float minutes, float seconds)
        {
            float sign = 1.0f;

            if(degrees < 0.0f)
            {
                sign = -1.0f;
            }

            return sign * (Math.Abs(degrees) + (minutes / 60.0f) + (seconds / 3600.0f));
        }


        public static float GetDegrees(float degrees, float minutes)
        {
            float sign = 1.0f;

            if(degrees < 0.0f)
            {
                sign = -1.0f;
            }

            return sign * (Math.Abs(degrees) + (minutes / 60.0f));
        }


        public static void GetDegreesMinutes(float degreeDecimals, out float degrees, out float minutes)
        {
            degrees = (float) Math.Truncate((double) degreeDecimals);
            minutes = (Math.Abs(degreeDecimals) - Math.Abs(degrees)) * 60.0f;
        }


        public static void GetDegreesMinutes(float degreeDecimals, float degreeMinutes, float degreeSeconds, out float degrees, out float minutes)
        {
            degrees = degreeDecimals;
            minutes = Math.Abs(degreeMinutes) + (Math.Abs(degreeSeconds) / 60.0f);
        }


        public static void GetDegreesMinutesSeconds(float degreeDecimals, float minuteDecimals, out float degrees, out float minutes, out float seconds)
        {
            degrees = degreeDecimals;
            minutes = Math.Abs((float) Math.Truncate((double) minuteDecimals));
            seconds = (Math.Abs(minuteDecimals) - minutes) * 60.0f;
        }


        public static void GetDegreesMinutesSeconds(float degreeDecimals, out float degrees, out float minutes, out float seconds)
        {
            degrees = (float) Math.Truncate((double) degreeDecimals);

            float totalMinutes = (Math.Abs(degreeDecimals) - Math.Abs(degrees)) * 60.0f;

            minutes = (float) Math.Truncate((double) totalMinutes);
            seconds = (totalMinutes - minutes) * 60.0f;
        }


        public void GetLatitudeDegreesMinutes(out float degrees, out float minutes)
        {
            GetDegreesMinutes(LatitudeDegrees, LatitudeMinutes, LatitudeSeconds, out degrees, out minutes);
        }


        public void GetLongitudeDegreesMinutes(out float degrees, out float minutes)
        {
            GetDegreesMinutes(LongitudeDegrees, LongitudeMinutes, LongitudeSeconds, out degrees, out minutes);
        }


        public void GetLatitudeDegreesMinutesSeconds(out float degrees, out float minutes, out float seconds)
        {
            degrees = LatitudeDegrees;
            minutes = LatitudeMinutes;
            seconds = LatitudeSeconds;
        }


        public void GetLongitudeDegreesMinutesSeconds(out float degrees, out float minutes, out float seconds)
        {
            degrees = LongitudeDegrees;
            minutes = LongitudeMinutes;
            seconds = LongitudeSeconds;
        }


        public void GetLatitudeDegrees(out float degrees)
        {
            degrees = GetDegrees(LatitudeDegrees, LatitudeMinutes, LatitudeSeconds);
        }


        public float GetLatitudeDegrees()
        {
            return GetDegrees(LatitudeDegrees, LatitudeMinutes, LatitudeSeconds);
        }


        public void GetLongitudeDegrees(out float degrees)
        {
            degrees = GetDegrees(LongitudeDegrees, LongitudeMinutes, LongitudeSeconds);
        }


        public float GetLongitudeDegrees()
        {
            return GetDegrees(LongitudeDegrees, LongitudeMinutes, LongitudeSeconds);
        }


        public static GpsCoordinate Parse(string latitude, string latitudeRef, string longitude, string longitudeRef)
        {
            // parse values that look like:  42 deg 16' 13.80"
            float latDegrees, latMinutes, latSeconds;
            float lngDegrees, lngMinutes, lngSeconds;

            LatitudeReference latRef = ParseLatitudeRef(latitudeRef);
            LongitudeReference lngRef = ParseLongitudeRef(longitudeRef);
            ParseGpsCoordinate(latitude, out latDegrees, out latMinutes, out latSeconds);
            ParseGpsCoordinate(longitude, out lngDegrees, out lngMinutes, out lngSeconds);

            if(latRef == LatitudeReference.South)
            {
                latDegrees *= -1.0f;
            }
            if(lngRef == LongitudeReference.West)
            {
                lngDegrees *= -1.0f;
            }

            return new GpsCoordinate(latDegrees, latMinutes, latSeconds, lngDegrees, lngMinutes, lngSeconds);
        }


        public static LatitudeReference ParseLatitudeRef(string latitudeRef)
        {
            LatitudeReference latRef;

            if(string.Equals(latitudeRef, "north", StringComparison.OrdinalIgnoreCase))
            {
                latRef = LatitudeReference.North;
            }
            else
            {
                latRef = LatitudeReference.South;
            }

            return latRef;
        }


        public static LongitudeReference ParseLongitudeRef(string longitudeRef)
        {
            LongitudeReference lngRef;

            if(string.Equals(longitudeRef, "east", StringComparison.OrdinalIgnoreCase))
            {
                lngRef = LongitudeReference.East;
            }
            else
            {
                lngRef = LongitudeReference.West;
            }

            return lngRef;
        }


        public static void ParseGpsCoordinate(string coord, out float degrees, out float minutes, out float seconds)
        {
            string[] splitTerms = new string[] {" ", "'", "\"", "deg", "N", "S", "E", "W"};

            string[] parts = coord.Split(splitTerms, StringSplitOptions.RemoveEmptyEntries);

            if(parts.Length != 3)
            {
                throw new Exception("Expected to find deg, min, sec for the gps coord!");
            }

            degrees = float.Parse(parts[0]);
            minutes = float.Parse(parts[1]);
            seconds = float.Parse(parts[2]);
        }
    }
}
