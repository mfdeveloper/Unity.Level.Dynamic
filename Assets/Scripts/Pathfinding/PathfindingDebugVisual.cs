/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeMonkey.Utils;

public class PathfindingDebugVisual : MonoBehaviour {

    public static PathfindingDebugVisual Instance { get; private set; }

    [SerializeField] private Transform pathFindingDebugNode;
    private List<Transform> visualNodeList;
    private List<GridSnapshotAction> gridSnapshotActionList;
    private bool autoShowSnapshots;
    private float autoShowSnapshotsTimer;
    private Transform[,] visualNodeArray;

    private void Awake() {
        Instance = this;
        visualNodeList = new List<Transform>();
        gridSnapshotActionList = new List<GridSnapshotAction>();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ShowNextSnapshot();
        }

        if (Input.GetKeyDown(KeyCode.Return)) {
            autoShowSnapshots = true;
        }

        if (autoShowSnapshots) {
            float autoShowSnapshotsTimerMax = .05f;
            autoShowSnapshotsTimer -= Time.deltaTime;
            if (autoShowSnapshotsTimer <= 0f) {
                autoShowSnapshotsTimer += autoShowSnapshotsTimerMax;
                ShowNextSnapshot();
                if (gridSnapshotActionList.Count == 0) {
                    autoShowSnapshots = false;
                }
            }
        }
    }

    private void OnEnable()
    {
        Pathfinding.OnTakeFirstSnapshot += TakeFirstSnapshot;
        Pathfinding.OnTakeSnapshot += TakeSnapshot;
        Pathfinding.OnTakeNeighbourSnapshot += TakeSnapshot;
        Pathfinding.OnTakeFinalSnapshot += TakeSnapshotFinalPath;
    }

    private void OnDisable()
    {
        Pathfinding.OnTakeFirstSnapshot -= TakeFirstSnapshot;
        Pathfinding.OnTakeSnapshot -= TakeSnapshot;
        Pathfinding.OnTakeNeighbourSnapshot -= TakeSnapshot;
        Pathfinding.OnTakeFinalSnapshot -= TakeSnapshotFinalPath;
    }

    public void Setup(GridBase<PathNode> grid)
    {
        visualNodeArray = new Transform[grid.Width, grid.Height];

        for (int x = 0; x < grid.Width; x++)
        {
            for (int y = 0; y < grid.Height; y++)
            {
                Vector3 gridPosition = new Vector3(x, y) * grid.CellSize + Vector3.one * grid.CellSize * .5f;
                if (pathFindingDebugNode != null)
                {
                    Transform visualNode = CreateVisualNode(gridPosition);
                    visualNodeArray[x, y] = visualNode;
                    visualNodeList.Add(visualNode);
                }
            }
        }
        HideNodeVisuals();
    }

    private void ShowNextSnapshot() {
        if (gridSnapshotActionList.Count > 0) {
            GridSnapshotAction gridSnapshotAction = gridSnapshotActionList[0];
            gridSnapshotActionList.RemoveAt(0);
            gridSnapshotAction.TriggerAction();
        }
    }

    public void ClearSnapshots() {
        gridSnapshotActionList.Clear();
    }

    public void TakeFirstSnapshot(GridBase<PathNode> grid, PathNode current, List<PathNode> openList, List<PathNode> closedList)
    {
        ClearSnapshots();
        TakeSnapshot(grid, current, openList, closedList);
    }

    public void TakeSnapshot(GridBase<PathNode> grid, PathNode current, List<PathNode> openList, List<PathNode> closedList) {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);
        
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                PathNode pathNode = grid.GetCell(x, y);

                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                Vector3 gridPosition = new Vector3(pathNode.Position.x, pathNode.Position.y) * grid.CellSize + Vector3.one * grid.CellSize * .5f;
                bool isCurrent = pathNode == current;
                bool isInOpenList = openList.Contains(pathNode);
                bool isInClosedList = closedList.Contains(pathNode);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() => {
                    Transform visualNode = visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, gCost, hCost, fCost);

                    Color backgroundColor = UtilsClass.GetColorFromString("636363");

                    if (isInClosedList) {
                        backgroundColor = new Color(1, 0, 0);
                    }
                    if (isInOpenList) {
                        backgroundColor = UtilsClass.GetColorFromString("009AFF");
                    }
                    if (isCurrent) {
                        backgroundColor = new Color(0, 1, 0);
                    }

                    var sprite = visualNode.GetComponentInChildren<SpriteRenderer>();

                    if (sprite != null)
                    {
                        sprite.color = backgroundColor;
                    }

                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    public void TakeSnapshotFinalPath(GridBase<PathNode> grid, List<PathNode> path) {
        GridSnapshotAction gridSnapshotAction = new GridSnapshotAction();
        gridSnapshotAction.AddAction(HideNodeVisuals);
        
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                PathNode pathNode = grid.GetCell(x, y);

                int gCost = pathNode.gCost;
                int hCost = pathNode.hCost;
                int fCost = pathNode.fCost;
                Vector3 gridPosition = new Vector3(pathNode.Position.x, pathNode.Position.y) * grid.CellSize + Vector3.one * grid.CellSize * .5f;
                bool isInPath = path.Contains(pathNode);
                int tmpX = x;
                int tmpY = y;

                gridSnapshotAction.AddAction(() => { 
                    Transform visualNode = visualNodeArray[tmpX, tmpY];
                    SetupVisualNode(visualNode, gCost, hCost, fCost);

                    Color backgroundColor;

                    if (isInPath) {
                        backgroundColor = new Color(0, 1, 0);
                    } else {
                        backgroundColor = UtilsClass.GetColorFromString("636363");
                    }

                    var sprite = visualNode.GetComponentInChildren<SpriteRenderer>();

                    if (sprite != null)
                    {
                        sprite.color = backgroundColor;
                    }
                    
                });
            }
        }

        gridSnapshotActionList.Add(gridSnapshotAction);
    }

    public void HideNodeVisuals() {
        foreach (Transform visualNodeTransform in visualNodeList) {
            SetupVisualNode(visualNodeTransform, 9999, 9999, 9999);
        }
    }

    private Transform CreateVisualNode(Vector3 position) {
        Transform visualNodeTransform = Instantiate(pathFindingDebugNode, position, Quaternion.identity);
        return visualNodeTransform;
    }

    private void SetupVisualNode(Transform visualNodeTransform, int gCost, int hCost, int fCost) {

        TextMeshPro[] textValues = visualNodeTransform.GetComponentsInChildren<TextMeshPro>();
        foreach (var text in textValues)
        {
            if (fCost < 1000)
            {

                if (text.name.Equals("gCostText"))
                {
                    text.SetText(gCost.ToString());
                }

                if (text.name.Equals("hCostText"))
                {
                    text.SetText(hCost.ToString());
                }

                if (text.name.Equals("fCostText"))
                {
                    text.SetText(fCost.ToString());
                }
            } else
            {
                text.SetText("");
            }
        }
    }

    private class GridSnapshotAction {

        private Action action;

        public GridSnapshotAction() {
            action = () => { };
        }

        public void AddAction(Action action) {
            this.action += action;
        }

        public void TriggerAction() {
            action();
        }

    }

}

