using Godot;

namespace FelIdk.Game.Scripts;

public partial class Lobby : Control
{
    private MeshInstance3D _model1;
    private Viewport _viewport;
    private Label _gameCode;
    public override void _Ready()
    {
        _model1 = GetNode<MeshInstance3D>(
            "CanvasLayer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/SubViewportContainer/SubViewport/Model");
        _viewport = GetNode<Viewport>(
            "CanvasLayer/PanelContainer/MarginContainer/VBoxContainer/HBoxContainer/SubViewportContainer/SubViewport");
        _gameCode = GetNode<Label>("CanvasLayer/PanelContainer/MarginContainer/VBoxContainer/GameCode");
        
        _viewport.OwnWorld3D = true;
        _viewport.DebugDraw = Viewport.DebugDrawEnum.Unshaded;
    }

    public override void _Process(double delta)
    {
        _model1.RotateY((float)delta);
    }
}