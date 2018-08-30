using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Maw.Security;


namespace MawApi.Hubs
{
    [Authorize]
    [Authorize(Policy.CanUpload)]
    public class UploadHub
        : Hub
    {

    }
}
