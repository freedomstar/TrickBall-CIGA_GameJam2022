using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Congrats : MonoBehaviour
{
    public Material[] materials;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayLight()
    {
        foreach (Material mat in materials)
        {
            mat.color = Color.white;
            mat.EnableKeyword("_EMISSION");
        }
    }

    public void CloseLight()
    {
        foreach (Material mat in materials)
        {
            mat.color = Color.black;
            mat.DisableKeyword("_EMISSION");
        }
    }
}
