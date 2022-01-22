namespace MawAuth.ViewModels.Email;

public record class CreateUserEmailModel
(
    string Title,
    string Username,
    string Password,
    string FirstName
);
