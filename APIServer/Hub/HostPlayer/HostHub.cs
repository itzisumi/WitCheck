using APIServer.Manager;
using Database;
using Database.DbContext;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using Shared;
using WebGUI.DTOs;

namespace APIServer.Hub;

public class HostHub: Microsoft.AspNetCore.SignalR.Hub,IHostHub
{
    public static HostHub? instance;
    private readonly IHubContext<PlayerHub> _playerHubContext;
    private readonly UserManager _userManager;

    public HostHub(IHubContext<PlayerHub> playerHubContext)
    {
        instance = this;

        _playerHubContext = playerHubContext;
        _userManager = new UserManager(new UserRepository(new WitCheckContext()),new PasswordRepository(new WitCheckContext()));
    }

    public async Task<QuestionDTO?> NextQuestion(LoginRequest login,string lobbyId)
    {
        var u=await _userManager.LoginUser(login.userdto, login.password);
        if(!u.wasSuccesful)
            return null;
        
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby == null)
            return null;
        var question = lobby.NextQuestion();
        if (question != null)
        {
            QuestionDTO q = question.Adapt<QuestionDTO>();
            await _playerHubContext.Clients.Group(lobbyId).SendAsync(NextQuestionString(), q);
            
            foreach (var player in lobby.GetPlayers())
            {
                _ = _playerHubContext.Clients.Client(player.ConnectionId).SendAsync(NextQuestionString(),q,player.Adapt<PlayerDTO>());
            }
            
            return q;
        }
        
        lobby.FinishGame();
        await _playerHubContext.Clients.Group(lobbyId).SendAsync(GameOverString());
        return null;
    }

    public async Task AnswerTimeUp(LoginRequest login,string lobbyId)
    {
        var u=await _userManager.LoginUser(login.userdto, login.password);
        if(!u.wasSuccesful)
            return;
        
        GeneralManager.GetLobbyByLobbyId(lobbyId)!.FinishQuestion();
        await _playerHubContext.Clients.Group(lobbyId).SendAsync(FinishedQuestionString(),true);
    }

    private string NextQuestionString() => "NextQuestion";
    private string FinishedQuestionString() => "FinishedQuestion";
    private string StartGameString() => "StartGame";
    private string KickPlayerString() => "KickPlayer";
    private string AllPlayerFinishedString() => "AllPlayerFinished";
    private string GameOverString() => "GameOver";

    public async Task HostStartLobby(LoginRequest login,string lobbyId)
    {
        var u=await _userManager.LoginUser(login.userdto, login.password);
        if(!u.wasSuccesful)
            return ;
        
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby == null)
            return;
        await Groups.AddToGroupAsync(Context.ConnectionId,lobby.GetHost().UserId.ToString());
    }

    public async Task HostEndLobby(LoginRequest login,string lobbyId)
    {
        var u=await _userManager.LoginUser(login.userdto, login.password);
        if(!u.wasSuccesful)
            return;
        
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby == null)
            return;
        await Groups.RemoveFromGroupAsync(Context.ConnectionId,lobby.GetHost().UserId.ToString());
    }

    public async Task<QuestionDTO?> Start(LoginRequest login,string lobbyId)
    {
        var u=await _userManager.LoginUser(login.userdto, login.password);
        if(!u.wasSuccesful)
            return null;
        
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby == null)
            return null;
        var q=lobby.StartGame().Adapt<QuestionDTO>();
        await _playerHubContext.Clients.Group(lobbyId).SendAsync(StartGameString(),q);
        return q;
    }

    public async Task KickPlayer(LoginRequest login, string lobbyId, string playerId)
    {
        var u=await _userManager.LoginUser(login.userdto, login.password);
        if(!u.wasSuccesful)
            return ;
        
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby == null)
            return ;


        if (lobby.GetPlayerById(playerId)!=null)
        {
            lobby.LeaveLobby(playerId);
            await _playerHubContext.Clients.Group(playerId).SendAsync(KickPlayerString());
        }
    }

    public async Task AllPlayerFinished(string lobbyId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if(lobby==null)
            return;

        await Clients.Group(lobby.GetHost().UserId.ToString()).SendAsync(AllPlayerFinishedString());
    }
}
