using UnityEngine;

public class ObjectLogic:MonoBehaviour
{
    private bool searched;
    public int targetID;
    public int selectedBy;

    public GameObject sphereIndicator;
    Color sphereColor;

    public bool targetSearched
    {
        get { return searched; }
        //set { searched = value; }
    }

    Material m_Material;

    private void Start()
    {
        m_Material = GetComponent<Renderer>().material;
        sphereColor = sphereIndicator.GetComponent<Renderer>().material.color;
        sphereColor = Color.green;
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
        sphereColor = Color.yellow;
        transform.gameObject.tag = "searchedTarget";
        searched = true;
    }
}