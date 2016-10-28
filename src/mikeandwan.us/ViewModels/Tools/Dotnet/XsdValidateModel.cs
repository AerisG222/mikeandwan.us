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
	public class XsdValidateModel
	{
		private StringBuilder Errors { get; set; }
		private int CurrErr { get; set;  }
		
		[Required(ErrorMessage = "Please enter the XML schema source")]
		[Display(Name = "XML Schema Source")]
		[DataType(DataType.MultilineText)]
		public string XmlSchemaSource { get; set; }
		
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
		
		
		public void ValidateSchema()
		{
#if NET451
			ValidationAttempted = true;
			Errors = new StringBuilder();
			
			Stream xsdStream = null;
			XmlReader reader = null;
			
	        try
	        {
	            XmlReaderSettings settings = new XmlReaderSettings();
	
	            xsdStream = StreamUtils.ConvertStringToStream(XmlSchemaSource);
	
	            reader = XmlReader.Create(xsdStream, settings);
	
	            XmlSchema.Read(reader, ValidationHandler);
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
	
	            Errors.Append(string.Concat("[", CurrErr, "] Error Validating XSD:\n"));
	            Errors.Append(string.Concat("[", CurrErr, "] Line: ", ex.LineNumber, "\n"));
	            Errors.Append(string.Concat("[", CurrErr, "] Position: ", ex.LinePosition, "\n"));
	            Errors.Append(string.Concat("[", CurrErr, "] Message: ", ex.Message, "\n\n"));
	        }
			finally
			{
				if(reader != null)
				{
					reader.Close();
				}
				if(xsdStream != null)
				{
					xsdStream.Close();
				}
			}
#endif
	    }


#if NET451
	    private void ValidationHandler(object sender, ValidationEventArgs e)
	    {
	        CurrErr++;
	
	        Errors.Append(string.Concat("[", CurrErr, "] Error Validating XSD:\n"));
	        Errors.Append(string.Concat("[", CurrErr, "] Line: ", e.Exception.LineNumber, "\n"));
	        Errors.Append(string.Concat("[", CurrErr, "] Position: ", e.Exception.LinePosition, "\n"));
	        Errors.Append(string.Concat("[", CurrErr, "] Message: ", e.Exception.Message, "\n\n"));
	    }
#endif
	}
}
