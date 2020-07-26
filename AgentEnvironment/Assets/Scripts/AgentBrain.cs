using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

public class AgentBrain : Agent
{

    //Variables for relevent gameobjects
    Rigidbody m_AgentRb;
    TargetFinderArea m_TargetArea;
    public GameObject targets;
    public GameObject TargetArea;
    private int agentTag;

    private bool randomSpawn = false;

    //Variables for values
    [HideInInspector]
    public float range;
    [HideInInspector]
    int numTargets;
    int oldScore = 0;
    float hypotenuse;

    int internalScore; //To keep track of agent's own score

    public float turnSpeed = 300;
    public float moveSpeed = 2;



    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_TargetArea = TargetArea.GetComponent<TargetFinderArea>();
        range = m_TargetArea.range;
        numTargets = m_TargetArea.numTargets;
        //Add Environment Settings

        hypotenuse = Mathf.Sqrt(2f * (2 * Mathf.Pow(m_TargetArea.range, 2f)));

        //Add Environment Settings

        respawn();
    }

    void Update() {

        //        Debug.Log("Current Score is " + count);

        if (m_TargetArea.score >= m_TargetArea.numTargets || oldScore > m_TargetArea.score)
        {
            EndEpisode();
        }
        oldScore = m_TargetArea.score;
    }

    public override void OnEpisodeBegin()
    {
        m_TargetArea.ResetArea();
        m_AgentRb.velocity = Vector3.zero;
        respawn();
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        MoveAgent(vectorAction);
        AddReward(-0.0005f);
    }

    public void MoveAgent(float[] act)
    {
        //-2 for discrete action space (-1, 0, 1) from (1, 2, 3)
        float hAxis = act[0];
        float vAxis = act[1];

        Vector3 movement = new Vector3(hAxis, 0, vAxis) * moveSpeed * Time.deltaTime;

        m_AgentRb.MovePosition(transform.position + movement);
    }

    public float[] retrieveDistances(Vector3[] locations, int option)
    {
        //Option represents which object to retrieve distance: 0 means agent, 1 means target

        float[] nearestDistances = new float[3];
        float[] distances = new float[locations.Length];
        int index = 0; //bad for loop
        foreach(Vector3 location in locations)
        {
            if (location.x == 0 && location.z == 0 && option == 1) //means not a target
            {
                distances[index] = 0f;
            }
            else
            {
                distances[index] = Vector3.Distance(transform.localPosition, location);
            }
            index++;
        }

        Array.Sort(distances);
        nearestDistances = distances.Take(3).ToArray();

        return distances;
    }


    public Vector3[] retrieveNearestTargets(Vector3[] locations, float[] distances)
    {

        Vector3[] nearestLocations = new Vector3[1];

        int index = 0; //bad for loop
        foreach (Vector3 location in locations)
        {

            for (int i = 0; i < 1; i++)
            {
                if (distances[i] <=  Vector3.Distance(transform.localPosition, location) + 2.5f
                    && distances[i] >= Vector3.Distance(transform.localPosition,location) - 2.5f)
                {
                    //brute forced
                    nearestLocations[i] = locations[index];
                }
            }

        }

        return nearestLocations;
    }

    float normalizer(float value, float minimum, float maximum)
    {
        return (float)((value - minimum) / (maximum - minimum));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
         var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);

        var currentAgentLocation = m_AgentRb.transform.localPosition;

        var targetLocations = m_TargetArea.RetrieveTargetLocations();
        var targetDistance = retrieveDistances(targetLocations, 1);

        var agentLocations = m_TargetArea.RetrieveAgentLocations();
        var agentDistance = retrieveDistances(agentLocations, 0);

        float hypotenuse = Mathf.Sqrt(2f * (2 * Mathf.Pow(m_TargetArea.range, 2f)));

        //add distance between targets amd agent
        foreach (float distance in targetDistance)
        {
            sensor.AddObservation(normalizer(distance, 0f, hypotenuse));
        }

        //add angle between targets and agent
        foreach (Vector3 loc in targetLocations)
        {
            sensor.AddObservation(normalizer(Vector3.Angle(currentAgentLocation, loc), 0f, 180f));
        }

        //add x and z coordinates of agent locations, with itself being first
        foreach (Vector3 loc in prepAgentLocations())
        {
            sensor.AddObservation(normalizer(loc.x, -50, 50));
            sensor.AddObservation(normalizer(loc.z, -50, 50));
        }
    }

    public void obsZones(VectorSensor sensor)
    {
        //observe all agent's zones
        foreach (int zone in getAgentZones())
        {
            sensor.AddOneHotObservation(zone, 4);
        }
    }

    public override void Heuristic(float[] actionsOut)
    {

        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    void OnCollisionEnter(Collision other)
    {
        if(other.transform.CompareTag("target"))
        {
            //Destroy(other.gameObject);
            m_TargetArea.score += 1;
            AddReward(1f);
        }

    }

    void tagAgent(int tag)
    {
        agentTag = tag;
    }

    public int[] getAgentZones()
    {
        int[] zone = new int[m_TargetArea.numAgents];

        int idx = 0;
        foreach(Vector3 loc in prepAgentLocations())
        {
            zone[idx] = m_TargetArea.getZone(loc);
            idx++;
        }
        return zone;
    }

    public Vector3[] prepAgentLocations()
    {
        Vector3[] locations = new Vector3[m_TargetArea.numAgents];

        //ensure that own agent's postition is always first in array
        locations[0] = m_AgentRb.transform.localPosition;

        int idx = 1;
        foreach (GameObject agent in m_TargetArea.AgentsList)
        {
            if (agent.GetComponent<AgentBrain>().agentTag != agentTag)
            {
                locations[idx] = agent.transform.localPosition;
                idx++;
            }
        }
        return locations;
    }

    void respawn()
    {
        m_AgentRb.velocity = Vector3.zero;
        m_AgentRb.angularVelocity = Vector3.zero;
        gameObject.transform.position = m_TargetArea.GenerateNewPosition();
        gameObject.transform.localPosition = new Vector3(0,0.5f,0);
        gameObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 180f), 0f);
    }
}
