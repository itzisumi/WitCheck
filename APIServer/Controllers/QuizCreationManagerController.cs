using Database;
using Database.DbContext;
using Database.WitCheckEntities;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Shared;
using WebGUI.DTOs;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace APIServer.Controllers;

[ApiController]
[Route("QuizCreationManager")]
public class QuizCreationManagerController:ControllerBase
{
    #region Repositories

    private QuizRepository quizRepository;
    private QuestionRepository questionRepository;
    private AnswerRepository answerRepository;
    private UserManager _userManager;

    public QuizCreationManagerController()
    {
        quizRepository = new QuizRepository(new WitCheckContext());
        questionRepository = new QuestionRepository(new WitCheckContext());
        answerRepository = new AnswerRepository(new WitCheckContext());
        _userManager = new(new(new()),new (new ()));
    }

    #endregion

    #region Quiz

    [HttpPut("GetQuiz/{quizId:int}")]
    public async Task<ActionResult<QuizDTO?>> GetQuiz([FromRoute]int quizId,[FromBody] LoginRequest loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        Quiz? q=await quizRepository.GetByIdAsync(quizId);
        if(q==null||q.UserId!=loginRequest.userdto.UserId)
            return UnprocessableEntity("Your not the owner of this quiz");
        return Ok(q.Adapt<QuizDTO>());
    }

    [HttpPut("GetAllQuizzesByUser")]
    public async Task<ActionResult<IEnumerable<QuizDTO>?>> GetAllQuizzesByUser([FromBody] LoginRequest loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity();
        List<Quiz> qs = (await quizRepository.FindAsync((quiz) => quiz.UserId == loginRequest.userdto.UserId)).ToList();
        if (qs != null)
        {
            List<QuizDTO> dtos = new List<QuizDTO>();
            foreach (Quiz q in qs)
            {
                dtos.Add(q.Adapt<QuizDTO>());
            }
            return Ok(dtos);
        }

        return UnprocessableEntity();
    }

