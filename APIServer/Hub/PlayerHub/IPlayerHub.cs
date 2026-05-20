using APIServer.Lobby;
using Microsoft.AspNetCore.Mvc;
using WebGUI.DTOs;

namespace APIServer.Hub;

public interface IPlayerHub
{
    //avatar no idee how to get
    public Task<PlayerDTO> JoinLobby(string lobbyId, PlayerDTO player );
    public Task<bool> LeaveLobby(string lobbyId,string playerId);
}