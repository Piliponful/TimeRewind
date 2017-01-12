using System.Collections.Generic;
using UnityEngine;

public static class Town {
    // Register of all buildings in city
    public static Dictionary<GameObject, Building> buildings = new Dictionary<GameObject, Building>();

    static Main mainThread;

    // Town propertis

    // Number of buildings in x,z axis
    static int cityWidth, cityLength;
    // one single building w, h, l
    static int buildingWidth, buildingHeight, buildingLength;
    // x, y, z size of one single cube
    static Vector3Int cubeSize;
    // Gap between individual cubeHolders
    static int gutter;

    public static Main MainThread
    {
        get
        {
            return mainThread;
        }

        set
        {
            mainThread = value;
            cityWidth = mainThread.cityWidth;
            cityLength = mainThread.cityLength;
            buildingWidth = mainThread.buildingWidth;
            buildingHeight = mainThread.buildingHeight;
            buildingLength = mainThread.buildingLength;
            cubeSize = mainThread.cubeSize;
            gutter = mainThread.gutter;
        }
    }

    public static void generateCity()
    {
        for (int x = 0; x < cityWidth; x++)
        {
            for (int z = 0; z < cityLength; z++)
            {
                Vector3 buildingWorldPosition = new Vector3(x * cubeSize.x * buildingWidth + (x * gutter), 0,
                z * cubeSize.z * buildingLength + (z * gutter));
                Building building = new Building(buildingWidth, buildingHeight, buildingLength, cubeSize, buildingWorldPosition, new Vector3Int(3, 3, 3));
                building.computeBuilding();
                building.makeBuilding();
            }
        }
    }
}
