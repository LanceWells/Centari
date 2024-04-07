using System;
using System.Net;
using Godot;

public partial class ShapingInterface : Node2D
{
  private Area2D _forgeArea;

  private CollisionShape2D _forgeAreaShape;

  private TileMap _metalTileMap;

  private bool MousePressed = false;

  public override void _Ready()
  {
    _forgeArea = GetNode<Area2D>("ForgeArea");
    _metalTileMap = GetNode<TileMap>("MetalTileMap");
    _forgeAreaShape = GetNode<CollisionShape2D>("ForgeArea/ForgeAreaShape");

    base._Ready();
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouseMotion mouseMotionEvent)
    {
      if (!MousePressed)
      {
        return;
      }

      DrawPixel(mouseMotionEvent.Position);
    }
    if (@event is InputEventMouseButton mouseEvent)
    {
      if (mouseEvent.Pressed)
      {
        MousePressed = true;
        DrawPixel(mouseEvent.Position);
      }
      else
      {
        MousePressed = false;
      }
    }
  }

  void DrawPixel(Vector2 vpMousePosition)
  {
    Transform2D vpToCanvas = GetGlobalTransformWithCanvas().AffineInverse();
    Vector2 localMousePosition = vpToCanvas * vpMousePosition;

    Vector2I mapMousePosition = _metalTileMap.LocalToMap(localMousePosition);
    _metalTileMap.SetCell(0, mapMousePosition, 2, new(0, 0));
    Console.WriteLine($"Drawing cell at {mapMousePosition}");
  }
}
