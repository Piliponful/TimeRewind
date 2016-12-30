using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Part : MonoBehaviour {
    Rigidbody rb;
    Collider col;

	void Start () {
        this.gameObject.tag = "enemy_body_part";
        col = this.GetComponent<Collider>();
        rb = this.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        this.transform.root.GetComponent<Enemy>().onEnemyHit += Enemy_Part_onEnemyHit;
    }

    private void Enemy_Part_onEnemyHit(float force, Vector3 position)
    {
        rb.isKinematic = false;
        push(force, position);
    }

    public void push(float force, Vector3 position)
    {
        rb.AddForceAtPosition(force * position, position);
    }
}
