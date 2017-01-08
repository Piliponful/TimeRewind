public class BlockScheme
{
    public bool split;
    public bool composite;
    public int prevSplitX, prevSplitY, prevSplitZ;
    public BlockScheme[,,] innerBlocks;
    public Vector3Int position;
    public Neighbors neigbors;
    public bool visible;
    public BlockScheme(bool split, bool composite, int x, int y, int z, BlockScheme[,,] innerBlocks, Vector3Int position)
    {
        this.split = split;
        this.composite = composite;
        this.innerBlocks = innerBlocks;
        this.prevSplitX = x;
        this.prevSplitY = y;
        this.prevSplitZ = z;
        this.position = position;
        this.visible = true;
    }
}