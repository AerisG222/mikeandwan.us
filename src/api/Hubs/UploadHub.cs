using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Maw.Domain;
using Maw.Domain.Upload;
using Maw.Domain.Models.Upload;
using Maw.Security;

namespace MawApi.Hubs;

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

    public UploadHub(
        ILogger<UploadHub> log,
        IUploadService uploadService)
    {
        ArgumentNullException.ThrowIfNull(log);
        ArgumentNullException.ThrowIfNull(uploadService);

        _log = log;
        _uploadSvc = uploadService;
    }

    public IEnumerable<UploadedFile> GetAllFiles()
    {
        ArgumentNullException.ThrowIfNull(Context.User);

        return _uploadSvc.GetFileList(Context.User);
    }

    [HubMethodName("DeleteFiles")]
    public async Task<IEnumerable<FileOperationResult>> DeleteFilesAsync(List<string> files)
    {
        ArgumentNullException.ThrowIfNull(Context.User);

        var results = _uploadSvc.DeleteFiles(Context.User, files);

        foreach(var result in results)
        {
            if(result.WasSuccessful)
            {
                await FileDeletedAsync(result.UploadedFile);
            }
        }

        return results;
    }

    public override async Task OnConnectedAsync()
    {
        ArgumentNullException.ThrowIfNull(Context.User?.Identity);

        _log.LogDebug("User [{Username}] connected to {Hub}.", Context.User.Identity.Name, nameof(UploadHub));

        if(Context.User.IsAdmin())
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, GROUP_ADMINS);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        ArgumentNullException.ThrowIfNull(Context.User?.Identity);

        _log.LogDebug("User [{Username}] disconnected from {Hub}.", Context.User.Identity.Name, nameof(UploadHub));

        if(Context.User.IsAdmin())
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, GROUP_ADMINS);
        }

        await base.OnDisconnectedAsync(exception);
    }

    public static async Task FileAddedAsync(IHubContext<UploadHub> ctx, ClaimsPrincipal user, UploadedFile file)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        ArgumentNullException.ThrowIfNull(file);

        //_log.LogDebug($"Sending [{CALL_FILE_ADDED}] for file [{file.Location.RelativePath}].");

        if(!user.IsAdmin())
        {
            await ctx.Clients.User(file.Location.Username).SendAsync(CALL_FILE_ADDED, file);
        }

        await ctx.Clients.Group(GROUP_ADMINS).SendAsync(CALL_FILE_ADDED, file);
    }

    public static async Task FileDeletedAsync(IHubContext<UploadHub> ctx, ClaimsPrincipal user, UploadedFile file)
    {
        ArgumentNullException.ThrowIfNull(ctx);
        ArgumentNullException.ThrowIfNull(file);

        //_log.LogDebug($"Sending [{CALL_FILE_DELETED}] for file [{file.Location.RelativePath}].");

        if(!user.IsAdmin())
        {
            await ctx.Clients.User(file.Location.Username).SendAsync(CALL_FILE_DELETED, file);
        }

        await ctx.Clients.Group(GROUP_ADMINS).SendAsync(CALL_FILE_DELETED, file);
    }

    async Task FileDeletedAsync(UploadedFile file)
    {
        ArgumentNullException.ThrowIfNull(Context.User);

        if(!Context.User.IsAdmin())
        {
            await Clients.User(file.Location.Username).SendAsync(CALL_FILE_DELETED, file);
        }

        await Clients.Group(GROUP_ADMINS).SendAsync(CALL_FILE_DELETED, file);
    }
}
