using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinderArea : MonoBehaviour
{
    public GameObject target;
    public int numTargets;
    public int numAgents;
    public float range;

    public GameObject agent;
    public GameObject agent2;
    public GameObject agent3;

    public int score = 0;
    // can change for mutliple agents later; testing first
    private List<GameObject> agentsList;
    private List<GameObject> targetsList;

    public List<GameObject> TargetsList { get { return targetsList;  } }
    public List<GameObject> AgentsList { get { return agentsList;  } }


    public void Update()
    {
        Debug.Log("Score is " + score);
        if(score >= numTargets)
        {
            EndAllEpisodes();
            ResetArea();
        }
    }

    public void EndAllEpisodes()
    {
        score = 0;
        List<GameObject> spawnList = new List<GameObject>();
        spawnList.Add(agent);
        spawnList.Add(agent2);
        spawnList.Add(agent3);

        foreach (GameObject element in spawnList)
        {
            AgentBrain agent = element.GetComponent<AgentBrain>();
            agent.EndEpisode();
        }
    }

    public void RemoveSpecificTarget(GameObject targetObject)
    {
        targetsList.Remove(targetObject);
        Destroy(targetObject);
    }    
    
    public void RemoveAllTargets()
    {
        if(targetsList != null)
        {
            for(int i = 0; i < targetsList.Count; i++){
                if(targetsList[i] != null){
                    Destroy(targetsList[i]);
                }
            }
        }

        targetsList = new List<GameObject>();
    }

    public static Vector3 GenerateNewPosition(Vector3 center, float range)
    {
        Vector3 newPosition = center;
        newPosition.x += UnityEngine.Random.Range(-range,range);
        newPosition.y = 1f;
        newPosition.z += UnityEngine.Random.Range(-range,range);
        return newPosition;
    }

    public void SpawnTarget(int num, GameObject target)
    {

        for (int i = 0; i < num; i++)
        {
            GameObject t = Instantiate<GameObject>(target.gameObject);
            t.transform.position = GenerateNewPosition(transform.position,range);
            t.transform.rotation = Quaternion.Euler(0f,0f,0f);
            
            t.transform.SetParent(transform);

            targetsList.Add(t);

            
        }
 
 
    }

    public void RespawnAgent()
    {/*
        for (int i = 0; i < num; i++)
        {
            GameObject newAgent = Instantiate<GameObject>(agents.gameObject);

            agentsList.Add(newAgent);
        } */

        List <GameObject> spawnList = new List<GameObject>();
        spawnList.Add(agent);
        spawnList.Add(agent2);
        spawnList.Add(agent3);


        foreach (GameObject element in spawnList) {
            Rigidbody rigidbody = element.GetComponent<Rigidbody>();


            rigidbody.velocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
            element.transform.position = GenerateNewPosition(transform.position, range);
            element.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 180f), 0f);

        }
    }


    public void ResetArea()
    {
        RemoveAllTargets();
        RespawnAgent();
        SpawnTarget(numTargets,target);
    }
    private void Start()
    {
        ResetArea();
    }
}
