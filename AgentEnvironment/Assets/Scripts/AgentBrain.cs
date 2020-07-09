﻿using System.Collections;
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
      
    //Variables for values
    [HideInInspector]
    public float range;
    [HideInInspector]
    int numTargets;

    public float turnSpeed = 300;
    public float moveSpeed = 2;

 

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_TargetArea = TargetArea.GetComponent<TargetFinderArea>();
        range = m_TargetArea.range;
        numTargets = m_TargetArea.numTargets;
        //Add Environment Settings 
    }

    void Update() {
        
    }

    public override void OnEpisodeBegin()
    {
        m_TargetArea.ResetArea();
        m_AgentRb.velocity = Vector3.zero;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        MoveAgent(vectorAction);
        AddReward(-0.001f);
    }

    public void MoveAgent(float[] act)
    {

        float hAxis = act[0];
        float vAxis = act[1];
        
        Vector3 movement = new Vector3(hAxis, 0, vAxis) * moveSpeed * Time.deltaTime;

        m_AgentRb.MovePosition(transform.position + movement); 

    }

    public float[] retrieveTargetDistances(Vector3[] locations)
    {
        float[] nearestDistances = new float[3];
        float[] distances = new float[locations.Length];
        int index = 0; //bad for loop
        foreach(Vector3 location in locations)
        {
            if (location.x == 0 && location.z == 0) //means not a target
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

        return nearestDistances;
    }    
    
    public Vector3[] retrieveNearestTargets(Vector3[] locations, float[] distances)
    {

        Vector3[] nearestLocations = new Vector3[3];

        int index = 0; //bad for loop
        foreach (Vector3 location in locations)
        {

            for (int i = 0; i < 3; i++)
            {
                if (distances[i] ==  Vector3.Distance(transform.localPosition, location))
                {
                    //brute forced
                    nearestLocations[i] = locations[index];
                }
            }
            
            index++;
        }

        return nearestLocations;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);

        var currentAgentLocation = m_AgentRb.transform.localPosition;

        var targetLocations = m_TargetArea.RetrieveTargetLocations();
        var distancesAgent = retrieveTargetDistances(targetLocations);
        var nearestTargetLocations = retrieveNearestTargets(targetLocations, distancesAgent);

        foreach (float distance in distancesAgent)
        {
            sensor.AddObservation(distance);
        }

        foreach (Vector3 loc in nearestTargetLocations)
        {
            sensor.AddObservation(Vector3.Angle(currentAgentLocation, loc));
            Debug.Log(Vector3.Angle(currentAgentLocation, loc));
        }

        var agentLocations = m_TargetArea.RetrieveAgentLocations(); 
        /*
        foreach(Vector3 AgentLocation in agentLocations)
        {
            sensor.AddObservation(Vector3.Distance(currentAgentLocation, AgentLocation));
        }*/
        
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

}
