namespace DAL.Models;

public class LoginRequest // I created LoginRequest to handle the login request from the user, It specifically is not on the UserDTO because I only want it to be used for login -
                          // not for creating or updating users. But is probably a bit overkill for this application and redundant with the User entity
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}