using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;

public class TestingHeatMap : MonoBehaviour
{
    [SerializeField]
    protected HeatMapVisual heatMapVisual;

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
            heatMapVisual.Grid.OnCellFilter += ConvertCellValue;
        }
    }

    private void OnDisable()
    {
        if (heatMapVisual.Grid != null)
        {
            heatMapVisual.Grid.OnCellFilter -= ConvertCellValue;
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

    public virtual int ConvertCellValue(Vector2Int position, int value)
    {
        return Mathf.Clamp(value, HeatMapVisual.HEAT_MAP_MIN_VALUE, HeatMapVisual.HEAT_MAP_MAX_VALUE);
    }
}
