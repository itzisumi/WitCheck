using WebGUI.DTOs;

namespace Shared;

public class LoginRequest
{
    public UserDTO userdto { get; set; }
    public string password { get; set; }

    public LoginRequest()
    {
        
    }
    public LoginRequest(UserDTO userdto, string password)
    {
        this.userdto = userdto;
        this.password = password;
    }
}

public class LoginRequest<T> 
{
    public LoginRequest()
    {
        
    }
    
    public LoginRequest(LoginRequest loginRequest,T extrainfo)
    {
        userdto = loginRequest.userdto;
        password = loginRequest.password;
        this.extrainfo = extrainfo;
    }

    public UserDTO userdto { get; set; }
    public string password { get; set; }
    public T extrainfo { get; set; }
}