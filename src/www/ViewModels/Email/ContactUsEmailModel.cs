namespace MawMvcApp.ViewModels.Email;

public record class ContactUsEmailModel(
    string Title,
    string EmailAddress,
    string FirstName,
    string LastName,
    string Message
);
