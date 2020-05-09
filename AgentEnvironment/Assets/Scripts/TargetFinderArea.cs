using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinderArea : MonoBehaviour
{
    public GameObject targets;
    public int numTargets;
    public float range;

    GameObject[] agents;


    
    public void CreateTarget(int num, GameObject target)
    {

        for (int i = 0; i < num; i++)
        {
            GameObject t = Instantiate(target,new Vector3(Random.Range(-range, range)
                                                                    , gameObject.transform.position.y
                                                                    ,Random.Range(-range,range))
                                            ,Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            Debug.Log("Object instantiated at" + t.transform.localPosition);
        }
 
 
    }

    public void ClearObjects(GameObject[] objects)
    {

        foreach (var obj in objects)
        {
            //if (obj.transform.position.x > gameObject.transform.position.x - 50 && obj.transform.position.x < gameObject.transform.position.x + 50 
              //  && obj.transform.position.z > gameObject.transform.position.z - 50 && obj.transform.position.z < gameObject.transform.position.z + 50)
                Destroy(obj);
            // does not include y; cannot stack training areas vertically
        }
    }

    public void ResetTargetArea(GameObject[] agents)
    {
        ClearObjects(GameObject.FindGameObjectsWithTag("target"));
        foreach(GameObject agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                agent.transform.localPosition = new Vector3(Random.Range(-range, range), 2f,
                    Random.Range(-range, range))
                    + transform.localPosition;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }

        CreateTarget(numTargets, targets);
    }
    
    public void ResetEnvironment()
    {

        agents = GameObject.FindGameObjectsWithTag("agent");
        ResetTargetArea(agents);
        
    }

}
