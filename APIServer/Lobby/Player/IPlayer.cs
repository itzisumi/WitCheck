namespace APIServer.Lobby;

public interface IPlayer
{
    
    uint GetPoints();
    string GetName();
    string GetAvaterPic();
    void ChangePoints(uint points);
}