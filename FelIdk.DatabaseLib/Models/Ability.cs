﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FelIdk.DatabaseLib.Models;

public enum Target
{
    HOSTILE,
    FRIENDLY,
    AOE
}

public class Ability
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Key]
    public int Id { get; init; }

    [MaxLength(50)]
    public string Name { get; init; }
    public float Potency { get; init; }
    public float Cooldown { get; init; }
    public bool Channel { get; init; }
    public Target Target { get; init; }
    
}