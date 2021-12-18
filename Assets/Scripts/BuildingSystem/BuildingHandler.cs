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
    private GameObject currentBluePrint;
    [SerializeField] private List<GameObject> placedBluePrints = new List<GameObject>();
    [SerializeField] private List<Coordinate> bluePrintOccupiedCoords = new List<Coordinate>();
    [SerializeField] private bool inDrag = false;
    [SerializeField] private Coordinate lastCoord;

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
        DeselectActiveBuildingSelection();
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
                UpdateBluePrint(buildPos, yRotation, coords, currentBluePrint);
                if (Input.GetMouseButtonDown(1)) DeselectActiveBuildingSelection();
                if (!OverUI)
                {
                    if (Input.GetMouseButtonUp(0) && Input.GetKey(KeyCode.LeftShift) && !inDrag)
                    {
                        BuildSelectedBuilding(coords, currentBluePrint);
                        currentBluePrint = InstantiateBluePrint(buildPos, placeableObject.GetDirectionRotation(currentDirection));
                    }
                    else if (Input.GetMouseButtonUp(0) && !inDrag)
                    {
                        BuildSelectedBuilding(coords, currentBluePrint);
                        ToogleBuildMode(false);
                    }
                    else if (Input.GetMouseButtonUp(0) && inDrag)
                    {
                        foreach (GameObject gO in placedBluePrints)
                        {
                            BuildSelectedBuilding(gO.GetComponent<PlaceableObjectHandler>().myCoordinates, gO);
                        }
                        inDrag = false;
                        lastCoord = null;
                        placedBluePrints.Clear();
                    }
                    else if (Input.GetMouseButton(0) && placeableObject.isDragable)
                    {
                        if (inDrag)
                        {
                            if (!lastCoord.CheckForSameCoordinate(coord))
                            {
                                HandleDragBuild(buildPos, coord, coords);
                                lastCoord = coord;
                            }
                        }
                        else
                        {
                            inDrag = true;
                            lastCoord = coord;
                            Debug.Log(coord.ToString());
                            HandleDragBuild(buildPos, coord, coords);
                        }
                    }
                }
            }
        }
    }

    private void HandleDragBuild(Vector3 currentBuildPos, Coordinate currentCoord, List<Coordinate> coordinates)
    {
        //TODO Implement somekind of System to determin in wich direction we are going; 

        Debug.Log(lastCoord.GetRelativDirectionToCoord(currentCoord).ToString());

        if (CheckBuildPosition(currentBuildPos) && CheckNotAlreadyPlacedOnCoord(currentCoord))
        {
            placedBluePrints.Add(PreBuildGameObject(currentBluePrint, coordinates));
            bluePrintOccupiedCoords.Add(currentCoord);
            currentBluePrint = InstantiateBluePrint(currentBuildPos, placeableObject.GetDirectionRotation(currentDirection));
        }
    }

    private void BuildSelectedBuilding(List<Coordinate> coordinates, GameObject currentBluePrint)
    {
        if (CheckBuildPosition(coordinates))
        {
            PlaceableObjectHandler pOH = currentBluePrint.GetComponent<PlaceableObjectHandler>();
            pOH.myCoordinates = coordinates;
            foreach (Coordinate coord in pOH.myCoordinates)
            {
                gm.grid.gridFields[coord.x, coord.y].SetPlaceableObject(placeableObject, currentBluePrint);
            }
            pOH.OnBuild();
        }
        else
        {
            Debug.LogError("GridField already has an Object built on.");
            //TODO Implement proper error display workflow
        }
    }

    private GameObject PreBuildGameObject(GameObject currentBluePrint, List<Coordinate> coordinates)
    {
        currentBluePrint.GetComponent<PlaceableObjectHandler>().myCoordinates = coordinates;
        return currentBluePrint;
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
        foreach (GameObject gO in placedBluePrints)
        {
            DestroyImmediate(gO);
        }
        placedBluePrints.Clear();
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
        if (placedBluePrints.Count > 0) DeselectActiveBuildingSelection();
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

    private void UpdateBluePrint(Vector3 buildPostion, int directionRotation, List<Coordinate> coordinates, GameObject currentBluePrint)
    {
        currentBluePrint.transform.localPosition = buildPostion;
        currentBluePrint.transform.localRotation = Quaternion.Euler(0, directionRotation, 0);
        PlaceableObjectHandler pOH = currentBluePrint.GetComponent<PlaceableObjectHandler>();
        pOH.myCoordinates = coordinates;
        pOH.ToogleErrorVisual(!CheckBuildPosition(coordinates));
    }

    private void ToogleBuildMode(bool state)
    {
        buildMode = state;
        currentDirection = Direction.up;
        if (!buildMode)
        {
            placedBluePrints.Clear();
            placeableObject = null;
            //TODO implement CallBack "OnBuildModeDeactive"
        }
        else
        {
            currentBluePrint = InstantiateBluePrint(Vector3.zero, placeableObject.GetDirectionRotation(currentDirection));
            //TODO implement CallBack "OnBuildModeActive"
        }
    }

    private bool CheckNotAlreadyPlacedOnCoord(Coordinate coordinate)
    {
        bool canBuild = true;
        foreach (Coordinate coord in bluePrintOccupiedCoords)
        {
            if (coord.CheckForSameCoordinate(coordinate))
            {
                canBuild = false;
                break;
            }
        }
        return canBuild;
    }

    private bool CheckBuildPosition(int x, int y)
    {
        List<Coordinate> coordinates = placeableObject.GetNeededCoordinates(x, y, currentDirection);
        return CheckBuildPosition(coordinates);
    }

    private bool CheckBuildPosition(Vector3 buildPos)
    {
        Coordinate coord = gm.grid.GetGridCoordinateFromWorldPos(buildPos);
        List<Coordinate> coordinates = placeableObject.GetNeededCoordinates(coord.x, coord.y, currentDirection);
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