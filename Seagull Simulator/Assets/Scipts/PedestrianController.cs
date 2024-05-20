using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Pedestrian : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject player;
    public GameObject crumbs;

    private Boolean hasFries;
    private Boolean hostile;
    private GameObject holdPoint;

    private void Start()
    {
        hasFries = true;
        holdPoint = gameObject.transform.Find("HoldPoint").gameObject;
    }

    private void Update()
    {
        if (hostile)
        {
            Chase();
        }
    }

    void Chase()
    {
        agent.SetDestination(player.transform.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && hasFries)
        {
            FriesStolen();
        }
    }

    void FriesStolen()
    {
        holdPoint.SetActive(false);
        Instantiate(crumbs, holdPoint.transform.position, holdPoint.transform.rotation);
        hasFries = false;
        Invoke("SetHostile", 1.0f);
    }

    void SetHostile()
    {
        hostile = true;
    }
}
