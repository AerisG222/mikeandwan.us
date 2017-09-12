namespace MawMvcApp.ViewModels.Email
{
    public class CreateUserEmailModel
        : BaseEmailModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
    }
}
