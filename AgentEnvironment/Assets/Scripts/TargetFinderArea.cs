using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinderArea : MonoBehaviour
{
    public GameObject target;
    public GameObject obstacle;
    public int numTargets;
    public int numAgents;
    public float range;
    public int rotationStep;
    public int numObstacles;
    private int Step = 10;
    private int Total; //this is hard coded :( 

    public GameObject agent;
    public GameObject agent2;
    public GameObject agent3;

    public int score = 0;
    // can change for mutliple agents later; testing first
    private List<GameObject> agentsList;
    private List<GameObject> targetsList;
    private List<GameObject> obstacleList;
    private float[,] objLocations;

    public List<GameObject> TargetsList { get { return targetsList;  } }
    public List<GameObject> AgentsList { get { return agentsList;  } }

    


    public void Update()
    {
        //Debug.Log("Score is " + score);
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
        List<GameObject> obstacleList = new List<GameObject>();
        List<Vector3> objLocations = new List<Vector3>();
        spawnList.Add(agent);
        spawnList.Add(agent2);
        spawnList.Add(agent3);

        foreach (GameObject element in spawnList)
        {
            AgentBrain agent = element.GetComponent<AgentBrain>();
            agent.EndEpisode();
        }
    }

    private void generateLocations()
    {
        int total = Total;

        objLocations = new float[total,total];

        for (int i=0; i < total; i++)
        {
            for (int j=0; j < total; j++)
            {
                objLocations[i, j] = 1;
            }
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

    public void RemoveAllObstacles()
    {
        if (obstacleList != null)
        {
            for (int i = 0; i < obstacleList.Count; i++)
            {
                if (obstacleList[i] != null)
                {
                    Destroy(obstacleList[i]);
                }
            }
        }
        obstacleList = new List<GameObject>();
    }

    public static Vector3 GenerateNewPosition(Vector3 center, float range)
    {
        Vector3 newPosition = center;
        newPosition.x += UnityEngine.Random.Range(-range,range);
        newPosition.y = 1f;
        newPosition.z += UnityEngine.Random.Range(-range,range);    
        return newPosition;
    }

    public Vector3 GenerateNewPosition (Vector3 center)
    {
        Vector3 newPosition = center;
        int[] newRand = checkPosition();
        int newRandX = newRand[0];
        int newRandZ = newRand[1];

        newPosition.x += newRandX * Step;
        newPosition.y = 1f;
        newPosition.z += newRandZ * Step;
        return newPosition;
    }

    public int generateRotation(int step)
    {
        int rotate = 360 / step;
        step *= UnityEngine.Random.Range(0, rotate);

        return step;
    }

    public Vector3 generateRotationOffset(float rotate, float length)
    {
        Vector3 offset = new Vector3();

        float angle = rotate * Mathf.PI / 180f;

        offset.x = Mathf.Sin(angle) * length;
        offset.z = Mathf.Cos(angle) * length;

        return offset;
    }

    public int[] checkPosition()
    {
        int total = Total; 
        int randX = UnityEngine.Random.Range(0, total);
        int randZ = UnityEngine.Random.Range(0, total);
        return checkPosition(total, randX, randZ);
    }

    public int[] checkPosition(int total, int randX, int randZ)
    {
        if (objLocations[randX, randZ] == -1)
        {
            randX = UnityEngine.Random.Range(0, total);
            randZ = UnityEngine.Random.Range(0, total);
            checkPosition(total, randX, randZ);
        }

        objLocations[randX, randZ] = -1;

        int[] result = new int[2];
        result[0] = randX - total/2;
        result[1] = randZ - total/2;
        return result;
    }
    
    public void SpawnTarget(int num, GameObject target)
    {
        for (int i = 0; i < num; i++)
        {
            GameObject t = Instantiate<GameObject>(target.gameObject);
            t.transform.position = GenerateNewPosition(transform.position);

            float rotate = generateRotation(rotationStep);
            t.transform.rotation = Quaternion.Euler(0f, rotate, 0f);
            t.transform.position += generateRotationOffset(rotate, 5); //assuming longest side of object is 5
            
            t.transform.SetParent(transform);

            targetsList.Add(t);
        } 
    }

    public void SpawnObstacle(int num, GameObject target)
    {

        for (int i = 0; i < num; i++)
        {
            GameObject t = Instantiate<GameObject>(target.gameObject);
            t.transform.position = GenerateNewPosition(transform.position);

            float rotate = generateRotation(rotationStep);
            t.transform.rotation = Quaternion.Euler(0f, rotate, 0f);
            t.transform.position += generateRotationOffset(rotate, 5); //assuming longest side of object is 5

            t.transform.SetParent(transform);

            obstacleList.Add(t);


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

    public Vector3[] RetrieveTargetLocations()
    {
        Vector3[] locations = new Vector3[numTargets];
        foreach (GameObject Target in targetsList)
        {
            for(int idx = 0; idx < targetsList.Count; idx++)
            {
                if (Target.gameObject.tag == "target")
                {
                    locations[idx] = Target.gameObject.transform.localPosition;
                }
                else
                {
                    locations[idx] = Vector3.zero;
                }
            }
     

        }
        return locations;
    }

    public Vector3[] RetrieveAgentLocations()
    {
        Vector3[] locations = new Vector3[numAgents];
        foreach (GameObject Agent in AgentsList)
        {
            for (int idx = 0; idx < AgentsList.Count; idx++)
            {
                locations[idx] = Agent.gameObject.transform.localPosition;
            }
        }
        return locations;
    }

    public void ResetArea()
    {
        Total = (int)range / Step * 2 - 1;
        generateLocations();
        RemoveAllTargets();
        RemoveAllObstacles();
        RespawnAgent();
        SpawnTarget(numTargets,target);
        SpawnObstacle(numObstacles, obstacle);
    }
    private void Start()
    {
        //Destroy(target);
        //not sure why the thing causes bugs when deleted, likely due to the same frame bug i think. anyways, putting it below the playfeild
        //for now. hacky solution but works
        ResetArea();
    }
}
