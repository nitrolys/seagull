using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class PedestrianController : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject crumbs;

    public float walk_Range = 3f;

    private GameObject player;
    private bool hasFries;
    private bool hostile;
    private float minWaitTime = 2f;
    private float maxWaitTime = 8f;
    private GameObject holdPoint;

    public void SetHasFries(bool hasFries)
    {
        this.hasFries = hasFries;
        holdPoint.SetActive(hasFries);
    }

    private void Awake()
    {
        hostile = false;
        player = GameObject.Find("Seagull");
        holdPoint = gameObject.transform.Find("HoldPoint").gameObject;
        float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        Invoke("RandomWalk", waitTime);
    }

    private void Update()
    {

    }

    private void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && hasFries)
        {
            SetHasFries(false);
            Instantiate(crumbs, holdPoint.transform.position, holdPoint.transform.rotation);
            SetHostile();
        }
    }

    private void SetHostile()
    {
        CancelInvoke("RandomWalk");
        agent.SetDestination(transform.position);
        hostile = true;
        InvokeRepeating("Chase", 1.0f, 0.2f);
    }

    private void RandomWalk()
    {
        float r = UnityEngine.Random.Range(0f, walk_Range);
        float theta = UnityEngine.Random.Range(0f, 2 * Mathf.PI);
        Vector3 dest = transform.position + new Vector3(r * Mathf.Cos(theta), 0f, r * Mathf.Sin(theta));
        agent.SetDestination(dest);
        float waitTime = UnityEngine.Random.Range(minWaitTime, maxWaitTime);
        Invoke("RandomWalk", waitTime);
    }
}
