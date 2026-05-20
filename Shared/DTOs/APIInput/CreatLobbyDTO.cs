namespace WebGUI.DTOs.APIInput;

public class CreateLobbyRequest
{
    public UserDTO User { get; set; }
    public string Password { get; set; }
    public int QuizId { get; set; }
}