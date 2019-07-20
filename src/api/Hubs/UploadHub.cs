using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Maw.Domain.Upload;
using Maw.Security;


namespace MawApi.Hubs
{
    [Authorize]
    [Authorize(MawPolicy.CanUpload)]
    public class UploadHub
        : Hub
    {
        const string GROUP_ADMINS = "Admins";
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


        public IEnumerable<UploadedFile> GetAllFiles()
        {
            return _uploadSvc.GetFileList(Context.User);
        }


        [HubMethodName("DeleteFiles")]
        public async Task<IEnumerable<FileOperationResult>> DeleteFilesAsync(List<string> files)
        {
            var results = _uploadSvc.DeleteFiles(Context.User, files);

            foreach(var result in results)
            {
                if(result.WasSuccessful)
                {
                    await FileDeletedAsync(result.UploadedFile).ConfigureAwait(false);
                }
            }

            return results;
        }


        public override async Task OnConnectedAsync()
        {
            _log.LogDebug($"User [{Context.User.Identity.Name}] connected to the {nameof(UploadHub)}.");

            if(Role.IsAdmin(Context.User))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_ADMINS).ConfigureAwait(false);
            }

            await base.OnConnectedAsync().ConfigureAwait(false);
        }


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _log.LogDebug($"User [{Context.User.Identity.Name}] disconnected from the {nameof(UploadHub)}.");

            if(Role.IsAdmin(Context.User))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUP_ADMINS).ConfigureAwait(false);
            }

            await base.OnDisconnectedAsync(exception).ConfigureAwait(false);
        }


        public static async Task FileAddedAsync(IHubContext<UploadHub> ctx, ClaimsPrincipal user, UploadedFile file)
        {
            if(ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            if(file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            //_log.LogDebug($"Sending [{CALL_FILE_ADDED}] for file [{file.Location.RelativePath}].");

            if(!Role.IsAdmin(user))
            {
                await ctx.Clients.User(file.Location.Username).SendAsync(CALL_FILE_ADDED, file).ConfigureAwait(false);
            }

            await ctx.Clients.Group(GROUP_ADMINS).SendAsync(CALL_FILE_ADDED, file).ConfigureAwait(false);
        }


        public static async Task FileDeletedAsync(IHubContext<UploadHub> ctx, ClaimsPrincipal user, UploadedFile file)
        {
            if(ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            if(file == null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            //_log.LogDebug($"Sending [{CALL_FILE_DELETED}] for file [{file.Location.RelativePath}].");

            if(!Role.IsAdmin(user))
            {
                await ctx.Clients.User(file.Location.Username).SendAsync(CALL_FILE_DELETED, file).ConfigureAwait(false);
            }

            await ctx.Clients.Group(GROUP_ADMINS).SendAsync(CALL_FILE_DELETED, file).ConfigureAwait(false);
        }


        async Task FileDeletedAsync(UploadedFile file)
        {
            if(!Role.IsAdmin(Context.User))
            {
                await Clients.User(file.Location.Username).SendAsync(CALL_FILE_DELETED, file).ConfigureAwait(false);
            }

            await Clients.Group(GROUP_ADMINS).SendAsync(CALL_FILE_DELETED, file).ConfigureAwait(false);
        }
    }
}
