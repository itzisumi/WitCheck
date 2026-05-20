using APICaller.API;
using Shared;
using WebGUI.DTOs;

namespace TestCaller;

public class TestLobbyCaller
{
    private LobbyCaller _lobbyCaller;

    [SetUp]
    public async Task Setup()
    {
        _lobbyCaller = new LobbyCaller("http://localhost:5203");
        var str=await _lobbyCaller.CreateLobby(
            new UserDTO(1,"test","test","test","test@test.com"),
            "test",
            1
            );
        await _lobbyCaller.HostLobby(str,new LoginRequest(){password = "test",userdto = new UserDTO(1,"test","test","test","test@test.com")});
    }

    [Test]
    public void GetAllPlayersTest()
    {
        //_lobbyCaller.GetAllPlayers();
    }
}