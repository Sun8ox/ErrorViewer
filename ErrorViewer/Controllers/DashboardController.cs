using ErrorViewer.Models;
using Microsoft.AspNetCore.Mvc;

namespace ErrorViewer.Controllers;

public class DashboardController : Controller
{
    // GET
    public IActionResult Index()
    {
        var sessionId = Request.Cookies[AuthController.cookieName];
        
        if (sessionId != null)
        {
            if (AuthController.loggedInUsers.ContainsKey(sessionId))
            {
                User user = AuthController.loggedInUsers[sessionId]; ;
                return View(user);
            }
        } 
        
        return Redirect($"/Login/Index/?error=NotLoggedIn");
    }

    public IActionResult Explorer()
    {
        var sessionId = Request.Cookies[AuthController.cookieName];


        if (sessionId != null)
        {
            if (AuthController.loggedInUsers.ContainsKey(sessionId))
            {
                User user = AuthController.loggedInUsers[sessionId]; ;
                return View(user);
            }
        } 
        
        return Redirect($"/Login/Index/?error=NotLoggedIn");
    }

    public IActionResult Overview()
    {
        var sessionId = Request.Cookies[AuthController.cookieName];


        if (sessionId != null)
        {
            if (AuthController.loggedInUsers.ContainsKey(sessionId))
            {
                User user = AuthController.loggedInUsers[sessionId]; ;
                return View(user);
            }
        } 
        
        return Redirect($"/Login/Index/?error=NotLoggedIn");
    }

    public IActionResult Sources()
    {
        var sessionId = Request.Cookies[AuthController.cookieName];


        if (sessionId != null)
        {
            if (AuthController.loggedInUsers.ContainsKey(sessionId))
            {
                User user = AuthController.loggedInUsers[sessionId];
                ;
                return View(user);
            }
        }

        return Redirect($"/Login/Index/?error=NotLoggedIn");
    }

    public IActionResult Account()
    {
        var sessionId = Request.Cookies[AuthController.cookieName];


        if (sessionId != null)
        {
            if (AuthController.loggedInUsers.ContainsKey(sessionId))
            {
                User user = AuthController.loggedInUsers[sessionId];
                return View(user);
            }
        }

        return Redirect($"/Login/Index/?error=NotLoggedIn");
    }
}