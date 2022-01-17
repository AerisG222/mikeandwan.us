using Microsoft.Extensions.DependencyInjection;
using Maw.Domain.Blogs;
using Maw.Domain.Captcha;
using Maw.Domain.Email;
using Maw.Domain.Identity;
using Maw.Domain.Photos;
using Maw.Domain.Upload;
using Maw.Domain.Utilities;
using Maw.Domain.Videos;

namespace Maw.Domain;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddMawDomainServices(this IServiceCollection services)
    {
        services
            .AddDistributedMemoryCache()
            .AddSingleton<IImageCropper, ImageCropper>()
            .AddSingleton<IPhotoZipper, PhotoZipper>()
            .AddSingleton<LinuxFileTypeIdentifier>()
            .AddScoped<IBlogService, BlogService>()
            .AddScoped<IPhotoService, PhotoService>()
            .AddScoped<IVideoService, VideoService>()
            .AddScoped<ICaptchaService, GoogleCaptchaService>()
            //.AddScoped<IEmailService, SmtpEmailService>()
            .AddScoped<IEmailService, GmailApiEmailService>()
            .AddScoped<ILoginService, LoginService>()
            .AddScoped<IUploadService, UploadService>();

        return services;
    }
}
