using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingHandler : MonoBehaviour
{
    public static BuildingHandler _instance;
    [SerializeField] private PlaceableObjectSO placeableObject;
    [SerializeField] private Direction currentDirection = Direction.up;
    [SerializeField] private GridManager gm;
    [SerializeField] private bool buildMode = false;
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

        if (Input.GetKeyDown(KeyCode.Escape)) this.enabled = false;
        if (Input.GetKeyDown(KeyCode.R)) ToggleNextDirection();

        Coordinate coord = gm.grid.GetGridCoordinateFromWorldPos(gm.grid.GetMouseWorldPosition(Input.mousePosition));
        if (coord != null)
        {
            bool OverUI = EventSystem.current.IsPointerOverGameObject();
            Vector3 worldPos = gm.grid.GetWorldPositionFromGridCoords(coord);
            if (!buildMode) // STATE 1 
            {
                if (Input.GetMouseButtonDown(1) && !OverUI) DestroyBuilding(coord);

            }
            else // STATE 2
            {
                int yRotation = placeableObject.GetDirectionRotation(currentDirection);
                Vector3 buildPos = worldPos + placeableObject.GetDirectionOffsetXZ(currentDirection);
                List<Coordinate> coords = placeableObject.GetNeededCoordinates(coord.x, coord.y, currentDirection);
                UpdateBluePrint(buildPos, yRotation, coords);
                currentBluePrint.GetComponent<PlaceableObjectHandler>().ToogleErrorVisual(!CheckBuildPosition(coord.x, coord.y));

                if (Input.GetMouseButtonDown(1)) DeselectActiveBuildingSelection();
                if (!OverUI)
                {
                    if (Input.GetMouseButtonDown(0) && Input.GetKey(KeyCode.LeftShift))
                    {
                        BuildSelectedBuilding(false, worldPos, yRotation, coords);
                    }
                    else if (Input.GetMouseButtonDown(0))
                    {
                        BuildSelectedBuilding(true, worldPos, yRotation, coords);
                    }
                    else if (Input.GetMouseButton(0) && placeableObject.isDragable)
                    {
                        HandleDragBuild();
                    }
                }
            }
        }
        // Debug.Log(worldPos);
    }

    private void HandleDragBuild()
    {

    }

    private void BuildSelectedBuilding(bool multiBuild, Vector3 buildPosition, int directionRotation, List<Coordinate> coordinates)
    {
        if (CheckBuildPosition(coordinates))
        {
            PlaceableObjectHandler pOH = currentBluePrint.GetComponent<PlaceableObjectHandler>();
            pOH.myCoordinates = coordinates;
            pOH.UnsetTransparency();
            foreach (Coordinate coord in coordinates)
            {
                gm.grid.gridFields[coord.x, coord.y].SetPlaceableObject(placeableObject, currentBluePrint);
            }

            if (!multiBuild)
            {
                pOH.OnBuild();
                ToogleBuildMode(false);
            }
            else
            {
                pOH.OnBuild();
                currentBluePrint = InstantiateBluePrint(buildPosition, placeableObject.GetDirectionRotation(currentDirection));
            }
        }
        else
        {
            Debug.LogError("GridField already has an Object built on.");
            //TODO Implement proper error display workflow
        }
    }

    private void DestroyBuilding(Coordinate inCoord)
    {
        if (!gm.grid.gridFields[inCoord.x, inCoord.y].CanBuild())
        { // if CanBuild is false, means there is a object placed on this grid tile
            GameObject gO = gm.grid.gridFields[inCoord.x, inCoord.y].GetGameObject();
            List<Coordinate> coords = gO.GetComponent<PlaceableObjectHandler>().myCoordinates;
            foreach (Coordinate coord in coords)
            {
                gm.grid.gridFields[coord.x, coord.y].ClearPlaceableObject();
            }
            DestroyImmediate(gO);
        }
    }

    private void DeselectActiveBuildingSelection()
    {
        DestroyImmediate(currentBluePrint);
        ToogleBuildMode(false);
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

    public void SetCurrentPlaceableObject(PlaceableObjectSO pO)
    {
        if (currentBluePrint) DeselectActiveBuildingSelection();
        this.enabled = true;
        placeableObject = pO;
        ToogleBuildMode(true);
    }

    public void ActiveBuildingSystem()
    {
        this.enabled = true;
    }

    private GameObject InstantiateBluePrint(Vector3 worldPos, int directionRotation)
    {
        return Instantiate(placeableObject.prefab, worldPos, Quaternion.Euler(0, directionRotation, 0));
    }

    private void UpdateBluePrint(Vector3 buildPostion, int directionRotation, List<Coordinate> coordinates)
    {
        currentBluePrint.transform.localPosition = buildPostion;
        currentBluePrint.transform.localRotation = Quaternion.Euler(0, directionRotation, 0);
        currentBluePrint.GetComponent<PlaceableObjectHandler>().myCoordinates = coordinates;
        SetErrorVisual(CheckBuildPosition(coordinates));
    }

    private void ToogleBuildMode(bool state)
    {
        buildMode = state;
        currentDirection = Direction.up;
        if (!buildMode)
        {
            currentBluePrint = null;
            placeableObject = null;
            //TODO implement CallBack "OnBuildModeDeactive"
        }
        else
        {
            currentBluePrint = InstantiateBluePrint(Vector3.zero, placeableObject.GetDirectionRotation(currentDirection));
            //TODO implement CallBack "OnBuildModeActive"
        }
    }

    private void SetErrorVisual(bool error)
    {
        currentBluePrint.GetComponent<PlaceableObjectHandler>().ToogleErrorVisual(error);
    }

    private bool CheckBuildPosition(int x, int y)
    {
        List<Coordinate> coordinates = placeableObject.GetNeededCoordinates(x, y, currentDirection);
        return CheckBuildPosition(coordinates);
    }

    /// <summary>
    /// Returns Ture if you can build
    /// </summary>
    /// <param name="coordinates"></param>
    /// <returns></returns>
    private bool CheckBuildPosition(List<Coordinate> coordinates)
    {
        bool canBuild = true;
        foreach (Coordinate coord in coordinates)
        {
            if (gm.grid.ValidateCoords(coord))
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