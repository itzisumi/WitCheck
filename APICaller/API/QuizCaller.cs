using System.Text;
using System.Text.Json;
using Shared;
using WebGUI.DTOs;

namespace APICaller.API;

public class QuizCaller
{
    private HttpClient client;

    /// <summary>
    /// to created the base httpsclient
    /// </summary>
    /// <param name="baseAddress">needs to look like http://{IpAddress}:{Port}/</param>
    public QuizCaller(string baseAddress):base()
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

    private async Task SendRequest(HttpRequestMessage requestMessage)
    {
        var response= await client.SendAsync(requestMessage);
        if (response.IsSuccessStatusCode)
        {
            return;
        }
        throw new Exception(response.StatusCode.ToString()); 
    }

    private async Task<T?> SendRequest<T>(HttpRequestMessage requestMessage)
    {
        var response= await client.SendAsync(requestMessage);
        if (response.IsSuccessStatusCode)
        {
            T? result = JsonSerializer.Deserialize<T>(await response.Content.ReadAsStringAsync());
            return result;
        }
        throw new Exception(response.StatusCode.ToString()); 
    }

    #region strings

    private string baseAddress;
    private string controllerBase="/QuizCreationManager";

    #region Quiz

    private string GetQuizString(int quizId) => baseAddress+$"/GetQuiz/{quizId}";
    private string GetAllQuizzesByUserString() => baseAddress+$"/GetAllQuizzesByUser";

    private string AddQuizString() => baseAddress+$"/AddQuiz";
    private string UpdateQuizString() => baseAddress+$"/UpdateQuiz";

    private string RemoveQuizString() => baseAddress+$"/RemoveQuiz";
    private string RemoveQuizByIdString(int quizId) => baseAddress+$"/RemoveQuizById/{quizId}";

    #endregion

    #region Question

    private string GetQuestionByIdString(int questionId) => baseAddress+$"/GetQuestionById/{questionId}";
    private string GetQuestionsByQuizIdString(int quizId) => baseAddress+$"/GetQuestionsByQuizId/{quizId}";
    private string AddQuestionString() => baseAddress+$"/AddQuestion";
    private string UpdateQuestionString() => baseAddress+$"/UpdateQuestion";

    private string AddRangeQuestionsString() => baseAddress+$"/AddRangeQuestions";
    private string RemoveQuestionString() => baseAddress+$"/RemoveQuestion";
    private string RemoveQuestionByIdString(int questionId) => baseAddress+$"/RemoveQuestionById/{questionId}";

    #endregion

    #region Answer

    private string GetAnswerByIdString(int answerId) => baseAddress+$"/GetAnswerById/{answerId}";
    private string GetAnswerByQuestionIdString(int questionId) => baseAddress+$"/GetAnswerByQuestionId/{questionId}";
    private string AddAnswerString() => baseAddress+$"/AddAnswer";
    private string UpdateQAnswerString() => baseAddress+$"/UpdateAnswer";

    private string RemoveAnswerString() => baseAddress+$"/RemoveAnswer";
    private string RemoveAnswerByIdString(int answerId) => baseAddress+$"/RemoveAnswerById/{answerId}";

    #endregion

    #endregion

    #region Quiz

    public async Task<QuizDTO?> GetQuiz(int quizId,LoginRequest loginRequest)
    {
        string route= GetQuizString(quizId);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<QuizDTO>(request);
    }

    public async Task<IEnumerable<QuizDTO>?> GetAllQuizzesByUser(LoginRequest loginRequest)
    {
        string route= GetAllQuizzesByUserString();
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<IEnumerable<QuizDTO>>(request);
    }

