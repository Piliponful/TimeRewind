using UnityEngine;
using System.Collections.Generic;

public class Building
{
    public int width, height, length;
    public int widthOfSingleBlock, heightOfSingleBlock, lengthOfSingleBlock;
    public Vector3Int blockSize;
    public BlockScheme[,,] buildingScheme;
    private Vector3 worldPosition;
    private Vector3Int chilBlockDimensions;
    public Vector3Int childBlockSize;
    public Vector3Int ChildBlockDimensions
    {
        get
        {
            return chilBlockDimensions;
        }
        set
        {
            if(value != null)
            {
                if (this.blockSize.mod(value))
                {
                    this.chilBlockDimensions = value;
                }
                else
                {
                    this.chilBlockDimensions = computeChildDimensions();
                }
            }
            else
            {
                this.chilBlockDimensions = computeChildDimensions();
            }
        }
    }

    public Building(int width, int height, int length, Vector3Int blockSize, Vector3 worldPosition, Vector3Int childBlockDimensions)
    {
        this.width = width; this.height = height; this.length = length;
        this.buildingScheme = new BlockScheme[width, height, length];
        this.widthOfSingleBlock = blockSize.x;
        this.heightOfSingleBlock = blockSize.y;
        this.lengthOfSingleBlock = blockSize.z;
        this.blockSize = blockSize;
        this.worldPosition = worldPosition;
        this.ChildBlockDimensions = childBlockDimensions;
        this.childBlockSize = this.blockSize.div(this.ChildBlockDimensions);
    }

    public void computeBuilding()
    {
        makeBuildingScheme();
        computeNeighbors(this.buildingScheme);
    }

    public void reBuild()
    {
        computeNeighbors(this.buildingScheme);
        makeBuilding();
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
        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                for (int z = 0; z < this.length; z++)
                {
                    if (Random.value > .5)
                        continue;
                    if(Random.value > .5)
                    {
                        BlockScheme[,,] children = new BlockScheme[this.ChildBlockDimensions.x, this.ChildBlockDimensions.y, this.ChildBlockDimensions.z];
                        this.buildingScheme[x, y, z] = new BlockScheme(children, new Vector3Int(x * this.blockSize.x, y * this.blockSize.y, z * this.blockSize.z), this.blockSize);
                        makeChildrenScheme(children, this.buildingScheme[x, y, z], this.buildingScheme, this.childBlockSize);
                    }
                    else
                    {
                        this.buildingScheme[x, y, z] = null;
                    }
                }
            }
        }
    }

    private void makeChildrenScheme(BlockScheme[,,] blockSchemes, BlockScheme parent, BlockScheme[,,] parentScheme, Vector3Int blockSize)
    {
        for (int x = 0; x < blockSchemes.GetLength(0); x++)
        {
            for (int y = 0; y < blockSchemes.GetLength(1); y++)
            {
                for (int z = 0; z < blockSchemes.GetLength(2); z++)
                {
                    Vector3Int position = new Vector3Int(x, y, z).add(parent.position).multiply(blockSize);
                    blockSchemes[x, y, z] = new BlockScheme(null, position, blockSize, parentScheme, parent);
                }
            }
        }
    }

    private Vector3Int computeChildDimensions()
    {
        int[] possibleDivisorsX, possibleDivisorsY, possibleDivisorsZ;

        possibleDivisorsX = findDivisors(this.blockSize.x).ToArray();
        possibleDivisorsY = findDivisors(this.blockSize.y).ToArray();
        possibleDivisorsZ = findDivisors(this.blockSize.z).ToArray();

        return new Vector3Int(pickRandom(possibleDivisorsX), pickRandom(possibleDivisorsY), pickRandom(possibleDivisorsZ));
    }

    private int pickRandom(int[] numbers)
    {
        int i = 0;
        while (true)
        {
            if(Random.value > .5)
                return numbers[i];
            i++;
            if (i == numbers.Length)
                i = 0;
        }
    }

    private List<int> findDivisors(int number)
    {
        List<int> allPossibleDivisors = new List<int>();
        foreach (int divisor in Utils.NaturalNumbers())
        {
            if (divisor >= 100)
                return allPossibleDivisors;
            if (number % divisor == 0)
                allPossibleDivisors.Add(divisor);
        }
        return null;
    }

    public void computeNeighbors(BlockScheme[,,] blockSchemes)
    {
        for (int x = 0; x < blockSchemes.GetLength(0); x++)
        {
            for (int y = 0; y < blockSchemes.GetLength(1); y++)
            {
                for (int z = 0; z < blockSchemes.GetLength(2); z++)
                {
                    if(blockSchemes[x, y, z] != null)
                    {
                        if (blockSchemes[x, y, z].visible && blockSchemes[x, y, z].children == null)
                        {
                            NeighborsDetection nd = new NeighborsDetection(blockSchemes);
                            blockSchemes[x, y, z].neighbors = nd.getNeigbors(x, y, z);
                        }
                        if (blockSchemes[x, y, z].children != null)
                        {
                            computeNeighbors(blockSchemes[x, y, z].children);
                        }
                    }
                }
            }
        }
    }

    public BlockScheme setBlockInvisible(Vector3 point)
    {
        Vector3Int poInt = Vector3Int.toV3Int(point).floorToMultipleOfSize();
        //Debug.Log("Point: " + poInt);

        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                for (int z = 0; z < this.length; z++)
                {
                    if (this.buildingScheme[x, y, z] == null)
                        continue;

                    Vector3Int blockPosition = this.buildingScheme[x, y, z].position.add(worldPosition).multiply(blockSize);
                    //Debug.Log("BlockPosition: " + blockPosition);
                    if (poInt.compare(blockPosition))
                    {
                        this.buildingScheme[x, y, z].visible = false;
                        return buildingScheme[x, y, z];
                    }
                }
            }
        }
        return null;
    }

    public void splitBlock(BlockScheme blockScheme, Vector3Int cubeSize = null)
    {
        //BlockRewind.blocksPos.Add(pos);
        Vector3Int worldPosition = blockScheme.position.add(this.worldPosition).multiply(blockSize);
        int xWorld = worldPosition.x, yWorld = worldPosition.y, zWorld = worldPosition.y;

        for (int x = xWorld; x < xWorld + this.blockSize.x; x++)
        {
            for (int y = yWorld; y < yWorld + this.blockSize.y; y++)
            {
                for (int z = zWorld; z < zWorld + this.blockSize.z; z++)
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
