using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Medallion.Shell;


namespace Maw.Domain.Utilities
{
    public class LinuxFileTypeIdentifier
    {
        readonly ILogger _log;


        public LinuxFileTypeIdentifier(ILogger<LinuxFileTypeIdentifier> log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }


        public string GetMimeType(string filePath)
        {
            if(string.IsNullOrWhiteSpace(filePath))
            {
                return null;
            }

            if(!File.Exists(filePath))
            {
                return null;
            }

            try
            {
                var cmd = Command.Run("file", new string[] {
                    "--brief",
                    "--mime-type",
                    filePath
                });

                var mimeType = cmd.StandardOutput.GetLines().FirstOrDefault();

                _log.LogDebug("Linux file type for file [{File}] is [{MimeType}]", filePath, mimeType);

                return mimeType;
            }
            catch(Exception ex)
            {
                _log.LogWarning(ex, $"Error trying to determine mime type for file [{filePath}].  This likely only works on Linux, so the error may be ignored.");
            }

            return null;
        }
    }
}
