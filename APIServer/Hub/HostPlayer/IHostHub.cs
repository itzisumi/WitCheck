using Shared;
using WebGUI.DTOs;

namespace APIServer.Hub;

public interface IHostHub
{
    //could it be an api call?
    //public Task Next();
    Task<QuestionDTO?> NextQuestion(LoginRequest login,string lobbyId);
    public Task AnswerTimeUp(LoginRequest login,string lobbyId);
}