using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Xml.Schema;
using System.Xml;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Maw.Domain.Utilities;

namespace MawMvcApp.ViewModels.Tools.Dotnet;

public class XmlValidateModel
{
    private readonly StringBuilder _errors = new();
    private int CurrErr { get; set; }

    [Required(ErrorMessage = "Please enter the XML source")]
    [Display(Name = "XML Source")]
    [DataType(DataType.MultilineText)]
    public string XmlSource { get; set; } = null!;

    [Display(Name = "Schema / DTD Source")]
    [DataType(DataType.MultilineText)]
    public string? SchemaOrDtdSource { get; set; }

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

    public void ValidateXml()
    {
        ValidationAttempted = true;
        Stream? xmlStream = null;
        Stream? xsdStream = null;

        try
        {
            xmlStream = StreamUtils.ConvertStringToStream(XmlSource);

            if (!string.IsNullOrEmpty(SchemaOrDtdSource))
            {
                xsdStream = StreamUtils.ConvertStringToStream(SchemaOrDtdSource);
            }

            ValidateXml(xmlStream, xsdStream);
        }
        finally
        {
            if (xsdStream != null)
            {
                xsdStream.Close();
            }
            if (xmlStream != null)
            {
                xmlStream.Close();
            }
        }
    }

    void ValidateXml(Stream xmlStream, Stream? xsdStream)
    {
        XmlReader? reader = null;
        XmlReader? schemaReader = null;
        CurrErr = 0;

        try
        {
            var settings = new XmlReaderSettings();

            if (xsdStream != null)
            {
                schemaReader = XmlReader.Create(xsdStream);
                var schema = XmlSchema.Read(schemaReader, ValidationHandler);

                if(schema != null)
                {
                    settings.Schemas.Add(schema);
                }
            }
            else
            {
                settings.ValidationType = ValidationType.None;
            }

            reader = XmlReader.Create(xmlStream, settings);

            // now that we have fully prepared the validating reader,
            // read through all contents of the xml doc to validate
            while (reader.Read())
            {
                // do nothing
            }
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
            _errors.Append(string.Concat("[", CurrErr, "] Error Parsing XML\n"));
            _errors.Append(string.Concat("[", CurrErr, "] Line: ", ex.LineNumber, "\n"));
            _errors.Append(string.Concat("[", CurrErr, "] Position: ", ex.LinePosition, "\n"));
            _errors.Append(string.Concat("[", CurrErr, "] Message: ", ex.Message, "\n\n"));
        }
        catch (Exception ex)
        {
            CurrErr++;
            _errors.Append(string.Concat("[", CurrErr, "] Error Validating XML: ", ex.Message));
        }
        finally
        {
            if (schemaReader != null)
            {
                schemaReader.Close();
            }

            if (reader != null)
            {
                reader.Close();
            }
        }
    }

    void ValidationHandler(object? sender, ValidationEventArgs e)
    {
        CurrErr++;

        _errors.Append(string.Concat("[", CurrErr, "] Error Validating XML\n"));
        _errors.Append(string.Concat("[", CurrErr, "] Line: ", e.Exception.LineNumber, "\n"));
        _errors.Append(string.Concat("[", CurrErr, "] Position: ", e.Exception.LinePosition, "\n"));
        _errors.Append(string.Concat("[", CurrErr, "] Message: ", e.Exception.Message, "\n\n"));
    }
}
