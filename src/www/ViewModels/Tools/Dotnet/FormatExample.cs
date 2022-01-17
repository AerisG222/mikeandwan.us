namespace MawMvcApp.ViewModels.Tools.Dotnet;

public class FormatExample
{
    public string FunctionCall { get; set; }
    public string Result { get; set; }

    public FormatExample(string functionCall, string result)
    {
        FunctionCall = functionCall;
        Result = result;
    }
}
