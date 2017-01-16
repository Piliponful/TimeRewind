using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public List<Vector3> verts;
    public List<int> tris;
    public List<Vector2> uvs;
    public List<Vector3> normals;
    public MeshData()
    {
        this.verts = new List<Vector3>();
        this.tris = new List<int>();
        this.uvs = new List<Vector2>();
        this.normals = new List<Vector3>();
    }
}
