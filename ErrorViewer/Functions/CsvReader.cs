using Microsoft.VisualBasic.FileIO;

namespace ErrorViewer.Functions;

public class CsvReader
{
    private string filePath;
    
    public CsvReader(string filePath)
    {
        if (File.Exists(filePath))
        {
            this.filePath = filePath;    
        }
        else
        {
            throw new FileNotFoundException("File not found.");
        }
        
    }

    public List<List<string>> ReadAsListOfLists(int numberOfLines = 1000, int startFrom = 0)
    {
        
        var data = new List<List<string>>();
        
        using (TextFieldParser parser = new TextFieldParser(filePath))
        {
            parser.TextFieldType = FieldType.Delimited;
            parser.SetDelimiters(",");
            
            while (!parser.EndOfData)
            {
                string[] fields = parser.ReadFields();
                var values = fields.Select(value => value.Trim('"')).ToList();
                data.Add(values);
            }
        }
        
        return data;
    }
}