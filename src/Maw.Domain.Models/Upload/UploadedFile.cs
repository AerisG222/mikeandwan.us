using System;

namespace Maw.Domain.Models.Upload;

public class UploadedFile
{
    public FileLocation Location { get; set; } = null!;
    public DateTime CreationTime { get; set; }
    public long SizeInBytes { get; set; }
}
