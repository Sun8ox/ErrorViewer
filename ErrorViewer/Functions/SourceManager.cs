using ErrorViewer.Data;
using ErrorViewer.Models;
using Microsoft.EntityFrameworkCore;

namespace ErrorViewer.Functions;

public class SourceManager
{
    public static List<Source> sources { get; set; } = new List<Source>();

    public static Task UpdateDatabase(ErrorViewerDB db)
    {
        db.Sources.RemoveRange(db.Sources);
        db.Sources.AddRange(sources);

        db.SaveChanges();

        return Task.CompletedTask;
    }
    
    
    public static void RemoveSource(string name)
    {
        sources.RemoveAll(x => x.Name == name);
    }
    
}

