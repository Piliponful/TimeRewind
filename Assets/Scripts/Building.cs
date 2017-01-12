﻿using UnityEngine;

public class Building
{
    public int width, height, length;
    public int widthOfSingleBlock, heightOfSingleBlock, lengthOfSingleBlock;
    public Vector3Int blockSize;
    public BlockScheme[,,] buildingScheme;
    private Vector3 worldPosition;

    public Building(int width, int height, int length, Vector3Int blockSize, Vector3 worldPosition)
    {
        this.width = width; this.height = height; this.length = length;
        this.buildingScheme = new BlockScheme[width, height, length];
        this.widthOfSingleBlock = blockSize.x;
        this.heightOfSingleBlock = blockSize.y;
        this.lengthOfSingleBlock = blockSize.z;
        this.blockSize = blockSize;
        this.worldPosition = worldPosition;
    }

    public void computeBuilding()
    {
        makeBuildingScheme();
        computeNeighbors();
    }

    public void makeBuilding()
    {
        BlockMeshDataInterpreter.buildingGameObject(
        BlockMeshDataInterpreter.buildingBlocks(this),
        worldPosition,
        true, false, this);
    }

    public void makeBuildingScheme()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (Random.value > .5)
                    {
                        this.buildingScheme[x, y, z] = null;
                    }
                    else
                    {
                        Vector3Int position = new Vector3Int(x, y, z).multiply(this.blockSize).add(this.worldPosition);
                        if(Random.value > .5)
                        {
                            this.buildingScheme[x, y, z] = new BlockScheme(null, position, blockSize);
                        }
                        else
                        {
                            this.buildingScheme[x, y, z] = new BlockScheme(makeChildrenScheme(position), position, blockSize);
                        }
                    }
                }
            }
        }
    }

    public BlockScheme[,,] makeChildrenScheme(Vector3Int parentPosition)
    {
        BlockScheme[,,] children = new BlockScheme[1, 1, 1];
        for (int x = 0; x < children.GetLength(0); x++)
        {
            for (int y = 0; y < children.GetLength(1); y++)
            {
                for (int z = 0; z < children.GetLength(2); z++)
                {
                    children[x, y, z] = new BlockScheme(null, parentPosition.add(new Vector3Int(x, y, z)), null);
                }
            }
        }
        return children;
    }

    public void computeNeighbors()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (this.buildingScheme[x, y, z] != null && this.buildingScheme[x, y, z].visible)
                    {
                        NeighborsDetection nd = new NeighborsDetection(this.buildingScheme);
                        this.buildingScheme[x, y, z].neigbors = nd.getNeigbors(x, y, z);
                    }
                }
            }
        }
    }

    public BlockScheme setBlockInvisible(Vector3 point)
    {
        Vector3Int poInt = Vector3Int.toV3Int(point).floorToMultipleOfSize();

        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                for (int z = 0; z < this.length; z++)
                {
                    if (this.buildingScheme[x, y, z] == null)
                        continue;

                    if (poInt.compare(buildingScheme[x, y, z].position))
                    {
                        buildingScheme[x, y, z].visible = false;
                        return buildingScheme[x, y, z];
                    }
                }
            }
        }
        return null;
    }

    public void splitBlock(BlockScheme blockScheme, Vector3Int cubeSize = null)
    {
        for (int x = blockScheme.position.x; x < blockScheme.position.x + this.blockSize.x; x++)
        {
            for (int y = blockScheme.position.y; y < blockScheme.position.y + this.blockSize.y; y++)
            {
                for (int z = blockScheme.position.z; z < blockScheme.position.z + this.blockSize.z; z++)
                {
                    BlockMeshDataInterpreter.buildingGameObject(
                    BlockMeshDataInterpreter.oneSimpleCube(new Vector3Int(1, 1, 1)),
                    new Vector3(x, y, z),
                    true, true, null);
                }
            }
        }
    }
}
