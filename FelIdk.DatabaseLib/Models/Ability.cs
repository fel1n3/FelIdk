using System.ComponentModel.DataAnnotations;

namespace FelIdk.DatabaseLib.Models;

public class Ability
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; }
    
}