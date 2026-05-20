using APIServer.Lobby;
using APIServer.Manager;
using Mapster;
using Microsoft.AspNetCore.SignalR;
using WebGUI.DTOs;

namespace APIServer.Hub;

public class PlayerHub : Microsoft.AspNetCore.SignalR.Hub, IPlayerHub
{
    private readonly IHubContext<HostHub> _hostHubContext;
    private string PlayerJoin = "PlayerJoin";
    private string PlayerLeave = "PlayerLeave";

    public PlayerHub(IHubContext<HostHub> hostHubContext)
    {
        _hostHubContext = hostHubContext;
    }

    public async Task<PlayerDTO> JoinLobby(string lobbyId, PlayerDTO player)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby != null)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            
            var playerChanged = string.IsNullOrWhiteSpace(player.AvaterPic)
                ? new Player(player.Name, lobbyId, Context.ConnectionId)
                : new Player(player.Name, lobbyId, Context.ConnectionId, player.AvaterPic);
            
            await Groups.AddToGroupAsync(Context.ConnectionId,playerChanged.PlayerId);
            
            var p= lobby.JoinLobby(playerChanged).Adapt<PlayerDTO>();
            var client =  _hostHubContext.Clients.Group(lobby.GetHost().UserId.ToString());
            await client.SendAsync(PlayerJoin,p.Adapt<PlayerDTO>());
            return p;
        }
        
        return new PlayerDTO(null,"",0);
    }

    public async Task<bool> LeaveLobby(string lobbyId, string playerId)
    {
        var lobby = GeneralManager.GetLobbyByLobbyId(lobbyId);
        if (lobby != null)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobbyId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, playerId);
            var p=lobby.LeaveLobby(playerId);
            await _hostHubContext.Clients.Group(lobby.GetHost().UserId.ToString()).SendAsync(PlayerLeave,p.Adapt<PlayerDTO>());
            return true;
        }

        return false;
    }
}
    
