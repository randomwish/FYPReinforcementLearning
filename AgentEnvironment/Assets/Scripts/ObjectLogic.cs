using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectLogic : MonoBehaviour
{
    private bool searched = false;
    Material m_Material;
    TargetFinderArea m_TargetArea;
    public GameObject TargetArea;
    public GameObject searchZone;
    private int zoneSearched;

    private List<GameObject> zoneList;

    private void Start()
    {
        m_TargetArea = TargetArea.GetComponent<TargetFinderArea>();
        m_Material = GetComponent<Renderer>().material;
        m_Material.color = Color.green;
        transform.gameObject.tag = "target";
        spawnSearchZone();
        zoneSearched = 0;
    }

    private void spawnSearchZone()
    {

        zoneList = new List<GameObject>();

        GameObject w = Instantiate<GameObject>(searchZone.gameObject);
        w.transform.position = transform.position + Vector3.left * 3;
        zoneList.Add(w);

        GameObject n = Instantiate<GameObject>(searchZone.gameObject);
        n.transform.position = transform.position + Vector3.forward * 3;
        zoneList.Add(n);

        GameObject s = Instantiate<GameObject>(searchZone.gameObject);
        s.transform.position = transform.position + Vector3.back * 3;
        zoneList.Add(s);

        GameObject e = Instantiate<GameObject>(searchZone.gameObject);
        e.transform.position = transform.position + Vector3.right * 3;
        zoneList.Add(e);
    }
    /*
    private void OnCollisionExit(Collision collision)
    {
        m_Material.color = Color.yellow;
        transform.gameObject.tag = "searchedTarget";
    } */

    private void targetSearched()
    {            
        m_TargetArea.score++;
        m_Material.color = Color.yellow;
        transform.gameObject.tag = "searchedTarget";
        Debug.Log("target searched");
    }

    private void OnDestroy()
    {
        if (zoneList != null)
        {
            for (int i = 0; i < zoneList.Count; i++)
            {
                if (zoneList[i] != null)
                {
                    Destroy(zoneList[i]);
                }
            }
        }
        zoneList = new List<GameObject>();
    }

    private void OnTriggerExit(Collider other)
    {
       if (other.transform.CompareTag("zone"))
        {
            zoneSearched ++;
        }
       if (zoneSearched >= 4)
        {
            targetSearched();

        }

    }
}
