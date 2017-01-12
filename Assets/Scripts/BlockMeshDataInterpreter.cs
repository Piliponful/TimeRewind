using System.Collections.Generic;
using UnityEngine;

public static class BlockMeshDataInterpreter
{

    static int i = 0;

    public static Mesh buildingBlocks(Building building)
    {
        MeshData meshData = new MeshData();

        meshData = blockBlocks(building.buildingScheme);

        // returning result of the function that takes
        // (V, T, U) and returns completed mesh
        return buildingMesh(meshData.verts, meshData.tris, meshData.uvs);
    }

    private static MeshData blockBlocks(BlockScheme[,,] blockScheme)
    {
        MeshData meshData = new MeshData();
        
        int trisIndexStart = 0;
        for (int x = 0; x < blockScheme.GetLength(0); x++)
        {
            for (int y = 0; y < blockScheme.GetLength(1); y++)
            {
                for (int z = 0; z < blockScheme.GetLength(2); z++)
                {
                    if (blockScheme[x, y, z] == null)
                        continue;
                    if(blockScheme[x, y, z].children == null)
                    {
                        if (!blockScheme[x, y, z].visible)
                            continue;
                        BlockMeshData Cube = new BlockMeshData(blockScheme[x, y, z], blockScheme[x, y, z].blockSize, trisIndexStart);
                        Cube.generateCubeMeshData();
                        trisIndexStart = Cube.triIndexEnd;
                        meshData.verts.AddRange(Cube.verts);
                        meshData.tris.AddRange(Cube.tris);
                        meshData.uvs.AddRange(Cube.uvs);
                    }
                    else
                    {
                        MeshData meshDataR = new MeshData();
                        meshDataR = blockBlocks(blockScheme[x, y, z].children);
                        meshData.verts.AddRange(meshDataR.verts);
                        meshData.tris.AddRange(meshDataR.tris);
                        meshData.uvs.AddRange(meshDataR.uvs);
                    }
                }
            }
        }

        return meshData;
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
        go.name = "building " + i++;
        if (split)
        {
            go.tag = "atom";
            go.name = "cube";
        }
        else
        {
            if (building != null)
                go.tag = "building";
            else
                go.tag = "block";

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

public class MeshData
{
    public List<Vector3> verts;
    public List<int> tris;
    public List<Vector2> uvs;

    public MeshData()
    {
        this.verts = new List<Vector3>();
        this.tris = new List<int>();
        this.uvs = new List<Vector2>();
    }
}
