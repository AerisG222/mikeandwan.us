using System.Collections.Generic;

namespace MawMvcApp.ViewModels.Tools.Dotnet;

public class FormatExampleGroup
{
    public string Name { get; private set; }
    public IList<FormatExample> FormatExampleList { get; private set; }

    public FormatExampleGroup(string name)
    {
        Name = name;
        FormatExampleList = new List<FormatExample>();
    }
}
