using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace ErrorViewer.Models;

[PrimaryKey("Name")]
public class Source
{
    [MinLength(3), MaxLength(50), Required]
    public string Name { get; set; }
    [MinLength(3), MaxLength(50), Required]
    public string Type { get; set; }
    [Required]
    public string ConnectionString { get; set; }
    [Required]
    public int cacheTime { get; set; } = 60000;
    [Required]
    public string errorRow { get; set; } = "ecode";
}