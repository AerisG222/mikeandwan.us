using Microsoft.AspNetCore.Identity;

namespace MawAuth.ViewModels.Admin;

public class IdentityResultModel
{
    public IdentityResult Result { get; set; }
    public string EntityName { get; set; }

    public IdentityResultModel(string entityName, IdentityResult result)
    {
        Result = result;
        EntityName = entityName;
    }
}
