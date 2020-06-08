using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pathfinding
{

    public const int MOVE_STRAIGHT_COST = 10;
    public const int MOVE_DIAGONAL_COST = 14;

    // --- Events ---
    public static Action<GridBase<PathNode>, PathNode, List<PathNode>, List<PathNode>> OnTakeFirstSnapshot;
    public static Action<GridBase<PathNode>, PathNode, List<PathNode>, List<PathNode>> OnTakeSnapshot;
    public static Action<GridBase<PathNode>, List<PathNode>> OnTakeFinalSnapshot;
    public static Action<GridBase<PathNode>, PathNode, List<PathNode>, List<PathNode>> OnTakeNeighbourSnapshot;

    protected List<PathNode> openList;
    protected List<PathNode> closedList = new List<PathNode>();

    protected Vector2Int initialPosition = Vector2Int.zero;
    protected List<PathNode> path = new List<PathNode>();
    public GridBase<PathNode> Grid { get; set; }

    public PathNode LastNode {
        get => path.Count > 0 ? path.Last() : null;
        protected set { }
    }
    public Pathfinding(int width, int height, Vector2Int startPosition = default, bool debugGrid = true)
    {
        Grid = new GridBase<PathNode>(width, height, 10f, Vector3.zero, debugGrid);
        initialPosition = startPosition;
    }

    public virtual List<PathNode> FindPath(Vector2Int endPos, bool startFromLastNode = true)
    {
        var startPos = startFromLastNode && LastNode != null ? LastNode.Position : initialPosition;

        if (LastNode != null)
        {
            LastNode.IsLast = false;
        }

        return FindPath(startPos, endPos);
    }

    public virtual List<PathNode> FindPath(Vector2Int startPos, Vector2Int endPos)
    {
        PathNode startNode = Grid.GetCell(startPos.x, startPos.y);
        PathNode endNode = Grid.GetCell(endPos.x, endPos.y);

        if (startNode == null || endNode == null)
        {
            return null;
        }

        openList = new List<PathNode> { startNode };
        closedList = new List<PathNode>();

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                PathNode node = Grid.GetCell(x, y);
                node.gCost = PathNode.DEFAULT_GCOST;
                node.CalculateF();

                node.cameFromNode = null;
            }
        }

        startNode.gCost = 0;
        startNode.hCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateF();

        OnTakeFirstSnapshot?.Invoke(Grid, startNode, openList, closedList);

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openList);
            if (currentNode == endNode)
            {
                // Found the final node!
                path = CalculatePath(endNode);

                OnTakeSnapshot?.Invoke(Grid, currentNode, openList, closedList);
                OnTakeFinalSnapshot?.Invoke(Grid, path);

                LastNode.IsLast = true;
                return path;
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighbours(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                // Avoid unwallkable obstracles (e.g. walls) to follow the path
                if (!neighbourNode.IsWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.gCost)
                {
                    neighbourNode.cameFromNode = currentNode;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.hCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateF();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }

                OnTakeNeighbourSnapshot?.Invoke(Grid, currentNode, openList, closedList);

            }
        }

        //Out of nodes on the openList
        return null;
    }

    private List<PathNode> GetNeighbours(PathNode currentNode)
    {
        List<PathNode> neighbours = new List<PathNode>();

        //Left
        if (currentNode.Position.x - 1 >= 0)
        {
            neighbours.Add(Grid.GetCell(currentNode.Position.x - 1, currentNode.Position.y));

            //Left Down
            if (currentNode.Position.y - 1 >= 0)
            {
                neighbours.Add(Grid.GetCell(currentNode.Position.x - 1, currentNode.Position.y - 1));
            }

            //Left Up
            if (currentNode.Position.y + 1 < Grid.Height)
            {
                neighbours.Add(Grid.GetCell(currentNode.Position.x - 1, currentNode.Position.y + 1));
            }
        }

        // Right
        if (currentNode.Position.x + 1 < Grid.Width)
        {
            neighbours.Add(Grid.GetCell(currentNode.Position.x + 1, currentNode.Position.y));

            // Right Down
            if (currentNode.Position.y - 1 >= 0)
            {
                neighbours.Add(Grid.GetCell(currentNode.Position.x + 1, currentNode.Position.y - 1));
            }

            // Right Up
            if (currentNode.Position.y + 1 < Grid.Height)
            {
                neighbours.Add(Grid.GetCell(currentNode.Position.x + 1, currentNode.Position.y + 1));
            }
        }

        //Down
        if (currentNode.Position.y - 1 >= 0)
        {
            neighbours.Add(Grid.GetCell(currentNode.Position.x, currentNode.Position.y - 1));
        }

        //Up
        if (currentNode.Position.y + 1 < Grid.Height)
        {
            neighbours.Add(Grid.GetCell(currentNode.Position.x, currentNode.Position.y + 1));
        }

        return neighbours;
    }

    private List<PathNode> CalculatePath(PathNode endNode)
    {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);

        PathNode currentNode = endNode;
        while (currentNode.cameFromNode != null)
        {
            path.Add(currentNode.cameFromNode);
            currentNode = currentNode.cameFromNode;
        }
        path.Reverse();

        return path;
    }

    protected virtual int CalculateDistanceCost(PathNode nodeA, PathNode nodeB)
    {
        int xDistance = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        int yDistance = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);
        int remaining = Mathf.Abs(xDistance - yDistance);

        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    protected PathNode GetLowestFCostNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostNode = pathNodeList[0];

        for (int i = 1; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].fCost < lowestFCostNode.fCost)
            {
                lowestFCostNode = pathNodeList[i];
            }
        }

        return lowestFCostNode;
    }
}

