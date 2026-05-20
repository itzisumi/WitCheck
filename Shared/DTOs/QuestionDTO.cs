namespace WebGUI.DTOs;

public record QuestionDTO(int QuestionId,int QuizId,int OrderNumber,int Points,string QuestionText,short Time);