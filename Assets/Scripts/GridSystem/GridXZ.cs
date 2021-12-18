using System;
using UnityEngine;
using TMPro;

public class GridXZ<TGridObject>
{
    private int width, height, cellSize;
    private Vector3 originPosition;
    public TGridObject[,] gridFields { get; protected set; }
    private TextMeshPro[,] debugTextMeshes;
    public Plane rayCastPlane { get; protected set; }

    public GridXZ(int width, int height, int cellSize, Vector3 originPosition, Transform parent, Func<GridXZ<TGridObject>, int, int, TGridObject> createObject)
    {
        this.width = width;
        this.height = height;
        this.originPosition = originPosition;
        this.cellSize = cellSize;
        this.rayCastPlane = new Plane(originPosition, (new Vector3(0, 0, height) * cellSize + originPosition), (new Vector3(width, 0, height) * cellSize + originPosition));
        gridFields = new TGridObject[width, height];
        debugTextMeshes = new TextMeshPro[width, height];

        for (int x = 0; x < gridFields.GetLength(0); x++)
        {
            for (int z = 0; z < gridFields.GetLength(1); z++)
            {
                gridFields[x, z] = createObject(this, x, z);
                debugTextMeshes[x, z] = Utils.CreateWorldText(parent, gridFields[x, z].ToString(), GetWorldPosFromGridCoords(x, z) + new Vector3(cellSize, 0, cellSize) * .5f);
                debugTextMeshes[x, z].transform.Rotate(new Vector3(90, 0, 0));
                debugTextMeshes[x, z].enabled = false;
            }
        }

        ToogleDebug(true);
    }

    public Vector3 GetMouseWorldPosition(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
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

    public void UpdateDebugText(int x, int z)
    {
        if (ValidateCoords(x, z)) debugTextMeshes[x, z].text = gridFields[x, z].ToString();
    }

    /// <summary>
    /// Returns a Vector 3 centered on the specified Grid Coordinates
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Nullable<Vector3> GetWorldPositionFromGridCoords(int x, int z)
    {
        if (ValidateCoords(x, z)) return GetWorldPosFromGridCoords(x, z);
        return null;
    }

    /// <summary>
    /// If we provide a coordinate then it can be asumed that this coordinate is already verified
    /// </summary>
    /// <param name="coord"></param>
    /// <returns></returns>
    public Vector3 GetWorldPositionFromGridCoords(Coordinate coord)
    {
        return GetWorldPosFromGridCoords(coord.x, coord.y);
    }

    /// <summary>
    /// Only for class internal use, dont have an coordination validation
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private Vector3 GetWorldPosFromGridCoords(int x, int z)
    {
        return new Vector3(x, 0, z) * cellSize + originPosition;
    }

    /// <summary>
    /// Returns a Coordinate which contains the Gridposition calculated from specified worldPosition
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <returns></returns>
    public Coordinate GetGridCoordinateFromWorldPos(Vector3 worldPosition)
    {
        int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
        int z = Mathf.FloorToInt((worldPosition - originPosition).z / cellSize);
        if (ValidateCoords(x, z)) return new Coordinate(x, z);
        return null;
    }

    public void ToogleDebug(bool debugMode)
    {
        if (debugMode)
        {
            for (int x = 0; x < this.gridFields.GetLength(0); x++)
            {
                for (int z = 0; z < gridFields.GetLength(1); z++)
                {
                    debugTextMeshes[x, z].enabled = true;
                    Debug.DrawLine(GetWorldPosFromGridCoords(x, z), GetWorldPosFromGridCoords(x, z + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPosFromGridCoords(x, z), GetWorldPosFromGridCoords(x + 1, z), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPosFromGridCoords(0, height), GetWorldPosFromGridCoords(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPosFromGridCoords(width, 0), GetWorldPosFromGridCoords(width, height), Color.white, 100f);

        }
        else
        {

        }
    }

    /// <summary>
    /// Validates if given Coords are in bounds of the the grid array
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <returns>True if Coordinates in ArrayBounds</returns>
    public bool ValidateCoords(int x, int z)
    {
        return (x >= 0 && z >= 0 && x < width && z < height);
    }

    public bool ValidateCoords(Coordinate coord)
    {
        return ValidateCoords(coord.x, coord.y);
    }


}