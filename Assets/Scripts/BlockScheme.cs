public class BlockScheme
{
    public BlockScheme[,,] children;
    public Vector3Int position;
    public Vector3Int size;
    public Neighbors neigbors;
    public bool visible;
    public BlockScheme(BlockScheme[,,] children, Vector3Int position, Vector3Int size)
    {
        this.children = children;
        this.position = position;
        this.size = size;
        this.visible = true;
    }
}