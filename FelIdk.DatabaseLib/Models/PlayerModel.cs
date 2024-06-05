using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FelIdk.DatabaseLib.Models;

public class PlayerModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; init; }

    [MaxLength(50)]
    public string Name { get; init; }

    public virtual ICollection<int> Spellbook { get; set; }

}