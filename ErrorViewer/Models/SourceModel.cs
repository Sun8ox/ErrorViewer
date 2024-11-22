using System.ComponentModel.DataAnnotations;
using ErrorViewer.Functions;
using Microsoft.EntityFrameworkCore;

namespace ErrorViewer.Models;



public class addSourceModel
{
    [MinLength(3), MaxLength(50), Required]
    public string Name { get; set; }
    [MinLength(3), MaxLength(50), Required]
    public string Type { get; set; }
    
    [Required]
    public string ConnectionString { get; set; }

    [Required]
    public int cacheTime { get; set; }
    [Required]
    public string errorRow { get; set; }
}

[PrimaryKey("Name")]
public class Source
{
    public string Name { get; set; }
    public string Type { get; set; }
    public string ConnectionString { get; set; }
    public int cacheTime { get; set; } = 60000;
    public string errorRow { get; set; } = "ecode";
}