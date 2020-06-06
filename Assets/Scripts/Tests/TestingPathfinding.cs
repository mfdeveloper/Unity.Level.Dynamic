using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestingPathfinding : MonoBehaviour
{

    [SerializeField]
    protected PathfindingDebugVisual pathfindingDebugVisual;

    [SerializeField]
    protected PathfindingVisual pathfindingVisual;

    protected Pathfinding pathfinding;

    [SerializeField]
    protected bool debugGrid = true;

    [SerializeField]
    protected bool startFromLastNode = false;

    [SerializeField]
    [Tooltip("X = width, Y = height")]
    protected Vector2Int gridSize = new Vector2Int(10, 10);

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = new Pathfinding(width: gridSize.x, height: gridSize.y, startPosition: Vector2Int.zero, debugGrid: debugGrid);

        if (pathfindingDebugVisual != null)
        {
            pathfindingDebugVisual.Setup(pathfinding.Grid);
        }

        if (pathfindingVisual != null)
        {
            pathfindingVisual.SetGrid(pathfinding.Grid);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var worldPosition = UtilsClass.GetMouseWorldPosition();
            var cellPosition = pathfinding.Grid.WorldToCell(worldPosition);

            List<PathNode> path = pathfinding.FindPath(cellPosition, startFromLastNode);

            if (path != null)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Vector3 start = new Vector3(path[i].Position.x, path[i].Position.y) * 10f + Vector3.one * 5f;
                    Vector3 end = new Vector3(path[i + 1].Position.x, path[i + 1].Position.y) * 10f + Vector3.one * 5f;
                    Debug.DrawLine(start, end, Color.green, 10f);
                }
            }
        }

        // Put walls
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mouseWorldPosition = UtilsClass.GetMouseWorldPosition();
            var cellPosition = pathfinding.Grid.WorldToCell(mouseWorldPosition);
            var node = pathfinding.Grid.GetCell(cellPosition.x, cellPosition.y);

            if (node != null)
            {
                node.IsWalkable = !node.IsWalkable;
            }
        }
    }
}
