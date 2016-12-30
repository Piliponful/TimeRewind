using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GBAlg : MonoBehaviour {
    public enum ChoseHeight { SameHeight, DifferentHeight, SameFor3InSequence, SameFor2InSequence }
    public int maxWidthOfOneRect = 5;
    public int maxLenghtOfOneRect = 5;
    public int maxHeightOfOneRect = 10;

    public bool randomHeight = false;

    public ChoseHeight choseH = ChoseHeight.SameHeight;
    public int maxHeightOfBuilding = 10;
    public int maxWidthOfBuilding = 10;
    public int maxLenghtOfBuilding = 10;
    public bool constraintRect = false;
    public int maxRects = 50;
    public int minRects = 2;

    private int width;
    private int lenght;
    private int height;
    private int plus = 1;
    private int recurseCount = 0;
    int rand;

    int cellsCount = 0;
    public int maxCellCount = 1000;

    [Range(0, 1)]
    public float newRectPossibility = .15f;

    bool[,,] building;

    //**Ignore This stuff. Its all about mesh creation.
    Vector3 p1;
    Vector3 p2;
    Vector3 p3;
    Vector3 p4;

    Vector3 bottomP1;
    Vector3 bottomP2;
    Vector3 bottomP3;
    Vector3 bottomP4;

    List<Vector3> newVerts = new List<Vector3>();
    List<int> newTris = new List<int>();
    List<Vector2> newUvs = new List<Vector2>();
    int vertsCount = 0;
    int squareCount = 0;
    int meshCount = 0;

    int prevMeshHolderCount = 0;

    int meshHolderCount = 0;
    GameObject[] meshHolder;
    bool isFirst = true;

    public Material material;

    int scale = 3;
    int divisions = 3;
    [Range(0, 1)]
    public float randomValue2 = .5f;
    [Range(0, 1)]
    public float randomValue3 = .5f;
    [Range(0, 1)]
    public float randomValue4 = .5f;
    [Range(1, 5)]
    public int subDiv = 1;

    Rect[] newRectCoords;
    bool[,,] splitCube;

    public bool newMethod = true;

    int xIndex;
    int yIndex;
    int zIndex;

    bool[,,] split;

    public float XRand = 1;
    public float ZRand = 1;
    public int iteratorRandX = 3;
    public int iteratorRandZ = 3;

    void Start()
    {
        material = Resources.Load("New Material") as Material;
        GenetateCity();
    }

    public int cityLength = 0;
    public int cityWidth = 0;
    void GenetateCity()
    {
        for (int l = 0; l < cityLength; l++)
        {
            for (int w = 0; w < cityWidth; w++)
            {
                split = new bool[maxWidthOfBuilding, maxHeightOfBuilding, maxLenghtOfBuilding];
                newRectCoords = new Rect[maxRects];
                building = new bool[maxWidthOfBuilding, maxHeightOfBuilding, maxLenghtOfBuilding];
                RectangleGen();
                GenerateBuilding(l, w);
            }
        }
    }
    void RectangleGen()
    {
        int xValue = 0, yValue = 0, zValue = 0;
        height = Random.Range(1, maxHeightOfOneRect);
        for (int i = 0; i < Random.Range(minRects, maxRects); i++)
        {
            newRectCoords[i] = new Rect(0, 0, 0);
             
            if (i > 0 && newMethod)
            {
                xValue = newRectCoords[i - 1].x;
                yValue = newRectCoords[i - 1].y;
                zValue = newRectCoords[i - 1].z;
            }
            width = Random.Range(1, maxWidthOfOneRect);
            lenght = Random.Range(1, maxLenghtOfOneRect);

            if (choseH == ChoseHeight.DifferentHeight)
                height = Random.Range(1, maxHeightOfOneRect);

            if (choseH == ChoseHeight.SameFor2InSequence)
                if (i % 2 == 0) height = Random.Range(1, maxHeightOfOneRect);

            if (choseH == ChoseHeight.SameFor3InSequence)
                if (i % 3 == 0) height = Random.Range(1, maxHeightOfOneRect);

            for (int x = xValue; x < width + xValue; x++)
            {
                for (int y = yValue; y < height + xValue; y++)
                {
                    for (int z = zValue; z < lenght + xValue; z++)
                    {
                        cellsCount++;
                        if (y >= maxHeightOfBuilding || x >= maxWidthOfBuilding || z >= maxLenghtOfBuilding) return;

                        if (i >= maxRects && constraintRect) return;

                        if (cellsCount >= maxCellCount) return;

                        if (!randomHeight && y == 0 && newRectPossibility > Random.value)
                            newRectCoords[i] = new Rect(x, y, z);

                        if (randomHeight && newRectPossibility > Random.value)
                            newRectCoords[i] = new Rect(x, y, z);
                        if (Random.value < randomValue2)
                            split[x, y, z] = true;

                        building[x, y, z] = true;
                    }
                }
            }
        }
    }
    void GenerateBuilding(int l, int w)
    {
        for (int x = 0; x < building.GetLength(0); x++)
        {
            for (int y = 0; y < building.GetLength(1); y++)
            {
                for (int z = 0; z < building.GetLength(2); z++)
                {
                    if (building[x, y, z])
                    {
                        if (split[x, y, z])
                        {
                            CubeSplit(x, y, z, 1, l, w);
                            continue;
                        }
                        BuildCube(x, y, z, scale, l, w);
                    }
                }
            }
        }
        if (vertsCount != 0)
            NewMesh(l, w);
        Debug.Log(newVerts.Count);
        Debug.Log(newTris.Count);
    }
    void BuildCube(int x, int y, int z, float scale, int l, int w)
    {
        Points(x, y, z, scale);
        if (vertsCount >= 64000) NewMesh(l, w);
        //if (!GetByte(x, y + 1, z))
        CubeTop(newVerts, newUvs, newTris, x, y, z);
        //if (!GetByte(x, y - 1, z))
        CubeBottom(newVerts, newUvs, newTris, x, y, z);
        //if (!GetByte(x, y, z + 1))
        CubeNorth(newVerts, newUvs, newTris, x, y, z);
        //if (!GetByte(x, y, z - 1))
        CubeSouth(newVerts, newUvs, newTris, x, y, z);
        //if (!GetByte(x + 1, y, z))
        CubeEast(newVerts, newUvs, newTris, x, y, z);
        //if (!GetByte(x - 1, y, z))
        CubeWest(newVerts, newUvs, newTris, x, y, z);
    }
    bool GetByte(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= building.GetLength(0) || y >= building.GetLength(1) || z >= building.GetLength(2)) return false;
        return building[z, y, z];
    }
    void NewMesh(int l, int w)
    {
        GameObject building = GameObject.Find("Buildings Manager");
        int prevL = -1, prevW = -1;
        if(prevL != l || prevW != w)
        {
            prevL = l; prevW = w;
            building = new GameObject("building");
            building.transform.position = new Vector3(maxLenghtOfBuilding * scale + (20 * l), 0, maxWidthOfBuilding * scale + (20 * w));
        }
        vertsCount = 0;
        squareCount = 0;
        meshCount++;

        Mesh mesh = new Mesh();
        mesh.name = "mesh " + meshCount;
        mesh.vertices = newVerts.ToArray();
        mesh.uv = newUvs.ToArray();
        mesh.triangles = newTris.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        GameObject buildingPart = new GameObject("buildingPart");

        buildingPart.AddComponent<MeshRenderer>().material = material;
        buildingPart.AddComponent<MeshFilter>().sharedMesh = mesh;
        buildingPart.AddComponent<MeshCollider>().sharedMesh = mesh;

        buildingPart.transform.parent = building.transform;
        buildingPart.transform.localPosition = Vector3.zero;


        newTris.Clear();
        newUvs.Clear();
        newVerts.Clear();
    }
    void Points(float x, float y, float z, float _scale)
    {
        p1 = new Vector3(x * _scale, (y + 1) * _scale, (z + 1) * _scale);
        p2 = new Vector3((x + 1) * _scale, (y + 1) * _scale, (z + 1) * _scale);
        p3 = new Vector3((x + 1) * _scale, (y + 1) * _scale, z * _scale);
        p4 = new Vector3(x * _scale, (y + 1) * _scale, z * _scale);

        bottomP1 = new Vector3(x * _scale, y * _scale, z * _scale);
        bottomP2 = new Vector3((x + 1) * _scale, y * _scale, z * _scale);
        bottomP3 = new Vector3((x + 1) * _scale, y * _scale, (z + 1) * _scale);
        bottomP4 = new Vector3(x * _scale, y * _scale, (z + 1) * _scale);
    }
    void CubeSplit(int a, int b, int c, int pow, int l, int w)
    {
        float value2 = 0;
        if (pow == 1) value2 = 0;
        if (pow == 2) value2 = 1;
        if (pow == 3) value2 = 6;
        if (pow == 4) value2 = 23;
        if (pow == 5) value2 = 76;

        float _scale = scale / Mathf.Pow(divisions, pow);

        float value = pow + value2;

        a = Mathf.RoundToInt(a * (Mathf.Pow(divisions, pow) / value));
        b = Mathf.RoundToInt(b * (Mathf.Pow(divisions, pow) / value));
        c = Mathf.RoundToInt(c * (Mathf.Pow(divisions, pow) / value));

        for (int x = a; x < a + divisions; x++)
        {
            for (int y = b; y < b + divisions; y++)
            {
                for (int z = c; z < c + divisions; z++)
                {

                    if (isSierpinskiCarpetPixelFilled(x, z) || isSierpinskiCarpetPixelFilled(y, x) || isSierpinskiCarpetPixelFilled(y, z)) continue;

                    //if (y == b + divisions - 1 && Random.value > randomValue3) continue;

                    //if (pow < subDiv && Random.value > randomValue4) { CubeSplit(x, y, z, pow + 1); continue; }

                    BuildCube(x, y, z, _scale, l, w);
                }
            }
        }
    }
    bool isSierpinskiCarpetPixelFilled(int x, int z)
    {
        while (x > 0 || z > 0) // when either of these reaches zero the pixel is determined to be on the edge 
        // at that square level and must be filled
        {
            if (x % iteratorRandX == XRand && z % iteratorRandZ == ZRand) //checks if the pixel is in the center for the current square level
                return true;
            x /= 3; //x and y are decremented to check the next larger square level
            z /= 3;
        }
        return false; // if all possible square levels are checked and the pixel is not determined 
        // to be open it must be filled
    }
    void CubeTop(List<Vector3> V, List<Vector2> U, List<int> T, float x, float y, float z)
    {
        vertsCount += 4;
        V.Add(p1);V.Add(p2);V.Add(p3);V.Add(p4);
        CubeTU(T, U);
    }
    void CubeBottom(List<Vector3> V, List<Vector2> U, List<int> T, float x, float y, float z)
    {
        vertsCount += 4;
		V.Add(bottomP1);V.Add(bottomP2);V.Add(bottomP3);V.Add(bottomP4);
        CubeTU(T, U);
    }
    void CubeNorth(List<Vector3> V, List<Vector2> U, List<int> T, float x, float y, float z)
    {
        vertsCount += 4;
		V.Add(bottomP3);V.Add(p2);V.Add(p1);V.Add(bottomP4);
        CubeTU(T, U);
    }
    void CubeSouth(List<Vector3> V, List<Vector2> U, List<int> T, float x, float y, float z)
    {
        vertsCount += 4;
		V.Add(bottomP1);V.Add(p4);V.Add(p3);V.Add(bottomP2);
        CubeTU(T, U);
    }
    void CubeWest(List<Vector3> V, List<Vector2> U, List<int> T, float x, float y, float z)
    {
        vertsCount += 4;
		V.Add(bottomP4);V.Add(p1);V.Add(p4);V.Add(bottomP1);
        CubeTU(T, U);
    }
    void CubeEast(List<Vector3> V, List<Vector2> U, List<int> T, float x, float y, float z)
    {
        vertsCount += 4;
		V.Add(bottomP2);V.Add(p3);V.Add(p2);V.Add(bottomP3);
        CubeTU(T, U);
    }
    void CubeTU(List<int> T, List<Vector2> U)
    {
        T.Add(squareCount * 4); T.Add((squareCount * 4) + 1); T.Add((squareCount * 4) + 2); T.Add((squareCount * 4)); T.Add((squareCount * 4) + 2); T.Add((squareCount * 4) + 3);
        U.Add(new Vector2(0, 0)); U.Add(new Vector2(1, 0)); U.Add(new Vector2(1, 1)); U.Add(new Vector2(0, 1));
        squareCount++;
    }
}
public class Rect
{
    public int x, z, y;
    public Rect(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}
