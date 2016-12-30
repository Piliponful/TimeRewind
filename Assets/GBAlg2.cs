using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GBAlg2 : MonoBehaviour {

    // number of "buildings" in city
    public int cityWidth = 10, cityLength = 10;
    // one single building w, h, l
    public int buildingWidth = 5, buildingHeight = 10, buildingLength = 5;

    // x, y, z size of one single cube
    private int cubeSizeX = 1, cubeSizeY = 1, cubeSizeZ = 1;
    // GameObject(meshHolder) size
    public Vector3 cubeSize = new Vector3(6, 6, 6);

    // Gap between individual cubeHolders
    public int gutter = 2;

    // Script reference
    private BlockControl blockControl;

	void Start ()
    {
        blockControl = FindObjectOfType<BlockControl>();
        for (int x = 0; x < cityWidth; x++)
        {
            for (int z = 0; z < cityLength; z++)
            {
                // Generate mesh of all cubes combined
                // Generate gameObject that will hold the cubes rows
                // add mesh to it
                generateMeshHolder(
            generateRowsOfCubeMeshes(buildingWidth, buildingHeight, buildingLength, cubeSizeX, cubeSizeY, cubeSizeZ),
            new Vector3(x * buildingWidth + (x * gutter), 0, z * buildingLength + (z * gutter)),
            cubeSize,
            true, false, Vector3.zero);
            }
        }
	}

    public Mesh generateRowsOfCubeMeshes (int width, int height, int length, int cubeSizeX, int cubeSizeY, int cubeSizeZ)
    {
        // All data needed for mesh creation = (V, T, U)
        // Declare all vars needed to generate rows of cube meshes
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        Building building = new Building(width, height, length, cubeSize);

        int i = 0;
        // generating rows of cubes
        for (int x = 0; x < building.width; x++)
        {
            for (int y = 0; y < building.height; y++)
            {
                for (int z = 0; z < building.length; z++)
                {
                    if (building.buildingScheme[x, y, z] == null)
                        continue;
                    CubeMeshData Cube = new CubeMeshData(building.buildingScheme[x, y, z], new Vector3Int(cubeSizeX, cubeSizeZ, cubeSizeZ));

                    int times;
                    // On each iteration generationg single cube (V, T, U)
                    // and concat with previous value
                    verts.AddRange(generateVerts4Cube(cubeSizeX, cubeSizeY, cubeSizeZ, x, y, z, building.buildingScheme[x, y, z], out times));

                    for (int j = 0; j < times; j++)
                    {
                        tris.AddRange(generateTris4Quad(i));
                        i++;
                    }

                    //uv is just static coordinates for every cube of all sizes. Nothing to look at here:)
                    uvs.AddRange(generateUvs4Cube(times));
                }
            }
        }

        // returning result of the function that takes
        // (V, T, U) and returns completed mesh
        return generateMesh(verts, tris, uvs);
    }

    // Create verts for quad from input of width, height
    List<Vector3> generateVerts4Cube(float sizeX, float sizeY, float sizeZ, int x, int y, int z, BlockScheme cubeScheme, out int times)
    {
        times = 0;
        // Declare list of verts
        List<Vector3> verts = new List<Vector3>();

        // Declare Points one at a time
        Vector3 LeftBottomP, LeftUpperP, RightBottomP, RightUpperP, BackleftBottomP, BackLeftUpperP, BackRightBottomP, BackRightUpperP;
        
        // Front quad
        LeftBottomP = new Vector3(x * sizeX + 0, y * sizeY + 0, z * sizeZ + 0);
        LeftUpperP = new Vector3(x * sizeX + 0, y * sizeY + sizeY, z * sizeZ + 0);
        RightBottomP = new Vector3(x * sizeX + sizeX, y * sizeY + 0, z * sizeZ + 0);
        RightUpperP = new Vector3(x * sizeX + sizeX, y * sizeY + sizeY, z * sizeZ + 0);

        // Back quad
        BackleftBottomP = new Vector3(x * sizeX + 0, y * sizeY + 0, z * sizeZ + (-sizeZ));
        BackLeftUpperP = new Vector3(x * sizeX + 0, y * sizeY + sizeY, z * sizeZ + (-sizeZ));
        BackRightBottomP = new Vector3(x * sizeX + sizeX, y * sizeY + 0, z * sizeZ + (-sizeZ));
        BackRightUpperP = new Vector3(x * sizeX + sizeX, y * sizeY + sizeY, z * sizeZ + (-sizeZ));


        // Add points to list so that thay form a cube

        // North side of cube
        if (!cubeScheme.neigbors.frontNeighbor)
        {
            verts.AddRange(
            generateVerts4Quad(RightBottomP, RightUpperP, LeftUpperP, LeftBottomP));
            times++;
        }

        // South
        if (!cubeScheme.neigbors.backNeighbor)
        {
            verts.AddRange(
            generateVerts4Quad(BackleftBottomP, BackLeftUpperP, BackRightUpperP, BackRightBottomP));
            times++;
        }

        // West
        if (!cubeScheme.neigbors.leftNeighbor)
        {
            verts.AddRange(
            generateVerts4Quad(LeftBottomP, LeftUpperP, BackLeftUpperP, BackleftBottomP));
            times++;
        }

        // East
        if (!cubeScheme.neigbors.rightNeighbor)
        {
            verts.AddRange(
            generateVerts4Quad(BackRightBottomP, BackRightUpperP, RightUpperP, RightBottomP));
            times++;
        }

        // Up
        if (!cubeScheme.neigbors.topNeighbor)
        {
            verts.AddRange(
            generateVerts4Quad(BackRightUpperP, BackLeftUpperP, LeftUpperP, RightUpperP));
            times++;
        }

        // Bottom
        if (!cubeScheme.neigbors.bottomNeighbor)
        {
            verts.AddRange(
            generateVerts4Quad(BackleftBottomP, BackRightBottomP, RightBottomP, LeftBottomP));
            times++;
        }


        return verts;
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

    List<int> generateTris4Cube(int iteration, int times)
    {
        List<int> tris = new List<int>();

        // 6 times for each of cube face
        for (int i = 0; i < times; i++)
        {
            tris.AddRange(generateTris4Quad(iteration));
        }

        return tris;
    }

    List<Vector2> generateUvs4Cube(int times)
    {
        // Declare list of uvs
        List<Vector2> uvs = new List<Vector2>();

        // one iteration for each side of the cube
        for (int iCube = 0; iCube < times; iCube++)
        {
            // Adding uvs to list
            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));
        }

        return uvs;
    }

    // Create Mesh from intput of verts, tris and uvs lists
    Mesh generateMesh(List<Vector3> verts, List<int> tris, List<Vector2> uvs)
    {
        // Create a mesh and assign props needed
        Mesh mesh = new Mesh();
        mesh.name = "procedural mesh";

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.RecalculateNormals();

        return mesh;
    }

    // Create GameObject that will hold mesh we entered / Impure Function
    public GameObject generateMeshHolder(Mesh mesh, Vector3 position, Vector3 cubeSize, bool isKinematic, bool split, Vector3 blockPos)
    {
        // Create a game object and add components to it
        GameObject cube = new GameObject("cube");
        if (split)
            cube.tag = "atom";
        else
            cube.tag = "block";

        // Define mesh real scale
        //cube.transform.localScale = cubeSize; 

        // Add Components that crucial for rendering mesh and collision detection
        cube.AddComponent<MeshFilter>();
        cube.AddComponent<MeshRenderer>();
        cube.AddComponent<MeshCollider>();
        cube.AddComponent<Rigidbody>();

        // Assign components
        cube.GetComponent<Rigidbody>().isKinematic = isKinematic;
        cube.GetComponent<MeshCollider>().convex = !isKinematic;
        cube.GetComponent<MeshFilter>().sharedMesh = mesh; // Assign mesh from input
        cube.GetComponent<MeshCollider>().sharedMesh = mesh;
        cube.GetComponent<MeshRenderer>().material = Resources.Load("cube") as Material; // Assign material
        cube.transform.position = position; // Assign position

        if(split)
           blockControl.blockDictionary.Add(cube, new BlockControl.rbPosRot(cube.GetComponent<Rigidbody>(), blockPos));

        return cube;
    }

    public void splitIntoCubes(GameObject target)
    {
        Vector3 pos = target.transform.position;
        Vector3 size = target.transform.localScale;
        blockControl.blocksPos.Add(pos);
        GameObject.DestroyImmediate(target);

        for (int x = (int)pos.x; x < (int)pos.x + (int)size.x; x++)
        {
            for (int y = (int)pos.y; y < (int)pos.y + (int)size.y; y++)
            {
                for (int z = (int)pos.z - ((int)size.z - 1); z < (int)pos.z + 1; z++)
                {
                    generateMeshHolder(generateRowsOfCubeMeshes(1, 1, 1, 1, 1, 1), new Vector3(x, y, z), Vector3.one, false, true, pos);
                }
            }
        }
    }
}

