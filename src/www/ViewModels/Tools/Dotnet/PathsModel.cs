using System.Globalization;

namespace MawMvcApp.ViewModels.Tools.Dotnet;

public class PathsModel
{
    public IList<string[]> Paths { get; } = new List<string[]>();

    public void PreparePaths()
    {
        Paths.Add(new string[] { @"Path.ChangeExtension(""C:\windows\test.txt"", "".dat"")", Path.ChangeExtension(@"C:\windows\test.txt", ".dat").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.ChangeExtension(""C:\windows\test.txt"", ""dat"")", Path.ChangeExtension(@"C:\windows\test.txt", "dat").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.ChangeExtension(""/etc/passwd"", "".txt"")", Path.ChangeExtension("/etc/passwd", ".txt").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.ChangeExtension(""/etc/passwd"", ""txt"")", Path.ChangeExtension("/etc/passwd", "txt").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.ChangeExtension(""/etc/passwd/"", ""txt"")", Path.ChangeExtension("/etc/passwd/", "txt").ToString(CultureInfo.InvariantCulture) });

        Paths.Add(new string[] { @"Path.Combine(""C:\test\"", ""file.txt"")", Path.Combine(@"C:\test\", "file.txt") });
        Paths.Add(new string[] { @"Path.Combine(""C:\test"", ""file.txt"")", Path.Combine(@"C:\test", "file.txt") });
        Paths.Add(new string[] { @"Path.Combine(""C:\test"", ""C:\file.txt"")", Path.Combine(@"C:\test", @"C:\file.txt") });
        Paths.Add(new string[] { @"Path.Combine(""/etc"", ""passwd"")", Path.Combine("/etc", "passwd") });
        Paths.Add(new string[] { @"Path.Combine(""etc"", ""passwd"")", Path.Combine("etc", "passwd") });
        Paths.Add(new string[] { @"Path.Combine(""/etc"", ""/usr"")", Path.Combine("/etc", "/usr") });

        Paths.Add(new string[] { @"Path.GetDirectoryName(""/etc/passwd"")", Path.GetDirectoryName("/etc/passwd")! });
        Paths.Add(new string[] { @"Path.GetDirectoryName(""/etc/passwd/"")", Path.GetDirectoryName("/etc/passwd/")! });
        Paths.Add(new string[] { @"Path.GetDirectoryName(""C:\windows\system32"")", Path.GetDirectoryName(@"C:\windows\system32")! });
        Paths.Add(new string[] { @"Path.GetDirectoryName(""C:\windows\system32\"")", Path.GetDirectoryName(@"C:\windows\system32\")! });

        Paths.Add(new string[] { @"Path.GetExtension(""/etc/passwd"")", Path.GetExtension("/etc/passwd") });
        Paths.Add(new string[] { @"Path.GetExtension(""/etc/passwd/"")", Path.GetExtension("/etc/passwd/") });
        Paths.Add(new string[] { @"Path.GetExtension(""/etc/passwd/.txt"")", Path.GetExtension("/etc/passwd/.txt") });
        Paths.Add(new string[] { @"Path.GetExtension(""C:\windows\test.txt"")", Path.GetExtension(@"C:\windows\test.txt") });

        Paths.Add(new string[] { @"Path.GetFileName(""/etc/passwd"")", Path.GetFileName("/etc/passwd") });
        Paths.Add(new string[] { @"Path.GetFileName(""/etc/passwd/"")", Path.GetFileName("/etc/passwd/") });
        Paths.Add(new string[] { @"Path.GetFileName(""/etc/passwd/.txt"")", Path.GetFileName("/etc/passwd/.txt") });
        Paths.Add(new string[] { @"Path.GetFileName(""C:\windows\test.txt"")", Path.GetFileName(@"C:\windows\test.txt") });

        Paths.Add(new string[] { @"Path.GetFileNameWithoutExtension(""/etc/passwd"")", Path.GetFileNameWithoutExtension("/etc/passwd") });
        Paths.Add(new string[] { @"Path.GetFileNameWithoutExtension(""/etc/passwd/"")", Path.GetFileNameWithoutExtension("/etc/passwd/") });
        Paths.Add(new string[] { @"Path.GetFileNameWithoutExtension(""/etc/passwd/.txt"")", Path.GetFileNameWithoutExtension("/etc/passwd/.txt") });
        Paths.Add(new string[] { @"Path.GetFileNameWithoutExtension(""C:\windows\test.txt"")", Path.GetFileNameWithoutExtension(@"C:\windows\test.txt") });
        Paths.Add(new string[] { @"Path.GetFileNameWithoutExtension(""C:\windows\test.txt.pgp"")", Path.GetFileNameWithoutExtension(@"C:\windows\test.txt.pgp") });

        Paths.Add(new string[] { @"Path.GetFullPath(""/etc/passwd"")", Path.GetFullPath("/etc/passwd") });
        Paths.Add(new string[] { @"Path.GetFullPath(""/etc/passwd/"")", Path.GetFullPath("/etc/passwd/") });
        Paths.Add(new string[] { @"Path.GetFullPath(""/etc/passwd/.txt"")", Path.GetFullPath("/etc/passwd/.txt") });
        Paths.Add(new string[] { @"Path.GetFullPath(""etc/passwd"")", Path.GetFullPath("etc/passwd") });
        Paths.Add(new string[] { @"Path.GetFullPath(""C:\windows\test.txt"")", Path.GetFullPath(@"C:\windows\test.txt") });

        Paths.Add(new string[] { @"Path.GetInvalidFileNameChars()", string.Join(",", CharArrayToStringArray(Path.GetInvalidFileNameChars())) });

        Paths.Add(new string[] { @"Path.GetInvalidPathChars()", string.Join(",", CharArrayToStringArray(Path.GetInvalidPathChars())) });

        Paths.Add(new string[] { @"Path.GetPathRoot(""/etc/passwd"")", Path.GetPathRoot("/etc/passwd")! });
        Paths.Add(new string[] { @"Path.GetPathRoot(""/etc/passwd/"")", Path.GetPathRoot("/etc/passwd/")! });
        Paths.Add(new string[] { @"Path.GetPathRoot(""/etc/passwd/.txt"")", Path.GetPathRoot("/etc/passwd/.txt")! });
        Paths.Add(new string[] { @"Path.GetPathRoot(""etc/passwd"")", Path.GetPathRoot("etc/passwd")! });
        Paths.Add(new string[] { @"Path.GetPathRoot(""C:\windows\test.txt"")", Path.GetPathRoot(@"C:\windows\test.txt")! });

        Paths.Add(new string[] { @"Path.GetRandomFileName()", Path.GetRandomFileName() });

        Paths.Add(new string[] { @"Path.GetTempFileName()", Path.GetTempFileName() });

        Paths.Add(new string[] { @"Path.GetTempPath()", Path.GetTempPath() });

        Paths.Add(new string[] { @"Path.HasExtension(""/etc/passwd"")", Path.HasExtension("/etc/passwd").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.HasExtension(""/etc/passwd/"")", Path.HasExtension("/etc/passwd/").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.HasExtension(""/etc/passwd/.txt"")", Path.HasExtension("/etc/passwd/.txt").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.HasExtension(""etc/passwd"")", Path.HasExtension("etc/passwd").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.HasExtension(""C:\windows\test.txt"")", Path.HasExtension(@"C:\windows\test.txt").ToString(CultureInfo.InvariantCulture) });

        Paths.Add(new string[] { @"Path.IsPathRooted(""/etc/passwd"")", Path.IsPathRooted("/etc/passwd").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.IsPathRooted(""/etc/skel/"")", Path.IsPathRooted("/etc/skel/").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.IsPathRooted(""C:\windows\system32"")", Path.IsPathRooted(@"C:\windows\system32").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.IsPathRooted(""C:\windows\system32\"")", Path.IsPathRooted(@"C:\windows\system32\").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.IsPathRooted(""etc/passwd"")", Path.IsPathRooted("etc/passwd").ToString(CultureInfo.InvariantCulture) });
        Paths.Add(new string[] { @"Path.IsPathRooted(""windows\system32\"")", Path.IsPathRooted(@"windows\system32\").ToString(CultureInfo.InvariantCulture) });
    }

    private string[] CharArrayToStringArray(char[] array)
    {
        string[] result = new string[array.Length];

        for (int i = 0; i < array.Length; i++)
        {
            result[i] = array[i].ToString(CultureInfo.InvariantCulture);
        }

        return result;
    }
}
