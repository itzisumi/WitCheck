using System.Runtime.InteropServices.Marshalling;
using WebGUI.DTOs;

namespace Shared.Builder;

public class QuizBuilder
{
    private List<AnswerDTO> answers=new List<AnswerDTO>();
    private List<QuestionDTO> questions=new List<QuestionDTO>();
    private QuizDTO quiz;

    public void SetQuiz(QuizDTO quiz)
    {
        this.quiz = quiz;
    }

    public void AddQuestion(QuestionDTO question)
    {
        questions.Add(question);
    }

    public void RemoveQuestion(QuestionDTO question)
    {
        questions.Remove(question);
    }

    public void RemoveQuestionById(int questionId)
    {
        questions.Remove(questions.First(q => q.QuestionId == questionId));
    }

    public void AddAnswer(AnswerDTO answer,int QuestionId)
    {
         answers.Add(new AnswerDTO(answer.AnswerId,answer.Answer1,answer.IsCorrect,QuestionId,answer.OrderNumber));
    }

    public void RemoveAnswer(AnswerDTO answer, int QuestionId)
    {
        answers.Remove(new AnswerDTO(answer.AnswerId,answer.Answer1,answer.IsCorrect,QuestionId,answer.OrderNumber));
    }

    public void RemoveAnswerById(int answerId, int QuestionId)
    {
        answers.Remove(answers.Find(X => X.AnswerId == answerId && X.QuestionId == QuestionId));
    }
}