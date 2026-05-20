using WebGUI.DTOs;

namespace WebGUI.Services;

public class PlayerStateService
{
    public PlayerDTO? Player { get; private set; }
    public string? LobbyCode { get; private set; }
    public PlayerQuestionResultSnapshot? LastQuestionResult { get; private set; }

    public string? PlayerId => Player?.PlayerId;

    public void Set(PlayerDTO player, string lobbyCode)
    {
        Player = player;
        LobbyCode = lobbyCode;
        LastQuestionResult = null;
    }

    public void UpdatePlayer(PlayerDTO player)
    {
        Player = player;
    }

    public void SetLastQuestionResult(bool isCorrect, uint? totalPoints)
    {
        LastQuestionResult = new PlayerQuestionResultSnapshot(isCorrect, totalPoints);
    }

    public void ClearLastQuestionResult()
    {
        LastQuestionResult = null;
    }

    public void Clear()
    {
        Player = null;
        LobbyCode = null;
        LastQuestionResult = null;
    }
}

public sealed record PlayerQuestionResultSnapshot(bool IsCorrect, uint? TotalPoints);
