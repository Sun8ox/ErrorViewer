using ErrorViewer.Models;
using Microsoft.AspNetCore.HttpLogging;

namespace ErrorViewer.Functions;

public class DataManager
{

    private static Dictionary<string, List<List<string>>>? _cache;
    
    
    private static void RemoveDataFromCacheAfterDelay(string sourceName, int cacheTime = 60000)
    {
        Console.WriteLine($"[INFO] {sourceName} cached for {cacheTime}.");
        Thread.Sleep(cacheTime);
        
        if (_cache != null && _cache.ContainsKey(sourceName))
        {
            _cache.Remove(sourceName);
            Console.WriteLine($"[INFO] Removed {sourceName} from cache.");
        }
    }
    
    public static List<List<string>>? GetDataFromSource(Source source, int numberOfLines = 1000, int startFrom = 0)
    {

        var type = source.Type;
        var connectionString = source.ConnectionString;
        
        List<List<string>> output = new List<List<string>>();
        
        if (_cache == null) _cache = new Dictionary<string, List<List<string>>>();
        
        if (type == "csv")
        {

            if (!_cache.ContainsKey(source.Name))
            {
                var reader = new CsvReader(connectionString);
                var readData = reader.ReadAsListOfLists(numberOfLines, startFrom);
                _cache.Add(source.Name, readData);
                
                new Thread(() => RemoveDataFromCacheAfterDelay(source.Name, source.cacheTime)).Start();
                
                return readData;
            }
            else
            {
                return _cache[source.Name];
            }
        }

        if (type == "LocalDatabase")
        {

            return null;
        }
        
        if (type == "RemoteDatabase")
        {

            return null;
        }

        return output;
    }
    
    
    
}