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
    public int startZone;

    private bool randomSpawn = false;
      
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

        respawn();
    }

    void Update() {

        //        Debug.Log("Current Score is " + count);
        if (m_TargetArea.score <= m_TargetArea.numTargets)
            respawn();

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
        if (m_TargetArea.getZone(gameObject.transform.localPosition) != startZone)
            AddReward(-0.002f);
        if (m_TargetArea.score >= m_TargetArea.numTargets)
            respawn();
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

        //not sure if this will work, possible to do a bool for if correct zone instead
        sensor.AddOneHotObservation(startZone, 4);
        sensor.AddOneHotObservation(m_TargetArea.getZone(transform.localPosition), 4);

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

    void respawn() 
    {       
        m_AgentRb.velocity = Vector3.zero;
        m_AgentRb.angularVelocity = Vector3.zero;

        if (randomSpawn)
        {
            gameObject.transform.position = m_TargetArea.GenerateNewPosition();
            gameObject.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 180f), 0f);
        }
        else
        { 
            Vector3 newPows = new Vector3();

            if (startZone < 2)
                newPows.z = 25;
            else
                newPows.z = -25;
            if (startZone % 2 == 1)
                newPows.x = 25;
            else
                newPows.x = -25;

            gameObject.transform.position = m_TargetArea.GeneratePositionOffset(newPows);

        }

        m_TargetArea.score = 0;
        EndEpisode();
    }

}
