using UnityEngine;
using System.Collections.Generic;

public class Building
{
    public int width, height, length;
    public int widthOfSingleBlock, heightOfSingleBlock, lengthOfSingleBlock;
    public Vector3Int blockSize;
    public BlockScheme[,,] buildingScheme;
    public Vector3 worldPosition;
    public Vector3Int childDimensions;

    public Building(int width, int height, int length, Vector3Int blockSize, Vector3 worldPosition, Vector3Int childDimensions)
    {
        this.width = width; this.height = height; this.length = length;
        this.buildingScheme = new BlockScheme[width, height, length];
        this.widthOfSingleBlock = blockSize.x;
        this.heightOfSingleBlock = blockSize.y;
        this.lengthOfSingleBlock = blockSize.z;
        this.blockSize = blockSize;
        this.worldPosition = worldPosition;
        this.childDimensions = childDimensions;
    }

    public void computeBuilding()
    {
        makeBuildingScheme();
        computeNeighbors();
    }

    public GameObject makeBuilding()
    {
        return BlockMeshDataInterpreter.buildingGameObject(
        BlockMeshDataInterpreter.buildingBlocks(this),
        worldPosition,
        true, false, this);
    }

    public GameObject reBuild()
    {
        computeNeighbors();
        return makeBuilding();
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
                        Vector3Int position = new Vector3Int(x, y, z).multiply(this.blockSize);
                        if(Random.value > .5)
                        {
                            this.buildingScheme[x, y, z] = new BlockScheme(null, null, position, blockSize);
                        }
                        else
                        {
                            this.buildingScheme[x, y, z] = makeScheme(position);
                        }
                    }
                }
            }
        }
    }

    public BlockScheme makeScheme(Vector3Int position)
    {
        if(childDimensions == null)
        {
            this.childDimensions = childrenDimensions();
        }
        Vector3Int size = blockSize.div(this.childDimensions);
        BlockScheme[,,] children = new BlockScheme[childDimensions.x, childDimensions.y, childDimensions.z];
        BlockScheme parent = new BlockScheme(children, null, position, blockSize);
        for (int x = 0; x < children.GetLength(0); x++)
        {
            for (int y = 0; y < children.GetLength(1); y++)
            {
                for (int z = 0; z < children.GetLength(2); z++)
                {
                    children[x, y, z] = new BlockScheme(null, parent, position.add(new Vector3Int(x, y, z).multiply(size)), size);
                }
            }
        }
        return parent;
    }


    private Vector3Int childrenDimensions()
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
            if (Random.value > .5)
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


    public void computeNeighbors()
    {
        List<BlockScheme> parents = new List<BlockScheme>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int z = 0; z < length; z++)
                {
                    if (this.buildingScheme[x, y, z] != null && this.buildingScheme[x, y, z].visible)
                    {
                        if (this.buildingScheme[x, y, z].children != null)
                            parents.Add(this.buildingScheme[x, y, z]);
                        NeighborsDetection nd = new NeighborsDetection(this.buildingScheme);
                        this.buildingScheme[x, y, z].neigbors = nd.getNeigbors(x, y, z);
                    }
                }
            }
        }

        foreach (BlockScheme parent in parents)
            computeChildNeighbors(parent);
    }

    public void computeChildNeighbors(BlockScheme parent)
    {
        for (int x = 0; x < parent.children.GetLength(0); x++)
        {
            for (int y = 0; y < parent.children.GetLength(1); y++)
            {
                for (int z = 0; z < parent.children.GetLength(2); z++)
                {
                    if (parent.children[x, y, z] != null && parent.children[x, y, z].visible)
                    {
                        NeighborsDetection nd = new NeighborsDetection(parent.children);
                        parent.children[x, y, z].neigbors = nd.getChildNeighbors(x, y, z, parent);
                    }
                }
            }
        }
    }

    public BlockScheme setBlockInvisible(Vector3 point)
    {
        Vector3Int poInt = Vector3Int.toV3Int(point).floorToMultipleOfSize(Vector3Int.toV3Int(worldPosition));

        for (int x = 0; x < this.width; x++)
        {
            for (int y = 0; y < this.height; y++)
            {
                for (int z = 0; z < this.length; z++)
                {
                    if (this.buildingScheme[x, y, z] == null)
                        continue;

                    if (poInt.compare(buildingScheme[x, y, z].position.add(this.worldPosition)))
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
        Vector3Int position = blockScheme.position.add(this.worldPosition);
        for (int x = position.x; x < position.x + this.blockSize.x; x++)
        {
            for (int y = position.y; y < position.y + this.blockSize.y; y++)
            {
                for (int z = position.z; z < position.z + this.blockSize.z; z++)
                {
                    BlockMeshDataInterpreter.buildingGameObject(
                    BlockMeshDataInterpreter.oneSimpleCube(Vector3Int.one),
                    new Vector3(x, y, z),
                    false, true, null);
                }
            }
        }
    }
}
