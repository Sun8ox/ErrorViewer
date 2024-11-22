using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks.Dataflow;
using ErrorViewer.Data;
using ErrorViewer.Models;
using Microsoft.AspNetCore.Mvc;

namespace ErrorViewer.Controllers;

public class AuthController : Controller
{
    public static readonly string cookieName = ".ErrorViewer.SessionID";
    
    public readonly ErrorViewerDB _database;
    public static Dictionary<string, User> loggedInUsers = new();
    public static readonly Random Random = new();
    
    public AuthController(ErrorViewerDB context)
    {
        _database = context;
    }

    public static string generatePasswordHash(string password)
    {
        string hash = "";

        try
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                hash = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        return hash;
    }

    public static string generateUniqueToken(string username, string host)
    {
        string output = "";
        
        // Kinda solid unique token generation ngl
        using (SHA256 sha256 = SHA256.Create())
        {
            output = username + Convert.ToBase64String(
                sha256.ComputeHash(
                    Encoding.UTF8.GetBytes(
                        username + host + DateTime.Now + Random.NextInt64(100000, 999999) 
                        )));
        }

        return output;
    }
    

    [HttpPost]
    public IActionResult Login(LoginUserModel? model)
    {

        if (model == null)
        {
            return Redirect($"/Login/Index/?error=InvalidUsernameOrPassword");
        }
        if (model.Username == null || model.Password == null)
        {
            return Redirect($"/Login/Index/?error=InvalidUsernameOrPassword");
        }
        
        var user = _database.Users.FirstOrDefault(u => u.Username == model.Username);
        if (user == null)
        {
            return Redirect($"/Login/Index/?error=InvalidUsernameOrPassword");
        }
        
        var hash = generatePasswordHash(model.Password);
        if (user.PasswordHash != hash)
        {
            return Redirect($"/Login/Index/?error=InvalidUsernameOrPassword");
        }
        
        if (user.isBanned)
        {
            return Redirect($"/Login/Index/?error=UserBanned");
        }

        if(user.Username == model.Username && user.PasswordHash == hash)
        {
            var sessionId = generateUniqueToken(user.Username, Request.Host.Value);
            Response.Cookies.Append(cookieName, sessionId);
            loggedInUsers.Add(sessionId, user);
            return Redirect($"/Dashboard/Index/");
        }

        return Redirect($"/Dashboard/Index/");
    }

    [HttpPost]
    public IActionResult RegisterUser(RegisterUserModel model)
    {
        var sessionId = Request.Cookies[cookieName];

        if (sessionId != null)
        {
            if (loggedInUsers.ContainsKey(sessionId))
            {
                var user = loggedInUsers[sessionId];
                if (!user.isAdmin) return Json(new SimpleError() { error = "NotAdmin" });
                if (model.Username == null || model.Password == null) return Json(new SimpleError() { error = "InvalidUsernameOrPassword" });
                if (model.Username.Length < 3) return Json(new SimpleError() { error = "UsernameTooShort" });
                if (model.Password.Length < 7) return Json(new SimpleError() { error = "PasswordTooShort" });
                
                if (_database.Users.Any(u => u.Username == model.Username)) return Json(new SimpleError() { error = "UsernameTaken" });
                

                var userToAdd = new User()
                {
                    Username = model.Username,
                    PasswordHash = generatePasswordHash(model.Password),
                    isAdmin = model.isAdmin,
                    isBanned = model.isBanned,
                    CreatedAt = DateOnly.FromDateTime(DateTime.Now)
                };
                _database.Users.Add(userToAdd);
                _database.SaveChanges();
                
                return Ok();
                
            }
        }
        
        return NoContent();
    }

    [HttpPost]
    public IActionResult EditUser(EditUserModel model)
    {
        Console.WriteLine(JsonSerializer.Serialize(model));
        
        var sessionId = Request.Cookies[cookieName];

        if (sessionId != null)
        {
            if (loggedInUsers.ContainsKey(sessionId))
            {
                var user = loggedInUsers[sessionId];
                if (!user.isAdmin) return Json(new SimpleError() { error = "NotAdmin" });
                if (user.isBanned) return Json(new SimpleError() { error = "UserBanned" });
                
                var userToEdit = _database.Users.FirstOrDefault(u => u.Username == model.Username);
                if (userToEdit == null) return Json(new SimpleError() { error = "UserNotFound" });
                
                if (model.PasswordToChange != null && model.PasswordToChange.Length > 0)
                {
                    if (model.PasswordToChange.Length < 7) return Json(new SimpleError() { error = "PasswordTooShort" });
                    
                    var hash = generatePasswordHash(model.PasswordToChange);
                    userToEdit.PasswordHash = hash;
                }
                
                if (model.isBanned != null) userToEdit.isBanned = model.isBanned.Value;
                if (model.isAdmin != null) userToEdit.isAdmin = model.isAdmin.Value;
                
                _database.Users.Update(userToEdit);
                _database.SaveChanges();
                
                return Ok();
                
            }    
            
        }
        
        return NoContent();
    }

