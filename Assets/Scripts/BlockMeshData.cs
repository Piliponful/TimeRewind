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
    public List<Vector3> normals = new List<Vector3>();
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
        LeftBottomP = new Vector3(x, y, z  + sizeZ);
        LeftUpperP = new Vector3(x, y + sizeY, z + sizeZ);
        RightBottomP = new Vector3(x + sizeX, y, z + sizeZ);
        RightUpperP = new Vector3(x + sizeX, y + sizeY, z + sizeZ);

        // Back quad
        BackleftBottomP = new Vector3(x, y, z);
        BackLeftUpperP = new Vector3(x, y + sizeY, z);
        BackRightBottomP = new Vector3(x + sizeX, y, z);
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
        if(cubeScheme.neigbors == null)
        {
            Verts4SingleCube();
            normals4SimpleCube();
        }
        else
        {
            // Add verts(of predefinded points) to list so that thay form a cube
            if (!cubeScheme.neigbors.frontNeighbor)
                addFrontFaceVerts();
            if (!cubeScheme.neigbors.backNeighbor)
                addBackFaceVerts();
            if (!cubeScheme.neigbors.rightNeighbor)
                addRightFaceVerts();
            if (!cubeScheme.neigbors.leftNeighbor)
                addLeftFaceVerts();
            if (!cubeScheme.neigbors.topNeighbor)
                addUpFaceVerts();
            if (!cubeScheme.neigbors.bottomNeighbor)
                addBottomFaceVerts();
        }
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

    void generateVerts4Quad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        verts.Add(p1);
        verts.Add(p2);
        verts.Add(p3);
        verts.Add(p4);
    }

    void normals4SimpleCube()
    {
        // North side of a cube
        generateNormals4Quad(RightBottomP, RightUpperP, LeftUpperP, LeftBottomP);
        // South side of a cube
        generateNormals4Quad(BackleftBottomP, BackLeftUpperP, BackRightUpperP, BackRightBottomP);
        // East side of a cube
        generateNormals4Quad(BackRightBottomP, BackRightUpperP, RightUpperP, RightBottomP);
        // West side of a cube
        generateNormals4Quad(LeftBottomP, LeftUpperP, BackLeftUpperP, BackleftBottomP);

        generateNormals4Quad(BackRightUpperP, BackLeftUpperP, LeftUpperP, RightUpperP);
        generateNormals4Quad(BackleftBottomP, BackRightBottomP, RightBottomP, LeftBottomP);
    }

    void generateNormals4Quad(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4)
    {
        normals.Add(p1);
        normals.Add(p2);
        normals.Add(p3);
        normals.Add(p4);
    }

    void generateTris4Cube()
    {
        // 6 times for each of cube face
        for (int i = this.triIndexStart; i < this.triIndexEnd; i++)
        {
            generateTris4Quad(i);
        }
    }

    void generateTris4Quad(int iteration)
    {
        // Adding tris to list

        // on each iteration we use 4 verts to make 2 triangles that make up quad
        // so with each iteration verts index should increase in 4
        tris.Add(iteration * 4 + 0);
        tris.Add(iteration * 4 + 1);
        tris.Add(iteration * 4 + 2);

        tris.Add(iteration * 4 + 2);
        tris.Add(iteration * 4 + 3);
        tris.Add(iteration * 4 + 0);
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
        generateVerts4Quad(RightBottomP, RightUpperP, LeftUpperP, LeftBottomP);
        generateNormals4Quad(RightBottomP, RightUpperP, LeftUpperP, LeftBottomP);
        this.triIndexEnd++;
    }

    void addBackFaceVerts()
    {
        // South side of a cube
        generateVerts4Quad(BackleftBottomP, BackLeftUpperP, BackRightUpperP, BackRightBottomP);
        generateNormals4Quad(BackleftBottomP, BackLeftUpperP, BackRightUpperP, BackRightBottomP);
        this.triIndexEnd++;
    }

    void addRightFaceVerts()
    {
        // East side of a cube
        generateVerts4Quad(BackRightBottomP, BackRightUpperP, RightUpperP, RightBottomP);
        generateNormals4Quad(BackRightBottomP, BackRightUpperP, RightUpperP, RightBottomP);
        this.triIndexEnd++;
    }

    void addLeftFaceVerts()
    {
        // West side of a cube
        generateVerts4Quad(LeftBottomP, LeftUpperP, BackLeftUpperP, BackleftBottomP);
        generateNormals4Quad(LeftBottomP, LeftUpperP, BackLeftUpperP, BackleftBottomP);
        this.triIndexEnd++;
    }

    void addUpFaceVerts()
    {
        generateVerts4Quad(BackRightUpperP, BackLeftUpperP, LeftUpperP, RightUpperP);
        generateNormals4Quad(BackRightUpperP, BackLeftUpperP, LeftUpperP, RightUpperP);
        this.triIndexEnd++;
    }

    void addBottomFaceVerts()
    {
        generateVerts4Quad(BackleftBottomP, BackRightBottomP, RightBottomP, LeftBottomP);
        generateNormals4Quad(BackleftBottomP, BackRightBottomP, RightBottomP, LeftBottomP);
        this.triIndexEnd++;
    }
}