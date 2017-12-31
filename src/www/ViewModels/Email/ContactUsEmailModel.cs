namespace MawMvcApp.ViewModels.Email
{
    public class ContactUsEmailModel
        : BaseEmailModel
    {
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Message { get; set; }
    }
}
