namespace WebGUI.ViewModels;

public class QuestionVM {
    public int QuestionId { get; set; }
    public int QuizId { get; set; }
    public int OrderNumber { get; set; }
    public int Points { get; set; }
    public string QuestionText { get; set; } = "";
    public int Time { get; set; }
}
