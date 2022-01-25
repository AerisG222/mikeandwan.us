using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace MawMvcApp.ViewModels.Gps;

public class GpsConversionModel
    : IValidatableObject
{
    public float? DmsLatDegrees { get; set; }
    public float? DmsLatMinutes { get; set; }
    public float? DmsLatSeconds { get; set; }
    public string? DmsLatString { get; set; }
    public LatitudeReference? DmsLatReference { get; set; }
    public float? DmsLngDegrees { get; set; }
    public float? DmsLngMinutes { get; set; }
    public float? DmsLngSeconds { get; set; }
    public string? DmsLngString { get; set; }
    public LongitudeReference? DmsLngReference { get; set; }

    public float? DmLatDegrees { get; set; }
    public float? DmLatMinutes { get; set; }
    public string? DmLatString { get; set; }
    public LatitudeReference? DmLatReference { get; set; }
    public float? DmLngDegrees { get; set; }
    public float? DmLngMinutes { get; set; }
    public string? DmLngString { get; set; }
    public LongitudeReference? DmLngReference { get; set; }

    public float? DLatDegrees { get; set; }
    public string? DLatString { get; set; }
    public LatitudeReference? DLatReference { get; set; }
    public float? DLngDegrees { get; set; }
    public string? DLngString { get; set; }
    public LongitudeReference? DLngReference { get; set; }

    public GpsConversionMode? ConversionMode { get; set; }

    public void Convert()
    {
        GpsCoordinate coord = ConversionMode switch
        {
            GpsConversionMode.FromDegrees => new GpsCoordinate(
                GetLatitudeDegrees(DLatDegrees, DLatReference),
                GetLongitudeDegrees(DLngDegrees, DLngReference)
            ),
            GpsConversionMode.FromDegreesMinutes => new GpsCoordinate(
                GetLatitudeDegrees(DmLatDegrees, DmLatReference), GetFloat(DmLatMinutes),
                GetLongitudeDegrees(DmLngDegrees, DmLngReference), GetFloat(DmLngMinutes)
            ),
            GpsConversionMode.FromDegreesMinutesSeconds => new GpsCoordinate(
                GetLatitudeDegrees(DmsLatDegrees, DmsLatReference), GetFloat(DmsLatMinutes), GetFloat(DmsLatSeconds),
                GetLongitudeDegrees(DmsLngDegrees, DmsLngReference), GetFloat(DmsLngMinutes), GetFloat(DmsLngSeconds)
            ),
            _ => throw new InvalidOperationException($"Specified conversion mode [{ ConversionMode }] is not supported")
        };

        // prepare degree portion
        coord.GetLatitudeDegrees(out float degrees);
        DLatDegrees = Math.Abs(degrees);
        DLatReference = coord.LatitudeRef;
        DLatString = $"{Math.Abs(degrees):f4} deg {coord.LatitudeRef}";

        coord.GetLongitudeDegrees(out degrees);
        DLngDegrees = Math.Abs(degrees);
        DLngReference = coord.LongitudeRef;
        DLngString = $"{Math.Abs(degrees):f4} deg {coord.LongitudeRef}";

        // prepare degree minute portion
        coord.GetLatitudeDegreesMinutes(out degrees, out float minutes);
        DmLatDegrees = Math.Abs(degrees);
        DmLatMinutes = minutes;
        DmLatReference = coord.LatitudeRef;
        DmLatString = $"{Math.Abs(degrees)} deg {minutes:f4}' {coord.LatitudeRef}";

        coord.GetLongitudeDegreesMinutes(out degrees, out minutes);
        DmLngDegrees = Math.Abs(degrees);
        DmLngMinutes = minutes;
        DmLngReference = coord.LongitudeRef;
        DmLngString = $"{Math.Abs(degrees)} deg {minutes:f4}' {coord.LongitudeRef}";

        // prepare degree minute seconds portion
        coord.GetLatitudeDegreesMinutesSeconds(out degrees, out minutes, out float seconds);
        DmsLatDegrees = Math.Abs(degrees);
        DmsLatMinutes = minutes;
        DmsLatSeconds = seconds;
        DmsLatReference = coord.LatitudeRef;
        DmsLatString = $"{Math.Abs(degrees)} deg {minutes}' {seconds:f4}\" {coord.LatitudeRef}";

        coord.GetLongitudeDegreesMinutesSeconds(out degrees, out minutes, out seconds);
        DmsLngDegrees = Math.Abs(degrees);
        DmsLngMinutes = minutes;
        DmsLngSeconds = seconds;
        DmsLngReference = coord.LongitudeRef;
        DmsLngString = $"{Math.Abs(degrees)} deg {minutes}' {seconds:f4}\" {coord.LongitudeRef}";
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var resultList = new List<ValidationResult>();

        switch (ConversionMode)
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
        if (val == null)
        {
            resultList.Add(new ValidationResult(string.Concat(fieldDescription, " must be specified."), new string[] { fieldName }));
        }
        else
        {
            float fval = (float)val;

            if (fval < min || fval > max)
            {
                resultList.Add(new ValidationResult(string.Concat(fieldDescription, " must be within the range of [", min.ToString("N1", CultureInfo.InvariantCulture), ", ", max.ToString("N1", CultureInfo.InvariantCulture), "]."), new string[] { fieldName }));
            }
        }
    }

    float GetLatitudeDegrees(float? degree, LatitudeReference? latRef)
    {
        if (degree == null)
        {
            throw new ArgumentNullException(nameof(degree));
        }

        if (latRef == null)
        {
            throw new ArgumentNullException(nameof(latRef));
        }

        if (latRef == LatitudeReference.South)
        {
            return (float)degree * -1.0f;
        }

        return (float)degree;
    }

    float GetLongitudeDegrees(float? degree, LongitudeReference? lngRef)
    {
        if (degree == null)
        {
            throw new ArgumentNullException(nameof(degree));
        }

        if (lngRef == null)
        {
            throw new ArgumentNullException(nameof(lngRef));
        }

        if (lngRef == LongitudeReference.West)
        {
            return (float)degree * -1.0f;
        }

        return (float)degree;
    }

    float GetFloat([NotNull] float? val)
    {
        return val ?? throw new ArgumentNullException(nameof(val));
    }
}
