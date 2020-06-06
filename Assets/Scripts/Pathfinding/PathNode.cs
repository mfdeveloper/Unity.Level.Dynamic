using UnityEngine;

public class PathNode
{
    public const int DEFAULT_GCOST = 99999999; //int.MaxValue;

    public int gCost = DEFAULT_GCOST;
    public int hCost;
    public int fCost;

    protected bool walkable = true;

    public PathNode cameFromNode = null;
    public Vector2Int Position { get; set; }

    public bool IsWalkable {

        get => walkable;

        set {
            walkable = value;
            if (grid != null)
            {
                grid.TriggerCellChange(Position);
            }
        }
    }

    protected GridBase<PathNode> grid;

    public PathNode() { }

    public PathNode(GridBase<PathNode> grid, int width, int height) 
    {
        this.grid = grid;
        Position = new Vector2Int(width, height);
    }

    public override string ToString()
    {
        return $"{Position.x},{Position.y}";
    }

    public int CalculateF()
    {
        fCost = gCost + hCost;
        return fCost;
    }
}
