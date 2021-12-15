using UnityEngine;
public class GridObject
{
    protected PlaceableObject placeableObject;
    protected GameObject placedGameobject;

    public virtual void SetPlaceableObject(PlaceableObject placeableObject, GameObject gameObject)
    {
        this.placeableObject = placeableObject;
        this.placedGameobject = gameObject;
    }

    public virtual void ClearPlaceableObject()
    {
        this.placeableObject = null;
        this.placedGameobject = null;
    }

    public PlaceableObject GetPlaceableObject()
    {
        return this.placeableObject;
    }

    public GameObject GetGameObject()
    {
        return this.placedGameobject;
    }

    public bool CanBuild()
    {
        return placeableObject == null;
    }


}


public class GridObjectXZ : GridObject
{
    private GridXZ<GridObjectXZ> grid;
    private int x, z;

    public GridObjectXZ(int x, int z, GridXZ<GridObjectXZ> grid)
    {
        this.grid = grid;
        this.x = x;
        this.z = z;
    }

    public override void ClearPlaceableObject()
    {
        base.ClearPlaceableObject();
        grid.UpdateDebugText(x, z);
    }

    public override void SetPlaceableObject(PlaceableObject PlaceableObject, GameObject gameObject)
    {
        base.SetPlaceableObject(PlaceableObject, gameObject);
        grid.UpdateDebugText(x, z);
    }

    public override string ToString()
    {
        return x + "," + z + "\n" + placeableObject?.name;
    }
}

public class GridObjectXY : GridObject
{
    private GridXY<GridObjectXY> grid;
    private int x, y;

    public GridObjectXY(int x, int y, GridXY<GridObjectXY> grid)
    {
        this.grid = grid;
        this.x = x;
        this.y = y;
    }

    public override void ClearPlaceableObject()
    {
        base.ClearPlaceableObject();
        grid.UpdateDebugText(x, y);
    }

    public override void SetPlaceableObject(PlaceableObject PlaceableObject, GameObject gameObject)
    {
        base.SetPlaceableObject(PlaceableObject, gameObject);
        grid.UpdateDebugText(x, y);
    }

    public override string ToString()
    {
        return x + "," + y + "\n" + placeableObject?.name;
    }
}