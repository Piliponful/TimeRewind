using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building
{
    public BuildingBlock[,,] building;
    public int width, height, length;
    public int widthOfSingleBlock, heightOfSingleBlock, lengthOfSingleBlock;
    public BlockScheme[,,] buildingScheme;

    public Building(int width, int height, int length, Vector3 blockSize)
    {
        this.width = width; this.height = height; this.length = length;
        this.building = new BuildingBlock[width, height, length];
        this.buildingScheme = new BlockScheme[width, height, length];
        this.widthOfSingleBlock = (int)blockSize.x;
        this.heightOfSingleBlock = (int)blockSize.y;
        this.lengthOfSingleBlock = (int)blockSize.z;
        generateBuilding();
    }

    public void generateBuilding()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (Random.value > .5)
                        continue;
                    else
                        this.buildingScheme[x, y, z] = new BlockScheme(false, false, x, y, z, null, new Vector3Int(x, y, z));
                }
            }
        }
        setNeighborsForEach();
    }

    public void setNeighborsForEach()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (this.buildingScheme[x, y, z] != null)
                    {
                        NeighborsDetection nd = new NeighborsDetection(this.buildingScheme);
                        this.buildingScheme[x, y, z].neigbors = nd.getNeigbours(x, y, z);
                    }
                }
            }
        }
    }

    private Neighbor[] getNeighbors(BlockScheme[,,] buildingScheme, int x, int y, int z)
    {
        Neighbor[] neighbors = new Neighbor[6];

        int prevX = buildingScheme[x, y, z].prevSplitX;
        int prevY = buildingScheme[x, y, z].prevSplitY;
        int prevZ = buildingScheme[x, y, z].prevSplitZ;

        Vector3[] vs = new Vector3[] { Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down };

        int i = 0;
        foreach (Vector3 v in vs)
        {
            int newX = x + (int)v.x;
            int newY = y + (int)v.y;
            int newZ = z + (int)v.z;
            if (newX < 0 || newY < 0 || newZ < 0
            || newX > buildingScheme.GetLength(0) || newY > buildingScheme.GetLength(1) || newZ > buildingScheme.GetLength(2))
            {
                if (prevX + v.x < 0 || prevY + v.y < 0 || prevZ + v.z < 0
                    || prevX + v.x > buildingScheme[x, y, z].innerBlocks.GetLength(0) || prevY + v.y > buildingScheme.GetLength(1) || prevZ + v.z > buildingScheme.GetLength(2))
                {
                    if (buildingScheme[x, y, z].innerBlocks == null)
                        neighbors[i] = null;
                    else
                        neighbors[i] = new Neighbor(prevX + (int)v.x, prevY + (int)v.y, prevZ + (int)v.z);
                }
            }
            if (buildingScheme[newX, newY, newY] == null)
                neighbors[i] = null;
            else
                neighbors[i] = new Neighbor(newX, newY, newZ);
            i++;
        }

        return neighbors;
    }


    public void generate()
    {
        generateBuildingScheme(this.buildingScheme, this.width, this.height, this.length);
    }
    int prevX, prevY, prevZ;
    BlockScheme[,,] innerBlocks;
    public BlockScheme[,,] generateBuildingScheme(BlockScheme[,,] buildingScheme, int width, int height, int length, bool composite = false)
    {
        for (int x = 0, i = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++, i++)
                {
                    bool split = (i % 2 == 0) && !composite;
                    if (split)
                    {
                        prevX = x;
                        prevY = y;
                        prevZ = z;

                        innerBlocks = new BlockScheme[this.widthOfSingleBlock, this.heightOfSingleBlock, this.lengthOfSingleBlock];
                        generateBuildingScheme(innerBlocks, this.widthOfSingleBlock, this.heightOfSingleBlock, this.lengthOfSingleBlock, true);
                    }
                    if (!composite)
                    {
                        prevX = 0;
                        prevY = 0;
                        prevZ = 0;
                    }
                    else
                        innerBlocks = null;

                    buildingScheme[x, y, z] = new BlockScheme(split, composite, prevX, prevY, prevZ, innerBlocks, null);
                }
            }
        }
        return buildingScheme;
    }
}

public class BlockScheme
{
    public bool split;
    public bool composite;
    public int prevSplitX, prevSplitY, prevSplitZ;
    public BlockScheme[,,] innerBlocks;
    public Vector3Int position;
    public Neighbors neigbors;
    public BlockScheme(bool split, bool composite, int x, int y, int z, BlockScheme[,,] innerBlocks, Vector3Int position)
    {
        this.split = split;
        this.composite = composite;
        this.innerBlocks = innerBlocks;
        this.prevSplitX = x;
        this.prevSplitY = y;
        this.prevSplitZ = z;
        this.position = position;
    }
}

public class Vector3Int
{
    public int x, y, z;
    public Vector3Int(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public class NeighborsDetection
{
    BlockScheme[,,] buildingScheme;
    public NeighborsDetection(BlockScheme[,,] buildingScheme)
    {
        this.buildingScheme = buildingScheme;
    }

    public Neighbors getNeigbours(int x, int y, int z)
    {
        Neighbors neighbors = new Neighbors(
            checkNeighbor(x, y, z + 1),
            checkNeighbor(x, y, z - 1),
            checkNeighbor(x, y + 1, z),
            checkNeighbor(x, y - 1, z),
            checkNeighbor(x + 1, y, z),
            checkNeighbor(x - 1, y, z));

        return neighbors;
    }

    private bool checkNeighbor(int x, int y, int z)
    {
       
        if (x < 0 || y < 0 || z < 0 || x >= buildingScheme.GetLength(0) || y >= buildingScheme.GetLength(1) || z >= buildingScheme.GetLength(2))
            return false;
        if (buildingScheme[x, y, z] == null)
            return false;

        return true;
    }
}
