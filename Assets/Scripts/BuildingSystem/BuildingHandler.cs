using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingHandler : MonoBehaviour
{
    public static BuildingHandler _instance;
    [SerializeField] private PlaceableObject placeableObject;
    [SerializeField] private Direction currentDirection = Direction.up;
    [SerializeField] private GridManager gm;

    private void Awake()
    {
        if (_instance)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }
    }

    void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                HandleLeftClick();
            }
            if (Input.GetMouseButtonDown(1))
            {
                HandleRightClick();
            }
            //TODO Implement some kind of Drag and Drop functionallity for Roads, Rails etc.
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleNextDirection();
        }
    }

    private void HandleLeftClick()
    {
        gm.grid.GetGridPositionFromWorldPos(gm.grid.GetMouseWorldPosition(Input.mousePosition), out int x, out int z);
        if (gm.grid.ValidateCoords(x, z))
        {
            List<Coordinate> coordinates = placeableObject.GetNeededCoordinates(x, z, currentDirection);
            bool canBuild = true;
            foreach (Coordinate coord in coordinates)
            {
                if (gm.grid.ValidateCoords(coord.x, coord.y))
                {
                    if (!gm.grid.gridFields[coord.x, coord.y].CanBuild())
                    {
                        canBuild = false;
                        break;
                    }
                }
                else
                {
                    canBuild = false;
                    break;
                }
            }

            if (canBuild)
            {
                Vector3 worldPosition = gm.grid.GetWorldPositionFromGridCoords(x, z) + placeableObject.GetDirectionOffsetXZ(currentDirection);
                GameObject gO = Instantiate(placeableObject.prefab, worldPosition, Quaternion.Euler(0, placeableObject.GetDirectionRotation(currentDirection), 0));
                gO.AddComponent<PlaceableObjectHandler>().myCoordinates = coordinates;
                foreach (Coordinate coord in coordinates)
                {
                    gm.grid.gridFields[coord.x, coord.y].SetPlaceableObject(placeableObject, gO);
                }
            }
            else
            {
                Debug.LogError("GridField already has an Object built on.");
                //TODO Implement proper error display workflow
            }
        }
    }

    private void HandleRightClick()
    {
        gm.grid.GetGridPositionFromWorldPos(gm.grid.GetMouseWorldPosition(Input.mousePosition), out int x, out int z);
        if (gm.grid.ValidateCoords(x, z))
        {
            if (!gm.grid.gridFields[x, z].CanBuild())
            { // if CanBuild is false, means there is a object placed on this grid tile
                GameObject gO = gm.grid.gridFields[x, z].GetGameObject();
                List<Coordinate> coords = gO.GetComponent<PlaceableObjectHandler>().myCoordinates;
                foreach (Coordinate coord in coords)
                {
                    gm.grid.gridFields[coord.x, coord.y].ClearPlaceableObject();
                }
                DestroyImmediate(gO);
            }
        }
    }

    private void ToggleNextDirection()
    {
        switch (currentDirection)
        {
            case Direction.up: currentDirection = Direction.right; break;
            case Direction.down: currentDirection = Direction.left; break;
            case Direction.left: currentDirection = Direction.up; break;
            case Direction.right: currentDirection = Direction.down; break;
        }
    }

    public void SetCurrentPlaceableObject(PlaceableObject pO)
    {
        placeableObject = pO;
    }
}