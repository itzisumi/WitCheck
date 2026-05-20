using WebGUI.DTOs;

namespace Shared;

public class ResetPasswordRequest
{
    public UserDTO userdto { get; set; }
    public string password { get; set; }
    public string newPassword { get; set; }
}