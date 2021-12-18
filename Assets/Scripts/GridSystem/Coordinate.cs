public class Coordinate
{
    public int x { get; protected set; }
    public int y { get; protected set; }

    public Coordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return x + "_" + y;
    }
}