    [HttpPost("AddQuiz")]
    public async Task<ActionResult<QuizDTO>> AddQuiz([FromBody] LoginRequest<QuizDTO> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        try
        {
            var re=await quizRepository.AddAsync(loginRequest.extrainfo.Adapt<Quiz>());
            return Ok(re.Adapt<QuizDTO>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpPut("UpdateQuiz")]
    public async Task<ActionResult<QuizDTO>> UpdateQuiz([FromBody] LoginRequest<QuizDTO> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var existing = (await quizRepository.FindAsync(x => x.QuizId == loginRequest.extrainfo.QuizId)).FirstOrDefault();
        if (existing == null)
            return UnprocessableEntity("Quiz not found");
        if (existing.UserId != loginRequest.userdto.UserId)
            return UnprocessableEntity("Your not the owner of this quiz");
        try
        {
            existing.Quizname = loginRequest.extrainfo.Quizname;
            existing.ColorCode = loginRequest.extrainfo.ColorCode;
            var re = await quizRepository.UpdateAsync(existing);
            return Ok(re.Adapt<QuizDTO>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpDelete("RemoveQuiz")]
    public async Task<ActionResult<QuizDTO>> RemoveQuiz([FromBody] LoginRequest<QuizDTO> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var quiz =(await quizRepository.FindAsync(x=>x.QuizId==loginRequest.extrainfo.QuizId)).First();
        if((quiz.UserId!=loginRequest.userdto.UserId))
            return UnprocessableEntity("Your not the owner of this quiz");

        try
        {
            var questions=await questionRepository.FindAsync(q=>q.QuizId == quiz.QuizId);
            foreach (var question in questions)
            {
                await RemoveQuestion(new LoginRequest<QuestionDTO>(new LoginRequest(loginRequest.userdto,loginRequest.password),question.Adapt<QuestionDTO>()));
            }
            var re=await quizRepository.RemoveAsync(quiz);
            return Ok(re.Adapt<QuizDTO>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpDelete("RemoveQuizById/{quizId:int}")]
    public async Task<ActionResult<QuizDTO>> RemoveQuizById([FromRoute]int quizId,[FromBody] LoginRequest loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login Failed");
        
        var q = await quizRepository.GetByIdAsync(quizId);
        if (q != null &&q.UserId==loginRequest.userdto.UserId)
        {
            try
            {
                var questions=await quizRepository.FindAsync(q=>q.QuizId == quizId);

                foreach (var question in questions)
                {
                    await RemoveQuestion(new LoginRequest<QuestionDTO>(new LoginRequest(loginRequest.userdto,loginRequest.password),question.Adapt<QuestionDTO>()));
                }
                var re=await quizRepository.RemoveAsync(q);
                return Ok(re.Adapt<QuizDTO>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return UnprocessableEntity(e.Message);
            }
        }
        return UnprocessableEntity("Your not the owner of this quiz");
    }

    #endregion

    #region Question

    [HttpPut("GetQuestionById/{questionId:int}")]
    public async Task<ActionResult<QuestionDTO>> GetQuestionById([FromRoute]int questionId,[FromBody] LoginRequest loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var q=await questionRepository.GetByIdAsync(questionId);
        if(q==null)
            return UnprocessableEntity("no question found at this id");
        var quiz=await quizRepository.FindAsync((quiz1)=>quiz1.QuizId==q.QuizId);
        if (quiz.First().UserId==loginRequest.userdto.UserId)
        {
            return Ok(q.Adapt<QuestionDTO>());
        }
        return UnprocessableEntity("Your not the owner of this quiz");
    }

    [HttpPut("GetQuestionsByQuizId/{quizId:int}")]
    public async Task<ActionResult<IEnumerable<QuestionDTO>?>> GetQuestionsByQuizId([FromRoute]int quizId,[FromBody] LoginRequest loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var q=await questionRepository.FindAsync(x=>x.QuizId==quizId);
        return Ok(q.Adapt<IEnumerable<QuestionDTO>>());
    }

    [HttpPost("AddQuestion")]
    public async Task<ActionResult<QuestionDTO>> AddQuestion([FromBody] LoginRequest<QuestionDTO> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        try
        {
            var re=await questionRepository.AddAsync(loginRequest.extrainfo.Adapt<Question>());
            return Ok(re.Adapt<QuestionDTO>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpPut("UpdateQuestion")]
    public async Task<ActionResult<QuestionDTO>> UpdateQuestion([FromBody] LoginRequest<QuestionDTO> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        if(((await quizRepository.FindAsync(x=>x.QuizId==loginRequest.extrainfo.QuizId)).First().UserId!=loginRequest.userdto.UserId))
            return UnprocessableEntity("Your not the owner of this quiz");

        try
        {
            var re=await questionRepository.UpdateAsync(loginRequest.extrainfo.Adapt<Question>());
            return Ok(re.Adapt<QuestionDTO>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpPost("AddRangeQuestions")]
    public async Task<ActionResult> AddRangeQuestions([FromBody] LoginRequest<IEnumerable<QuestionDTO>> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        try
        {
            await questionRepository.AddRangeAsync(loginRequest.extrainfo.Adapt<IEnumerable<Question>>());
            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpDelete("RemoveQuestion")]
    public async Task<ActionResult<QuestionDTO>> RemoveQuestion([FromBody] LoginRequest<QuestionDTO> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var quiz = (await quizRepository.FindAsync(x => x.QuizId == loginRequest.extrainfo.QuizId)).FirstOrDefault();
        if (quiz == null)
            return UnprocessableEntity("Quiz not found");
        if (quiz.UserId != loginRequest.userdto.UserId)
            return UnprocessableEntity("Your not the owner of this quiz");
        var question = (await questionRepository.FindAsync(x => x.QuestionId == loginRequest.extrainfo.QuestionId)).FirstOrDefault();
        if (question == null)
            return UnprocessableEntity("Question not found");
        try
        {
            var answers = (await answerRepository.FindAsync(x => x.QuestionId == loginRequest.extrainfo.QuestionId)).ToList();
            foreach (var answer in answers)
            {
                await RemoveAnswer(new LoginRequest<AnswerDTO>(new LoginRequest(loginRequest.userdto,loginRequest.password),answer.Adapt<AnswerDTO>()));
            }

            var re=await questionRepository.RemoveAsync(question);
            return Ok(question.Adapt<QuestionDTO>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpDelete("RemoveQuestionById/{id:int}")]
    public async Task<ActionResult<QuestionDTO>> RemoveQuestionById([FromRoute]int id,[FromBody] LoginRequest loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        

        var q=await questionRepository.GetByIdAsync(id);
        if(q==null)
            return UnprocessableEntity("no Question found at this id");
        var quiz=await quizRepository.FindAsync((quiz1)=>quiz1.QuizId==q.QuizId);
        if (quiz.First().UserId==loginRequest.userdto.UserId)
        {
            try
            {
                var re=await questionRepository.RemoveAsync(q);
                return Ok(re.Adapt<QuestionDTO>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return UnprocessableEntity(e.Message);
            }
        }
        return UnprocessableEntity("Your not the owner of this quiz");
    }

    #endregion

    #region Answer

    [HttpPut("GetAnswerById/{AnswerId:int}")]
    public async Task<ActionResult<AnswerDTO>> GetAnswerById([FromRoute]int AnswerId,[FromBody] LoginRequest loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var q=await answerRepository.GetByIdAsync(AnswerId);
        if(q==null)
            return UnprocessableEntity("no Answer found at this id");
        var question = await questionRepository.FindAsync((x) => x.QuestionId == q.QuestionId);
        if(!question.Any())
            return UnprocessableEntity("no question found for this answer");
        var quiz=await quizRepository.FindAsync((quiz1)=>quiz1.QuizId==question.First().QuizId);
        if (quiz.First().UserId==loginRequest.userdto.UserId)
        {
            return Ok(q.Adapt<AnswerDTO>());
        }
        return UnprocessableEntity("Your not the owner of this quiz");
    }

    [HttpPut("GetAnswerByQuestionId/{questionId:int}")]
    public async Task<ActionResult<IEnumerable<AnswerDTO>>> GetAnswerByQuestionId([FromRoute]int questionId,[FromBody] LoginRequest loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var q=await answerRepository.FindAsync(x=>x.QuestionId==questionId);
        return Ok(q.Adapt<IEnumerable<AnswerDTO>>());
    }

    [HttpPost("AddAnswer")]
    public async Task<ActionResult<AnswerDTO>> AddAnswer([FromBody] LoginRequest<AnswerDTO> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var quiz = await questionRepository.FindAsync((x) => x.QuestionId == loginRequest.extrainfo.QuestionId);

        if(!quiz.Any())
            return UnprocessableEntity("no question found at this id");
        if(((await quizRepository.FindAsync(x=>x.QuizId==quiz.First().QuizId)).First().UserId!=loginRequest.userdto.UserId))
            return UnprocessableEntity("Your not the owner of this quiz");
        try
        {
            var re=await answerRepository.AddAsync(loginRequest.extrainfo.Adapt<Answer>());
            return Ok(re.Adapt<AnswerDTO>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpPut("UpdateAnswer")]
    public async Task<ActionResult<AnswerDTO>> UpdateAnswer([FromBody] LoginRequest<AnswerDTO> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var quiz = (await questionRepository.FindAsync((x) => x.QuestionId == loginRequest.extrainfo.QuestionId)).ToList();
        if(!quiz.Any())
            return UnprocessableEntity("no question found at this id");
        var ent = await quizRepository.FindAsync(x => x.QuizId == quiz.First().QuizId);
        ent = ent.ToList();
        if(ent.First().UserId!=loginRequest.userdto.UserId)
            return UnprocessableEntity("Your not the owner of this quiz");
        var answer=loginRequest.extrainfo.Adapt<Answer>();
        answer.Question= await questionRepository.GetByIdAsync(loginRequest.extrainfo.QuestionId);
        try
        {
            var re=await answerRepository.UpdateAsync(answer);
            return Ok(re.Adapt<AnswerDTO>());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpDelete("RemoveAnswer")]
    public async Task<ActionResult<AnswerDTO>> RemoveAnswer([FromBody] LoginRequest<AnswerDTO> loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var quiz = await questionRepository.FindAsync((x) => x.QuestionId == loginRequest.extrainfo.QuestionId);
        if(!quiz.Any())
            return UnprocessableEntity("no question found at this id");
        var question = await quizRepository.FindAsync(x => x.QuizId == quiz.First().QuizId);
        if((question.First().UserId!=loginRequest.userdto.UserId))
            return UnprocessableEntity("Your not the owner of this quiz");

        try
        {
            var answer=(await answerRepository.FindAsync(a=>a.AnswerId==loginRequest.extrainfo.AnswerId)).FirstOrDefault();
            var re=await answerRepository.RemoveAsync(answer);
            return Ok(re.Adapt<AnswerDTO>());
        }
        catch (Exception e)
        {
            return UnprocessableEntity(e.Message);
        }
    }

    [HttpDelete("RemoveAnswerById/{answer:int}")]
    public async Task<ActionResult<AnswerDTO>> RemoveAnswerById([FromRoute]int answer,[FromBody] LoginRequest loginRequest)
    {
        var u=await _userManager.LoginUser(loginRequest.userdto, loginRequest.password);
        if(!u.wasSuccesful)
            return UnprocessableEntity("Login failed");
        var q = await answerRepository.GetByIdAsync(answer);
        if(q==null)
            return UnprocessableEntity("no Answer found at this id");
        var question = await questionRepository.FindAsync((x) => x.QuestionId == q.QuestionId);
        if(!question.Any())
            return UnprocessableEntity("no question found for the answer");
        var quiz=await quizRepository.FindAsync((quiz1)=>quiz1.QuizId==question.First().QuizId);
        if (quiz.First().UserId==loginRequest.userdto.UserId)
        {
            try
            {
                var re=await answerRepository.RemoveAsync(q);
                return Ok(re.Adapt<AnswerDTO>());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return UnprocessableEntity(e.Message);
            }
        }
        return UnprocessableEntity("Your not the owner of this quiz");
    }

    #endregion
}