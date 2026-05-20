using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using WebGUI.DTOs;

namespace APICaller.Hub;

public class PlayerHubCaller
{
    private IDisposable Connection;
    private HubConnection hubConnection;

    public PlayerHubCaller(string baseAddress)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(baseAddress + "/PlayerHub",
                options => { options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling; })
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task Start()
    {
        if(hubConnection.State==HubConnectionState.Disconnected)
            await hubConnection.StartAsync();
    }

    public async Task<PlayerDTO> JoinLobby(string lobbyId, PlayerDTO player)
    {
        return await hubConnection.InvokeAsync<PlayerDTO>(JoinLobbyString(), lobbyId, player);
    }

    public async Task<bool> LeaveLobby(string lobbyId, string playerId)
    {
        return await hubConnection.InvokeAsync<bool>(LeaveLobbyString(), lobbyId, playerId);
    }

    public IDisposable NextQuestion(Action<QuestionDTO?,PlayerDTO?> func) =>
        hubConnection.On(NextQuestionString(), func);

    public IDisposable NextQuestion(Func<QuestionDTO?,PlayerDTO?, Task> func) =>
        hubConnection.On(NextQuestionString(), func);


    public IDisposable FinishedQuestion(Action<bool> func) =>
        hubConnection.On<bool>(FinishedQuestionString(), func);

    public IDisposable FinishedQuestion(Func<bool> func) =>
        hubConnection.On<bool>(FinishedQuestionString(), func);

    public IDisposable FinishedQuestion(Func<bool, Task> func) =>
        hubConnection.On<bool>(FinishedQuestionString(), func);

    public IDisposable StartGame(Action<QuestionDTO?> func) =>
        hubConnection.On<QuestionDTO?>(StartGameString(), func);

    public IDisposable StartGame(Func<QuestionDTO?> func) =>
        hubConnection.On<QuestionDTO?>(StartGameString(), func);

    public IDisposable StartGame(Func<QuestionDTO?, Task> func) =>
        hubConnection.On<QuestionDTO?>(StartGameString(), func);

    public IDisposable KickPlayer(Action func)=>
        hubConnection.On(KickPlayerString(), func);

    public IDisposable GameOver(Func<Task> func) =>
        hubConnection.On(GameOverString(), func);

    public IDisposable GameOver(Action func) =>
        hubConnection.On(GameOverString(), func);

    private static string JoinLobbyString() => "JoinLobby";
    private static string LeaveLobbyString() => "LeaveLobby";
    private static string NextQuestionString() => "NextQuestion";
    private static string FinishedQuestionString() => "FinishedQuestion";
    private static string StartGameString() => "StartGame";
    private static string KickPlayerString() => "KickPlayer";
    private static string GameOverString() => "GameOver";
}