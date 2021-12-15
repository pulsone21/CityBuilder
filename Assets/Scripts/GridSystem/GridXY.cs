using System;
using UnityEngine;
using TMPro;

public class GridXY<TGridObject>
{

    private int width, height, cellSize;
    private Vector3 originPosition;
    public TGridObject[,] gridFields { get; protected set; }
    private TextMeshPro[,] debugTextMeshes;
    public Plane rayCastPlane { get; protected set; }

    public GridXY(int width, int height, int cellSize, Vector3 originPosition, Transform parent, Func<GridXY<TGridObject>, int, int, TGridObject> createObject)
    {
        this.width = width;
        this.height = height;
        this.originPosition = originPosition;
        this.cellSize = cellSize;

        this.rayCastPlane = new Plane(originPosition, GetWorldPositionFromGridCoords(0, height), GetWorldPositionFromGridCoords(width, height));
        gridFields = new TGridObject[width, height];
        debugTextMeshes = new TextMeshPro[width, height];

        for (int x = 0; x < gridFields.GetLength(0); x++)
        {
            for (int y = 0; y < gridFields.GetLength(1); y++)
            {
                gridFields[x, y] = createObject(this, x, y);
                debugTextMeshes[x, y] = Utils.CreateWorldText(parent, gridFields[x, y].ToString(), GetWorldPositionFromGridCoords(x, y) + new Vector3(cellSize, 0, cellSize) * .5f);
                debugTextMeshes[x, y].enabled = false;
            }
        }

        ToogleDebug(true);
    }

    public Vector3 GetMouseWorldPosition(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (rayCastPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        };
        return Vector3.zero;
    }

    public int GetHeight()
    {
        return height;
    }

    public int GetWidth()
    {
        return width;
    }

    public int GetCellSize()
    {
        return cellSize;
    }

    public Vector3 GetOriginVector()
    {
        return originPosition;
    }

    public void UpdateDebugText(int x, int y)
    {
        if (ValidateCoords(x, y)) debugTextMeshes[x, y].text = gridFields[x, y].ToString();
    }

    public Vector3 GetWorldPositionFromGridCoords(int x, int y)
    {
        return new Vector3(x, y) * cellSize + originPosition;
    }

    public void GetGridPositionFromWorldPos(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
    }


    public void ToogleDebug(bool debugMode)
    {
        if (debugMode)
        {
            for (int x = 0; x < this.gridFields.GetLength(0); x++)
            {
                for (int y = 0; y < gridFields.GetLength(1); y++)
                {
                    debugTextMeshes[x, y].enabled = true;
                    Debug.DrawLine(GetWorldPositionFromGridCoords(x, y), GetWorldPositionFromGridCoords(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPositionFromGridCoords(x, y), GetWorldPositionFromGridCoords(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPositionFromGridCoords(0, height), GetWorldPositionFromGridCoords(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPositionFromGridCoords(width, 0), GetWorldPositionFromGridCoords(width, height), Color.white, 100f);

        }
        else
        {

        }
    }

    /// <summary>
    /// Validates if given Coords are in bounds of the the grid array
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>True if Coordinates in ArrayBounds</returns>
    public bool ValidateCoords(int x, int y)
    {
        return (x >= 0 && y >= 0 && x < width && y < height);
    }


}