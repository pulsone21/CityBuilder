public class GridObject
{
    public PlaceableObject PlaceableObject;

    public virtual void SetPlaceableObject(PlaceableObject PlaceableObject)
    {
        this.PlaceableObject = PlaceableObject;
    }

    public virtual void ClearPlaceableObject()
    {
        this.PlaceableObject = null;
    }

    public PlaceableObject GetPlaceableObject()
    {
        return this.PlaceableObject;
    }

    public bool CanBuild()
    {
        return PlaceableObject == null;
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
        //TODO find a proper way to delete the GameObject
        this.PlaceableObject = null;
        grid.UpdateDebugText(x, z);
    }

    public override void SetPlaceableObject(PlaceableObject PlaceableObject)
    {
        this.PlaceableObject = PlaceableObject;
        grid.UpdateDebugText(x, z);
    }


    public override string ToString()
    {
        return x + "," + z + "\n" + PlaceableObject?.name;
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
        // this.PlaceableObject.
        //TODO find a proper way to delete the GameObject
        this.PlaceableObject = null;
        grid.UpdateDebugText(x, y);
    }

    public override void SetPlaceableObject(PlaceableObject PlaceableObject)
    {
        this.PlaceableObject = PlaceableObject;
        grid.UpdateDebugText(x, y);
    }

    public override string ToString()
    {
        return x + "," + y + "\n" + PlaceableObject?.name;
    }
}