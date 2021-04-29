using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ManagerScript : MonoBehaviour
{
    List<GuardScript> guards;
    TargetScript target;
    public GameObject guardTemplate;
    public GameObject targetTemplate;
    GameObject[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
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
        if(Input.GetKeyDown(KeyCode.Space))
        {
            guards[0].Patrolling();
            guards[1].Patrolling();
            guards[2].Patrolling();
            guards[3].Patrolling();
        }
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
}
