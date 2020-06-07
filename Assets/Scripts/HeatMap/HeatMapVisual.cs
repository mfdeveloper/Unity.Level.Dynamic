using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HeatMapVisual : MonoBehaviour
{

    public const int HEAT_MAP_MIN_VALUE = 0;
    public const int HEAT_MAP_MAX_VALUE = 100;

    protected MeshFilter meshFilter;
    protected MeshRenderer meshRenderer;
    protected Mesh mesh;

    public GridBase<int> Grid { get; set; }

    private void Awake()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        Grid = new GridBase<int>(10, 5, 10f, Vector3.zero, showDebug: true);
    }

    private void OnEnable()
    {
        if (Grid != null)
        {
            Grid.OnCellChange += CellChange;
        }
    }

    private void OnDisable()
    {
        if (Grid != null)
        {
            Grid.OnCellChange -= CellChange;
        }
    }

    public virtual void CellChange(Vector2Int position, int value)
    {
        UpdateVisual();
    }

    public virtual void UpdateVisual()
    {
        var meshData = CreateEmptyMeshQuads(Grid.Width * Grid.Height);

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                int index = x * Grid.Height + y;
                Vector3 quadSize = Vector3.one * Grid.CellSize;
                Vector3 position = Grid.CellToWorld(x, y) + quadSize * 0.5f;

                //Calculave UV for the texture
                int value = Grid.GetCell(x, y);
                float normalizedValue = (float) 50 / HEAT_MAP_MAX_VALUE;
                Vector2 gridValueUv = new Vector2(normalizedValue, 0f);

                MeshUtils.AddToMeshArrays(meshData.vertices, meshData.uvs, meshData.triangles, index, position, 0f, quadSize, gridValueUv, gridValueUv);
            }
        }

        mesh.vertices = meshData.vertices;
        mesh.uv = meshData.uvs;
        mesh.triangles = meshData.triangles;
    }

    // TODO: Move this method to another class
    public static (Vector3[] vertices, Vector2[] uvs, int[] triangles) CreateEmptyMeshQuads(int quadCount)
    {
        var vertices = new Vector3[4 * quadCount];
        var uvs = new Vector2[4 * quadCount];
        var triangles = new int[6 * quadCount];

        return (vertices, uvs, triangles);
    }

}
