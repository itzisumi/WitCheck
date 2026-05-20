using System.Globalization;
using APICaller.API;
using APICaller.Hub;
using Shared;
using WebGUI.DTOs;

namespace TestCaller;

public class TestHubsCaller
{
    private HostHubCaller _hostHubCaller;

    private PlayerHubCaller _playerHubCaller;

    private string lobby;

    [SetUp]
    public async Task Setup()
    {
        var _lobbyCaller = new LobbyCaller("http://localhost:5203");
        lobby = await _lobbyCaller.CreateLobby(
            new UserDTO(1,"test","test","test","test@test.com"),
            "test",
            1
        );
        await _lobbyCaller.HostLobby(lobby,new LoginRequest(){password = "test",userdto = new UserDTO(1,"test","test","test","test@test.com")});
        
        _hostHubCaller = new HostHubCaller("http://localhost:5203");
        _playerHubCaller=new PlayerHubCaller("http://localhost:5203");

        await _hostHubCaller.StartHub();
        await _playerHubCaller.Start();
    }

    [Test]
    public async Task PlayerJoinTest()
    {
        await _hostHubCaller.Join(new LoginRequest(){password = "test",userdto = new UserDTO(1,"test","test","test","test@test.com")},lobby);
        string msg = "";
        _hostHubCaller.PlayerJoin(dto =>
        {
            msg = dto.Name;
        });
        await _playerHubCaller.JoinLobby(lobby,new PlayerDTO(null,"bert",100));
    }
}