namespace Maw.Domain.Models.Upload;

public class FileLocation
{
    public string Username { get; init; }
    public string Filename { get; init; }

    public FileLocation(string username, string filename)
    {
        ArgumentNullException.ThrowIfNull(username);
        ArgumentNullException.ThrowIfNull(filename);

        Username = username;
        Filename = filename;
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
        ArgumentException.ThrowIfNullOrWhiteSpace(relativePath);

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
