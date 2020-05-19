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
    //public int count = 0;
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
        /*Debug.Log("Current Time Step: "+ Time.time);
        Debug.Log("Position of agent is " + transform.localPosition);    
        Debug.Log("Current Score is " + count);*/

    }
    public override void OnEpisodeBegin()
    {
        m_TargetArea.ResetArea();
        m_AgentRb.velocity = Vector3.zero;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        MoveAgent(vectorAction);
        AddReward(-0.0001f);

        foreach (GameObject element in m_TargetArea.TargetsList)
        {
            float targDist = Vector3.Distance(this.transform.localPosition, element.transform.localPosition);
            if (targDist < 5f)
            {
                m_TargetArea.Score++;
                DestroyTarget(element.gameObject);
                }
        }

        if (m_TargetArea.Score == m_TargetArea.numTargets)
        {
            m_TargetArea.Score = 0;
            EndEpisode();
        }
             
    }

    public void MoveAgent(float[] act)
    {
        //Action Space of 2, namely, forward/backward motion and rotation
        /* Vector3 controlSignal = Vector3.zero;
        controlSignal.x = act[0];
        controlSignal.z = act[1];
        m_AgentRb.AddForce(controlSignal * moveSpeed); */

        float hAxis = act[0];
        float vAxis = act[1];

        Vector3 movemvent = new Vector3(hAxis, 0, vAxis) * moveSpeed * Time.deltaTime;

        m_AgentRb.MovePosition(transform.position + movemvent);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
       sensor.AddObservation(localVelocity.x);
       sensor.AddObservation(localVelocity.z); 
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
    {/*
        if(other.transform.CompareTag("target"))
        {
            DestroyTarget(other.gameObject);
        } */
    }

    void DestroyTarget(GameObject targetObject)
    {
        m_TargetArea.RemoveSpecificTarget(targetObject);
        AddReward(1f);
    }

}