public class CubeMeshData
{
    public Vector3Int cubeSize;
    private BlockScheme cubeScheme;
    // cube relative position decoupled for ease of use
    private int x, y, z;
    // x, y, z size of one single cube decoupled for ease of use
    private int sizeX, sizeY, sizeZ;
    // Declare Points one at a time
    private Vector3 LeftBottomP, LeftUpperP, RightBottomP, RightUpperP, BackleftBottomP, BackLeftUpperP, BackRightBottomP, BackRightUpperP;
    
    // ========ACTUAL CUBE MESH DATA DECLORATION==========
    List<Vector3> verts = new List<Vector3>();
    List<int> tris = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    // ======================END==================

    public CubeMeshData(BlockScheme cubeScheme, Vector3Int cubeSize)
    {
        this.cubeScheme = cubeScheme;
        this.sizeX = cubeSize.x;
        this.sizeY = cubeSize.y;
        this.sizeZ = cubeSize.z;
        this.x = cubeScheme.position.x;
        this.y = cubeScheme.position.y;
        this.z = cubeScheme.position.z;

        // Front quad
        LeftBottomP = new Vector3(x * sizeX + 0, y * sizeY + 0, z * sizeZ + 0);
        LeftUpperP = new Vector3(x * sizeX + 0, y * sizeY + sizeY, z * sizeZ + 0);
        RightBottomP = new Vector3(x * sizeX + sizeX, y * sizeY + 0, z * sizeZ + 0);
        RightUpperP = new Vector3(x * sizeX + sizeX, y * sizeY + sizeY, z * sizeZ + 0);

        // Back quad
        BackleftBottomP = new Vector3(x * sizeX + 0, y * sizeY + 0, z * sizeZ + (-sizeZ));
        BackLeftUpperP = new Vector3(x * sizeX + 0, y * sizeY + sizeY, z * sizeZ + (-sizeZ));
        BackRightBottomP = new Vector3(x * sizeX + sizeX, y * sizeY + 0, z * sizeZ + (-sizeZ));
        BackRightUpperP = new Vector3(x * sizeX + sizeX, y * sizeY + sizeY, z * sizeZ + (-sizeZ));
    }

