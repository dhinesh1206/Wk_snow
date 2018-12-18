using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiskMovement : MonoBehaviour {

    public Vector3 direction;
    Rigidbody rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        StartCoroutine(changeDirection(1.5f));
    }

    // Update is called once per frame
    void Update () 
    {
        rb.angularVelocity = direction*Time.deltaTime*2;
	}


    IEnumerator changeDirection(float t)
    {
        yield return new WaitForSeconds(t);
        direction = -direction;
        StartCoroutine(changeDirection(1.5f));
    }
}
