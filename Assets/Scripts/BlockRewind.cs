﻿using UnityEngine;
using System.Collections.Generic;

public static class BlockRewind
{
    public static Dictionary<GameObject, PositionAndRotation> blockDictionary = new Dictionary<GameObject, PositionAndRotation>();
    public static List<Vector3> blocksPos = new List<Vector3>();
    private static bool isRecording = false, isRewinding = false;

    public static void ToUpdate()
    {
        if (Input.GetKeyDown(KeyCode.R))
            isRewinding = true;
    }

    public static void ToFixedUpdate()
    {
        if (isRecording && !isRewinding)
            Record();
        if (isRewinding)
            Rewind();
    }

    static void Record()
    {
        foreach (KeyValuePair<GameObject, PositionAndRotation> kvp in blockDictionary)
        {
            if (!kvp.Value.rb.IsSleeping())
            {
                kvp.Value.position.Push(kvp.Key.transform.position);
                kvp.Value.rotation.Push(kvp.Key.transform.rotation);
            }
        }
    }

    static void Rewind()
    {
        bool done = true;

        foreach (KeyValuePair<GameObject, PositionAndRotation> kvp in blockDictionary)
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
            foreach (Vector3 pos in blocksPos)
            {
                //gb.generateMeshHolder(gb.generateRowsOfCubeMeshes(1, 1, 1, 1, 1, 1), pos, true, false, pos);
            }
            blockDictionary.Clear();
            blocksPos.Clear();
            isRecording = isRewinding = false;
        }
    }
}
