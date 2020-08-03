using UnityEngine;

public class ObjectLogic:MonoBehaviour
{
    private bool searched;
    public int targetID;

    public bool targetSearched
    {
        get { return searched; }
        //set { searched = value; }
    }

    Material m_Material;

    private void Start()
    {
        m_Material = GetComponent<Renderer>().material;
        m_Material.color = Color.green;
        transform.gameObject.tag = "target";
        searched = false;
    }

    private void Update()
    {
        if (!targetSearched)
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 5);
            
            foreach(Collider other in hitColliders)
            {
                if (other.CompareTag("agent"))
                {
                    afterSearched();
                    other.SendMessage("checkTarget", targetID);
                }
            }
        }
    } 

    /*private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("agent"))
        {
            afterSearched();
            other.gameObject.SendMessage("checkTarget", targetID);
        }
    }*/

    private void afterSearched()
    {
        m_Material.color = Color.yellow;
        transform.gameObject.tag = "searchedTarget";
        searched = true;
    }
}