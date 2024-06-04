using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpawnController : MonoBehaviour
{
    public GameObject pedestrian;
    public float spawnRadiusMin;
    public float spawnRadiusMax;
    public int numInitial;
    public int numHasFries;
    void Start()
    {
        for (int i = 0; i < numInitial; i++)
        {
            float r = UnityEngine.Random.Range(spawnRadiusMin, spawnRadiusMax);
            float theta = UnityEngine.Random.Range(0, 2 * Mathf.PI);
            Vector3 p = transform.position + new Vector3(r * Mathf.Cos(theta), 0, r * Mathf.Sin(theta));
            NavMeshHit hit;
            NavMesh.SamplePosition(p, out hit, 100f, NavMesh.AllAreas);
            p = hit.position;
            float rotation = UnityEngine.Random.Range(0, 180);
            GameObject newSpawn = Instantiate(pedestrian, p, Quaternion.Euler(new Vector3(0, rotation, 0)));
            newSpawn.GetComponent<PedestrianController>().SetHasFries(i < numHasFries);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
