using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ErrorViewer.Models;

[PrimaryKey("Username")]
public class User
{
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public bool isAdmin { get; set; }
    public bool isBanned { get; set; }
    public DateOnly? CreatedAt { get; set; }

}

public class LoginUserModel
{
    [Required]
    [DataType(DataType.Text)]
    [MinLength(3), MaxLength(64)]
    public string? Username { get; set; }
    
    [Required]
    [DataType(DataType.Password)]
    [MinLength(8), MaxLength(64)]
    public string? Password { get; set; }
}

public class RegisterUserModel : LoginUserModel
{
    [Required]
    public bool isAdmin { get; set; }
    
    [Required]
    public bool isBanned { get; set; }
}

public class changePasswordModel : LoginUserModel
{
    [MinLength(8)]
    [MaxLength(64)]
    public string? newPassword { get; set; }
    [MinLength(8)]
    [MaxLength(64)]
    public string? passwordVerify { get; set; }
}

public class EditUserModel : LoginUserModel
{
    [MinLength(8)]
    [MaxLength(64)]
    public string? PasswordToChange { get; set; }
    public bool? isAdmin { get; set; }
    public bool? isBanned { get; set; }
}