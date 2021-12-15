using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class GridManager : MonoBehaviour
{
    public static GridManager _instance;
    [SerializeField] private int width, height, cellSize;
    public GridXZ<GridObjectXZ> grid { get; protected set; }
    [SerializeField] private List<PlaceableObject> placeableObjects;

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

    public List<PlaceableObject> GetPlaceableObjects()
    {
        return placeableObjects;
    }

    private void LoadPlacableObjects()
    {
        PlaceableObject[] pOs = Resources.LoadAll<PlaceableObject>("ScriptableObjects/PlaceableObjects");
        foreach (PlaceableObject pO in pOs)
        {
            placeableObjects.Add(pO);
        }
    }
}