    // Create verts for quad from input of width, height
    void generateVerts4Cube()
    {
        // Add verts(of predefinded points) to list so that thay form a cube

        addFrontFaceVerts();
        //generateTris4Quad();

        addBackFaceVerts();

        addRightFaceVerts();

        addLeftFaceVerts();

        addUpFaceVerts();

        addBottomFaceVerts();
    }

    void addFrontFaceVerts()
    {
        // North side of a cube
        verts.AddRange(
        generateVerts4Quad(RightBottomP, RightUpperP, LeftUpperP, LeftBottomP));
    }

    void addBackFaceVerts()
    {
        // South side of a cube
        verts.AddRange(
        generateVerts4Quad(BackleftBottomP, BackLeftUpperP, BackRightUpperP, BackRightBottomP));
    }

    void addRightFaceVerts()
    {
        // East side of a cube
        verts.AddRange(
        generateVerts4Quad(BackRightBottomP, BackRightUpperP, RightUpperP, RightBottomP));
    }

    void addLeftFaceVerts()
    {
        // West side of a cube
        verts.AddRange(
        generateVerts4Quad(LeftBottomP, LeftUpperP, BackLeftUpperP, BackleftBottomP));
    }

    void addUpFaceVerts()
    {
        verts.AddRange(
        generateVerts4Quad(BackRightUpperP, BackLeftUpperP, LeftUpperP, RightUpperP));
    }

    void addBottomFaceVerts()
    {
        verts.AddRange(
        generateVerts4Quad(BackleftBottomP, BackRightBottomP, RightBottomP, LeftBottomP));
    }

    void addFrontFaceTris()
    {

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

    void generateTris4Cube(int iteration)
    {
        // 6 times for each of cube face
        for (int i = iteration; i < iteration + 6; i++)
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
        // one iteration for each side of the cube
        for (int iCube = 0; iCube < 6; iCube++)
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
}