namespace FelIdk.Game.Scripts.Abilities;

public enum Target
{
    HOSTILE,
    FRIENDLY,
    AOE
}

public interface ISpellEffect
{
    string Name { get; set; }
    int Id { get; set; }
    float Potency { get; set; }
    float Cooldown { get; set; }
    bool Channel { get; set; }
    Target Target { get; set; }
    
    void OnCast();
}