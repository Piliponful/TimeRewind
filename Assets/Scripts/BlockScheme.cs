public class BlockScheme
{
    public BlockScheme parent;
    public BlockScheme[,,] children;
    public BlockScheme[,,] parentBlockSchemes;
    public Vector3Int position;
    public Neighbors neighbors;
    public bool visible;
    public Vector3Int blockSize;
    public BlockScheme(BlockScheme[,,] children, Vector3Int position, Vector3Int blockSize,
        BlockScheme[,,] parentBlockSchemes = null, BlockScheme parent = null)
    {
        this.children = children;
        this.position = position;
        this.visible = true;
        this.parent = parent;
        this.blockSize = blockSize;
        this.parentBlockSchemes = parentBlockSchemes;
    }
}