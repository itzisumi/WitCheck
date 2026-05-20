using System.Timers;
using APIServer.Hub;
using APIServer.Lobby.LobbyManager;
using Database;
using Database.DbContext;
using Database.WitCheckEntities;
using Mapster;
using WebGUI.DTOs;

namespace APIServer.Manager;

public abstract class GeneralManager:IManager
{
    private GeneralManager()
    {
        
    }

    /// <summary>
    /// string for lobbyId
    /// </summary>
    private static Dictionary<string, LobbyManager> LobbyManagers { get; } = new();

    private static UserManager UserManager { get; }= new (new (new WitCheckContext()),
    new (new WitCheckContext()));

    public static LobbyManager? GetLobbyByLobbyId(string lobbyId)
    {
        if(LobbyManagers.Keys.Contains(lobbyId))
            return LobbyManagers[lobbyId];
        return null;
    }

    public static LobbyManager? GetLobbyByHostId(int hostId)
    {
        if(LobbyManagers.Values.ToList().Find(x=>x.GetHost().UserId==hostId)!=null)
            return LobbyManagers["lobbyId"];
        return null;
    }

    public static async Task<LobbyManager?> AddLobby(User host, Quiz quiz)
    {
        var lobbies = LobbyManagers.Where(x => x.Value.GetHost().UserId == host.UserId);
        var any =  new List<string>();
        if (lobbies.Any())
        {
            foreach (var lob in lobbies)
            {
                if (lob.Value.GetState() == ELobbyState.Done)
                {
                    await GeneralManager.DeleteLobby(lob.Key, host.UserId);
                }
                else
                {
                    any.Add(lob.Key);
                }
            }
        }

        if (any.Any())
            return null;
        
        string key = HashCode.Combine(host.UserId, quiz.QuizId).ToString();
        LobbyManager lobby = new LobbyManager(host,quiz,key);
        LobbyManagers.Add(key,lobby);
        return LobbyManagers[key];
    }

    public static async Task<LobbyManager?> DeleteLobby(string LobbyId, int id)
    {
        var existing = GetLobbyByLobbyId(LobbyId);
        if (existing == null || existing.GetHost().UserId != id)
            return null;
        if(LobbyManagers.ContainsKey(LobbyId))
        {
            var l= LobbyManagers[LobbyId];
            LobbyManagers.Remove(LobbyId);
            return l;
        }
            
        return null;
    }

    public static int CleanLobbies()
    {
        List<string> delKeys = new();
        foreach (var k in LobbyManagers)
        {
            switch (k.Value.GetState())
            {
                case ELobbyState.Done:
                    if (k.Value.GetTimeStamp().Subtract(DateTime.Now).TotalMinutes >= 20)
                        delKeys.Add(k.Key);
                    break;
                case ELobbyState.InActive:                
                    if (k.Value.GetTimeStamp().Subtract(DateTime.Now).TotalMinutes >= 30)
                        delKeys.Add(k.Key);
                    break;
                case ELobbyState.Running:
                    if (k.Value.GetTimeStamp().Subtract(DateTime.Now).TotalMinutes >= 60)
                        delKeys.Add(k.Key);
                    break;
            }
        }

        int delNum = 0;
        foreach (var delKey in delKeys)
        {
            if (LobbyManagers.Remove(delKey))
                delNum++;
        }
        return delNum;
    }

    public static Dictionary<string, LobbyManager> GetAllLobbies() => LobbyManagers;

    /// <summary>
    /// timers is for the clean timer
    /// </summary>
    #region timer
    
    private static System.Timers.Timer timer;

    private static double intervale = 60000;

    public static void SetIntervale(double intervalInMs)
    {
        intervale = intervalInMs;
    }

    public static void StartCleanTimer()
    {
        timer = new System.Timers.Timer();
        timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
        timer.Interval = intervale; 
        timer.Start();
    }

    public static void StopCleanTimer()
    {
        timer.Stop();
        timer.Close();
    }

    // Specify what you want to happen when the Elapsed event is raised.
    private static void OnTimedEvent(object source, ElapsedEventArgs e)
    {
        Console.WriteLine("lobbiesCleaned:"+CleanLobbies());
    }

    #endregion
}