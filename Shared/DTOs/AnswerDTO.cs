namespace WebGUI.DTOs;

public record AnswerDTO(int AnswerId,string? Answer1, bool IsCorrect,int QuestionId,int OrderNumber);