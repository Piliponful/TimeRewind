using System.Collections.Generic;
using UnityEngine;

public static class BlockMeshDataInterpreter
{

    static int i = 0;

    public static Mesh buildingBlocks(Building building)
    {
        // All data needed for mesh creation = (V, T, U)
        // Declare all vars needed to generate rows of cube meshes
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

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
                    if (!building.buildingScheme[x, y, z].visible)
                        continue;
                    BlockMeshData Cube = new BlockMeshData(building.buildingScheme[x, y, z], building.blockSize, trisIndexStart);
                    Cube.generateCubeMeshData();
                    trisIndexStart = Cube.triIndexEnd;

                    verts.AddRange(Cube.verts);
                    tris.AddRange(Cube.tris);
                    uvs.AddRange(Cube.uvs);
                }
            }
        }

        // returning result of the function that takes
        // (V, T, U) and returns completed mesh
        return buildingMesh(verts, tris, uvs);
    }
    public static Mesh oneSimpleCube(Vector3Int cubeSize)
    {
        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Vector2> uvs = new List<Vector2>();

        BlockMeshData Cube = new BlockMeshData(null, cubeSize, 0);

        Cube.makeSingleCubeMeshData();

        verts.AddRange(Cube.verts);
        tris.AddRange(Cube.tris);
        uvs.AddRange(Cube.uvs);

        return buildingMesh(verts, tris, uvs);
    }
    // Create Mesh from intput of verts, tris and uvs lists
    private static Mesh buildingMesh(List<Vector3> verts, List<int> tris, List<Vector2> uvs)
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
    // Create GameObject that will hold mesh we entered
    public static GameObject buildingGameObject(Mesh mesh, Vector3 position, bool isKinematic, bool split, Building building)
    {
        // Create a game object and add components to it
        GameObject go = new GameObject();
        if (split)
        {
            go.tag = "atom";
            go.name = "cube";
        }
        else
        {
            if (building != null)
            {
                go.name = "building " + i++;
                go.tag = "building";
            }
            else
            {
                go.tag = "block";
            }

            // Adding to dictrionary current building gameObject 
            // to later retrive through it specific building object/class
            // instance from which gameObject(or its mesh) was created
            Town.buildings.Add(go, building);
        }

        // Add Components that crucial for rendering mesh and collision detection
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.AddComponent<MeshCollider>();
        go.AddComponent<Rigidbody>();

        // Assign components
        go.GetComponent<Rigidbody>().isKinematic = isKinematic;
        go.GetComponent<MeshCollider>().convex = !isKinematic;
        go.GetComponent<MeshFilter>().sharedMesh = mesh; // Assign mesh from input
        go.GetComponent<MeshCollider>().sharedMesh = mesh; // Assign mesh from input
        go.GetComponent<MeshRenderer>().material = Resources.Load("cube") as Material; // Assign material

        // Assign position
        go.transform.position = position;

        if (split)
            BlockRewind.blockDictionary.Add(go, new PositionAndRotation(go.GetComponent<Rigidbody>()));

        return go;
    }
}
