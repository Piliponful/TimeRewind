public class NeighborsDetection
{
    BlockScheme[,,] buildingScheme;
    public NeighborsDetection(BlockScheme[,,] buildingScheme)
    {
        this.buildingScheme = buildingScheme;
    }

    public Neighbors getNeigbors(int x, int y, int z)
    {
        Vector3Int f = new Vector3Int(x, y, z + 1),
                   b = new Vector3Int(x, y, z - 1),
                   u = new Vector3Int(x, y + 1, z),
                   d = new Vector3Int(x, y - 1, z),
                   r = new Vector3Int(x + 1, y, z),
                   l = new Vector3Int(x - 1, y, z);

        Vector3Int fD = new Vector3Int(0, 0, 0 + 1),
                   bD = new Vector3Int(0, 0, 0 - 1),
                   uD = new Vector3Int(0, 0 + 1, 0),
                   dD = new Vector3Int(0, 0 - 1, 0),
                   rD = new Vector3Int(0 + 1, 0, 0),
                   lD = new Vector3Int(0 - 1, 0, 0);

        Neighbors neighbors = new Neighbors(false, false, false,
                                            false, false, false);
  //      Neighbors neighbors = new Neighbors(checkBlockBool(f, fD), checkBlockBool(b, bD), checkBlockBool(u, uD),
//                                    checkBlockBool(d, dD), checkBlockBool(r, rD), checkBlockBool(l, lD));

        return neighbors;
    }

    private byte checkBlock(int x, int y, int z, BlockScheme[,,] blockSchemes = null)
    {
        if (blockSchemes == null)
            blockSchemes = this.buildingScheme;

        if (x < 0 || y < 0 || z < 0 || x >= blockSchemes.GetLength(0) || y >= blockSchemes.GetLength(1) || z >= blockSchemes.GetLength(2))
            return 1;
        if (buildingScheme[x, y, z] == null || !buildingScheme[x, y, z].visible)
            return 2;

        return 3;
    }

    private bool checkBlockBool(Vector3Int d, Vector3Int nD)
    {
        Vector3Int o = new Vector3Int(d.x - nD.x, d.y - nD.y, d.z - nD.z);
        byte face = checkBlock(d.x, d.y, d.z);
        if (face == 1)
        {
            if (this.buildingScheme[o.x, o.y, o.z].parentBlockSchemes == null)
            {
                return false;
            }
            else
            {
                Vector3Int parent = new Vector3Int(this.buildingScheme[o.x, o.y, o.z].parent.position.x,
                                                   this.buildingScheme[o.x, o.y, o.z].parent.position.y,
                                                   this.buildingScheme[o.x, o.y, o.z].parent.position.z);
                byte faceP = checkBlock(parent.x + nD.x, parent.y + nD.y, parent.z + nD.z, this.buildingScheme[o.x, o.y, o.z].parentBlockSchemes);
                if (faceP == 2 && faceP == 1)
                {
                    return false;
                }
                if (faceP == 3)
                {
                    return true;
                }
            }
        }
        if (face == 2)
        {
            return false;
        }
        if (face == 3)
        {
            return true;
        }

        return false;
    }
}
