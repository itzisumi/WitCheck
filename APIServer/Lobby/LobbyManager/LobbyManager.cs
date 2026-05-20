using APIServer.Hub;
using Database.WitCheckEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;

namespace APIServer.Lobby.LobbyManager;

public class LobbyManager:ILobbyManager
{
    public LobbyManager(User Host,Quiz Quiz,string LobbyId)
    {
        this.Host = Host;
        this.LobbyId = LobbyId;
        this.Quiz = Quiz;
        CurrentQuestion = Quiz.Questions
            .OrderBy(q => q.OrderNumber)
            .First();
        State = ELobbyState.InActive;
        Players = new();
        TimeStamp = DateTime.Now;
    }

    #region Variables

    private User Host{ get; set; }
    private List<Player> Players{ get; set; }
    private string LobbyId{ get; set; }
    private Quiz Quiz{ get; set; }
    private Question CurrentQuestion{ get; set; }
    private ELobbyState State{ get; set; }
    private DateTime TimeStamp{ get; set; }

    #endregion

    #region Gets

    public ELobbyState GetState() => State;
    public string GetLobbyId() => LobbyId;
    public User GetHost() => Host;
    public List<Player> GetPlayers() => Players;
    public DateTime GetTimeStamp() => TimeStamp;
    public Question GetCurrentQuestion() => CurrentQuestion;
    public Quiz GetQuiz() => Quiz;

    #endregion

    #region Methods

    public Player JoinLobby(Player player)
    {
        if (GetState() != ELobbyState.Active)
            throw new Exception();
        
        if (Players.All(x => x.Name != player.Name))
        {
            Players.Add(player);
            return player;
        }
        else
        {
            Player newPlayer;
            if (player.AvaterPic is not null)
            {
                newPlayer=new Player(player.Name, LobbyId,player.ConnectionId, player.AvaterPic);
            }
            else
            {
                newPlayer=new Player(player.Name, LobbyId,player.ConnectionId);
            }
            Players.Add(newPlayer);
            return newPlayer;
        }
    }


    public Player? GetPlayerById(string playerId)
    {
        return GetPlayers().Find(x => x.PlayerId == playerId);
    }

    public Player LeaveLobby(string playerId)
    {
        Player p= GetPlayers().Find(x => x.PlayerId == playerId);
        Players.Remove(p);
        return p;
    }

    public List<Player> FinishQuestion()
    {
        if (GetState() != ELobbyState.Running)
            throw new Exception();

        foreach (var player in GetPlayers())
        {
            if (!player.AnsweredCurrentQuestion)
            {
                player.AnswerForCurrentQuestion = null;
                player.LastQuestionWasCorrect = false;
            }

            player.AnsweredCurrentQuestion = true;
        }

        SetPlayersPosition();
        return GetPlayers();
    }

    private List<Player> SetPlayersPosition()
    {
        var l = GetPlayers().OrderByDescending(x => x.GetPoints()).ToList();
        for (int i = 1; i < l.Count; i++)
        {
            l[i-1].Position=(uint)i;
        }
        return l;
    }

    private void SetPlayersAnwseredQuestion(bool bl)
    {
        foreach (var pl in GetPlayers())
        {
            pl.AnsweredCurrentQuestion = bl;
            pl.AnswerForCurrentQuestion = null;
        }
    }

    public Question? NextQuestion()
    {
        if (GetState() != ELobbyState.Running)
            throw new Exception();

        var nextQuestion = Quiz.Questions
            .Where(q => q.OrderNumber > CurrentQuestion.OrderNumber)
            .OrderBy(q => q.OrderNumber)
            .FirstOrDefault();

        if (nextQuestion != null)
        {
            CurrentQuestion = nextQuestion;
            SetPlayersAnwseredQuestion(false);
            return CurrentQuestion;
        }

        return null;
    }

    public void HostLobby()
    {
        if (GetState() != ELobbyState.InActive)
            throw new Exception();
        State = ELobbyState.Active;
    }

    public Question StartGame()
    {
        if (GetState() == ELobbyState.InActive)
            throw new Exception();
        SetPlayersAnwseredQuestion(false);
        State=ELobbyState.Running;
        return CurrentQuestion;
    }

    public List<Player> FinishGame()
    {
        if (GetState() != ELobbyState.Running)
            throw new Exception();
        SetPlayersAnwseredQuestion(true);
        State=ELobbyState.Done;
        return Players;
    }

    public Task<bool> PlayerAnswerQuestion(string playerId, int answerId,int timeLeft,CancellationToken c =default(CancellationToken))
    {
        if (GetState() != ELobbyState.Running)
            throw new Exception();

        var player = Players.Find(x => x.PlayerId == playerId)
            ?? throw new Exception("player not found");

        if (player.AnsweredCurrentQuestion)
            return Task.FromResult(false);

        if (answerId == -1)
        {
            player.AnsweredCurrentQuestion = true;
            player.AnswerForCurrentQuestion = null;
            player.LastQuestionWasCorrect = false;
            return Task.FromResult(Players.All(x => x.AnsweredCurrentQuestion));
        }

        var ans = CurrentQuestion.Answers.FirstOrDefault(x => x.AnswerId == answerId)
            ?? throw new Exception("answer not found");

        player.AnsweredCurrentQuestion = true;
        player.AnswerForCurrentQuestion = ans;
        player.LastQuestionWasCorrect = ans.IsCorrect.Equals(1);

        CurrentQuestion.Time = CurrentQuestion.Time <= 0 ? 1 : CurrentQuestion.Time;
        
        if (ans.IsCorrect.Equals(1))
        {
            uint losPerSecond = (uint)(CurrentQuestion.Points / CurrentQuestion.Time);
            uint pointGained = (uint)(CurrentQuestion.Points - losPerSecond * (CurrentQuestion.Time - timeLeft));
            player.ChangePoints(pointGained);
        }

        return Task.FromResult(Players.All(x => x.AnsweredCurrentQuestion));
    }

    #endregion
}
