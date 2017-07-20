public class NeighborsDetection
{
    BlockScheme[,,] scheme;
    public NeighborsDetection(BlockScheme[,,] buildingScheme)
    {
        this.scheme = buildingScheme;
    }

    public Neighbors getNeigbors(int x, int y, int z)
    {
        Neighbors neighbors = new Neighbors(
            checkBlock(x, y, z + 1),
            checkBlock(x, y, z - 1),
            checkBlock(x, y + 1, z),
            checkBlock(x, y - 1, z),
            checkBlock(x + 1, y, z),
            checkBlock(x - 1, y, z));

        return neighbors;
    }

    public Neighbors getChildNeighbors(int x, int y, int z, BlockScheme parent)
    {
        Neighbors neighbors = new Neighbors(
    checkChildBlock(new Vector3Int(x, y, z + 1), parent),
    checkChildBlock(new Vector3Int(x, y, z - 1), parent),
    checkChildBlock(new Vector3Int(x, y + 1, z), parent),
    checkChildBlock(new Vector3Int(x, y - 1, z), parent),
    checkChildBlock(new Vector3Int(x + 1, y, z), parent),
    checkChildBlock(new Vector3Int(x - 1, y, z), parent));

        return neighbors;
    }

    private bool checkBlock(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= scheme.GetLength(0) || y >= scheme.GetLength(1) || z >= scheme.GetLength(2))
        {
            return false;
        }
        if (scheme[x, y, z] == null || !scheme[x, y, z].visible)
            return false;

        return true;
    }
    
    private bool checkChildBlock(Vector3Int p, BlockScheme parent)
    {
        if (p.x < 0)
            return parent.neigbors.leftNeighbor;
        if (p.y < 0)
            return parent.neigbors.bottomNeighbor;
        if (p.z < 0)
            return parent.neigbors.backNeighbor;
        if (p.x >= scheme.GetLength(0))
            return parent.neigbors.rightNeighbor;
        if (p.y >= scheme.GetLength(1))
            return parent.neigbors.topNeighbor;
        if (p.z >= scheme.GetLength(2))
            return parent.neigbors.frontNeighbor;

        if (this.scheme[p.x, p.y, p.z] == null)
            return false;

        return true;
    }
}
