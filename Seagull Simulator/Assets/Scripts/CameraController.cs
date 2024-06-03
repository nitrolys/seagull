using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private static Vector3 position_offset = new Vector3(0, 3, -3);
    void Start()
    {
        transform.rotation = Quaternion.Euler(45, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = player.transform.position + position_offset;
    }
}
