using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Pedestrian : MonoBehaviour
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

    private void Start()
    {
        hasFries = true;
        hostile = false;
        player = GameObject.Find("Player");
        holdPoint = gameObject.transform.Find("HoldPoint").gameObject;
        RandomWalk();
    }

    private void Update()
    {
        if (hostile)
        {
            CancelInvoke("RandomWalk");
            Chase();
        }
    }

    private void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player && hasFries)
        {
            FriesStolen();
        }
    }

    private void FriesStolen()
    {
        holdPoint.SetActive(false);
        Instantiate(crumbs, holdPoint.transform.position, holdPoint.transform.rotation);
        hasFries = false;
        Invoke("SetHostile", 1.0f);
    }

    private void SetHostile()
    {
        hostile = true;
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
