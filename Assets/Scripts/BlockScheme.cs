public class BlockScheme
{
    public BlockScheme[,,] children;
    public Vector3Int position;
    public Neighbors neigbors;
    public bool visible;
    public BlockScheme(BlockScheme[,,] children, Vector3Int position)
    {
        this.children = children;
        this.position = position;
        this.visible = true;
    }
}