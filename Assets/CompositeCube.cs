using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeCube {
    public Neighbor[] neigbors;
    public bool topFace, bottomFace, frontFace, backFace, leftFace, rightFace;
    public int x, y, z;

    public CompositeCube(Neighbor[] neighbors, int x, int y, int z)
    {
        this.neigbors = neigbors;
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
