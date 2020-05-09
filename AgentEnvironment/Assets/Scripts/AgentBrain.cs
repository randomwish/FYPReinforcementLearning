using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;

public class AgentBrain : Agent
{
    public GameObject TargetArea;
    TargetFinderArea m_TargetArea;
    public GameObject targets;
    public int numTargets;
    public float range;

    Rigidbody m_AgentRb;

    public int count = 0;
    
    public float turnSpeed = 300;
    public float moveSpeed = 2;

    public TargetFinderSettings targetFinderSettings;
    public TargetFinderArea targetFinderArea;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_TargetArea = TargetArea.GetComponent<TargetFinderArea>();
        //Add Environment Settings 
    }

    void Update() {
        Debug.Log("Position of agent is " + transform.localPosition);    

    }
    public override void OnEpisodeBegin()
    {
        m_AgentRb.velocity = Vector3.zero;

        transform.localPosition = new Vector3(Random.Range(-m_TargetArea.range,m_TargetArea.range),0.5f,
                                        Random.Range(-m_TargetArea.range,m_TargetArea.range));
        gameObject.transform.position = new Vector3(Random.Range(-range, range), 0.5f,
                                                    Random.Range(-range, range))
                                                    + transform.position;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
        
        m_TargetArea.CreateTarget(numTargets, targets);
        

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        MoveAgent(vectorAction);
        AddReward(-0.01f);

        if (count >= numTargets)
        { 
            //can housekeep to not hardcode this but later
            count = 0;
            
            EndEpisode();
        }
             
    }

    public void MoveAgent(float[] act)
    {
        Vector3 dirToGo = Vector3.zero;
        Vector3 rotateDir = Vector3.zero;

        //Action Space of 2, namely, forward/backward motion and rotation
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = act[0];
        controlSignal.z = act[1];
        m_AgentRb.AddForce(controlSignal * moveSpeed);
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
            count++;
        }
    }

}
