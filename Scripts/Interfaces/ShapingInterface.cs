using System;
using Godot;
using Interfaces.Tiles;

public partial class ShapingInterface : Node2D
{
  private Node2D _topLeft;

  private Node2D _botRight;

  private TileMap _metalTileMap;

  private bool _mousePressed = false;

  private bool _mouseInArea = false;

  public override void _Ready()
  {
    _metalTileMap = GetNode<TileMap>("MetalTileMap");
    _topLeft = GetNode<Node2D>("Area/TopLeft");
    _botRight = GetNode<Node2D>("Area/BotRight");

    base._Ready();
  }

  public override void _Input(InputEvent @event)
  {
    if (@event is InputEventMouseMotion mouseMotionEvent)
    {
      Vector2 localCoords = ViewportToLocal(mouseMotionEvent.Position);
      if (!IsInArea(localCoords))
      {
        return;
      }

      Rec

      if (!_mousePressed)
      {
        return;
      }

      DrawPixel(mouseMotionEvent.Position, MetalTile.TileDetails);
    }
    if (@event is InputEventMouseButton mouseEvent)
    {
      if (mouseEvent.Pressed)
      {
        _mousePressed = true;
        DrawPixel(mouseEvent.Position, MetalTile.TileDetails);
      }
      else
      {
        _mousePressed = false;
      }
    }
  }

  bool IsInArea(Vector2 localPosition)
  {
    Rect2 area = new(_topLeft.Position, _botRight.Position);
    bool isInArea = area.HasPoint(localPosition);
    return isInArea;
  }

  Vector2 ViewportToLocal(Vector2 viewportPosition)
  {
    Transform2D vpToCanvas = GetGlobalTransformWithCanvas().AffineInverse();
    Vector2 localMousePosition = vpToCanvas * viewportPosition;

    return localMousePosition;
  }

  void DrawPixel(Vector2 vpMousePosition, TileDrawingDetails drawingDetails)
  {
    Vector2 localMousePosition = ViewportToLocal(vpMousePosition);
    Vector2I mapMousePosition = _metalTileMap.LocalToMap(localMousePosition);
    _metalTileMap.SetCell(
      drawingDetails.Layer,
      mapMousePosition,
      drawingDetails.SourceAtlas,
      drawingDetails.TileIndex
    );
  }
}
