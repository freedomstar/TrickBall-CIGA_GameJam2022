using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{  
    public enum GameState
    {
        Place,
        Ready,
        Shot,
        Shoting,
        Over,
    };

    public int Level = 0;

    public GameObject Shotball;

    public GameObject pickObject;

    public GameObject RayObject;
    
    public List<GameObject> Walls;

    public List<GameObject> Obstacles;

    public GameState State = GameState.Place;

    public Text ComboText;

    int ComboCount = 0;

    // Start is called before the first frame updateS
    void Start()
    {
        CloseWallsCollision();
    }

    // Update is called once per frame
    void Update()
    {
        if (State == GameState.Place)
        {
            RayCheck();
        }
    }

    void RayCheck() 
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hitinfo;
            if (Physics.Raycast(ray, out hitinfo))
            {
                string tag = "Obstacle";
                if (hitinfo.collider != null && hitinfo.collider.gameObject)
                {
                    if (hitinfo.collider.gameObject.tag == tag)
                    {
                        pickObject = hitinfo.collider.gameObject;
                    }
                    else
                    {
                        pickObject = null;
                    }
                    RayObject = hitinfo.collider.gameObject;
                }
            }
        }
    }

    public void CloseWallsCollision() 
    {
        for (int i = 0; i < Walls.Count; i++) 
        {
            Walls[i].GetComponent<Collider>().enabled = false;
        }
    }

    public void SwitchPlaceState()
    {
        State = GameState.Place;
        
        SwitchCollider(false);

        if (Shotball != null)
        {
            Shotball.GetComponent<shot>().ResetPos();
        }

        CloseShotballTrail();
    }

    public void SwitchReadyState()
    {
        SwitchCollider(true);
        State = GameState.Ready;
        pickObject = null;
        ResetComboCount();
        CloseShotballTrail();
    }

    public void DestroyPickObject()
    {
        if (pickObject != null)
        {
            Obstacles.Remove(pickObject);
            pickObject.GetComponent<Obstacle>().Sample.Count--;
            Destroy(pickObject);
            pickObject = null;
        }
    }

    void SwitchCollider(bool bOpen)
    {
        for (int i = 0; i < Walls.Count; i++)
        {
            Walls[i].GetComponent<Collider>().enabled = bOpen;
        }
        for (int i = 0; i < Obstacles.Count; i++)
        {
            Obstacles[i].GetComponent<Collider>().isTrigger = !bOpen;
        }
    }

    public void AddComboCount() 
    {
        ComboCount++;
        ComboText.text = ComboCount.ToString();
    }

    void ResetComboCount()
    {
        ComboCount = 0;
        ComboText.text = ComboCount.ToString();
    }

    public void CloseShotballTrail() 
    {
        Shotball.GetComponent<TrailRenderer>().enabled = false;
    }

    public void SwitchOverState() 
    {
        State = GameState.Over;
        Level++;
        SwitchPlaceState();
    }
}