    [HttpPost]
    public IActionResult ChangePassword(changePasswordModel model)
    {
        var sessionId = Request.Cookies[cookieName];

        if (sessionId != null)
        {
            if (loggedInUsers.ContainsKey(sessionId))
            {
                var user = loggedInUsers[sessionId];
                
                if (user.isBanned) return Json(new SimpleError() { error = "UserBanned" });
                if (model.newPassword == null || model.Password == null) return Json(new SimpleError() { error = "InvalidPassword" });
                if (model.newPassword.Length < 7) return Json(new SimpleError() { error = "PasswordTooShort" });
                
                if (generatePasswordHash(model.newPassword) == generatePasswordHash(model.Password)) return Json(new SimpleError() { error = "PasswordSameAsOld" });
                
                if (user.PasswordHash != generatePasswordHash(model.Password)) return Json(new SimpleError() { error = "InvalidPassword" });

                user.PasswordHash = generatePasswordHash(model.newPassword);
                var userInDb = _database.Users.FirstOrDefault(u => u.Username == user.Username);
                if (userInDb != null)
                {
                    userInDb.PasswordHash = user.PasswordHash;
                    _database.SaveChanges();
                    
                    return Ok();
                }
                else
                {
                    return Json(new SimpleError() { error = "AccountNotFound" });
                }
            }
        }
        
        return NoContent();
    }

    public IActionResult Logout()
    {
        var sessionId = Request.Cookies[cookieName];
        if (sessionId != null)
        {
            if (loggedInUsers.ContainsKey(sessionId))
            {
                loggedInUsers.Remove(sessionId);
            }

            try
            {
                Response.Cookies.Delete(cookieName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        return Redirect($"/Login/Index/");
    }
    
    public IActionResult GetUsers()
    {
        var sessionId = Request.Cookies[cookieName];

        if (sessionId != null)
        {
            if (loggedInUsers.ContainsKey(sessionId))
            {
                var user = loggedInUsers[sessionId];
                if (!user.isAdmin) return Json(new SimpleError() { error = "NotAdmin" });
                if (user.isBanned) return Json(new SimpleError() { error = "UserBanned" });
                
                var users = _database.Users.ToList();
                
                return Json(users);
            }   
            
        }

        return NoContent();
    }

    
    [HttpGet ("/Auth/BanUser/{username}")]
    public IActionResult BanUser(string? username)
    {
        var sessionId = Request.Cookies[cookieName];

        if (sessionId != null)
        {
            if (loggedInUsers.ContainsKey(sessionId))
            {
                var user = loggedInUsers[sessionId];
                
                if(user.isBanned) return Json(new SimpleError() { error = "UserBanned" });
                if (!user.isAdmin) return Json(new SimpleError() { error = "NotAdmin" });

                if (username != null)
                {
                    if(username == "admin") return Json(new SimpleError() { error = "CannotBanAdmin" });
                    
                    var userToBan = _database.Users.FirstOrDefault(u => u.Username == username);
                    if (userToBan == null) return Json(new SimpleError() { error = "UserNotFound" });
                    userToBan.isBanned = !userToBan.isBanned;
                    
                    foreach (var sesId in loggedInUsers.Keys)
                    {
                        if (loggedInUsers[sesId].Username == userToBan.Username)
                        {
                            loggedInUsers.Remove(sessionId);
                        }
                    }
                    
                    _database.Users.Update(userToBan);
                    _database.SaveChanges();
                    
                    return Ok();
                }
                
            }   
            
        }

        return NoContent();
    }
    
    [HttpGet ("/Auth/RemoveUser/{username}")]
    public IActionResult RemoveUser(string? username)
    {

        var sessionId = Request.Cookies[cookieName];

        if (sessionId != null)
        {
            if (loggedInUsers.ContainsKey(sessionId))
            {
                var user = loggedInUsers[sessionId];
                
                if(user.isBanned) return Json(new SimpleError() { error = "UserBanned" });
                if (!user.isAdmin) return Json(new SimpleError() { error = "NotAdmin" });
                
                if (username != null)
                {
                    if(username == "admin") return Json(new SimpleError() { error = "CannotRemoveAdmin" });
                    
                    var userToRemove = _database.Users.FirstOrDefault(u => u.Username == username);
                    if (userToRemove == null) return Json(new SimpleError() { error = "UserNotFound" });


                    foreach (var sesId in loggedInUsers.Keys)
                    {
                        if (loggedInUsers[sesId].Username == userToRemove.Username)
                        {
                            loggedInUsers.Remove(sessionId);
                        }
                    }
                    
                    
                    _database.Users.Remove(userToRemove);
                    _database.SaveChanges();
                    return Ok();

                }
                
            }   
            
        }

        return NoContent();
    }
    
}