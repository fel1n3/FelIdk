using System;
using System.Linq;
using System.Threading.Tasks;
using FelIdk.Game.Scripts.Entities.Player;
using Godot;

namespace FelIdk.Game.Scripts;

public partial class Main : Node
{
    private readonly PackedScene _player = ResourceLoader.Load<PackedScene>("res://Scenes/player.tscn");
    private readonly PackedScene _lobby = ResourceLoader.Load<PackedScene>("res://Scenes/lobby.tscn");
    private static readonly GDScript HolePunchScript = GD.Load<GDScript>("res://addons/Holepunch/holepunch_node.gd");
    
    private ENetMultiplayerPeer _enetPeer = new ENetMultiplayerPeer();
    
    private PanelContainer _mainMenu;
    private LineEdit _addressEntry;
    private Node _holePunch;
    public override void _Ready()
    {
        _mainMenu = GetNode<PanelContainer>("GUI/CanvasLayer/MainMenu");
        _addressEntry = GetNode<LineEdit>("GUI/CanvasLayer/MainMenu/MarginContainer/VBoxContainer/AddressEntry");
        
         _holePunch = (Node)HolePunchScript.New();
         _holePunch.Set("rendevouz_address", "172.188.81.201");
         _holePunch.Set("rendevouz_port", 3000);
         AddChild(_holePunch);

         
         /*Name { get; set; }
         public int Id { get; set; }
         public float Potency { get; set; }
         public float Cooldown { get; set; }
         public bool Channel { get; set; }
         public Target Target { get; set; }*/
         //using var db = new GameContext();
         /*db.Add(new Ability
         {
             Name = "Fireball",
             Potency = 100f,
             Cooldown = 60f,
             Channel = false,
             Target = Target.HOSTILE
         });
         db.SaveChanges();
         var fireball = db.Abilities.Single(b => b.Name == "Fireball");
         var player = db.Players.Single(b => b.Name == "Riley");
         player.Spellbook.Add(
             fireball.Id);
         db.SaveChanges();
         GD.Print(fireball," : ",player);*/
    }

    private void _on_singleplayer_button_pressed()
    {
        _mainMenu.QueueFree();
        
        _enetPeer.CreateServer(3000,2);
        Multiplayer.MultiplayerPeer = _enetPeer;
        Multiplayer.PeerConnected += add_player;
        Multiplayer.PeerDisconnected += remove_player;
        
        add_player(Multiplayer.GetUniqueId());
    }

    private async void _on_host_button_pressed()
    {
        _mainMenu.QueueFree();
        var lobby = _lobby.Instantiate();
        AddChild(lobby);
        
        var gameCode = generate_room_code();
        lobby.GetNode<Label>("CanvasLayer/PanelContainer/MarginContainer/VBoxContainer/GameCode").Text = gameCode;
        
        var result = await traverse_nat(true,gameCode);

        var _myPort = result[0];

        _enetPeer.CreateServer((int)_myPort);
        Multiplayer.MultiplayerPeer = _enetPeer;
        Multiplayer.PeerConnected += add_player;
        Multiplayer.PeerDisconnected += remove_player;

        add_player(Multiplayer.GetUniqueId());
    }
 
    private async void _on_join_button_pressed()
    {
        _mainMenu.QueueFree();

        var result = await traverse_nat(false, _addressEntry.Text);
        
        var (_myPort, _hostPort, _addr) = (result[0], result[1], result[2]);

        _enetPeer.CreateClient(_addr.ToString(), (int)_hostPort, 0,0,0,(int)_myPort);
        Multiplayer.MultiplayerPeer = _enetPeer;
    }

    private void add_player(long peerId)
    {
        
        var player = _player.Instantiate();
        player.Name = peerId.ToString();
        AddChild(player);
        
        if (peerId == Multiplayer.GetUniqueId()) return;
        
        var lobby = GetNode<Control>("Lobby");
        lobby.QueueFree();
    }
    private void remove_player(long peerId)
    {
        var player = GetNodeOrNull<Player>(peerId.ToString());
        player?.QueueFree();
    }

    private static string generate_room_code()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var rand = new Random();
        return new string(Enumerable.Range(1, 5).Select(_ => chars[rand.Next(chars.Length)]).ToArray());
    }

    private async Task<Variant[]> traverse_nat(bool host,string gamecode)
    {
        var playerHost = host ? "host" : "client";
        var traversalId = $"{OS.GetUniqueId()}_{playerHost}";
        _holePunch.Call("start_traversal", gamecode, playerHost, traversalId);
        var result = await ToSignal(_holePunch, "hole_punched");
        await ToSignal(GetTree().CreateTimer(0.1), "timeout");
        return result;
    }
    
}