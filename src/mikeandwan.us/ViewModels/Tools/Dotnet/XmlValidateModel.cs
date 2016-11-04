using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Text;
using System.Xml;

#if NET451
using System.Xml.Schema;
#endif

using Maw.Domain.Utilities;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace MawMvcApp.ViewModels.Tools.Dotnet
{
	public class XmlValidateModel
	{
		private StringBuilder Errors { get; set; }
		private int CurrErr { get; set; }
		
		
		[Required(ErrorMessage = "Please enter the XML source")]
		[Display(Name = "XML Source")]
		[DataType(DataType.MultilineText)]
		public string XmlSource { get; set; }
		
		[Display(Name = "Schema / DTD Source")]
		[DataType(DataType.MultilineText)]
		public string SchemaOrDtdSource { get; set; }
		
		[BindNever]
		public bool ValidationAttempted { get; set; }
		
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
				if(Errors == null || Errors.Length == 0)
				{
					return string.Empty;
				}
				
				return Errors.ToString();
			}
		}
		
		
		public void ValidateXml()
		{
#if NET451
			ValidationAttempted = true;
			Stream xmlStream = null;
			Stream xsdStream = null;			
			
			try
			{
				xmlStream = StreamUtils.ConvertStringToStream(XmlSource);
	
		        if(!string.IsNullOrEmpty(SchemaOrDtdSource))
		        {
		            xsdStream = StreamUtils.ConvertStringToStream(SchemaOrDtdSource);
		        }
				
				ValidateXml(xmlStream, xsdStream);
			}
			finally
			{
				if(xsdStream != null)
				{
					xsdStream.Close();
				}
				if(xmlStream != null)
				{
					xmlStream.Close();
				}
			}
#endif
		}
		

#if NET451		
		private void ValidateXml(Stream xmlStream, Stream xsdStream)
	    {
			XmlReader reader = null;
			Errors = new StringBuilder();
			CurrErr = 0;
			
	        try
	        {
	            XmlReaderSettings settings = new XmlReaderSettings();
	
	            if(xsdStream != null)
	            {
	                XmlSchema schema = XmlSchema.Read(xsdStream, ValidationHandler);
	                settings.Schemas.Add(schema);
	            }
	            else
	            {
	                settings.ValidationType = ValidationType.None;
	            }
	
	            reader = XmlReader.Create(xmlStream, settings);
	
	            // now that we have fully prepared the validating reader,
	            // read through all contents of the xml doc to validate
	            while(reader.Read())
	            {
					// do nothing
	            }
	        }
            catch(XmlSchemaException ex)
            {
                CurrErr++;
                Errors.Append(string.Concat("[", CurrErr, "] Error Parsing XML Schema\n"));
                Errors.Append(string.Concat("[", CurrErr, "] Line: ", ex.LineNumber, "\n"));
                Errors.Append(string.Concat("[", CurrErr, "] Position: ", ex.LinePosition, "\n"));
                Errors.Append(string.Concat("[", CurrErr, "] Message: ", ex.Message, "\n\n"));
            }
	        catch(XmlException ex)
	        {
	            CurrErr++;
				Errors.Append(string.Concat("[", CurrErr, "] Error Parsing XML\n"));
	            Errors.Append(string.Concat("[", CurrErr, "] Line: ", ex.LineNumber, "\n"));
	            Errors.Append(string.Concat("[", CurrErr, "] Position: ", ex.LinePosition, "\n"));
	            Errors.Append(string.Concat("[", CurrErr, "] Message: ", ex.Message, "\n\n"));
	        }
	        catch(Exception ex)
	        {
	            CurrErr++;
	            Errors.Append(string.Concat("[", CurrErr, "] Error Validating XML: ", ex.Message));
	        }
			finally
			{
				if(reader != null)
				{
					reader.Close();
				}
			}
	    }
	
	
	    private void ValidationHandler(object sender, ValidationEventArgs e)
	    {
	        CurrErr++;
	
	        Errors.Append(string.Concat("[", CurrErr, "] Error Validating XML\n"));
	        Errors.Append(string.Concat("[", CurrErr, "] Line: ", e.Exception.LineNumber, "\n"));
	        Errors.Append(string.Concat("[", CurrErr, "] Position: ", e.Exception.LinePosition, "\n"));
	        Errors.Append(string.Concat("[", CurrErr, "] Message: ", e.Exception.Message, "\n\n"));
	    }
#endif
	}
}
