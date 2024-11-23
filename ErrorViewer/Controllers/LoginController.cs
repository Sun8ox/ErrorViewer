using Microsoft.AspNetCore.Mvc;

namespace ErrorViewer.Controllers;

public class LoginController : Controller
{
    // GET
    public IActionResult Index(string? error)
    {

        // Redirect to homepage if user is already logged in
        if (Request.Cookies.ContainsKey(AuthController.cookieName))
        {
            var sessionId = Request.Cookies[AuthController.cookieName];

            if (sessionId != null)
            {
                if (AuthController.loggedInUsers.ContainsKey(sessionId))
                {
                    var user = AuthController.loggedInUsers[sessionId];

                    if (!user.isBanned)
                    {
                        return Redirect($"/Dashboard/Index/");
                    }
                    else
                    {
                        error = "UserBanned";
                    }
                    
                }
            }
        }
        
        // Handle errors
        if (error != null)
        {
            if (error == "InvalidUsernameOrPassword")
            {
                ViewBag.AuthError = "Invalid Username or Password.";
            }
            
            else if (error == "NotLoggedIn")
            {
                ViewBag.AuthError = "Log in to continue.";
            }

            else if (error == "UserBanned")
            {
                ViewBag.AuthError = "Access denited.";
            }
        }
        
        
        // Return view
        return View();
    }
}