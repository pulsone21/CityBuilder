using UnityEngine;
[System.Serializable]
public class Coordinate
{
    [SerializeField] public int x;
    [SerializeField] public int y;

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return x + "_" + y;
    }

    public bool CheckForSameCoordinate(Coordinate coord)
    {
        return coord.x == x && coord.y == y;
    }

    public Direction GetRelativDirectionToCoord(Coordinate coord)
    {
        //TODO Figure out how correctly get the direction of the mouse movement.
        Coordinate computedCoordinate = ComputeCoord(coord);
        Debug.Log(computedCoordinate.ToString());
        if (computedCoordinate.x == 1) return Direction.right;
        if (computedCoordinate.x == -1) return Direction.left;
        if (computedCoordinate.y == 1) return Direction.up;
        return Direction.down;
    }

    private Coordinate ComputeCoord(Coordinate coord)
    {
        int newX = coord.x > x ? 1 : -1;
        int newY = coord.y > y ? 1 : -1;
        return new Coordinate(newX, newY);
    }
}