    public async Task<QuizDTO?> AddQuiz(LoginRequest<QuizDTO> loginRequest)
    {
        string route= AddQuizString();
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Post,send);
        return await SendRequest<QuizDTO?>(request);
    }

    public async Task<QuizDTO?> UpdateQuiz(LoginRequest<QuizDTO> quiz)
    {
        string route= UpdateQuizString();
        var send = new StringContent(JsonSerializer.Serialize(quiz),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<QuizDTO?>(request);
    }

    public async Task<QuizDTO?> RemoveQuiz(LoginRequest<QuizDTO> quiz)
    {
        string route= RemoveQuizString();
        var send = new StringContent(JsonSerializer.Serialize(quiz),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Delete,send);
        return await SendRequest<QuizDTO?>(request);
    }

    public async Task<QuizDTO?> RemoveQuizById(int quizId,LoginRequest loginRequest)
    {
        string route= RemoveQuizByIdString(quizId);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Delete,send);
        return await SendRequest<QuizDTO?>(request);
    }

    #endregion

    #region Question

    public async Task<QuestionDTO?> GetQuestionById(int questionId,LoginRequest loginRequest)
    {
        string route= GetQuestionByIdString(questionId);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<QuestionDTO>(request);
    }

    public async Task<IEnumerable<QuestionDTO>?> GetQuestionsByQuizId(int quizId,LoginRequest loginRequest)
    {
        string route= GetQuestionsByQuizIdString(quizId);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<IEnumerable<QuestionDTO>>(request);
    }

    public async Task<QuestionDTO?> AddQuestion(LoginRequest<QuestionDTO> question)
    {
        string route= AddQuestionString();
        var send = new StringContent(JsonSerializer.Serialize(question),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Post,send); 
        return await SendRequest<QuestionDTO?>(request);
    }

    public async Task<QuestionDTO?> UpdateQuestion(LoginRequest<QuestionDTO> question)
    {
        string route= UpdateQuestionString();
        var send = new StringContent(JsonSerializer.Serialize(question),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<QuestionDTO?>(request);
    }

    public async Task AddRangeQuestions(LoginRequest<IEnumerable<QuestionDTO>> questions)
    {
        string route= AddRangeQuestionsString();
        var send = new StringContent(JsonSerializer.Serialize(questions),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Post,send);
        await SendRequest(request);
    }

    public async Task<QuestionDTO?> RemoveQuestion(LoginRequest<QuestionDTO> question)
    {
        string route= RemoveQuestionString();
        var send = new StringContent(JsonSerializer.Serialize(question),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Delete,send);
        return await SendRequest<QuestionDTO>(request);
    }

    public async Task<QuestionDTO?> RemoveQuestionById(int id,LoginRequest loginRequest)
    {
        string route= RemoveQuestionByIdString(id);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Delete,send);
        return await SendRequest<QuestionDTO?>(request);
    }

    #endregion

    #region Answer

    public async Task<AnswerDTO?> GetAnswerById(int AnswerId,LoginRequest loginRequest)
    {
        string route= GetAnswerByIdString(AnswerId);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<AnswerDTO>(request);
    }

    public async Task<IEnumerable<AnswerDTO>?> GetAnswerByQuestionId(int questionId,LoginRequest loginRequest)
    {
        string route= GetAnswerByQuestionIdString(questionId);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<IEnumerable<AnswerDTO>>(request);
    }

    public async Task<AnswerDTO?> AddAnswer( LoginRequest<AnswerDTO> answer)
    {   
        string route= AddAnswerString();
        var send = new StringContent(JsonSerializer.Serialize(answer),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Post,send);
        return await SendRequest<AnswerDTO?>(request);
    }

    public async Task<AnswerDTO?> UpdateAnswer(LoginRequest<AnswerDTO> answer)
    {
        Console.WriteLine(JsonSerializer.Serialize(answer));
        string route= UpdateQAnswerString();
        var send = new StringContent(JsonSerializer.Serialize(answer),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Put,send);
        return await SendRequest<AnswerDTO?>(request);
    }

    public async Task<AnswerDTO?> RemoveAnswer(LoginRequest<AnswerDTO> answer)
    {
        string route= RemoveAnswerString();
        var send = new StringContent(JsonSerializer.Serialize(answer),Encoding.UTF8, "application/json");
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Delete,send);
        return await SendRequest<AnswerDTO?>(request);
    }

    public async Task<AnswerDTO?> RemoveAnswerById(int answer,LoginRequest loginRequest)
    {
        string route= RemoveAnswerByIdString(answer);
        var send = new StringContent(JsonSerializer.Serialize(loginRequest),Encoding.UTF8, "application/json");
        
        HttpRequestMessage request = CreateRequest(route,HttpMethod.Delete,send);
        return await SendRequest<AnswerDTO?>(request);
    }

    #endregion
}