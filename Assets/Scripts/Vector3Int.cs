using UnityEngine;

[System.Serializable]
public class Vector3Int
{
    public int x, y, z;
    public Vector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    static public Vector3Int toV3Int(Vector3 v)
    {
        return new Vector3Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y), Mathf.FloorToInt(v.z));
    }

    public Vector3Int add(Vector3Int v)
    {
        return new Vector3Int(v.x + x, v.y + y, v.z + z);
    }

    public Vector3Int add(Vector3 v)
    {
        return new Vector3Int((int)v.x + x, (int)v.y + y, (int)v.z + z);
    }

    public bool compare (Vector3Int v)
    {
        return v.x == this.x && v.y == this.y && v.z == this.z;
    }

    public Vector3Int multiply (Vector3Int v)
    {
        return new Vector3Int(v.x * this.x, v.y * this.y, v.z * this.z);
    }

    public Vector3 toV3()
    {
        return new Vector3(this.x, this.y, this.z);
    }

    public override string ToString()
    {
        return "(" + this.x + ", " + this.y + ", " + this.z + ")";
    }

    public Vector3Int floorToMultipleOfSize(Vector3Int k)
    {
        return new Vector3Int(multipleOfSize(x, k.x), multipleOfSize(y, k.y), multipleOfSize(z, k.z));
    }

    public Vector3Int floor()
    {
        return new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y), Mathf.FloorToInt(z));
    }

    private int multipleOfSize (int x, int k = 0)
    {
        foreach (int num in Utils.NaturalNumbers())
        {
            int multiple = 6;
            int curMultiple = k + multiple * num;
            if (x <= curMultiple)
            {
                x = curMultiple - multiple;
                break;
            }
        }
        return x;
    }

    public Vector3Int div(Vector3Int vi)
    {
        return new Vector3Int(x / vi.x, y / vi.y, z / vi.z);
    }

    public static Vector3Int zero = new Vector3Int(0, 0, 0);
    public static Vector3Int one = new Vector3Int(1, 1, 1);
}
