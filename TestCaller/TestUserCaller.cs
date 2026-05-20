using APICaller.API;
using Mapster;
using WebGUI.DTOs;

namespace TestCaller;

public class TestUserCaller
{
    private UserDTO _user;
    private UserCaller _userCaller;

    [TearDown]
    public async Task TearDown()
    {
        
    }

    [SetUp]
    public async Task Setup()
    {
        _userCaller = new UserCaller("http://localhost:5203");
        try
        {
            _user=await _userCaller.CreateUser(new UserDTO(1,"test","test","test","test@test.com"),"test");
        }
        catch (Exception e)
        {
            var temp = await _userCaller.GetUserByUsername("test");
            Console.WriteLine("user exists");
            _user = temp;
        }
    }

    [Test]
    public async Task Test()
    {
        try
        {
            await _userCaller.LoginUserRoute(_user, "test");
            Assert.Pass();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            if(e.GetType()==typeof(SuccessException))
                Assert.Pass();
        }
    }
}