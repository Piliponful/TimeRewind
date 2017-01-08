using UnityEngine;

public class Main : MonoBehaviour
{
    // Number of buildings in x,z axis
    public int cityWidth = 10, cityLength = 10;

    // one single building w, h, l
    public int buildingWidth = 5, buildingHeight = 10, buildingLength = 5;

    // x, y, z size of one single cube
    public Vector3Int cubeSize = new Vector3Int(6, 6, 6);

    // Gap between individual cubeHolders
    public int gutter = 2;

    void Start()
    {
        // Town init
        Town.MainThread = this;

        Town.generateCity();
    }

    public void Update()
    {
        BlockRewind.ToUpdate();
    }

    public void FixedUpdate()
    {
        BlockRewind.ToFixedUpdate();
    }
}