using System.Text;
using System.Text.Json;
using Shared;
using WebGUI.DTOs;
using WebGUI.DTOs.APIInput;

namespace APICaller.API;

public class LobbyCaller
{
    private HttpClient client;

    /// <summary>
    /// to created the base httpsclient
    /// </summary>
    /// <param name="baseAddress">needs to look like http://{IpAddress}:{Port}/</param>
    public LobbyCaller(string baseAddress):base()
    {
        this.baseAddress = baseAddress+controllerBase;
        client = new HttpClient(){BaseAddress = new Uri(baseAddress)};
    }

    private HttpRequestMessage CreateRequest(string route,HttpMethod method,HttpContent? content = null)
    {
        var request = new HttpRequestMessage
        {
            Method = method,
            RequestUri = new Uri(route),
            Headers =
            {
                { "accept", "application/json" },
            },
            Content = content
        };
        return request;
    }

    private async Task<T?> SendRequest<T>(HttpRequestMessage requestMessage)
    {
        var response= await client.SendAsync(requestMessage);
        if (response.IsSuccessStatusCode)
        {
            T? result = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
            return result;
        }

        var errorMessage = await response.Content.ReadAsStringAsync();
        if (string.IsNullOrWhiteSpace(errorMessage))
        {
            errorMessage = response.StatusCode.ToString();
        }
        else
        {
            errorMessage = $"{response.StatusCode}: {errorMessage}";
        }

        throw new Exception(errorMessage); 
    }

    public async Task<string?> GetLobbyIdByHostId(int hostId)
    {
        string route= GetLobbyIdByHostIdString(hostId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<string>(request);
    }

    public async Task<UserDTO?> GetLobbyHost(string lobbyId)
    {
        string route= GetLobbyHostString(lobbyId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<UserDTO>(request);
    }

    public async Task<IEnumerable<PlayerDTO>?> GetAllPlayers(string lobbyId)
    {
        string route= GetAllPlayersString(lobbyId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<IEnumerable<PlayerDTO>>(request);
    }

    public async Task<IEnumerable<AnswerDTO>?> GetAllAnswers(string lobbyId)
    {
        string route= GetAllAnswersString(lobbyId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<IEnumerable<AnswerDTO>>(request);
    }

    public async Task<string?> GetQuizColor(string lobbyId)
    {
        string route = GetQuizColorString(lobbyId);
        HttpRequestMessage request = CreateRequest(route, HttpMethod.Get);
        return await SendRequest<string>(request);
    }


    public async Task<bool?> AnswerQuestion(string lobbyId,string playerId,int answerId,int timeLeft)
    {
        string route= AnswerQuestionString(lobbyId,playerId);
        var a =new
        {
            answerId=answerId,
            timeLeft=timeLeft
        };
        var send = new StringContent(JsonSerializer.Serialize(a),Encoding.UTF8, "application/json");
            
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<bool>(request);
    }

    public async Task<QuestionDTO?> GetQuestion(string lobbyId)
    {
        string route= GetQuestionString(lobbyId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<QuestionDTO>(request);
    }

    public async Task<IEnumerable<AnswerDTO>?> GetQuestionAnswer(string lobbyId)
    {
        string route= GetQuestionAnswerString(lobbyId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<IEnumerable<AnswerDTO>>(request);
    }

    public async Task<int?> GetQuestionPoints(string lobbyId)
    {
        string route= GetQuestionPointsString(lobbyId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<int>(request);
    }

    public async Task<uint?> GetPlayerPoints(string lobbyId,string playerId)
    {
        string route= GetPlayerPointsString(lobbyId,playerId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<uint>(request);
    }

    public async Task<bool?> GetPlayerLastIsCorrect(string lobbyId,string playerId)
    {
        string route = GetPlayerLastIsCorrectString(lobbyId, playerId);
        HttpRequestMessage request = CreateRequest(route, HttpMethod.Get);
        return await SendRequest<bool?>(request);
    }

    public async Task<uint?> GetPlayerPosition(string lobbyId,string playerId)
    {
        string route= GetPlayerPositionString(lobbyId,playerId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<uint>(request);
    }


    public async Task<string?> CreateLobby(UserDTO userDto, string password, int quizId)
    {
        string route= CreateLobbyString();
        var send = new StringContent(JsonSerializer.Serialize(new CreateLobbyRequest()
            { User = userDto, Password = password, QuizId = quizId }),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<string>(request);
    }

    public async Task<bool> HostLobby(string lobbyId,LoginRequest loginRequest)
    {
        string route= HostLobbyString(lobbyId);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<bool>(request);
    }

    public async Task<bool> DeleteLobby(string lobbyId,LoginRequest loginRequest)
    {
        string route= DeleteLobbyString(lobbyId);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<bool>(request);
    }

    public async Task<Dictionary<int,int>?> GetAnswersAsDict(string lobbyId)
    {
        string route= GetAnswersAsDictString(lobbyId);
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Get);
        return await SendRequest<Dictionary<int,int>>(request);
    }

    #region strings

    private string baseAddress;
    private string controllerBase = "/Lobbies";

    private string GetLobbyIdByHostIdString(int HostId) => baseAddress+$"/LobbyIdByHost/{HostId}";
    private string GetLobbyHostString(string LobbyId) => baseAddress+$"/LobbyHost/{LobbyId}";
    private string GetAllPlayersString(string LobbyId) => baseAddress+$"/AllPlayers/{LobbyId}";
    private string GetAllAnswersString(string LobbyId) => baseAddress+$"/AllAnswers/{LobbyId}";
    private string GetQuizColorString(string LobbyId) => baseAddress + $"/QuizColor/{LobbyId}";

    private string AnswerQuestionString(string lobbyId,string playerId) => baseAddress+$"/AnswerQuestion/{lobbyId}/{playerId}";
    private string GetQuestionString(string lobbyId) => baseAddress+$"/GetQuestion/{lobbyId}";
    private string GetQuestionAnswerString(string lobbyId) => baseAddress+$"/GetQuestionAnswer/{lobbyId}";
    private string GetQuestionPointsString(string lobbyId) => baseAddress+$"/GetQuestionPoints/{lobbyId}";
    private string GetPlayerPointsString(string lobbyId,string playerId) => baseAddress+$"/GetPlayerPoints/{lobbyId}/{playerId}";
    private string GetPlayerLastIsCorrectString(string lobbyId,string playerId) => baseAddress+$"/GetPlayerLastIsCorrect/{lobbyId}/{playerId}";
    private string GetPlayerPositionString(string lobbyId,string playerId) => baseAddress+$"/GetPlayerPosition/{lobbyId}/{playerId}";
    private string CreateLobbyString() => baseAddress+$"/CreateLobby";

    private string HostLobbyString(string lobbyId) => baseAddress+$"/HostLobby/{lobbyId}";

    private string DeleteLobbyString(string lobbyId) => baseAddress+$"/DeleteLobby/{lobbyId}";

    private string GetAnswersAsDictString(string lobbyId) => baseAddress+$"/GetAnswersAsDict/{lobbyId}";

    #endregion
}
