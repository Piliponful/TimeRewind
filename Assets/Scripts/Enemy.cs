using UnityEngine;
using System;

public class Enemy : MonoBehaviour
{
    public event Action<float, Vector3> onEnemyHit;

    public void kill(float force, Vector3 position)
    {
        this.GetComponent<Animator>().enabled = false;
        onEnemyHit(force, position);
    }
}
