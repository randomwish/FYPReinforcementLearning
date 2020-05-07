using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinderArea : MonoBehaviour
{
  public GameObject targets;
  public int numTargets;
  public float range;

  void CreateTarget(int num, GameObject target)
  {
      for(int i = 0; i < num; i++)
      {
          GameObject t = Instantiate(target,new Vector3(Random.Range(-range, range)
                                                                    ,1.0f
                                                                    ,Random.Range(-range,range))
                                            ,Quaternion.Euler(new Vector3(0f, Random.Range(0f, 360f), 90f)));
            t.GetComponent<ObjectLogic>().myArea = this;                 
      }
 
 
  }

  void ResetEnvironment()
  {
    CreateTarget(numTargets,targets);   
  }
}
