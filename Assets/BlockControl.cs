using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BlockControl : MonoBehaviour
{
    static GBAlg2 gb;
    public Dictionary<GameObject, rbPosRot> blockDictionary = new Dictionary<GameObject, rbPosRot>();
    public List<Vector3> blocksPos = new List<Vector3>();

    public class rbPosRot
    {
        public Rigidbody rb;
        public Vector3 blockStartPos;
        public Stack<Vector3> position = new Stack<Vector3>();
        public Stack<Quaternion> rotation = new Stack<Quaternion>();

        public rbPosRot(Rigidbody r, Vector3 blockStartPos)
        {
            this.rb = r;
            this.blockStartPos = blockStartPos;
        }
    }

    [RuntimeInitializeOnLoadMethod]
    static void init()
    {
        gb = FindObjectOfType<GBAlg2>();
        GameObject blockControler = new GameObject("Block Controler");
        blockControler.transform.position = Vector3.zero;
        blockControler.AddComponent<BlockControl>();
    }

    public bool isRecording = false, isRewinding = false;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            isRewinding = true;
    }

    public void FixedUpdate()
    {
        if (isRecording && !isRewinding)
            Record();
        if (isRewinding)
            Rewind();
    }

    void Record()
    {
        foreach(KeyValuePair<GameObject, rbPosRot> kvp in blockDictionary)
        {
            if (!kvp.Value.rb.IsSleeping())
            {
                kvp.Value.position.Push(kvp.Key.transform.position);
                kvp.Value.rotation.Push(kvp.Key.transform.rotation);
            }
        }
    }

    void Rewind()
    {
        bool done = true;

        foreach(KeyValuePair<GameObject, rbPosRot> kvp in blockDictionary)
        {
            kvp.Value.rb.Sleep();
            kvp.Value.rb.isKinematic = true;

            if (kvp.Value.rotation.Count > 0)
            {
                kvp.Key.transform.position = kvp.Value.position.Pop();
                kvp.Key.transform.rotation = kvp.Value.rotation.Pop();
                done = false;
            }
        }

        if (done)
        {
            foreach (GameObject cube in blockDictionary.Keys)
            {
                GameObject.DestroyImmediate(cube);
            }
            foreach(Vector3 pos in blocksPos)
            {
                gb.generateMeshHolder(gb.generateRowsOfCubeMeshes(1, 1, 1, 1, 1, 1), pos, gb.cubeSize, true, false, pos);
            }
            blockDictionary.Clear();
            blocksPos.Clear();
            isRecording = isRewinding = false;
        }
    }
}
