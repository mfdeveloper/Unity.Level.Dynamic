using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reflection;
using CodeMonkey.Utils;

public class GridBase<T> where T : new()
{
    protected T[,] gridArray;
    protected TextMesh[,] debugTextArray;

    // --- Events ---
    public Action<Vector2Int, T> OnCellChange;
    public Func<Vector2Int, T, T> OnCellFilter;
    public bool showDebug;

    public int Width { get; set; }
    public int Height { get; set; }
    public float CellSize { get; set; }
    public Vector3 OriginPosition { get; set; }

    protected static ConstructorInfo genericConstructor;
    public static ConstructorInfo DefaultConstructor { 
        get {
            if (genericConstructor == null)
            {
                // A constructor for generic T type, with parameters: This grid, width and height
                genericConstructor = typeof(T).GetConstructor(new Type[] { typeof(GridBase<T>), typeof(int), typeof(int) });
            }

            return genericConstructor;
        }
        private set {}
    }

    public GridBase(int width, int height, float cellSize = 10f, Vector3 origin = default, bool showDebug = true)
    {
        Width = width;
        Height = height;
        CellSize = cellSize;
        OriginPosition = origin;
        this.showDebug = showDebug;

        gridArray = new T[Width, Height];
        debugTextArray = new TextMesh[Width, Height];

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                if (gridArray[x, y] == null && !typeof(T).IsValueType)
                {
                    if (DefaultConstructor != null)
                    {
                        gridArray[x, y] = (T)DefaultConstructor.Invoke(new object[] { this, x, y });
                    } else
                    {
                        gridArray[x, y] = new T();
                    }
                }

                if (this.showDebug)
                {
                    var worldPosition = CellToWorld(x, y);

                    debugTextArray[x, y] = UtilsClass.CreateWorldText(
                                            text: gridArray[x, y].ToString(),
                                            parent: null,
                                            localPosition: worldPosition + new Vector3(cellSize, cellSize) * 0.5f,
                                            fontSize: 20,
                                            color: Color.white,
                                            textAnchor: TextAnchor.MiddleCenter
                                        );

                    Debug.DrawLine(worldPosition, CellToWorld(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(worldPosition, CellToWorld(x + 1, y), Color.white, 100f);
                }
            }
        }

        if (showDebug)
        {
            Debug.DrawLine(CellToWorld(0, height), CellToWorld(width, height), Color.white, 100f);
            Debug.DrawLine(CellToWorld(width, 0), CellToWorld(width, height), Color.white, 100f);
        }
    }

    public virtual void TriggerCellChange(Vector2Int cellPosition, T value = default)
    {
        OnCellChange?.Invoke(cellPosition, value);
    }

    public void SetCell(int x, int y, T value)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            if (OnCellFilter != null)
            {
                value = OnCellFilter.Invoke(new Vector2Int(x, y), value);
            }

            gridArray[x, y] = value;
            debugTextArray[x, y].text = value.ToString();
        }

        OnCellChange?.Invoke(new Vector2Int(x, y), value);
    }

    public void SetCell(Vector3 worldPosition, T value)
    {
        var cellPosition = WorldToCell(worldPosition);
        SetCell(cellPosition.x, cellPosition.y, value);
    }

    public T GetCell(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Width && y < Height)
        {
            return gridArray[x, y];
        }

        return default;
    }

    public T GetCell(Vector3 worldPosition)
    {
        var cellPosition = WorldToCell(worldPosition);

        return GetCell(cellPosition.x, cellPosition.y);
    }

    public Vector2Int WorldToCell(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.FloorToInt((worldPosition - OriginPosition).x / CellSize), Mathf.FloorToInt((worldPosition - OriginPosition).y / CellSize));
    }

    public Vector3 CellToWorld(int x, int y)
    {
        return new Vector3(x, y) * CellSize + OriginPosition;
    }
}
