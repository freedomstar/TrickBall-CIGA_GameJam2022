using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class StartSence : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ToMainSence() 
    {
        SceneManager.LoadScene(1);
    }
}
