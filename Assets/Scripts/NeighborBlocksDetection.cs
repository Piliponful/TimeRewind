public class NeighborsDetection
{
    BlockScheme[,,] buildingScheme;
    public NeighborsDetection(BlockScheme[,,] buildingScheme)
    {
        this.buildingScheme = buildingScheme;
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

    private bool checkBlock(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= buildingScheme.GetLength(0) || y >= buildingScheme.GetLength(1) || z >= buildingScheme.GetLength(2))
            return false;
        if (buildingScheme[x, y, z] == null || !buildingScheme[x, y, z].visible)
            return false;

        return true;
    }
}
