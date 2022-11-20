using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Maw.Domain.Utilities;

namespace MawMvcApp.ViewModels.Tools.Dotnet;

public class XslTransformModel
{
    private readonly StringBuilder _errors = new();

    [Required(ErrorMessage = "Please enter the XML source")]
    [Display(Name = "XML Source")]
    [DataType(DataType.MultilineText)]
    public string XmlSource { get; set; } = null!;

    [Required(ErrorMessage = "Please enter the XSLT source")]
    [Display(Name = "XSLT Source")]
    [DataType(DataType.MultilineText)]
    public string XsltSource { get; set; } = null!;

    [BindNever]
    public bool AttemptedTransform { get; set; }

    [BindNever]
    public string? TransformResult { get; set; }

    [BindNever]
    public bool HasErrors { get; set; }

    [BindNever]
    public bool AreErrors
    {
        get
        {
            return _errors != null && _errors.Length > 0;
        }
    }

    [BindNever]
    public string TransformErrors
    {
        get
        {
            if (_errors == null || _errors.Length == 0)
            {
                return string.Empty;
            }

            return _errors.ToString();
        }
    }

    public void ExecuteTransform()
    {
        AttemptedTransform = true;

        if (string.IsNullOrEmpty(XmlSource))
        {
            throw new InvalidOperationException("XmlSource must be specified prior to execution.");
        }
        if (string.IsNullOrEmpty(XsltSource))
        {
            throw new InvalidOperationException("XsltSource must be specified prior to execution.");
        }

        int currErr = 0;
        Stream xmlStream = StreamUtils.ConvertStringToStream(XmlSource);
        Stream xslStream = StreamUtils.ConvertStringToStream(XsltSource);

        XmlReader? xmlReader = null;
        XmlReader? xslReader = null;
        MemoryStream? ms = null;
        XmlWriter? xmlWriter = null;
        TextReader? tr = null;

        try
        {
            var settings = new XmlReaderSettings();

            xmlReader = XmlReader.Create(xmlStream, settings);
            xslReader = XmlReader.Create(xslStream, settings);

            var xslTransform = new XslCompiledTransform(true);
            xslTransform.Load(xslReader);

            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true,
                NewLineHandling = NewLineHandling.Replace
            };

            ms = new MemoryStream();
            xmlWriter = XmlWriter.Create(ms);

            xslTransform.Transform(xmlReader, xmlWriter);

            ms.Seek(0, SeekOrigin.Begin);
            tr = new StreamReader(ms);

            TransformResult = tr.ReadToEnd();
        }
        catch (XsltException ex)
        {
            currErr++;
            _errors.Append(string.Concat("[", currErr, "] Error Executing XSLT:\n"));
            _errors.Append(string.Concat("[", currErr, "] ", ex.Source, "\n"));
            _errors.Append(string.Concat("[", currErr, "] Line: ", ex.LineNumber, "\n"));
            _errors.Append(string.Concat("[", currErr, "] Position: ", ex.LinePosition, "\n"));
            _errors.Append(string.Concat("[", currErr, "] Message: ", ex.Message, "\n\n"));
        }
        catch (XmlException ex)
        {
            currErr++;

            _errors.Append(string.Concat("[", currErr, "] Error Parsing XSL:\n"));
            _errors.Append(string.Concat("[", currErr, "] ", ex.Source, "\n"));
            _errors.Append(string.Concat("[", currErr, "] Line: ", ex.LineNumber, "\n"));
            _errors.Append(string.Concat("[", currErr, "] Position: ", ex.LinePosition, "\n"));
            _errors.Append(string.Concat("[", currErr, "] Message: ", ex.Message, "\n\n"));
        }
        catch (Exception ex)
        {
            currErr++;

            _errors.Append(string.Concat("[", currErr, "] Error Executing transform: ", ex.Message, "\n"));
        }
        finally
        {
            xslReader?.Close();
            xmlReader?.Close();
            xmlWriter?.Close();
            ms?.Close();
            tr?.Close();
        }
    }
}
