using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/PlaceableObject")]
public class PlaceableObject : ScriptableObject
{
    public enum Direction { up, down, left, right };
    public Direction dir = Direction.up;
    public new string name;
    private int x, z;
    public GameObject prefab;
    public int width, height;
    public Sprite itemPicture;

    public int GetDirectionRotation()
    {
        switch (this.dir)
        {
            case Direction.down: return 180;
            case Direction.left: return 270;
            case Direction.right: return 90;
            default: return 0; // should always be set to Up, so 0;
        }
    }

    public Vector3 GetDirectionOffset()
    {
        switch (this.dir)
        {
            case Direction.down: return new Vector3(10, 0, 10);
            case Direction.left: return new Vector3(10, 0, 0);
            case Direction.right: return new Vector3(0, 0, 10);
            default: return Vector3.zero; // should always be set to Up, so 0;
        }
    }

    public void ToggleNextDirection()
    {
        switch (this.dir)
        {
            case Direction.up: dir = Direction.right; break;
            case Direction.down: dir = Direction.left; break;
            case Direction.left: dir = Direction.up; break;
            case Direction.right: dir = Direction.down; break;
        }
    }

}
