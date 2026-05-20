using Database.WitCheckEntities;

namespace APIServer.Lobby;

public class Player:IPlayer
{
    public Player()
    {
        Name = "";
        AnsweredCurrentQuestion = true;
        PlayerId = "";
        Points = 0;
        Position = 0;
        LastQuestionWasCorrect = null;
    }

    public Player(string name,string lobbyId,string connectionId)
    {
        Name = name;
        Points = 0;
        Position = 0;
        AnsweredCurrentQuestion=true;
        ConnectionId = connectionId;
        PlayerId = Guid.NewGuid().ToString();
        LastQuestionWasCorrect = null;
    }

    public Player(string name,string lobbyId,string connectionId,string AvaterPic):this(name,lobbyId,connectionId)
    {
        this.AvaterPic = AvaterPic;
    }

    public string ConnectionId { get; private set; }
    public string PlayerId { get; private set; }
    public string Name { get; init; }
    public uint Points { get; private set; }
    public string? AvaterPic { get; init; } = null;
    public uint Position { get; set; }
    public bool AnsweredCurrentQuestion { get; set; }
    public Answer? AnswerForCurrentQuestion { get; set; } = null;
    public bool? LastQuestionWasCorrect { get; set; }

    public uint GetPoints() => Points;

    public string GetName() => Name;

    public string GetAvaterPic() => AvaterPic;

    public void ChangePoints(uint points)
    {
        Points += points;
    }

    public string SetConnectionId(string id) => ConnectionId=id;

    public void GenerateId(string lobbyId)
    {
        PlayerId = Guid.NewGuid().ToString();
    }
}
