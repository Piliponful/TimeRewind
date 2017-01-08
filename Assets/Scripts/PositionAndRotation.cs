using UnityEngine;
using System.Collections.Generic;

public class PositionAndRotation
{
    public Rigidbody rb;
    public Stack<Vector3> position = new Stack<Vector3>();
    public Stack<Quaternion> rotation = new Stack<Quaternion>();

    public PositionAndRotation(Rigidbody rigidbody)
    {
        this.rb = rigidbody;
    }
}