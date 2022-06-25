using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public bool bCollided = false;

    public Color NoCollidedColor;

    public Color OriginaColor;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<MeshRenderer>().material.SetColor("_Color", OriginaColor);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        bCollided = true;
        GetComponent<MeshRenderer>().material.SetColor("_Color", OriginaColor);
    }

    public void ResetSetColor(bool bCheckCollided) 
    {
        if (bCheckCollided)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", NoCollidedColor);
        }
        else 
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", OriginaColor);
        }       
    }
}
