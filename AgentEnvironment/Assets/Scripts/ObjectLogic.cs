using UnityEngine;

public class ObjectLogic:MonoBehaviour
{
    public TargetFinderArea myArea;


    public void OnFound()
    {
       
        Destroy(gameObject);
        
    }

}