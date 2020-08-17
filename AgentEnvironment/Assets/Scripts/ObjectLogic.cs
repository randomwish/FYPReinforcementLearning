using UnityEngine;

public class ObjectLogic:MonoBehaviour
{
    private bool searched;
    public int targetID;
    public int selectedBy;

    public GameObject sphereIndicator;
   

    public bool targetSearched
    {
        get { return searched; }
        //set { searched = value; }
    }

    Material m_Material;

    private void Start()
    {
        m_Material = GetComponent<Renderer>().material;
        sphereIndicator.GetComponent<Renderer>().material.color = Color.green;
        transform.gameObject.tag = "target";
        searched = false;
        selectedBy = 0;
    }

    private void OnCollisionExit(Collision other)
    {
        if (!targetSearched)
        {
            if (other.gameObject.CompareTag("agent"))
            {
                afterSearched();
                other.gameObject.SendMessage("checkTarget", targetID);
            }
        }
    }

    private void afterSearched()
    {
        sphereIndicator.GetComponent<Renderer>().material.color = Color.yellow;
        transform.gameObject.tag = "searchedTarget";
        searched = true;
    }
}