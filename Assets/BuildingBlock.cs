using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock {
    public bool split;
    public BuildingBlock[,,] atomMatrix;
    public Neighbor[] neighbors;

    public BuildingBlock(Neighbor[] neighbors, bool split, BuildingBlock[,,] atomMatrix = null)
    {
        this.split = split;
        this.atomMatrix = atomMatrix;
        this.neighbors = neighbors;
    }
}
