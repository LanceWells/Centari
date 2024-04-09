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

      CircleMidpoint(mouseMotionEvent.Position.X, mouseMotionEvent.Position.Y, 10);

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

  private void CirclePoints(float cx, float cy, int x, int y)
  {
    if (x == 0)
    {
      DrawPixel(new(cx, cy + y), InterfaceTile.TileDetails);
      DrawPixel(new(cx, cy - y), InterfaceTile.TileDetails);
      DrawPixel(new(cx + y, cy), InterfaceTile.TileDetails);
      DrawPixel(new(cx - y, cy), InterfaceTile.TileDetails);
    }
    if (x == y)
    {
      DrawPixel(new(cx + x, cy + y), InterfaceTile.TileDetails);
      DrawPixel(new(cx - x, cy + y), InterfaceTile.TileDetails);
      DrawPixel(new(cx + x, cy - y), InterfaceTile.TileDetails);
      DrawPixel(new(cx - x, cy - y), InterfaceTile.TileDetails);
    }
    if (x < y)
    {
      DrawPixel(new(cx + x, cy + y), InterfaceTile.TileDetails);
      DrawPixel(new(cx - x, cy + y), InterfaceTile.TileDetails);
      DrawPixel(new(cx + x, cy - y), InterfaceTile.TileDetails);
      DrawPixel(new(cx - x, cy - y), InterfaceTile.TileDetails);
      DrawPixel(new(cx + y, cy + x), InterfaceTile.TileDetails);
      DrawPixel(new(cx - y, cy + x), InterfaceTile.TileDetails);
      DrawPixel(new(cx + y, cy - x), InterfaceTile.TileDetails);
      DrawPixel(new(cx - y, cy - x), InterfaceTile.TileDetails);
    }
  }

  /// <summary>
  /// 
  /// </summary>
  /// <param name="xCenter"></param>
  /// <param name="yCenter"></param>
  /// <param name="radius"></param>
  /// https://groups.csail.mit.edu/graphics/classes/6.837/F98/Lecture6/circle.html
  private void CircleMidpoint(float xCenter, float yCenter, int radius)
  {
    _metalTileMap.ClearLayer(InterfaceTile.TileDetails.Layer);

    int x = 0;
    int y = radius;
    int p = (5 - radius * 4) / 4;

    CirclePoints(xCenter, yCenter, x, y);
    while (x++ < y)
    {
      if (p < 0)
      {
        p += 2 * x + 1;
      }
      else
      {
        y--;
        p += 2 * (x - y) + 1;
      }
      CirclePoints(xCenter, yCenter, x, y);
    }
  }
}
