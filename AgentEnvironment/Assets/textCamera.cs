using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class textCamera : MonoBehaviour
{

    public GameObject objectToLookAt;
    int targetID;
    public TextMeshPro textMeshPro;
    void Start()
    {
        targetID = objectToLookAt.GetComponent<ObjectLogic>().targetID;
    }

    void Update()
    {
        textMeshPro.SetText("{0}", targetID);
        transform.LookAt(objectToLookAt.transform);
        Quaternion q = transform.rotation;
 
    }

}