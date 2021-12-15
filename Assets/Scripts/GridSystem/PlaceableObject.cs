using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlaceableObject")]
public class PlaceableObject : ScriptableObject
{
    public new string name;
    public GameObject prefab;
    public int width, height;
    public Sprite itemPicture;

    /// <summary>
    /// Returns List of Coordinates which needed to build the object
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public List<Coordinate> GetNeededCoordinates(int x, int y, Direction dir)
    {
        List<Coordinate> coordinates = new List<Coordinate>();

        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                switch (dir)
                {
                    case Direction.down:
                        coordinates.Add(new Coordinate(x - w, y - h));
                        break;
                    case Direction.left:
                        coordinates.Add(new Coordinate(x - h, y + w));
                        break;
                    case Direction.right:
                        coordinates.Add(new Coordinate(x + h, y - w));
                        break;
                    default:
                        coordinates.Add(new Coordinate(x + w, y + h));
                        break; // should always be set to Up, so 0;
                }
            }
        }

        return coordinates;
    }

    public int GetDirectionRotation(Direction dir)
    {
        switch (dir)
        {
            case Direction.down: return 180;
            case Direction.left: return 270;
            case Direction.right: return 90;
            default: return 0; // should always be set to Up, so 0;
        }
    }

    public Vector3 GetDirectionOffsetXZ(Direction dir)
    {
        switch (dir)
        {
            case Direction.down: return new Vector3(10, 0, 10);
            case Direction.left: return new Vector3(10, 0, 0);
            case Direction.right: return new Vector3(0, 0, 10);
            default: return Vector3.zero; // should always be set to Up, so 0;
        }
    }

    public Vector3 GetDirectionOffsetXY(Direction dir)
    {
        switch (dir)
        {
            case Direction.down: return new Vector3(10, 10, 0);
            case Direction.left: return new Vector3(10, 0, 0);
            case Direction.right: return new Vector3(0, 10, 0);
            default: return Vector3.zero; // should always be set to Up, so 0;
        }
    }



}
