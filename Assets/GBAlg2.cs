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
    public int cubeSizeX = 1, cubeSizeY = 1, cubeSizeZ = 1;
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
            new Vector3(x * cubeSizeX * buildingWidth + (x * gutter), 0, z * cubeSizeZ * buildingLength + (z * gutter)),
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

        int trisIndexStart = 0;
        // generating rows of cubes
        for (int x = 0; x < building.width; x++)
        {
            for (int y = 0; y < building.height; y++)
            {
                for (int z = 0; z < building.length; z++)
                {
                    if (building.buildingScheme[x, y, z] == null)
                        continue;
                    CubeMeshData Cube = new CubeMeshData(building.buildingScheme[x, y, z], new Vector3Int(cubeSizeX, cubeSizeZ, cubeSizeZ), trisIndexStart);
                    Cube.generateAllMeshDataForCube();
                    trisIndexStart = Cube.triIndexEnd;

                    verts.AddRange(Cube.verts);
                    tris.AddRange(Cube.tris);
                    uvs.AddRange(Cube.uvs);
                }
            }
        }

        // returning result of the function that takes
        // (V, T, U) and returns completed mesh
        return generateMesh(verts, tris, uvs);
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
        {
            cube.tag = "atom";
            cube.name = "liquid";
        }
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
                    Debug.Log("lol inside");
                    generateMeshHolder(generateRowsOfCubeMeshes(1, 1, 1, 1, 1, 1), new Vector3(x, y, z), Vector3.one, false, true, pos);
                }
            }
        }
    }
}