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

    public GameObject TargetObject;

    public List<GameObject> Walls;

    public List<GameObject> Obstacles;

    public GameState State = GameState.Place;

    public Text ComboText;

    public int ComboCount = 0;

    int TargetCombo = 0;

    public int TargetComboScale = 10;

    public int LevelWallCheckCollided = 3;

    // Start is called before the first frame updateS
    void Start()
    {
        CloseWallsCollision();
        RandomTargetPos();
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
        ResetWallbCollided();
        CloseShotballTrail();
    }

    public void SwitchReadyState()
    {
        ResetWallbCollided();
        SwitchCollider(true);
        State = GameState.Ready;
        pickObject = null;
        ResetComboCount();
        CloseShotballTrail();
        if (Shotball != null)
        {
            Shotball.GetComponent<shot>().ResetPos();
        }
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

    private void ResetWallbCollided()
    {
        for (int i = 0; i < Walls.Count; i++)
        {
            Wall wall = Walls[i].GetComponent<Wall>();
            if (wall) 
            {
                wall.bCollided = false;
                wall.ResetSetColor(Level >= LevelWallCheckCollided);               
            }
        }
    }

    private bool bAllWallbCollided()
    {
        for (int i = 0; i < Walls.Count; i++)
        {
            if (!Walls[i].GetComponent<Wall>().bCollided)
            {
                return false;
            }
        }
        return true;
    }

    public bool CheckWallCollided() 
    {
        if (Level < LevelWallCheckCollided)
        {
            return true;
        }
        return bAllWallbCollided();
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
        TargetCombo = Level * TargetComboScale;
        RandomTargetPos();
        SwitchPlaceState();
        ResetWallbCollided();
    }

    public bool IsFinshedCombo() 
    {
        return ComboCount >= TargetCombo;
    }

    public void RandomTargetPos() 
    {
        float x = Random.Range(0, 40);
        float y = Random.Range(15, -13);
        if (TargetObject)
        {
            TargetObject.transform.position = new Vector3(x, y, 0);
        }
    }
}
