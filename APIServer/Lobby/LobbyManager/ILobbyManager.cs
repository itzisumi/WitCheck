using Database.WitCheckEntities;

namespace APIServer.Lobby.LobbyManager;

public interface ILobbyManager
{
    ELobbyState GetState();
    string GetLobbyId();
    User GetHost();
    List<Player> GetPlayers();
    DateTime GetTimeStamp();
    Question GetCurrentQuestion();
    Player JoinLobby(Player player);
    Player? GetPlayerById(string playerId);
    Player LeaveLobby(string playerId);
    List<Player> FinishQuestion();
    Question NextQuestion();
    Question StartGame();
    List<Player> FinishGame();
    Task<bool> PlayerAnswerQuestion(string playerId,int answerId,int timeLeft,CancellationToken c);
}
