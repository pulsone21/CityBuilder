using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GridManager : MonoBehaviour
{
    public GridManager instance;
    [SerializeField] private int width, height, cellSize;
    private GridXZ<GridObjectXZ> grid;
    [SerializeField] private List<PlaceableObject> placeableObjects;
    private PlaceableObject currentPlacableObject;


    void Awake()
    {
        if (instance)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        grid = new GridXZ<GridObjectXZ>(width, height, cellSize, this.transform.position, this.transform, (GridXZ<GridObjectXZ> g, int x, int z) => new GridObjectXZ(x, z, g));

    }

    void Update()
    {

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                grid.GetGridPositionFromWorldPos(GetMouseWorldPosition(Input.mousePosition), out int x, out int z);
                if (grid.ValidateCoords(x, z))
                    if (grid.gridFields[x, z].CanBuild())
                    {
                        grid.gridFields[x, z].SetPlaceableObject(currentPlacableObject);
                        Instantiate(currentPlacableObject.prefab, grid.GetWorldPositionFromGridCoords(x, z) + currentPlacableObject.GetDirectionOffset(), Quaternion.Euler(0, currentPlacableObject.GetDirectionRotation(), 0));
                    }
                    else
                    {
                        Debug.LogError("GridField already has an Object built on.");
                        //TODO Implement proper error display workflow
                    }
            }


            if (Input.GetMouseButtonDown(1))
            {
                grid.GetGridPositionFromWorldPos(GetMouseWorldPosition(Input.mousePosition), out int x, out int z);
                if (grid.ValidateCoords(x, z))
                {
                    if (!grid.gridFields[x, z].CanBuild())
                    { // if CanBuild is false, means there is a object placed on this grid tile
                        grid.gridFields[x, z].ClearPlaceableObject();
                    }
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentPlacableObject.ToggleNextDirection();

        }
    }

    public List<PlaceableObject> GetPlaceableObjects()
    {
        return placeableObjects;
    }

    private Vector3 GetMouseWorldPosition(Vector3 mousePos)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (grid.rayCastPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        };
        return Vector3.zero;
    }

    public void SetCurrentPlaceableObject(PlaceableObject pO)
    {
        currentPlacableObject = pO;
    }
}