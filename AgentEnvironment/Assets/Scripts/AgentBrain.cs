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
    Material m_Material;
    int stepCounter = 0;

    public float turnSpeed = 300;
    public float moveSpeed = 2;

 

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_TargetArea = TargetArea.GetComponent<TargetFinderArea>();
        range = m_TargetArea.range;
        numTargets = m_TargetArea.numTargets;
        m_Material = GetComponent<Renderer>().material;
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
            
            AddReward(-1f);
        } 
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("zone"))
        {
            int oldScore = m_TargetArea.score;
            other.transform.position += Vector3.down * 5;
            AddReward(0.5f);
            //Destroy(other.gameObject);

            //honestly, super hacky way to get this done. possibly update in the future

            if (oldScore <= m_TargetArea.score)
            {
                AddReward(2f);
                Debug.Log("Target fully searched");
            }
                
                
            
        } 
    }

}
