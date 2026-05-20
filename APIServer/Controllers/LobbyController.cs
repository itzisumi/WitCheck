using APIServer.Hub;
using APIServer.Manager;
using Database;
using Database.DbContext;
using Database.WitCheckEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mapster;
using Shared;
using WebGUI.DTOs;
using WebGUI.DTOs.APIInput;

namespace APIServer.Controllers;
[ApiController]
[Route("Lobbies")]
public class LobbyController:ControllerBase
{
    private readonly IHubContext<HostHub> _hostHubContext;
    private readonly IHubContext<PlayerHub> _playerHubContext;
    private readonly QuizRepository _quizRepository;
    private readonly UserManager UserManager;


    public LobbyController(IHubContext<HostHub> hostHubContext, IHubContext<PlayerHub> playerHubContext)
    {
        _hostHubContext = hostHubContext;
        _playerHubContext = playerHubContext;
        _quizRepository = new QuizRepository(new WitCheckContext());
        UserManager = new UserManager(new UserRepository(new WitCheckContext()),new PasswordRepository(new WitCheckContext()));
    }

    [HttpGet("LobbyIdByHost/{HostId:int}")]
    public ActionResult<string> GetLobbyIdByHostId([FromRoute]int HostId)
    {
        var host=GeneralManager.GetLobbyByHostId(HostId);
        if (host!=null)
        {
            return Ok(host.GetLobbyId());
        }
        else
        {
            return UnprocessableEntity("no lobby found");
        }
    }

    [HttpGet("LobbyHost/{LobbyId}")]
    public ActionResult<UserDTO> GetLobbyHost([FromRoute]string LobbyId)
    {
        var host=GeneralManager.GetLobbyByLobbyId(LobbyId);
        if (host!=null)
        {
            return Ok(host.GetHost().Adapt<UserDTO>());
        }
        else
        {
            return UnprocessableEntity("no lobby found");
        }
    }


    [HttpGet("AllPlayers/{LobbyId}")]
    public ActionResult<IEnumerable<PlayerDTO>> GetAllPlayers([FromRoute]string LobbyId)
    {
        var host=GeneralManager.GetLobbyByLobbyId(LobbyId);
        if (host!=null)
        {
            return Ok(host.GetPlayers().Adapt<List<PlayerDTO>>());
        }
        else
        {
            return UnprocessableEntity("no lobby found");
        }
    }

    [HttpGet("AllAnswers/{LobbyId}")]
    public ActionResult<IEnumerable<AnswerDTO>> GetAllAnswers([FromRoute]string LobbyId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(LobbyId);
        if (lobby != null)
        {
            return Ok(lobby.GetCurrentQuestion()
                .Answers
                .OrderBy(x => x.OrderNumber)
                .Adapt<List<AnswerDTO>>());
        }
        else
        {
            return UnprocessableEntity("no lobby found");
        }
    }


    [HttpGet("QuizColor/{LobbyId}")]
    public ActionResult<string> GetQuizColor([FromRoute]string LobbyId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(LobbyId);
        if (lobby == null)
            return UnprocessableEntity("no lobby found");

        var color = lobby.GetQuiz().ColorCode;
        return Ok(string.IsNullOrWhiteSpace(color) ? "#5BB933" : color);
    }

    //methods that didnt make sens in the hub and got transported here

    [HttpPut("CreateLobby")]
    public async Task<ActionResult<string?>> CreateLobby([FromBody]CreateLobbyRequest createLobbyRequest)
    {
        var login=await UserManager.LoginUser(createLobbyRequest.User,createLobbyRequest.Password);
        if (!login.wasSuccesful)
            return UnprocessableEntity("Login failed");
        int id= (int)login.UserID!;
        
        var user=await UserManager.GetUserById(id)!;
        var quiz=(List<Quiz>)await _quizRepository.FindAsync(x => x.UserId == user.UserId && createLobbyRequest.QuizId==x.QuizId);
        if (!quiz.Any()) return NotFound("no quiz with questions found to host");
        var quizWithQuestionAndAnswers = await _quizRepository.GetByIdAsync(quiz.First().QuizId);
        if (quizWithQuestionAndAnswers == null || quizWithQuestionAndAnswers.Questions == null || !quizWithQuestionAndAnswers.Questions.Any())
            return UnprocessableEntity("quiz has no questions");
        try
        {
            var lobbyManager = await GeneralManager.AddLobby(user,quizWithQuestionAndAnswers);
            if(lobbyManager!=null)
                return Ok(lobbyManager.GetLobbyId());
            
        }
        catch (Exception e)
        {
            return UnprocessableEntity("Could not creat lobby");
        }
        return UnprocessableEntity("lobby not created");
    }

