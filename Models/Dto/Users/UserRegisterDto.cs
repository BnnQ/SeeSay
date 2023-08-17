namespace SeeSay.Models.Dto.Users;

public class UserRegisterDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ConfirmPassword { get; set; } = null!;
    
    public bool Validate(out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(FirstName))
        {
            errorMessage = "The first name field is required";
            return false;
        }
        else if (string.IsNullOrWhiteSpace(LastName))
        {
            errorMessage = "The last name field is required";
            return false;
        }
        else if (string.IsNullOrWhiteSpace(Email))
        {
            errorMessage = "The email field is required";
            return false;
        }
        else if (string.IsNullOrWhiteSpace(UserName))
        {
            errorMessage = "The username field is required";
            return false;
        }
        else if (string.IsNullOrWhiteSpace(Password))
        {
            errorMessage = "The password field is required";
            return false;
        }
        else if (string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            errorMessage = "The password confirmation field is required";
            return false;
        }
        else if (!Password.Equals(ConfirmPassword))
        {
            errorMessage = "Password and confirmation password do not match";
            return false;
        }

        errorMessage = null;
        return true;
    }
    
}