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
    
    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        m_TargetArea = TargetArea.GetComponent<TargetFinderArea>();
        //Add Environment Settings and Reset Params
    }

    public override void OnEpisodeBegin()
    {

    }

    public override void OnActionReceived(float[] vectorAction)
    {
        MoveAgent(vectorAction);
    }
    public void MoveAgent(float[] act)
    {

    }
    public override void CollectObservations(VectorSensor sensor)
    {

    }

    public override void Heuristic(float[] actionsOut)
    {

    }

    void OnCollisionEnter(Collision other) 
    {

    }

}
