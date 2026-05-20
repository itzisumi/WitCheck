using System.Text;
using System.Text.Json;
using Shared;
using WebGUI.DTOs;

namespace APICaller.API;

public class UserCaller
{
  private HttpClient client;

  /// <summary>
    /// to created the base httpsclient
    /// </summary>
    /// <param name="baseAddress">needs to look like http://{IpAddress}:{Port}/</param>
    public UserCaller(string baseAddress):base()
    {
        this.baseAddress = baseAddress+controllerBase;
        client = new HttpClient(){BaseAddress = new Uri(baseAddress)};
    }

  public async Task<UserDTO?> GetUserById(int id)
  {
      string route= GetUserByIdString(id);
      HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
      var response= await client.SendAsync(request);
      if (response.IsSuccessStatusCode)
      {
        UserDTO? user = JsonSerializer.Deserialize<UserDTO>(response.Content.ReadAsStringAsync().Result);
        return user;
      }
      throw new Exception(response.StatusCode.ToString());
  }

  public async Task<UserDTO?> GetUserByEmail(string email)
  {
    string route= GetUserByEmailString(email);
    HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
    var response= await client.SendAsync(request);
    if (response.IsSuccessStatusCode)
    {
      UserDTO? user = JsonSerializer.Deserialize<UserDTO>(response.Content.ReadAsStringAsync().Result);
      return user;
    }
    throw new Exception(response.StatusCode.ToString());
  }

  public async Task<UserDTO?> GetUserByUsername(string username)
  {
    string route= GetUserByUsernameString(username);
    HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
    var response= await client.SendAsync(request);
    if (response.IsSuccessStatusCode)
    {
      var json = await response.Content.ReadAsStringAsync();
      UserDTO? user = JsonSerializer.Deserialize<UserDTO>(json);
      return user;
    }
    throw new Exception(response.StatusCode.ToString());
  }

  public async Task<UserDTO> CreateUser(UserDTO userdto,string password)
  {
    string route= CreateUserString(password);
    HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,new StringContent(JsonSerializer.Serialize(userdto),Encoding.UTF8, "application/json"));
    var response= await client.SendAsync(request);
    if (response.IsSuccessStatusCode)
    {
      return userdto;
    }
    Console.WriteLine(response.ReasonPhrase);
    throw new Exception(response.StatusCode.ToString());
  }

  public async Task<int> LoginUserRoute(UserDTO userdto,string password)
  {
    string route= LoginUserString(); 
    var content=new LoginRequest(){userdto=userdto,password=password};
    HttpRequestMessage request = CreateRequest(route,HttpMethod.Post,new StringContent(JsonSerializer.Serialize(content),Encoding.UTF8, "application/json"));
    var response= await client.SendAsync(request);
    if (response.IsSuccessStatusCode)
    {
      string json = await response.Content.ReadAsStringAsync();
      int userId = JsonSerializer.Deserialize<int>(json);
      return userId;
    }
    throw new Exception(response.StatusCode.ToString());
  }

  public async Task<UserDTO> ResetPasswordRoute(UserDTO userdto,string password,string newPassword)
  {
    string route= ResetPasswordString();
    ResetPasswordRequest content=new (){userdto = userdto,password = password,newPassword = newPassword};
    HttpRequestMessage request = CreateRequest(route,HttpMethod.Post,new StringContent(JsonSerializer.Serialize(content),Encoding.UTF8, "application/json"));
    var response= await client.SendAsync(request);
    if (response.IsSuccessStatusCode)
    {
      return userdto;
    }
    throw new Exception(response.StatusCode.ToString());
  }

  public async Task<IEnumerable<QuizDTO>?> GetAllQuizzesByUser(LoginRequest login)
  {
    string route= GetAllQuizzesByUserString();
    HttpRequestMessage request = CreateRequest(route,HttpMethod.Post,new StringContent(JsonSerializer.Serialize(login),Encoding.UTF8, "application/json"));
    var response= await client.SendAsync(request);
    if (response.IsSuccessStatusCode)
    {
      var json = await response.Content.ReadAsStringAsync();
      IEnumerable<QuizDTO>? Quizzes = JsonSerializer.Deserialize<IEnumerable<QuizDTO>>(json);
      return Quizzes;
    }
    throw new Exception(response.StatusCode.ToString());
  }

  private HttpRequestMessage CreateRequest(string route,HttpMethod method,HttpContent? content = null)
  {
    var request = new HttpRequestMessage
    {
      Method = method,
      RequestUri = new Uri(route),
      Headers =
      {
        { "accept", "application/json" },
      },
      Content = content
    };
    return request;
  }


  #region strings

  private string baseAddress;
  private string controllerBase = "/UserManager";

  private string GetUserByIdString(int UserId) => baseAddress+$"/GetUserById/{UserId}";
  private string GetUserByEmailString(string EMail) => baseAddress+$"/GetUserByEmail/{EMail}";
  private string GetUserByUsernameString(string Username) => baseAddress+$"/GetUserByUsername/{Username}";
  private string CreateUserString(string password) => baseAddress+$"/CreateUser/"+password;
  private string LoginUserString() => baseAddress+$"/LoginUser";
  private string ResetPasswordString() => baseAddress+$"/ResetPassword";

  private string GetAllQuizzesByUserString() => baseAddress+$"/GetAllQuizzesByUser";

  #endregion
}