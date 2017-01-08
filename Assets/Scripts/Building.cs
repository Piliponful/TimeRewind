using UnityEngine;

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
                        continue;
                    else
                        this.buildingScheme[x, y, z] = new BlockScheme(false, false, x, y, z, null, new Vector3Int(x, y, z));
                }
            }
        }
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
