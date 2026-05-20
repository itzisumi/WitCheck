using System.Formats.Asn1;
using System.Text.Json;
using Database.WitCheckEntities;
using WebGUI.DTOs;

namespace Database;

public class UserManager
{
    PasswordRepository passwordRepository;
    UserRepository userRepository;

    public UserManager(UserRepository userRepository,PasswordRepository passwordRepository)
    {
        this.userRepository = userRepository;
        this.passwordRepository = passwordRepository;
    }

    public async Task<User?> GetUserById(int UserId)
    {
        return await userRepository.GetByIdAsync(UserId);
    }

    public async Task<User> GetUserByEmail(string Email)
    {
        return (await userRepository.FindAsync(x=>x.Email == Email)).First();
    }

    public async Task<User> GetUserByUsername(string Username)
    {
        return (await userRepository.FindAsync(x=>x.Username == Username)).First();
    }

    public async Task<(bool wasSuccesful,int? UserID)> CreateUser(UserDTO userdto, string password)
    {
        try
        {
            User user = new User();
            Random rng = new Random();
            
            user.Email = userdto.Email;
            user.Username = userdto.Username;
            user.Firstname = userdto.Firstname;
            user.Lastname = userdto.Lastname;
            await userRepository.AddAsync(user);
            var temp=(await userRepository.FindAsync(x=>x.Email == userdto.Email)).First();
            await passwordRepository.AddAsync(new Password() { Password1 = password, UserId=temp.UserId});
            return (true, temp.UserId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (false, null);
        }
    }

    public async Task<(bool wasSuccesful,int? UserID)> LoginUser(UserDTO userdto, string password)
    {
        try
        { 
            Console.WriteLine(password);
            Console.WriteLine(JsonSerializer.Serialize(userdto));
            var user = (await userRepository.FindAsync(u =>
                    u.Email == userdto.Email && u.Username == userdto.Username && u.Password.Password1 == password))
                .First();
            if (user != null)
            {
                return (true, user.UserId);
            }
            else
            {
                return (false, null);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            return (false, null);
        }
        
    }

    public async Task<(bool wasSuccesful,int? UserID)> ResetPassword(UserDTO userdto, string password,string newPassword)
    {
        try
        {
            var users = await userRepository.FindAsync(u => u.UserId == userdto.UserId && u.Password.Password1 == password);
            var user = users.FirstOrDefault();
            if (user == null) return (false, null);

            var passwords = await passwordRepository.FindAsync(p => p.UserId == user.UserId);
            var pw = passwords.FirstOrDefault();
            if (pw == null) return (false, null);

            pw.Password1 = newPassword;
            await passwordRepository.UpdateAsync(pw);
            return (true, user.UserId);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return (false, null);
        }
    }
}