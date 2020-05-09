using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinderArea : MonoBehaviour
{
  public GameObject targets;
  public int numTargets;
  public float range;
    
  public void CreateTarget(int num, GameObject target)
  {
      for(int i = 0; i < num; i++)
      {
          GameObject t = Instantiate(target,new Vector3(Random.Range(-range, range) + gameObject.transform.position.x
                                                                    , gameObject.transform.position.y
                                                                    ,Random.Range(-range,range) + gameObject.transform.position.z)
                                            ,Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            Debug.Log("Object instantiated at" + t.transform.position);
            t.GetComponent<ObjectLogic>().myArea = this;                 
      }
 
 
  }

  public void ResetTargetArea(GameObject[] agents)
    {
        foreach(GameObject agent in agents)
        {
            if (agent.transform.parent == gameObject.transform)
            {
                agent.transform.position = new Vector3(Random.Range(-range, range), 2f,
                    Random.Range(-range, range))
                    + transform.position;
                agent.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
            }
        }

        CreateTarget(numTargets, targets);
    }
    

}
