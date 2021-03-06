﻿using System.Collections;
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
    float fieldOfViewRadius;
    float fieldOfViewAngle;
    public LayerMask playerMask;
    public LayerMask obstacleMask;
    float timeToDraw;
    private float searchTime;
    private float turningSpeed;
    private float turningDirection;
    private float totalSearchTimer;
    private float totalDrawTimer;
    private float nextTurnTime;
    private Vector3 startingPosition;
    private int positioned = 0;
    private float guardsY;
    Animator guardAnimator;

    // Start is called before the first frame update
    void Start()
    {
        isCurrently = guardState.idle;
        speed = 20;
        fieldOfViewRadius = 20;
        fieldOfViewAngle = 90;
        timeToDraw = 2;
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
        guardAnimator = GetComponent<Animator>();
        guardAnimator.SetBool("isIdling", true);
        guardAnimator.SetBool("isWalking", false);
        guardAnimator.SetBool("isShooting", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (positioned == 20)
        {
            transform.position = startingPosition;
        }
        positioned++;

        switch (isCurrently)
        {
            case guardState.patrolling:
                {
                    if (DoISeePlayer())
                    {
                        Debug.Log("I See Player");
                        Shooting();
                    }
                    if (transform.position == agent.destination)
                    {
                        nextPoint.SetActive(true);
                        Searching();
                    }
                }
                break;

            case guardState.searching:
                {
                    if (DoISeePlayer())
                    {
                        Debug.Log("I See Player");
                        Shooting();
                    }
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

            case guardState.shooting:
                {
                    Vector3 dirToPlayer = (myManager.GetPlayerPosition() - transform.position).normalized;
                    transform.forward = new Vector3(dirToPlayer.x, 0, dirToPlayer.z);
                    if (DoISeePlayer())
                    {
                        totalDrawTimer += Time.deltaTime;
                        if (totalDrawTimer > timeToDraw)
                            Shoot();
                    }
                    else
                    {
                        agent.isStopped = false;
                        isCurrently = guardState.patrolling;
                        guardAnimator.SetBool("isIdling", false);
                        guardAnimator.SetBool("isWalking", true);
                        guardAnimator.SetBool("isShooting", false);
                    }
                }
                break;
        }
    }

    private void Searching()
    {
        isCurrently = guardState.searching;
        guardAnimator.SetBool("isIdling", true);
        guardAnimator.SetBool("isWalking", false);
        guardAnimator.SetBool("isShooting", false);
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
                guardAnimator.SetBool("isIdling", false);
                guardAnimator.SetBool("isWalking", true);
                guardAnimator.SetBool("isShooting", false);
            }
        }
    }

    private bool DoISeePlayer()
    {
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, fieldOfViewRadius, playerMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < fieldOfViewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    return true;
                }
            }
        }
        return false;
    }

    internal void Shooting()
    {
        isCurrently = guardState.shooting;
        guardAnimator.SetBool("isIdling", false);
        guardAnimator.SetBool("isWalking", false);
        guardAnimator.SetBool("isShooting", true);
        agent.isStopped = true;
        agent.velocity = new Vector3(0, 0, 0);
        totalDrawTimer = 0f;
    }

    private void Shoot()
    {
        myManager.PlayerKilled();
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
