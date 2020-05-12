using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFinderArea : MonoBehaviour
{
    public GameObject target;
    public int numTargets;
    public float range;

    public GameObject agent;
    private List<GameObject> agentsList;
    private List<GameObject> targetsList;


    public void RemoveSpecificTarget(GameObject targetObject)
    {
        targetsList.Remove(targetObject);
        Destroy(targetObject);
    }    
    
    public void RemoveAllTargets()
    {
        if(targetsList != null)
        {
            for(int i = 0; i < targetsList.Count; i++){
                if(targetsList[i] != null){
                    Destroy(targetsList[i]);
                }
            }
        }

        targetsList = new List<GameObject>();
    }

    public static Vector3 GenerateNewPosition(Vector3 center, float range)
    {
        Vector3 newPosition = center;
        newPosition.x += UnityEngine.Random.Range(-range,range);
        newPosition.z += UnityEngine.Random.Range(-range,range);
        return newPosition;
    }

    public void SpawnTarget(int num, GameObject target)
    {

        for (int i = 0; i < num; i++)
        {
            GameObject t = Instantiate<GameObject>(target.gameObject);
            t.transform.position = GenerateNewPosition(transform.position,range);
            t.transform.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f,360f),0f);
            
            t.transform.SetParent(transform);

            targetsList.Add(t);

            
        }
 
 
    }

    public void RespawnAgent()
    {
        Rigidbody rigidbody = agent.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        agent.transform.position = GenerateNewPosition(transform.position,range);
        agent.transform.rotation = Quaternion.Euler(0f,UnityEngine.Random.Range(0f,180f),0f); 
    }

    public void ResetArea()
    {
        RemoveAllTargets();
        RespawnAgent();
        SpawnTarget(numTargets,target);
    }
    private void Start()
    {
        ResetArea();
    }
}
