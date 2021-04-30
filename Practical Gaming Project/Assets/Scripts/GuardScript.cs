using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardScript : MonoBehaviour
{
    enum guardState {idle, patrolling, searching, chasing, shooting}
    guardState isCurrently;

    ManagerScript myManager;
    NavMeshAgent agent;
    GameObject[] patrolPoints;
    GameObject nextPoint;
    float speed;
    float fieldOfView;
    float timeToDraw;
    private float searchTime;
    private float turningSpeed;
    private float turningDirection;
    private float totalSearchTimer;
    private float nextTurnTime;
    private Vector3 startingPosition;
    private int positioned = 0;
    private float guardsY;

    // Start is called before the first frame update
    void Start()
    {
        isCurrently = guardState.idle;
        speed = 20;
        //agent = gameObject.AddComponent<NavMeshAgent>();
        agent = GetComponent<NavMeshAgent>();
        //agent.nextPosition = transform.position;
        agent.speed = speed;
        patrolPoints = GameObject.FindGameObjectsWithTag("Patrol Point");
        nextPoint = patrolPoints[0];
        searchTime = 5;
        nextTurnTime = 0;
        turningSpeed = 90;
        turningDirection = 1;
        guardsY = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        
        if(positioned == 20)
        {
            transform.position = startingPosition;
            positioned++;
        }
        else
            positioned++;

        switch (isCurrently)
        {
            case guardState.patrolling:
                if (transform.position == agent.destination)
                {
                    nextPoint.SetActive(true);
                    SwitchToSearching();
                }
                break;
            case guardState.searching:
                {
                    transform.Rotate(Vector3.up, turningSpeed * Time.deltaTime * turningDirection); //direction indicates on which side he turns
                    if (totalSearchTimer > nextTurnTime)
                    {
                        turningDirection *= -1;
                        nextTurnTime = GetNextTurnTime(nextTurnTime);
                    }

                    totalSearchTimer += Time.deltaTime;
                    if (totalSearchTimer > searchTime)
                        Patrolling();
                }
                break;
        }
    }

    private void SwitchToSearching()
    {
        isCurrently = guardState.searching;

        totalSearchTimer = 0f;

        nextTurnTime = GetNextTurnTime(0);
    }

    private float GetNextTurnTime(float nextTurnTime)
    {
        return nextTurnTime + Random.Range(1f, 3f);
    }

    internal void Patrolling(Vector3 destination)
    {
        agent.SetDestination(new Vector3(destination.x, guardsY, destination.z));
    }

    internal void Patrolling()
    {
        if (isCurrently != guardState.patrolling)
        {
            nextPoint = patrolPoints[Random.Range(0, patrolPoints.Length)];
            if (!nextPoint.activeSelf)
                Patrolling();
            else
            {
                nextPoint.SetActive(false);
                agent.SetDestination(new Vector3(nextPoint.transform.position.x, guardsY, nextPoint.transform.position.z));
                isCurrently = guardState.patrolling;
            }
        }
    }

    internal void SetManager(ManagerScript managerScript)
    {
        myManager = managerScript;
    }

    internal void SetPosition(Vector3 position)
    {
        startingPosition = new Vector3(position.x, guardsY, position.z);
        //transform.position = new Vector3(position.x, 0f, position.z);
    }
}
