using System.Collections.Generic;
using UnityEngine;

public static class BlockMeshDataInterpreter
{

    static int i = 0;
    static int trisIndexStart;

    public static Mesh buildingBlocks(Building building)
    {
        MeshData meshData = new MeshData();

        meshData = blockBlocks(building.buildingScheme);
        trisIndexStart = 0;

        // returning result of the function that takes
        // (V, T, U) and returns completed mesh
        return buildingMesh(meshData.verts, meshData.tris, meshData.uvs, meshData.normals);
    }

    private static MeshData blockBlocks(BlockScheme[,,] blockScheme)
    {
        MeshData meshData = new MeshData();

        for (int x = 0; x < blockScheme.GetLength(0); x++)
        {
            for (int y = 0; y < blockScheme.GetLength(1); y++)
            {
                for (int z = 0; z < blockScheme.GetLength(2); z++)
                {
                    if (blockScheme[x, y, z] == null)
                        continue;
                    if (blockScheme[x, y, z].children == null)
                    {
                        if (!blockScheme[x, y, z].visible)
                            continue;
                        BlockMeshData Cube = new BlockMeshData(blockScheme[x, y, z], blockScheme[x, y, z].size, trisIndexStart);
                        Cube.generateCubeMeshData();
                        trisIndexStart = Cube.triIndexEnd;
                        meshData.verts.AddRange(Cube.verts);
                        meshData.tris.AddRange(Cube.tris);
                        meshData.uvs.AddRange(Cube.uvs);
                        meshData.normals.AddRange(Cube.normals);
                    }
                    else
                    {
                        MeshData meshDataR = new MeshData();
                        meshDataR = blockBlocks(blockScheme[x, y, z].children);
                        meshData.verts.AddRange(meshDataR.verts);
                        meshData.tris.AddRange(meshDataR.tris);
                        meshData.uvs.AddRange(meshDataR.uvs);
                        meshData.normals.AddRange(meshDataR.normals);
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
        List<Vector3> normals = new List<Vector3>();

        BlockMeshData Cube = new BlockMeshData(null, cubeSize, 0);

        Cube.makeSingleCubeMeshData();

        verts.AddRange(Cube.verts);
        tris.AddRange(Cube.tris);
        uvs.AddRange(Cube.uvs);
        normals.AddRange(Cube.normals);

        return buildingMesh(verts, tris, uvs, normals);
    }

    // Create Mesh from intput of verts, tris and uvs lists
    private static Mesh buildingMesh(List<Vector3> verts, List<int> tris, List<Vector2> uvs, List<Vector3> normals)
    {
        // Create a mesh and assign props needed
        Mesh mesh = new Mesh();
        mesh.name = "procedural mesh";

        mesh.SetVertices(verts);
        mesh.SetTriangles(tris, 0);
        mesh.SetUVs(0, uvs);
        mesh.SetNormals(normals);
        //mesh.RecalculateNormals();

        return mesh;
    }
    // Create GameObject that will hold mesh we entered
    public static GameObject buildingGameObject(Mesh mesh, Vector3 position, bool isKinematic, bool split, Building building)
    {
        GameObject go = new GameObject();
        // Add Components that crucial for rendering mesh and collision detection
        go.AddComponent<MeshFilter>().sharedMesh = mesh; // Assign mesh from input
        go.AddComponent<MeshRenderer>().material = Main.material; // Assign material
        if (split)
            go.AddComponent<BoxCollider>();
        else
            go.AddComponent<MeshCollider>().sharedMesh = mesh;
        go.AddComponent<Rigidbody>().isKinematic = isKinematic;
        // Create a game object and add components to it
        if (split)
        {
            go.tag = "atom";
            go.name = "cube";
            BlockRewind.blockDictionary.Add(go, new PositionAndRotation(go.GetComponent<Rigidbody>()));
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


        // Assign position
        go.transform.position = position;

        return go;
    }
}