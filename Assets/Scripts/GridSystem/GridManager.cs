using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GridManager : MonoBehaviour
{
    public static GridManager _instance;
    [SerializeField] private int width, height, cellSize;
    public GridXZ<GridObjectXZ> grid { get; protected set; }
    [SerializeField] private List<PlaceableObjectSO> placeableObjects;

    void Awake()
    {
        if (_instance)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
        }

        LoadPlacableObjects();
        grid = new GridXZ<GridObjectXZ>(width, height, cellSize, this.transform.position, this.transform, (GridXZ<GridObjectXZ> g, int x, int z) => new GridObjectXZ(x, z, g));

    }

    public List<PlaceableObjectSO> GetPlaceableObjects()
    {
        return placeableObjects;
    }

    private void LoadPlacableObjects()
    {
        PlaceableObjectSO[] pOs = Resources.LoadAll<PlaceableObjectSO>("ScriptableObjects/PlaceableObjects");
        foreach (PlaceableObjectSO pO in pOs)
        {
            placeableObjects.Add(pO);
        }
    }
}