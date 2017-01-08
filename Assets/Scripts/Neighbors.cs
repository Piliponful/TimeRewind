public class Neighbors {
    public bool topNeighbor, leftNeighbor, rightNeighbor, bottomNeighbor, frontNeighbor, backNeighbor;

    public Neighbors(
    bool frontNeighbor,
    bool backNeighbor,
    bool topNeighbor,
    bool bottomNeighbor,
    bool rightNeighbor,
    bool leftNeighbor)
    {
        this.topNeighbor = topNeighbor;
        this.bottomNeighbor = bottomNeighbor;
        this.leftNeighbor = leftNeighbor;
        this.rightNeighbor = rightNeighbor;
        this.frontNeighbor = frontNeighbor;
        this.backNeighbor = backNeighbor;
    }
}
