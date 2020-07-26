using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System;

public class targetSelector : Agent
{

    AgentBrain agentBody;
    TargetFinderArea m_TargetArea;
    public GameObject TargetArea;

    private int numTargets;

    public override void Initialize()
    {
        agentBody = GetComponent<AgentBrain>();
        m_TargetArea = TargetArea.GetComponent<TargetFinderArea>();

        numTargets = m_TargetArea.numTargets;
    }

    public override void OnActionReceived(float[] vectorAction)
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
