using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Maw.Security;
using Maw.Domain.Upload;


namespace MawApi.Hubs
{
    [Authorize]
    [Authorize(Policy.CanUpload)]
    public class UploadHub
        : Hub
    {
        const string GROUP_ADMINS = "GroupAdmins";
        const string CALL_ALL_FILES = "AllFiles";
        const string CALL_FILE_ADDED = "FileAdded";
        const string CALL_FILE_DELETED = "FileDeleted";

        readonly ILogger _log;
        readonly IUploadService _uploadSvc;


        public UploadHub(ILogger<UploadHub> log,
                         IUploadService uploadService)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _uploadSvc = uploadService ?? throw new ArgumentNullException(nameof(uploadService));
        }


        public async Task GetAllFiles()
        {
            await Clients.Caller.SendAsync(CALL_ALL_FILES, _uploadSvc.GetFileList(Context.User));
        }


        public override async Task OnConnectedAsync()
        {
            _log.LogDebug($"User [{Context.User.Identity.Name}] connected to the {nameof(UploadHub)}.");

            if(Role.IsAdmin(Context.User))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_ADMINS);
            }

            await base.OnConnectedAsync();
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _log.LogDebug($"User [{Context.User.Identity.Name}] disconnected from the {nameof(UploadHub)}.");

            if(Role.IsAdmin(Context.User))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUP_ADMINS);
            }

            await base.OnDisconnectedAsync(exception);
        }


        internal async Task FileAdded(UploadedFile file)
        {
            _log.LogDebug($"Sending [{CALL_FILE_ADDED}] for file [{file.Location.RelativePath}].");

            if(!Role.IsAdmin(Context.User))
            {
                await Clients.User(file.Location.Username).SendAsync(CALL_FILE_ADDED, file);
            }

            await Clients.Group(GROUP_ADMINS).SendAsync(CALL_FILE_ADDED, file);
        }


        internal async Task FileDeleted(UploadedFile file)
        {
            _log.LogDebug($"Sending [{CALL_FILE_DELETED}] for file [{file.Location.RelativePath}].");

            if(!Role.IsAdmin(Context.User))
            {
                await Clients.User(file.Location.Username).SendAsync(CALL_FILE_DELETED, file);
            }

            await Clients.Group(GROUP_ADMINS).SendAsync(CALL_FILE_DELETED, file);
        }
    }
}
