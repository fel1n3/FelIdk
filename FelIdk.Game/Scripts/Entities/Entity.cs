using Godot;

namespace FelIdk.Game.Scripts.Entities;

public abstract partial class Entity : CharacterBody3D
{
    [Export] public int MaxHealth { get; set; } = 100;
    [Export] public float Speed = 5.0f;

    private int Health => MaxHealth;

}