using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;
using WebGUI.DTOs;

namespace APICaller.Hub;

public class HostHubCaller
{
    private HubConnection hubConnection;

    public HostHubCaller(string baseAddress)
    {
        hubConnection = new HubConnectionBuilder()
            .WithUrl(baseAddress+"/HostHub", options =>
            {
                options.Transports = HttpTransportType.WebSockets | HttpTransportType.LongPolling;
            })
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task StartHub()
    {
        if (hubConnection.State == HubConnectionState.Disconnected)
            await hubConnection.StartAsync();
    }

    public async Task Join(LoginRequest login,string lobbyId)
    {
        await hubConnection.InvokeAsync(HostStartLobbyString() ,login,lobbyId);
    }

    public async Task Leave(LoginRequest login,string lobbyId)
    {
        await hubConnection.InvokeAsync(HostEndLobbyString(),login,lobbyId);
    }

    public async Task<QuestionDTO?> NextQuestion(LoginRequest login,string lobbyId)
    {
        return await hubConnection.InvokeAsync<QuestionDTO?>(NextQuestionString(),login,lobbyId);
    }

    public async Task AnswerTimeUp(LoginRequest login,string lobbyId)
    {
         await hubConnection.InvokeAsync(AnswerTimeUpString(),login,lobbyId);
    }

    public async Task<QuestionDTO?> StartQuiz(LoginRequest login,string lobbyId)
    {
        return await hubConnection.InvokeAsync<QuestionDTO?>(StartString(),login,lobbyId);
    }

    public async Task KickPlayer(LoginRequest login, string lobbyId, string playerId)
    {
        await hubConnection.InvokeAsync(KickPlayerString(),login,lobbyId,playerId);
    }

    public IDisposable PlayerJoin(Action<PlayerDTO> func)=> 
        hubConnection.On<PlayerDTO>(PlayerJoinString(), func);

    public IDisposable PlayerJoin(Func<PlayerDTO> func)=>
         hubConnection.On<PlayerDTO>(PlayerJoinString(), func);

    public IDisposable PlayerJoin(Func<PlayerDTO,Task> func)=>
         hubConnection.On<PlayerDTO>(PlayerJoinString(), func);


    public IDisposable PlayerLeave(Action<PlayerDTO> func)=>
         hubConnection.On<PlayerDTO>(PlayerLeaveString(), func);

    public IDisposable PlayerLeave(Func<PlayerDTO> func)=>
         hubConnection.On<PlayerDTO>(PlayerLeaveString(), func);

    public IDisposable PlayerLeave(Func<PlayerDTO,Task> func)=>
         hubConnection.On<PlayerDTO>(PlayerLeaveString(), func);


    public IDisposable AllPlayerFinished(Func<Task> func)=>
        hubConnection.On(AllPlayerFinishedString(), func);

    public IDisposable AllPlayerFinished(Action func)=>
        hubConnection.On(AllPlayerFinishedString(), func);

    private static string HostStartLobbyString() => "HostStartLobby";
    private static string HostEndLobbyString() => "HostEndLobby";
    private static string NextQuestionString() => "NextQuestion";
    private static string AnswerTimeUpString() => "AnswerTimeUp";
    private static string StartString() => "Start";
    private static string PlayerJoinString() => "PlayerJoin";
    private static string PlayerLeaveString() => "PlayerLeave";
    private static string KickPlayerString() => "KickPlayer";
    private static string AllPlayerFinishedString() => "AllPlayerFinished";
}
