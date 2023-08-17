namespace SeeSay.Models.Dto.Users;

public class UserLoginDto
{
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;

    public bool Validate(out string? errorMessage)
    {
        if (string.IsNullOrWhiteSpace(UserName))
        {
            errorMessage = "The username field cannot is required";
            return false;
        }
        else if (string.IsNullOrWhiteSpace(Password))
        {
            errorMessage = "The password field is required";
            return false;
        }

        errorMessage = null;
        return true;
    }
    
}