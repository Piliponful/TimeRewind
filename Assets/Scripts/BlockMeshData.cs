using UnityEngine;
using System.Collections.Generic;

public class BlockMeshData
{
    public Vector3Int cubeSize;
    private BlockScheme cubeScheme;
    // cube relative position decoupled for ease of use
    private int x, y, z;
    // x, y, z size of one single cube decoupled for ease of use
    private int sizeX, sizeY, sizeZ;
    // Declare Points of cube one at a time
    private Vector3 LeftBottomP, LeftUpperP, RightBottomP, RightUpperP, BackleftBottomP, BackLeftUpperP, BackRightBottomP, BackRightUpperP;

    // ========ACTUAL SINGLE CUBE MESH DATA DECLORATION==========
    public List<Vector3> verts = new List<Vector3>();
    public List<int> tris = new List<int>();
    public List<Vector2> uvs = new List<Vector2>();
    // ======================END==================

    private int triIndexStart;
    public int triIndexEnd, uvsIterations;

    public BlockMeshData(BlockScheme blockScheme, Vector3Int blockSize, int startIndex)
    {
        this.cubeScheme = blockScheme;
        this.sizeX = blockSize.x;
        this.sizeY = blockSize.y;
        this.sizeZ = blockSize.z;
        if(blockScheme != null)
        {
            this.x = blockScheme.position.x;
            this.y = blockScheme.position.y;
            this.z = blockScheme.position.z;
        }
        this.triIndexStart = startIndex;
        this.triIndexEnd = startIndex;

        // Front quad
        LeftBottomP = new Vector3(x + 0, y + 0, z + sizeZ);
        LeftUpperP = new Vector3(x + 0, y + sizeY, z + sizeZ);
        RightBottomP = new Vector3(x + sizeX, y + 0, z + sizeZ);
        RightUpperP = new Vector3(x + sizeX, y + sizeY, z + sizeZ);

        // Back quad
        BackleftBottomP = new Vector3(x + 0, y + 0, z);
        BackLeftUpperP = new Vector3(x + 0, y + sizeY, z);
        BackRightBottomP = new Vector3(x + sizeX, y + 0, z);
        BackRightUpperP = new Vector3(x + sizeX, y + sizeY, z);
    }

    public void generateCubeMeshData()
    {
        generateVerts4Cube();
        generateTris4Cube();
        generateUvs4Cube();
    }

    public void makeSingleCubeMeshData()
    {
        Verts4SingleCube();
        generateTris4Cube();
        generateUvs4Cube();
    }

    void generateVerts4Cube()
    {
        // Add verts(of predefinded points) to list so that thay form a cube
        if (!cubeScheme.neighbors.frontNeighbor)
            addFrontFaceVerts();
        if (!cubeScheme.neighbors.backNeighbor)
            addBackFaceVerts();
        if (!cubeScheme.neighbors.rightNeighbor)
            addRightFaceVerts();
        if (!cubeScheme.neighbors.leftNeighbor)
            addLeftFaceVerts();
        if (!cubeScheme.neighbors.topNeighbor)
            addUpFaceVerts();
        if (!cubeScheme.neighbors.bottomNeighbor)
            addBottomFaceVerts();
    }

    void Verts4SingleCube()
    {
            addFrontFaceVerts();
            addBackFaceVerts();
            addRightFaceVerts();
            addLeftFaceVerts();
            addUpFaceVerts();
            addBottomFaceVerts();
    }

    List<Vector3> generateVerts4Quad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        List<Vector3> verts = new List<Vector3>();

        verts.Add(p1);
        verts.Add(p2);
        verts.Add(p3);
        verts.Add(p4);

        return verts;
    }

    void generateTris4Cube()
    {
        // 6 times for each of cube face
        for (int i = this.triIndexStart; i < this.triIndexEnd; i++)
        {
            tris.AddRange(generateTris4Quad(i));
        }
    }

    List<int> generateTris4Quad(int iteration)
    {
        // Declare list of tris
        List<int> tris = new List<int>();

        // Adding tris to list

        // on each iteration we use 4 verts to make 2 triangles that make up quad
        // so with each iteration verts index should increase in 4
        tris.Add(iteration * 4 + 0);
        tris.Add(iteration * 4 + 1);
        tris.Add(iteration * 4 + 2);

        tris.Add(iteration * 4 + 2);
        tris.Add(iteration * 4 + 3);
        tris.Add(iteration * 4 + 0);

        return tris;
    }

    void generateUvs4Cube()
    {
        this.uvsIterations -= this.triIndexStart;
        // one iteration for each side of the cube
        for (int i = 0; i < this.uvsIterations; i++)
        {
            // generating uv for one quad and adding it to the main list
            generateUvs4Quad();
        }
    }

    List<Vector2> generateUvs4Quad()
    {
        // Declare list of uvs
        List<Vector2> uvs = new List<Vector2>();

        // Adding uvs to list
        uvs.Add(new Vector2(0, 0));
        uvs.Add(new Vector2(0, 1));
        uvs.Add(new Vector2(1, 1));
        uvs.Add(new Vector2(1, 0));

        return uvs;
    }

    // Thin envelopes for making verts for each side of the cube
    void addFrontFaceVerts()
    {
        // North side of a cube
        verts.AddRange(
        generateVerts4Quad(RightBottomP, RightUpperP, LeftUpperP, LeftBottomP));
        this.triIndexEnd++;
    }

    void addBackFaceVerts()
    {
        // South side of a cube
        verts.AddRange(
        generateVerts4Quad(BackleftBottomP, BackLeftUpperP, BackRightUpperP, BackRightBottomP));
        this.triIndexEnd++;
    }

    void addRightFaceVerts()
    {
        // East side of a cube
        verts.AddRange(
        generateVerts4Quad(BackRightBottomP, BackRightUpperP, RightUpperP, RightBottomP));
        this.triIndexEnd++;
    }

    void addLeftFaceVerts()
    {
        // West side of a cube
        verts.AddRange(
        generateVerts4Quad(LeftBottomP, LeftUpperP, BackLeftUpperP, BackleftBottomP));
        this.triIndexEnd++;
    }

    void addUpFaceVerts()
    {
        verts.AddRange(
        generateVerts4Quad(BackRightUpperP, BackLeftUpperP, LeftUpperP, RightUpperP));
        this.triIndexEnd++;
    }

    void addBottomFaceVerts()
    {
        verts.AddRange(
        generateVerts4Quad(BackleftBottomP, BackRightBottomP, RightBottomP, LeftBottomP));
        this.triIndexEnd++;
    }
}