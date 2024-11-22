using System.Text.Json;
using ErrorViewer.Data;
using ErrorViewer.Functions;
using ErrorViewer.Models;
using Microsoft.AspNetCore.Mvc;

namespace ErrorViewer.Controllers;

public class ApiController : Controller
{

    private readonly ErrorViewerDB _database;
    
    public ApiController(ErrorViewerDB context)
    {
        _database = context;
    }

    [HttpGet("/Api/getDataFromSource/{sourceName}")]
    public IActionResult getDataFromSource(string sourceName, int? numberOfLines, int? startFrom)
    {
        var sessionId = Request.Cookies[AuthController.cookieName];

        if (sessionId != null)
        {
            if (AuthController.loggedInUsers.ContainsKey(sessionId))
            {
                var user = AuthController.loggedInUsers[sessionId];
                if (user.isBanned) return StatusCode(StatusCodes.Status403Forbidden);

                if (numberOfLines == null) numberOfLines = 1000;
                if (startFrom == null) startFrom = 0;

                var source = SourceManager.sources.Find(source => source.Name == sourceName);
                if (source == null) return Json(new SimpleError() { error = "SourceNotFound"});
                try
                {
                    var data = DataManager.GetDataFromSource(source, numberOfLines.Value, startFrom.Value);
                    return Json(data);
                }
                catch (Exception)
                {
                    return Json(new SimpleError() { error = "ErrorWhileGettingData"});
                }
                
                
            }
        }
        
        return StatusCode(StatusCodes.Status403Forbidden);
    }
    
    public IActionResult getSources()
    {
        var sessionId = Request.Cookies[AuthController.cookieName];
        
        if (sessionId != null)
        {
            if (AuthController.loggedInUsers.ContainsKey(sessionId))
            {
                var user = AuthController.loggedInUsers[sessionId];
                if (user.isBanned) return StatusCode(StatusCodes.Status403Forbidden);

                if (user.isAdmin)
                {
                    var sources = SourceManager.sources;
                    
                    return Json(sources);
                }
                else
                {
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }
        }


        return StatusCode(StatusCodes.Status403Forbidden);
    }

    public IActionResult addSource(addSourceModel? model)
    {   
        var sessionId = Request.Cookies[AuthController.cookieName];

        if (sessionId != null)
        {
            if (AuthController.loggedInUsers.ContainsKey(sessionId))
            {
                var user = AuthController.loggedInUsers[sessionId];

                if (!user.isAdmin) return StatusCode(StatusCodes.Status401Unauthorized);
                if (user.isBanned) return StatusCode(StatusCodes.Status403Forbidden);


                if (model != null)
                {
                    if (model.Name == null) return Json(new SimpleError { error = "NameIsNull" });
                    if (model.ConnectionString == null) return Json(new SimpleError { error = "ConStringIsNull" });
                    if (model.Type == null) return Json(new SimpleError { error = "TypeIsNull" });
                    if (model.cacheTime == 0) model.cacheTime = 60000;
                    if (model.errorRow == null) return Json(new SimpleError { error = "ErrorRowIsNull" });
                    if (SourceManager.sources.Find(source => source.Name == model.Name) != null) return Json(new SimpleError { error = "NameExists" });
                    
                    Source source = new Source();
                    
                    source.Name = model.Name;
                    source.ConnectionString = model.ConnectionString;
                    source.cacheTime = model.cacheTime;
                    source.errorRow = model.errorRow;
                    
                    if (model.Type == "File") 
                    {
                        source.Type = "csv";
                    } else if (model.Type == "RemoteDatabase")
                    {
                        source.Type = "RemoteDatabse";
                    } else if (model.Type == "LocalDatabase")
                    {
                        source.Type = "LocalDatabase";
                    }
                    SourceManager.sources.Add(source);
                    SourceManager.UpdateDatabase(_database);
                    
                    return Ok();
                }
            }
        }

        return StatusCode(StatusCodes.Status403Forbidden);
    }

    [HttpDelete("/Api/removeSource/{name}")]
    public IActionResult removeSource(string name)
    {
        var sessionId = Request.Cookies[AuthController.cookieName];

        if (sessionId != null)
        {
            if (AuthController.loggedInUsers.ContainsKey(sessionId))
            {
                var user = AuthController.loggedInUsers[sessionId];

                if (!user.isAdmin) return StatusCode(StatusCodes.Status401Unauthorized);
                if (user.isBanned) return StatusCode(StatusCodes.Status403Forbidden);

                if (SourceManager.sources.RemoveAll(source => source.Name == name) > 0)
                {
                    SourceManager.UpdateDatabase(_database);
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
        }
    


        return StatusCode(StatusCodes.Status403Forbidden);
    }
    
    public IActionResult getPlainData()
    {
        
        
        
        return NotFound();
    }
    
    public IActionResult getGroupedData()
    {
        
        
        
        return NotFound();
    }
}