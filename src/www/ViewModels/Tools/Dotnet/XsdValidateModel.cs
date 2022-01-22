using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Maw.Domain.Utilities;

namespace MawMvcApp.ViewModels.Tools.Dotnet;

public class XsdValidateModel
{
    private readonly StringBuilder _errors = new();
    private int CurrErr { get; set; }

    [Required(ErrorMessage = "Please enter the XML schema source")]
    [Display(Name = "XML Schema Source")]
    [DataType(DataType.MultilineText)]
    public string XmlSchemaSource { get; set; } = null!;

    [BindNever]
    public bool ValidationAttempted { get; set; }

    [BindNever]
    public bool HasErrors { get; set; }

    [BindNever]
    public bool AreErrors
    {
        get
        {
            return CurrErr > 0;
        }
    }

    [BindNever]
    public string ValidationErrors
    {
        get
        {
            if (_errors.Length == 0)
            {
                return string.Empty;
            }

            return _errors.ToString();
        }
    }

    public void ValidateSchema()
    {
        ValidationAttempted = true;
        Stream? xsdStream = null;
        XmlReader? reader = null;

        try
        {
            XmlReaderSettings settings = new XmlReaderSettings();

            xsdStream = StreamUtils.ConvertStringToStream(XmlSchemaSource);

            reader = XmlReader.Create(xsdStream, settings);

            XmlSchema.Read(reader, ValidationHandler);
        }
        catch (XmlSchemaException ex)
        {
            CurrErr++;
            _errors.Append(string.Concat("[", CurrErr, "] Error Parsing XML Schema\n"));
            _errors.Append(string.Concat("[", CurrErr, "] Line: ", ex.LineNumber, "\n"));
            _errors.Append(string.Concat("[", CurrErr, "] Position: ", ex.LinePosition, "\n"));
            _errors.Append(string.Concat("[", CurrErr, "] Message: ", ex.Message, "\n\n"));
        }
        catch (XmlException ex)
        {
            CurrErr++;

            _errors.Append(string.Concat("[", CurrErr, "] Error Validating XSD:\n"));
            _errors.Append(string.Concat("[", CurrErr, "] Line: ", ex.LineNumber, "\n"));
            _errors.Append(string.Concat("[", CurrErr, "] Position: ", ex.LinePosition, "\n"));
            _errors.Append(string.Concat("[", CurrErr, "] Message: ", ex.Message, "\n\n"));
        }
        finally
        {
            if (reader != null)
            {
                reader.Close();
            }
            if (xsdStream != null)
            {
                xsdStream.Close();
            }
        }
    }

    void ValidationHandler(object? sender, ValidationEventArgs e)
    {
        CurrErr++;

        _errors.Append(string.Concat("[", CurrErr, "] Error Validating XSD:\n"));
        _errors.Append(string.Concat("[", CurrErr, "] Line: ", e.Exception.LineNumber, "\n"));
        _errors.Append(string.Concat("[", CurrErr, "] Position: ", e.Exception.LinePosition, "\n"));
        _errors.Append(string.Concat("[", CurrErr, "] Message: ", e.Exception.Message, "\n\n"));
    }
}
