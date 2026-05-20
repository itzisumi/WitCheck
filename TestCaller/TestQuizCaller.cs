using APICaller.API;
using Shared;
using WebGUI.DTOs;

namespace TestCaller;

public class TestQuizCaller
{
    private QuizCaller _quizCaller;
    private LoginRequest _user;

    [SetUp]
    public async Task Setup()
    {
        _user = new LoginRequest()
        {
            password = "test",
            userdto = new UserDTO(1, "test", "test", "test", "test@test.com")
        };
        
        _quizCaller = new QuizCaller("http://localhost:5203");
        try
        {
            await _quizCaller.AddQuiz(new LoginRequest<QuizDTO>(_user,new QuizDTO(1, "test", 1, "#FFFFFF")));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
        try
        {
            await _quizCaller.AddQuestion(new (_user,new QuestionDTO(1, 1,1,1000,"test",30)));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
    }

    [Test]
    public async Task GetAllQuizesByPlayerTest()
    {
        var result=await _quizCaller.GetAllQuizzesByUser(_user);
        if(result.Any())
            Assert.Pass();
        else
            Assert.Fail();
    }
}