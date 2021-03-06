﻿/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using UnityEngine;

// TODO: Change to use only this class for pathfinding debug to show Meshes insteadof each gameObject to shows the path
public class PathfindingVisual : MonoBehaviour {

    private GridBase<PathNode> grid;
    private Mesh mesh;
    private bool updateMesh;

    private void Awake() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void OnDisable()
    {
        if (grid != null)
        {
            grid.OnCellChange -= OnCellChange;
        }
    }

    public void SetGrid(GridBase<PathNode> grid) {
        this.grid = grid;
        UpdateVisual();

        grid.OnCellChange += OnCellChange;
    }

    private void OnCellChange(Vector2Int cellPosition, PathNode value) {
        updateMesh = true;
    }

    private void LateUpdate() {
        if (updateMesh) {
            updateMesh = false;
            UpdateVisual();
        }
    }

    private void UpdateVisual() {
        MeshUtils.CreateEmptyMeshArrays(grid.Width * grid.Height, out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                int index = x * grid.Height + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.CellSize;

                PathNode pathNode = grid.GetCell(x, y);

                if (pathNode.IsWalkable) {
                    quadSize = Vector3.zero;
                }

                var uvMap = Vector2.zero;
                float normalizedValue = 0;
                if (pathNode.IsLast)
                {
                    quadSize = Vector3.one * grid.CellSize;
                    normalizedValue = 50;
                    normalizedValue = (float) normalizedValue / 100;
                    uvMap = new Vector2(normalizedValue, 0f);
                }

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.CellToWorld(x, y) + quadSize * .5f, 0f, quadSize, uvMap, uvMap);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
