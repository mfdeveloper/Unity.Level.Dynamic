using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CellMeshData
{
    public int gridIndex;
    public Vector2Int cellPosition;
    public Vector3 worldPosition, quadSize;
}

public abstract class ReaderMeshFile<T> : ReaderFile where T : new()
{
    public const int TEXTURE_MAP_MAX_VALUE = 100;

    // --- Events ---
    public Func<CellMeshData, Vector2> OnGenerateUv;

    protected new T[,] characters;
    protected Mesh mesh = new Mesh();
    protected bool updateMesh = false;

    public GridBase<T> Grid { get; set; }

    public new virtual ReaderMeshFile<T> Parse()
    {
        if (dimensions != default)
        {
            Grid = new GridBase<T>(width: dimensions.width, height: dimensions.height, 10f);
        }

        return this;
    }

    public new virtual Mesh Generate(Transform parent)
    {
        // Check if file is parsed
        if (!IsParsed)
        {
            try
            {
                Parse();
            }
            catch (NotImplementedException)
            {
                Debug.LogFormat("The class {0} file reader not contains a Parse() method implementation", GetType().Name);
            }
        }

        if (characters.Length == 0 || (dimensions.width == 0 && dimensions.height == 0))
        {
            return null;
        }

        var meshData = MeshUtils.CreateEmptyMeshQuads(Grid.Width * Grid.Height);

        for (int x = 0; x < Grid.Width; x++)
        {
            for (int y = 0; y < Grid.Height; y++)
            {
                Vector3 quadSize = Vector3.one * Grid.CellSize;

                var cellData = new CellMeshData
                {
                    gridIndex = x * Grid.Height + y,
                    cellPosition = new Vector2Int(x, y),
                    worldPosition = Grid.CellToWorld(x, y) + quadSize * 0.5f,
                    quadSize = quadSize
                };

                //Calculave UV for the texture
                Vector2 valueUv = OnGenerateUv?.Invoke(cellData) ?? Vector2.zero;   

                MeshUtils.AddToMeshArrays(meshData, cellData.gridIndex, cellData.worldPosition, 0f, quadSize, valueUv, valueUv);
            }
        }

        mesh.vertices = meshData.vertices;
        mesh.uv = meshData.uvs;
        mesh.triangles = meshData.triangles;

        return mesh;
    }

    protected override void AddElements(Vector3[] spawnPositions, Transform parent)
    {
        throw new NotImplementedException();
    }
}
