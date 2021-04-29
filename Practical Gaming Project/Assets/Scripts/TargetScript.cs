using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetScript : MonoBehaviour
{
    enum targetState { idle, walking }
    targetState isCurrently;
    
    ManagerScript myManager;
    NavMeshAgent agent;
    GameObject[] walkPoints;
    GameObject nextPoint;
    float speed;
    float fieldOfView;
    private float idleTime;
    private float totalIdleTime;

    // Start is called before the first frame update
    void Start()
    {
        isCurrently = targetState.idle;
        speed = 10;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        walkPoints = GameObject.FindGameObjectsWithTag("Walk Point");
        nextPoint = walkPoints[0];
        idleTime = 3;
    }

    // Update is called once per frame
    void Update()
    {
        switch (isCurrently)
        {
            case targetState.walking:
                if (transform.position == agent.destination)
                {
                    nextPoint.SetActive(true);
                    SwitchToIdle();
                }
                break;

            case targetState.idle:
                {
                    totalIdleTime += Time.deltaTime;
                    if (totalIdleTime > idleTime)
                        Walking();
                }
                break;
        }
    }

    private void SwitchToIdle()
    {
        isCurrently = targetState.idle;

        totalIdleTime = 0f;
    }

    internal void Walking()
    {
        if (isCurrently != targetState.walking)
        {
            nextPoint = walkPoints[Random.Range(0, walkPoints.Length)];
            if (!nextPoint.activeSelf)
                Walking();
            else
            {
                nextPoint.SetActive(false);
                agent.SetDestination(new Vector3(nextPoint.transform.position.x, 0, nextPoint.transform.position.z));
                isCurrently = targetState.walking;
            }
        }
    }

    internal void SetManager(ManagerScript managerScript)
    {
        myManager = managerScript;
    }

    internal void SetPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, 0f, position.z);
    }
}
