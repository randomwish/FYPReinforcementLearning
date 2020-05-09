using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class TargetFinderSettings : MonoBehaviour
{ /*
    [HideInInspector]
    public GameObject[] agents;
    [HideInInspector]
    public TargetFinderArea[] listArea;

    public int totalScore;
    StatsRecorder m_Recorder;

    public void Awake()
    {
        Academy.Instance.OnEnvironmentReset += EnvironmentReset;
        m_Recorder = Academy.Instance.StatsRecorder;
        Debug.Log("Settings awoken");
    }

    void EnvironmentReset()
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("target"));

        agents = GameObject.FindGameObjectsWithTag("agent");

        listArea = FindObjectsOfType<TargetFinderArea>();

        foreach(var fa in listArea)
        {
            fa.ResetTargetArea(agents);
        }
        totalScore = 0;
    }

    void ClearObjects(GameObject[] objects)
    {
        foreach(var obj in objects)
        {
            Destroy(obj);
        }
    }

    public void Update()
    {
        Debug.Log("Score is" + totalScore);
        if((Time.frameCount % 100) == 0)
        {
            m_Recorder.Add("TotalScore", totalScore);
        }
    }*/
}
