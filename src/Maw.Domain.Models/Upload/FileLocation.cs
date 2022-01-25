using System;
using System.IO;

namespace Maw.Domain.Models.Upload;

public class FileLocation
{
    public string Username { get; init; }
    public string Filename { get; init; }

    public FileLocation(string username, string filename)
    {
        Username = username ?? throw new ArgumentNullException(nameof(username));
        Filename = filename ?? throw new ArgumentNullException(nameof(filename));
    }

    public string RelativePath
    {
        get
        {
            return $"{Username}/{Filename}";
        }
    }

    public static FileLocation FromRelativePath(string relativePath)
    {
        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new ArgumentNullException(nameof(relativePath));
        }

        if (Path.IsPathRooted(relativePath))
        {
            throw new ArgumentException("Invalid file path");
        }

        var parts = relativePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid file path");
        }

        return new FileLocation(parts[0], parts[1]);
    }
}
