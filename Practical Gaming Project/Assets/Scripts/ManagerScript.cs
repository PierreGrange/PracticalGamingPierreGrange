using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManagerScript : MonoBehaviour
{
    PlayerControlScript player;
    List<GuardScript> guards;
    TargetScript target;
    public GameObject playerTemplate;
    public GameObject guardTemplate;
    public GameObject targetTemplate;
    GameObject[] spawnPoints;

    int gameStart;

    // Start is called before the first frame update
    void Start()
    {
        gameStart = 0;
        player = SpawnPlayer(new Vector3(0f, 0.1f, 0f));

        guards = new List<GuardScript>();
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
        for (int i = 0; i < 4; i++)
        {
            guards.Add(SpawnGuard(spawnPoints[i]));
        }

        target = SpawnTarget(spawnPoints[4]);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) & gameStart == 0)
        {
            gameStart = 1;
        }

        if (gameStart == 1)
        {
            guards[0].Patrolling();
            guards[1].Patrolling();
            guards[2].Patrolling();
            guards[3].Patrolling();
            gameStart = 2;
        }
    }

    private PlayerControlScript SpawnPlayer(Vector3 spawnPoint)
    {
        GameObject newPlayer = Instantiate(playerTemplate);

        PlayerControlScript newPlayerScript = newPlayer.GetComponent<PlayerControlScript>();
        newPlayerScript.SetManager(this);
        newPlayerScript.SetSpawnPosition(spawnPoint);
        return newPlayerScript;
    }

    private GuardScript SpawnGuard(GameObject spawnPoint)
    {
        GameObject newGuard = Instantiate(guardTemplate);

        GuardScript newGuardScript = newGuard.GetComponent<GuardScript>();
        newGuardScript.SetManager(this);
        newGuardScript.SetPosition(spawnPoint.transform.position);
        return newGuardScript;
    }

    private TargetScript SpawnTarget(GameObject spawnPoint)
    {
        GameObject newTarget = Instantiate(targetTemplate);

        TargetScript newTargetScript = newTarget.GetComponent<TargetScript>();
        newTargetScript.SetManager(this);
        newTargetScript.SetPosition(transform.TransformPoint(spawnPoint.transform.position));
        return newTargetScript;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    internal void TargetKilled()
    {
        target.KillTarget();
        Debug.Log("Target Killed. Game won! :D");
    }

    internal void PlayerKilled()
    {
        player.KillPlayer();
        Debug.Log("Player Killed. Game lost! :'(");
    }
}
