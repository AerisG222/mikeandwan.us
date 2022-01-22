namespace MawAuth.ViewModels.Email;

public record class ResetPasswordEmailModel
(
    string Title,
    string CallbackUrl
);
