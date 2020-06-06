/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using UnityEngine;

// TODO: Change the "PathfindingDebugVisual" class to show Meshes insteadof each gameObject to shows the path
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
            grid.OnValueChange -= OnCellChange;
        }
    }

    public void SetGrid(GridBase<PathNode> grid) {
        this.grid = grid;
        UpdateVisual();

        grid.OnValueChange += OnCellChange;
    }

    private void OnCellChange(Vector3 cellPosition, PathNode value) {
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

                if (pathNode.isWalkable) {
                    quadSize = Vector3.zero;
                }

                MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.CellToWorld(x, y) + quadSize * .5f, 0f, quadSize, Vector2.zero, Vector2.zero);
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }
}
