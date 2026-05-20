using APIServer.Lobby.LobbyManager;
using Database.WitCheckEntities;

namespace APIServer.Manager;

public interface IManager
{
    public static abstract LobbyManager? GetLobbyByLobbyId(string lobbyId);
    public static abstract LobbyManager? GetLobbyByHostId(int hostId);
    public static abstract Task<LobbyManager?> AddLobby(User host, Quiz quiz);
    public static abstract Task<LobbyManager?> DeleteLobby(string LobbyId, int host);

    /// <summary>
    /// to get ride of finsihed games
    /// </summary>
    /// <returns>int for how many lobbies got cleaned up</returns>
    public static abstract int CleanLobbies();
}