    [HttpPut("HostLobby/{lobbyId}")]
    public async Task<ActionResult<bool>> HostLobby([FromRoute]string LobbyId,[FromBody]LoginRequest loginRequest)
    {
        var login=await UserManager.LoginUser(loginRequest.userdto,loginRequest.password);
        if (!login.wasSuccesful)
            return UnprocessableEntity("Login failed");
        try
        {
            var lobby = GeneralManager.GetLobbyByLobbyId(LobbyId);
            if (lobby == null)
                return UnprocessableEntity("no lobby found");
            lobby.HostLobby();
            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpPut("DeleteLobby/{lobbyId}")]
    public async Task<ActionResult<bool>> DeleteLobby([FromRoute]string LobbyId,[FromBody]LoginRequest loginRequest)
    {
        var login=await UserManager.LoginUser(loginRequest.userdto,loginRequest.password);
        if (!login.wasSuccesful)
            return UnprocessableEntity("Login failed");
        try
        {
            var l = GeneralManager.GetLobbyByLobbyId(LobbyId);
            
            if(l==null)
                return UnprocessableEntity("no lobby found");

            foreach (var p in l.GetPlayers())
            {
                await HostHub.instance.KickPlayer(loginRequest, LobbyId, p.PlayerId);
            }
            
            await GeneralManager.DeleteLobby(LobbyId, login.UserID ?? 0);
            
            return Ok(true);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpGet("GetAllLobbies")]
    public async Task<ActionResult> GetAllLobbies()
    {
        var temp = new List<Lobbies>();
        foreach (var (key,val) in GeneralManager.GetAllLobbies())
        {
            temp.Add(new Lobbies()
            {
                id=key,
                PlayerCount = val.GetPlayers().Count,
                UserName = val.GetHost().Username,
                QuizName = val.GetQuiz().Quizname
            });
        }
        return Ok(temp);
    }

    struct Lobbies
    {
        public string id;
        public int PlayerCount;
        public string UserName;
        public string QuizName;
    }

    #region came from hub

    [HttpPut("AnswerQuestion/{lobbyId}/{playerId}")]
    public async Task<ActionResult<bool>> AnswerQuestion([FromRoute]string lobbyId, [FromRoute]string playerId, [FromBody]AnswerQuestionDTO answerQuestionDto)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby != null)
        {
            try
            {
                var allPlayersFinished = await lobby.PlayerAnswerQuestion(playerId,answerQuestionDto.answerId,answerQuestionDto.timeLeft);

                if (allPlayersFinished)
                {
                    await _hostHubContext.Clients
                        .Group(lobby.GetHost().UserId.ToString())
                        .SendAsync("AllPlayerFinished");
                }
                else if (lobby.GetPlayers().All(p => p.AnsweredCurrentQuestion))
                {
                    // FinishQuestion() was already called before this player's answer arrived.
                    // Unblock the player by sending FinishedQuestion directly to them.
                    await _playerHubContext.Clients.Group(playerId).SendAsync("FinishedQuestion", true);
                }
            }
            catch
            {
                // Lobby state is no longer Running — unblock the player.
                await _playerHubContext.Clients.Group(playerId).SendAsync("FinishedQuestion", true);
            }

            return Ok(true);
        }
        return UnprocessableEntity("no lobby found");
    }

    [HttpGet("GetQuestion/{lobbyId}")]
    public async Task<ActionResult<QuestionDTO>> GetQuestion([FromRoute]string lobbyId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby != null)
        {
            return Ok(lobby.GetCurrentQuestion().Adapt<QuestionDTO>());
        }
        return UnprocessableEntity("no lobby found");
    }

    [HttpGet("GetQuestionAnswer/{lobbyId}")]
    public async Task<ActionResult<IEnumerable<AnswerDTO>>> GetQuestionAnswer([FromRoute]string lobbyId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby != null)
        {
            return Ok(lobby.GetCurrentQuestion().Answers.Where(x=>x.IsCorrect==1).Adapt<IEnumerable<AnswerDTO>>());
        }
        return UnprocessableEntity("no lobby found");
    }

    [HttpGet("GetQuestionPoints/{lobbyId}")]
    public async Task<ActionResult<int?>> GetQuestionPoints([FromRoute]string lobbyId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby != null)
        {
            return Ok(lobby.GetCurrentQuestion().Points);
        }
        return UnprocessableEntity("no lobby found");
    }

    [HttpGet("GetPlayerPoints/{lobbyId}/{playerId}")]
    public async Task<ActionResult<uint?>> GetPlayerPoints([FromRoute]string lobbyId, [FromRoute]string playerId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby != null)
        {
            return lobby.GetPlayerById(playerId)!.Points;
        }
        return UnprocessableEntity("no lobby found");
    }

    [HttpGet("GetPlayerLastIsCorrect/{lobbyId}/{playerId}")]
    public async Task<ActionResult<bool?>> GetPlayerLastIsCorrect([FromRoute]string lobbyId, [FromRoute]string playerId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby == null)
            return UnprocessableEntity("no lobby found");

        var player = lobby.GetPlayerById(playerId);
        if (player == null)
            return UnprocessableEntity("no player found");

        return Ok(player.LastQuestionWasCorrect);
    }

    [HttpGet("GetPlayerPosition/{lobbyId}/{playerId}")]
    public async Task<ActionResult<uint?>> GetPlayerPosition([FromRoute]string lobbyId, [FromRoute]string playerId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby != null)
        {
            return Ok(lobby.GetPlayerById(playerId)!.Position);
        }
        return UnprocessableEntity("no lobby found");
    }

    [HttpGet("GetAnswersAsDict/{lobbyId}")]
    public async Task<ActionResult<IDictionary<int,int>>> GetAnswersAsDict([FromRoute]string lobbyId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby != null)
        {
            var groups= lobby.GetPlayers().GroupBy(x => x.AnswerForCurrentQuestion);
            var dict = new Dictionary<int, int>();
            foreach (var VARIABLE in groups)
            {
                if (VARIABLE.Key == null)
                    dict.Add(-1,VARIABLE.Count());
                else 
                    dict.Add(VARIABLE.Key.AnswerId,VARIABLE.Count());
            }
            return Ok(dict);
        }
        return UnprocessableEntity("no lobby found");
    }

    #endregion
}
