using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class AgentBrain : Agent
{
    public GameObject TargetArea;
    TargetFinderArea m_TargetArea;
    TargetFinderSettings m_TargetFinderSettings;

    Rigidbody m_AgentRb;
    
    public float turnSpeed = 300;
    public float moveSpeed = 2;
    
    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_TargetArea = TargetArea.GetComponent<TargetFinderArea>();
        m_TargetFinderSettings = FindObjectOfType<TargetFinderSettings>();
    }


    public override void OnEpisodeBegin()
    {
        m_AgentRb.velocity = Vector3.zero;
        transform.position = new Vector3(Random.Range(-m_TargetArea.range,m_TargetArea.range),1f,
                                        Random.Range(-m_TargetArea.range,m_TargetArea.range)) + TargetArea.transform.position;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));


    }

    public override void OnActionReceived(float[] vectorAction)
    {
        MoveAgent(vectorAction);
    }

    public void MoveAgent(float[] act)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        //Action Space of 2, namely, forward/backward motion and rotation
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = act[0];
        controlSignal.z = act[1];
        m_AgentRb.AddForce(controlSignal * moveSpeed,ForceMode.VelocityChange);
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
        if(other.gameObject.CompareTag("target"))
        {
            other.gameObject.GetComponent<ObjectLogic>().OnFound();
            AddReward(1f);
            m_TargetFinderSettings.totalScore++;
        }
    }

}
