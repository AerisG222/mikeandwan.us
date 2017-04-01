using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

#if NET451
using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Maw.Domain.Utilities;
#endif


namespace MawMvcApp.ViewModels.Tools.Dotnet
{
	public class XslTransformModel
	{
		private StringBuilder Errors { get; set; }
		
		[Required(ErrorMessage = "Please enter the XML source")]
		[Display(Name = "XML Source")]
		[DataType(DataType.MultilineText)]
		public string XmlSource { get; set; }
		
		[Required(ErrorMessage = "Please enter the XSLT source")]
		[Display(Name = "XSLT Source")]
		[DataType(DataType.MultilineText)]
		public string XsltSource { get; set; }
		
		[BindNever]
		public bool AttemptedTransform { get; set; }
		
		[BindNever]
		public string TransformResult { get; set; }
		
		[BindNever]
		public bool AreErrors 
		{ 
			get
			{
				return Errors != null && Errors.Length > 0;
			}
		}
		
		[BindNever]
		public string TransformErrors
		{
			get
			{
				if(Errors == null || Errors.Length == 0)
				{
					return string.Empty;
				}
				
				return Errors.ToString();
			}
		}
		
		
		public void ExecuteTransform()
		{
#if NET451
			AttemptedTransform = true;
			
	        if(string.IsNullOrEmpty(XmlSource))
	        {
				throw new InvalidOperationException("XmlSource must be specified prior to execution.");
	        }
			if(string.IsNullOrEmpty(XsltSource))
			{
				throw new InvalidOperationException("XsltSource must be specified prior to execution.");
			}

			int currErr = 0;
			Errors = new StringBuilder();
	        Stream xmlStream = StreamUtils.ConvertStringToStream(XmlSource);
	        Stream xslStream = StreamUtils.ConvertStringToStream(XsltSource);
	
			XmlReader xmlReader = null;
			XmlReader xslReader = null;
			MemoryStream ms = null;
			XmlWriter xmlWriter = null;
			TextReader tr = null;
			
	        try
	        {
				XmlReaderSettings settings = new XmlReaderSettings();
				
	            xmlReader = XmlReader.Create(xmlStream, settings);
	            xslReader = XmlReader.Create(xslStream, settings);
	
	            XslCompiledTransform xslTransform = new XslCompiledTransform(true);
	            xslTransform.Load(xslReader);
				
	            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
	            xmlWriterSettings.Indent = true;
	            xmlWriterSettings.NewLineHandling = NewLineHandling.Replace;
	
	            ms = new MemoryStream();
	            xmlWriter = XmlWriter.Create(ms);
	
	            xslTransform.Transform(xmlReader, xmlWriter);
	
	            ms.Seek(0, SeekOrigin.Begin);
	            tr = new StreamReader(ms);
	
	            TransformResult = tr.ReadToEnd();
	        }
            catch(XsltException ex)
            {
                currErr++;
                Errors.Append(string.Concat("[", currErr, "] Error Executing XSLT:\n"));
                Errors.Append(string.Concat("[", currErr, "] ", ex.Source, "\n"));
                Errors.Append(string.Concat("[", currErr, "] Line: ", ex.LineNumber, "\n"));
                Errors.Append(string.Concat("[", currErr, "] Position: ", ex.LinePosition, "\n"));
                Errors.Append(string.Concat("[", currErr, "] Message: ", ex.Message, "\n\n"));
            }
	        catch(XmlException ex)
	        {
	            currErr++;
	
	            Errors.Append(string.Concat("[", currErr, "] Error Parsing XSL:\n"));
	            Errors.Append(string.Concat("[", currErr, "] ", ex.Source, "\n"));
	            Errors.Append(string.Concat("[", currErr, "] Line: ", ex.LineNumber, "\n"));
	            Errors.Append(string.Concat("[", currErr, "] Position: ", ex.LinePosition, "\n"));
	            Errors.Append(string.Concat("[", currErr, "] Message: ", ex.Message, "\n\n"));
	        }
	        catch(Exception ex)
	        {
	            currErr++;
	
	            Errors.Append(string.Concat("[", currErr, "] Error Executing transform: ", ex.Message, "\n"));
	        }
			finally
			{
				if(xslReader != null)
				{
					xslReader.Close();
				}
				if(xmlReader != null)
				{
					xmlReader.Close();
				}
				if(xmlWriter != null)
				{
					xmlWriter.Close();
				}
				if(ms != null)
				{
					ms.Close();
				}
				if(tr != null)
				{
					tr.Close();
				}
			}
#endif
		}
	}
}
