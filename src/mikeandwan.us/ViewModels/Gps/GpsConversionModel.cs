using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace MawMvcApp.ViewModels.Gps
{
	public class GpsConversionModel
		: IValidatableObject
	{
		public float? DmsLatDegrees { get; set; }
		public float? DmsLatMinutes { get; set; }
		public float? DmsLatSeconds { get; set; }
		public string DmsLatString { get; set; }
		public LatitudeReference? DmsLatReference { get; set; }
		public float? DmsLngDegrees { get; set; }
		public float? DmsLngMinutes { get; set; }
		public float? DmsLngSeconds { get; set; }
		public string DmsLngString { get; set; }
		public LongitudeReference? DmsLngReference { get; set; }
		
		public float? DmLatDegrees { get; set; }
		public float? DmLatMinutes { get; set; }
		public string DmLatString { get; set; }
		public LatitudeReference? DmLatReference { get; set; }
		public float? DmLngDegrees { get; set; }
		public float? DmLngMinutes { get; set; }
		public string DmLngString { get; set; }
		public LongitudeReference? DmLngReference { get; set; }

		public float? DLatDegrees { get; set; }
		public string DLatString { get; set; }
		public LatitudeReference? DLatReference { get; set; }
		public float? DLngDegrees { get; set; }
		public string DLngString { get; set; }
		public LongitudeReference? DLngReference { get; set; }
		
		public GpsConversionMode ConversionMode { get; set; }


		public void Convert()
		{
			GpsCoordinate coord = null;
			
			switch(ConversionMode)
			{
				case GpsConversionMode.FromDegrees:
					coord = new GpsCoordinate(GetLatitudeDegrees((float)DLatDegrees, (LatitudeReference) DLatReference), GetLongitudeDegrees((float)DLngDegrees, (LongitudeReference) DLngReference));
					break;
				case GpsConversionMode.FromDegreesMinutes:
					coord = new GpsCoordinate(GetLatitudeDegrees((float)DmLatDegrees, (LatitudeReference) DmLatReference), (float)DmLatMinutes, GetLongitudeDegrees((float)DmLngDegrees, (LongitudeReference) DmLngReference), (float)DmLngMinutes);
					break;
				case GpsConversionMode.FromDegreesMinutesSeconds:
					coord = new GpsCoordinate(GetLatitudeDegrees((float)DmsLatDegrees, (LatitudeReference) DmsLatReference), (float)DmsLatMinutes, (float)DmsLatSeconds, GetLongitudeDegrees((float)DmsLngDegrees, (LongitudeReference) DmsLngReference), (float)DmsLngMinutes, (float)DmsLngSeconds);
					break;
			}
			
			float degrees;
			float minutes;
			float seconds;
			
			// prepare degree portion
			coord.GetLatitudeDegrees(out degrees);
			DLatDegrees = Math.Abs(degrees);
			DLatReference = coord.LatitudeRef;
			DLatString = string.Format("{0:f4} deg {1}", Math.Abs(degrees), coord.LatitudeRef.ToString());
			
			coord.GetLongitudeDegrees(out degrees);
			DLngDegrees = Math.Abs(degrees);
			DLngReference = coord.LongitudeRef;
			DLngString = string.Format("{0:f4} deg {1}", Math.Abs(degrees), coord.LongitudeRef.ToString());
			
			// prepare degree minute portion
			coord.GetLatitudeDegreesMinutes(out degrees, out minutes);
			DmLatDegrees = Math.Abs(degrees);
			DmLatMinutes = minutes;
			DmLatReference = coord.LatitudeRef;
			DmLatString = string.Format("{0} deg {1:f4}' {2}", Math.Abs(degrees), minutes, coord.LatitudeRef.ToString());
			
			coord.GetLongitudeDegreesMinutes(out degrees, out minutes);
			DmLngDegrees = Math.Abs(degrees);
			DmLngMinutes = minutes;
			DmLngReference = coord.LongitudeRef;
			DmLngString = string.Format("{0} deg {1:f4}' {2}", Math.Abs(degrees), minutes, coord.LongitudeRef.ToString());
			
			// prepare degree minute seconds portion
			coord.GetLatitudeDegreesMinutesSeconds(out degrees, out minutes, out seconds);
			DmsLatDegrees = Math.Abs(degrees);
			DmsLatMinutes = minutes;
			DmsLatSeconds = seconds;
			DmsLatReference = coord.LatitudeRef;
			DmsLatString = string.Format("{0} deg {1}' {2:f4}\" {3}", Math.Abs(degrees), minutes, seconds, coord.LatitudeRef.ToString());
			
			coord.GetLongitudeDegreesMinutesSeconds(out degrees, out minutes, out seconds);
			DmsLngDegrees = Math.Abs(degrees);
			DmsLngMinutes = minutes;
			DmsLngSeconds = seconds;
			DmsLngReference = coord.LongitudeRef;
			DmsLngString = string.Format("{0} deg {1}' {2:f4}\" {3}", Math.Abs(degrees), minutes, seconds, coord.LongitudeRef.ToString());
		}
		
		
		public IEnumerable<ValidationResult> Validate(ValidationContext context)
		{
			var resultList = new List<ValidationResult>();
			
			switch(ConversionMode)
			{
				case GpsConversionMode.FromDegreesMinutesSeconds:
					ValidateDegrees(resultList, DmsLatDegrees, "DmsLatDegrees", "Latitude Degrees");
					ValidateMinutesSeconds(resultList, DmsLatMinutes, "DmsLatMinutes", "Latitude Minutes");
					ValidateMinutesSeconds(resultList, DmsLatSeconds, "DmsLatSeconds", "Latitude Seconds");
					ValidateDegrees(resultList, DmsLngDegrees, "DmsLngDegrees", "Longitude Degrees");
					ValidateMinutesSeconds(resultList, DmsLngMinutes, "DmsLngMinutes", "Longitude Minutes");
					ValidateMinutesSeconds(resultList, DmsLngSeconds, "DmsLngSeconds", "Longitude Seconds");
					break;
				case GpsConversionMode.FromDegreesMinutes:
					ValidateDegrees(resultList, DmLatDegrees, "DmLatDegrees", "Latitude Degrees");
					ValidateMinutesSeconds(resultList, DmLatMinutes, "DmLatMinutes", "Latitude Minutes");
					ValidateDegrees(resultList, DmLngDegrees, "DmLngDegrees", "Longitude Degrees");
					ValidateMinutesSeconds(resultList, DmLngMinutes, "DmLngMinutes", "Longitude Minutes");
					break;
				case GpsConversionMode.FromDegrees:
					ValidateDegrees(resultList, DLatDegrees, "DLatDegrees", "Latitude Degrees");
					ValidateDegrees(resultList, DLngDegrees, "DLngDegrees", "Longitude Degrees");
					break;
				case GpsConversionMode.None:
					resultList.Add(new ValidationResult("An invalid conversion mode was specified!"));
					break;
			}
			
			return resultList;
		}
		
		
		void ValidateMinutesSeconds(IList<ValidationResult> resultList, float? val, string fieldName, string fieldDescription)
		{
			ValidateFloat(resultList, val, fieldName, fieldDescription, 0, 60);
		}
		
		
		void ValidateDegrees(IList<ValidationResult> resultList, float? val, string fieldName, string fieldDescription)
		{
			ValidateFloat(resultList, val, fieldName, fieldDescription, 0, 180);
		}
		
		
		void ValidateFloat(IList<ValidationResult> resultList, float? val, string fieldName, string fieldDescription, float min, float max)
		{
			if(val == null)
			{
				resultList.Add(new ValidationResult(string.Concat(fieldDescription, " must be specified."), new string[] {fieldName}));
			}
			else
			{
				float fval = (float)val;
				
				if(fval < min || fval > max)
				{
					resultList.Add(new ValidationResult(string.Concat(fieldDescription, " must be within the range of [", min.ToString("N1"), ", ", max.ToString("N1"), "]."), new string[] {fieldName}));
				}
			}
		}
		
		
		float GetLatitudeDegrees(float degree, LatitudeReference latRef)
		{
			if(latRef == LatitudeReference.South)
			{
				return degree * -1.0f;
			}
			
			return degree;
		}
		
		
		float GetLongitudeDegrees(float degree, LongitudeReference lngRef)
		{
			if(lngRef == LongitudeReference.West)
			{
				return degree * -1.0f;
			}
			
			return degree;
		}
	}
}
