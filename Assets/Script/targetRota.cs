using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class targetRota : MonoBehaviour
{
    public float RotaSpeed = 0.5f;

    public Color ActiveColor;

    public Color NoActiveColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z + RotaSpeed);
    }

    public void SetColor(bool bActive)
    {
        if (bActive)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", ActiveColor);
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", NoActiveColor);
        }
    }
}
