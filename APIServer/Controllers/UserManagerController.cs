using Database;
using Database.DbContext;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared;
using WebGUI.DTOs;

namespace APIServer.Controllers;

[ApiController]
[Route("UserManager")]
public class UserManagerController:ControllerBase
{
    private QuizRepository _quizRepository = new (new ());
    private UserManager UserManager = new UserManager(new UserRepository(new WitCheckContext()),new PasswordRepository(new WitCheckContext()));

    [HttpGet("GetUserById/{UserId:int}")]
    public async Task<ActionResult<UserDTO>> GetUserById([FromRoute]int UserId)
    {
        try
        {
            var user=await UserManager.GetUserById(UserId);
            return Ok(user.Adapt<UserDTO>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpGet("GetUserByEmail/{EMail}")]
    public async Task<ActionResult<UserDTO>> GetUserByEmail([FromRoute]string EMail)
    {
        try
        {
            var user=await UserManager.GetUserByEmail(EMail);
            return Ok(user.Adapt<UserDTO>());
        }
        catch (Exception e)
        {
           Console.WriteLine(e.Message);
           return UnprocessableEntity(e.Message);
        }
    }

    [HttpGet("GetUserByUsername/{Username}")]
    public async Task<ActionResult<UserDTO>> GetUserByUsername([FromRoute]string Username)
    {
        try
        {
            var user=await UserManager.GetUserByUsername(Username);
            var temp = user.Adapt<UserDTO>();
            return Ok(temp);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpPut("CreateUser/{password}")]
    public async Task<ActionResult> CreateUser([FromBody]UserDTO userdto, [FromRoute]string password)
    {
        var re=await UserManager.CreateUser(userdto,password);
        if (re.wasSuccesful)
            return Ok();
        else
            return UnprocessableEntity("Failed to created user");
    }

    [HttpPost("LoginUser")]
    public async Task<ActionResult<(bool wasSuccesful,int? UserID)>> LoginUser([FromBody] LoginRequest loginRequest)
    {
        var re=await UserManager.LoginUser(loginRequest.userdto,loginRequest.password);
        if (re.wasSuccesful)
            return Ok(re.UserID);
        else
            return UnprocessableEntity("Login failed");
    }

    [HttpPost("ResetPassword")]
    public async Task<ActionResult<(bool wasSuccesful,int? UserID)>> ResetPassword([FromBody]ResetPasswordRequest request)
    {
        Console.WriteLine($"[ResetPassword] UserId={request?.userdto?.UserId} password='{request?.password}' newPassword='{request?.newPassword}'");
        var re=await UserManager.ResetPassword(request.userdto,request.password,request.newPassword);
        Console.WriteLine($"[ResetPassword] result={re.wasSuccesful}");
        if (re.wasSuccesful)
            return Ok();
        else
            return UnprocessableEntity("Login failed");
    }

    [HttpPost("GetAllQuizzesByUser")]
    public async Task<ActionResult<IEnumerable<QuizDTO>>> GetAllQuizzesByUser([FromBody] LoginRequest loginRequest)
    {
        var user=await UserManager.LoginUser(loginRequest.userdto,loginRequest.password);
        if (!user.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var quizzes=await _quizRepository.FindAsync(x => x.UserId == user.UserID);
        return Ok(quizzes);
    }
}