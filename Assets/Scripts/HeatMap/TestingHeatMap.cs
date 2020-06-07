using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestingHeatMap : MonoBehaviour
{
    [SerializeField]
    protected HeatMapVisual heatMapVisual;

    //protected GridBase<int> grid;

    // Start is called before the first frame update
    void Awake()
    {
        
        //heatMapVisual.grid = grid;
    }

    private void Start()
    {
        if (heatMapVisual != null)
        {
            heatMapVisual.UpdateVisual();
        }
    }

    private void OnEnable()
    {
        if (heatMapVisual.Grid != null)
        {
            heatMapVisual.Grid.OnCellFilter += ConvertCell;
        }
    }

    private void OnDisable()
    {
        if (heatMapVisual.Grid != null)
        {
            heatMapVisual.Grid.OnCellFilter -= ConvertCell;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = UtilsClass.GetMouseWorldPosition();

            var value = heatMapVisual.Grid.GetCell(mousePos);
            heatMapVisual.Grid.SetCell(mousePos, value + 5);
        }
    }

    public virtual int ConvertCell(Vector2Int position, int value)
    {
        return Mathf.Clamp(value, HeatMapVisual.HEAT_MAP_MIN_VALUE, HeatMapVisual.HEAT_MAP_MAX_VALUE);
    }
}
