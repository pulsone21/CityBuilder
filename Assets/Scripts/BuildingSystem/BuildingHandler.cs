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
    [SerializeField] private bool toogleBuildMode = false;
    [SerializeField] private GameObject currentBluePrint;

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

    private void OnDisable()
    {
        if (currentBluePrint) DestroyImmediate(currentBluePrint);
        ToogleBuildMode(false);
    }

    void Update()
    {
        if (placeableObject != null)
        {
            if (currentBluePrint == null)
            {
                currentBluePrint = InstantiateBluePrint();
                EvalPosition(Input.mousePosition);
            }
            else
            {
                UpdateBluePrint();
                EvalPosition(Input.mousePosition);
            }
            if (Input.GetKeyDown(KeyCode.R)) ToggleNextDirection();
        }

        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (currentBluePrint != null)
            {
                if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
                {
                    HandleLeftClick(false);
                }
                else if (Input.GetMouseButtonDown(0))
                {
                    HandleLeftClick(true);
                }
            }
            if (Input.GetMouseButtonDown(1) && !toogleBuildMode) HandleRightClick();
            //TODO Implement some kind of Drag and Drop functionallity for Roads, Rails etc.
        }

        if (Input.GetKeyDown(KeyCode.Escape)) this.enabled = false;
    }

    private void HandleLeftClick(bool unsetGO)
    {
        gm.grid.GetGridPositionFromWorldPos(gm.grid.GetMouseWorldPosition(Input.mousePosition), out int x, out int z);
        if (gm.grid.ValidateCoords(x, z))
        {
            List<Coordinate> coordinates = placeableObject.GetNeededCoordinates(x, z, currentDirection);
            if (CheckBuildPosition(coordinates))
            {
                Vector3 worldPosition = gm.grid.GetWorldPositionFromGridCoords(x, z) + placeableObject.GetDirectionOffsetXZ(currentDirection);
                currentBluePrint.transform.localPosition = worldPosition;
                currentBluePrint.transform.localRotation = Quaternion.Euler(0, placeableObject.GetDirectionRotation(currentDirection), 0);
                PlaceableObjectHandler pOH = currentBluePrint.GetComponent<PlaceableObjectHandler>();
                pOH.myCoordinates = coordinates;
                pOH.UnsetTransparency();
                foreach (Coordinate coord in coordinates)
                {
                    gm.grid.gridFields[coord.x, coord.y].SetPlaceableObject(placeableObject, currentBluePrint);
                }

                if (unsetGO) //? we dont want to multi build
                {
                    ToogleBuildMode(false);
                }
                else//? we want to multi build, just create a new Instance
                {
                    currentBluePrint = InstantiateBluePrint();
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
        this.enabled = true;
        placeableObject = pO;
        ToogleBuildMode(true);
    }

    public void ActiveBuildingSystem()
    {
        this.enabled = true;
    }

    private GameObject InstantiateBluePrint()
    {
        gm.grid.GetGridPositionFromWorldPos(gm.grid.GetMouseWorldPosition(Input.mousePosition), out int x, out int z);
        Vector3 worldPosition = gm.grid.GetWorldPositionFromGridCoords(x, z) + placeableObject.GetDirectionOffsetXZ(currentDirection);
        return Instantiate(placeableObject.prefab, worldPosition, Quaternion.Euler(0, placeableObject.GetDirectionRotation(currentDirection), 0));
    }

    private void UpdateBluePrint()
    {
        if (currentBluePrint != null)
        {
            gm.grid.GetGridPositionFromWorldPos(gm.grid.GetMouseWorldPosition(Input.mousePosition), out int x, out int z);
            Vector3 worldPosition = gm.grid.GetWorldPositionFromGridCoords(x, z) + placeableObject.GetDirectionOffsetXZ(currentDirection);
            currentBluePrint.transform.localPosition = worldPosition;
            currentBluePrint.transform.localRotation = Quaternion.Euler(0, placeableObject.GetDirectionRotation(currentDirection), 0);
        }
    }

    private void ToogleBuildMode(bool state)
    {
        if (!state)
        {
            toogleBuildMode = state;
            currentDirection = Direction.up;
            currentBluePrint = null;
            placeableObject = null;
        }
        else
        {
            toogleBuildMode = state;
            currentDirection = Direction.up;
        }

        //TODO implement some UI to visualize that are u in Build mode;
    }

    private void EvalPosition(Vector3 worldPos)
    {
        gm.grid.GetGridPositionFromWorldPos(gm.grid.GetMouseWorldPosition(Input.mousePosition), out int x, out int z);
        List<Coordinate> coordinates = placeableObject.GetNeededCoordinates(x, z, currentDirection);
        if (!CheckBuildPosition(coordinates))
        {
            currentBluePrint.GetComponent<PlaceableObjectHandler>().ToogleErrorVisual(true);
        }
        else
        {
            currentBluePrint.GetComponent<PlaceableObjectHandler>().ToogleErrorVisual(false);
        }
    }

    private bool CheckBuildPosition(int x, int y)
    {
        List<Coordinate> coordinates = placeableObject.GetNeededCoordinates(x, y, currentDirection);
        return CheckBuildPosition(coordinates);
    }

    private bool CheckBuildPosition(List<Coordinate> coordinates)
    {
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
        return canBuild;
    }
}