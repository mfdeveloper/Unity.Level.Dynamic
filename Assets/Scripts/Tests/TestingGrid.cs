using UnityEngine;
using CodeMonkey.Utils;

public class TestingGrid : MonoBehaviour
{

    private GridBase<bool> grid;

    void Awake()
    {
        grid = new GridBase<bool>(width: 4, height: 2, cellSize: 10f, origin: new Vector3(20, 0));
    }

    void Update()
    {
        // Just for tests. Refactor this for best tests
        if (Input.GetMouseButtonDown(0))
        {
            var mousePos = UtilsClass.GetMouseWorldPosition();
            grid.SetCell(mousePos, true);
        }

        if (Input.GetMouseButtonDown(1))
        {
            var mousePos = UtilsClass.GetMouseWorldPosition();
            Debug.Log(grid.GetCell(mousePos));
        }
    }
}
