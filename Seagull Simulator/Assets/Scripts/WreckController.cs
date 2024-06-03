using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            Rigidbody rb = child.GetComponent<Rigidbody>();
            rb.velocity = 5 * Random.onUnitSphere;
            rb.angularVelocity = Random.onUnitSphere;
        }

    }
}
