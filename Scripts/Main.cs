using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FelIdk.Scripts.Entities.Player;
using Godot;

namespace FelIdk.Scripts;

public partial class Main : Node
{
    private readonly PackedScene _player = ResourceLoader.Load<PackedScene>("res://Scenes/player.tscn");
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
    }

    private async void _on_host_button_pressed()
    {
        _mainMenu.Hide();

        var gameCode = generate_room_code();
        GD.Print(gameCode);
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
        _mainMenu.Hide();

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

    private async Task<Variant[]> traverse_nat(bool host,string? gamecode)
    {
        var playerHost = host ? "host" : "client";
        var traversalId = $"{Multiplayer.GetUniqueId()}_{playerHost}";
        _holePunch.Call("start_traversal", gamecode, playerHost, traversalId);
        var result = await ToSignal(_holePunch, "hole_punched");
        await ToSignal(GetTree().CreateTimer(0.1), "timeout");
        return result;
    }
    
}
