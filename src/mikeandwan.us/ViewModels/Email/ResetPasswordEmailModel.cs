namespace MawMvcApp.ViewModels.Email
{
    public class ResetPasswordEmailModel
        : BaseEmailModel
    {
        public string CallbackUrl { get; set; }
    }
}
