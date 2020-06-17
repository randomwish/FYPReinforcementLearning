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
        MinimumDistance = 999f;
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

        Vector3 movemvent = new Vector3(hAxis, 0, vAxis) * moveSpeed * Time.deltaTime;

        m_AgentRb.MovePosition(transform.position + movemvent);
    }
    float MinimumDistance = 999f; //GLOBAL variable for ReturnClosest Target
    public float ReturnClosestTarget(Vector3[] TargetLocations, Vector3 AgentLocation)
    {
        // ReturnClosestTarget returns a float for the distance between the agent and the target which is closest to it.
        // If there are no agents detected, a value of 0 is returned.
        // PARAMETERS: Vector3[] TargetLocations, represents the locations of the various targets (Vector3 datatype)
        //             Vector3 AgentLocation, represents the location of the agent (Vector3 datatype)

        // RETURNS: float MinimumDistane, represents MINIMUM distance between target and agent

        
        foreach(Vector3 location in TargetLocations)
        {
            float distance = Vector3.Distance(AgentLocation, location);
            if(location.x  == 0 && location.y == 0) //NO targes present at the index
            {
                continue; 
            }
            if(distance < MinimumDistance)
            {
                MinimumDistance = distance;
            }
        }

        return MinimumDistance;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
       sensor.AddObservation(localVelocity.x);
       sensor.AddObservation(localVelocity.z);
       sensor.AddObservation(ReturnClosestTarget(m_TargetArea.RetrieveLocations(),m_AgentRb.transform.localPosition));
    }

    public override void Heuristic(float[] actionsOut)
    {
        /*actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical"); */
        //Debug.Log("Keys pressed");
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
