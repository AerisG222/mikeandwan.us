using System;

namespace Maw.Domain.Upload;

public class UploadedFile
{
    public FileLocation Location { get; set; } = null!;
    public DateTime CreationTime { get; set; }
    public long SizeInBytes { get; set; }
}
