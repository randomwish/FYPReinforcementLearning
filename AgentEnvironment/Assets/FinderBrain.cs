using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class FinderBrain : Agent
{
    public GameObject TargetArea;
    TargetFinderArea m_TargetArea;

    Rigidbody m_AgentRb;
    
    public float turnSpeed = 300;
    public float moveSpeed = 2;

    public Transform Target;
    
    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_TargetArea = TargetArea.GetComponent<TargetFinderArea>();
        //Add Environment Settings 
        Target.localPosition = new Vector3(Random.Range(-m_TargetArea.range, m_TargetArea.range), 0.5f,
                                Random.Range(-m_TargetArea.range, m_TargetArea.range));

    }

    public override void OnEpisodeBegin()
    {
        m_AgentRb.velocity = Vector3.zero;
        transform.position = new Vector3(Random.Range(-m_TargetArea.range,m_TargetArea.range),0.5f,
                                        Random.Range(-m_TargetArea.range,m_TargetArea.range));

        Target.localPosition = new Vector3(Random.Range(-m_TargetArea.range, m_TargetArea.range), 0.5f,
                                        Random.Range(-m_TargetArea.range, m_TargetArea.range));
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        MoveAgent(vectorAction);

        float distanceToTarget = Vector3.Distance(this.transform.localPosition, Target.localPosition);
        // Reached target
        if (distanceToTarget < 1.42f)
        {
            SetReward(1.0f);
            EndEpisode();
        }
    }
    public void MoveAgent(float[] act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        //Action Space of 2, namely, forward/backward motion and rotation
        var forwardAxis = (int)act[0];
        var rotateAxis = (int)act[1];
        
        switch(forwardAxis)
        {
            case 1:
                dirToGo = transform.forward;
                break;
            
            case 2:
                dirToGo = -transform.forward;
                break;
        }

        switch(rotateAxis)
        {
            case 1:
                rotateDir = -transform.up;
                break;
            case 2:
                rotateDir = transform.up;
                break;

        }
        m_AgentRb.AddForce(dirToGo * moveSpeed,ForceMode.VelocityChange);
        transform.Rotate(rotateDir, Time.deltaTime * turnSpeed);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
       var localVelocity = transform.InverseTransformDirection(m_AgentRb.velocity);
       sensor.AddObservation(localVelocity.x);
       sensor.AddObservation(localVelocity.z);
    }

    public override void Heuristic(float[] actionsOut)
    {

        if (Input.GetKey(KeyCode.W))
        {
            actionsOut[0] = 1f;
        }
        if(Input.GetKey(KeyCode.S))
        {
            actionsOut[0] = 2f;
        }
        if(Input.GetKey(KeyCode.D))
        {
            actionsOut[1] = 2f;
        }
        if(Input.GetKey(KeyCode.S))
        {
            actionsOut[1] = 1f;
        } 
    }

    void OnCollisionEnter(Collision other) 
    {

    }

}
