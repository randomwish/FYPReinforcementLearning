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

    public int score = 0;
    private List<GameObject> agentsList;
    private List<GameObject> targetsList;
    private List<GameObject> obstacleList;
    private float[,] objLocations;

    public List<GameObject> TargetsList { get { return targetsList;  } }
    public List<GameObject> AgentsList { get { return agentsList; } set { agentsList = AgentsList; } }




    public void Update()
    {
        //Debug.Log("Score is " + score);
        if(score >= numTargets)
        {
            //EndAllEpisodes();
            ResetArea();
        }
    }

    public void EndAllEpisodes()
    {
        //score = 0;
        //RemoveAllAgents();
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

    public Vector3 GenerateNewPosition()
    {
        return GenerateNewPosition(transform.position, range);
    }

    public Vector3 GeneratePositionOffset(Vector3 offset)
    {
        Vector3 newPosition = gameObject.transform.position;
        newPosition += offset;
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

    public GameObject[] RetrieveTargetObjects()
    {
        GameObject[] objects = new GameObject[numTargets];
        foreach (GameObject Target in objects)
        {
            for (int idx = 0; idx < targetsList.Count; idx++)
            {
                if (Target.gameObject.tag == "target")
                {
                    objects[idx] = Target;
                }
                else
                {
                    objects[idx] = null;
                }
            }


        }
        return objects;
    }

    public Vector3[] RetrieveAgentLocations()
    {
        Vector3[] locations = new Vector3[AgentsList.Count];
        int idx = 0;
        foreach (GameObject Agent in AgentsList)
        {
            locations[idx] = Agent.gameObject.transform.localPosition;
            idx++;
        }
        return locations;
    }

    public int getZone(Vector3 currentLoc)
    {
        int zone = 0;

        /*
         * -------------------
         * |        |        |
         * |   0    |    1   |
         * |        |        |
         * |--------|--------|
         * |        |        |
         * |   2    |   3    |
         * |        |        |
         * -------------------
         */
        if (currentLoc.x < 0 && currentLoc.z > 0)
            zone = 0;
        else if (currentLoc.x > 0 && currentLoc.z > 0)
            zone = 1;
        else if (currentLoc.x < 0 && currentLoc.z < 0)
            zone = 2;
        else if (currentLoc.x > 0 && currentLoc.z < 0)
            zone = 3;

        return zone;
    }

    public void ResetArea()
    {
        score = 0;

        Total = (int)range / Step * 2 - 1;
        generateLocations();
        RemoveAllTargets();
        RemoveAllObstacles();
        SpawnTarget(numTargets,target);
        SpawnObstacle(numObstacles, obstacle);

    }

    public void getAgentList()
    {
        agentsList = new List<GameObject>();

        Collider[] allObjects = Physics.OverlapBox(gameObject.transform.position, transform.localScale / 2);
        int idx = 0;
        foreach (Collider col in allObjects)
        {
            if (col.gameObject.tag == "agent")
            {
                col.gameObject.SendMessage("tagAgent", idx);
                AgentsList.Add(col.gameObject);
                idx++;
            }
        }
    }

    void Start()
    {
        ResetArea();
        getAgentList();
    }
}
