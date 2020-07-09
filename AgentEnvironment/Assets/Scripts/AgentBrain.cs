using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

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
         
//        Debug.Log("Current Score is " + count);

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

    public override void CollectObservations(VectorSensor sensor)
    {
        var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
        sensor.AddObservation(localVelocity.x);
        sensor.AddObservation(localVelocity.z);

        var currentAgentLocation = m_AgentRb.transform.localPosition;
        sensor.AddObservation(currentAgentLocation);
        var targetLocations = m_TargetArea.RetrieveTargetLocations();
        foreach(Vector3 location in targetLocations)
        {
            //sensor.AddObservation(Vector3.Distance(currentAgentLocation, location));
            sensor.AddObservation(location);
        }
        var agentLocations = m_TargetArea.RetrieveAgentLocations();
        /*
        foreach (Vector3 AgentLocation in agentLocations)
        {
            sensor.AddObservation(Vector3.Distance(currentAgentLocation, AgentLocation));
        }*/
        
    }

    public Vector3 RetrieveSingleAgentLocation()
    {
        Vector3 location = new Vector3();

        location = gameObject.transform.localPosition;

        return location;
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
