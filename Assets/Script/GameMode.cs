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

    public RawImage starImage;

    public Texture2D max;

    public Texture2D Nomax;


    public PictureNumText curPictureNumText;

    public PictureNumText curMaxPictureNumText;

    public PictureNumText TargetPictureNumText;

    public Slider targetSlider;

    public int Level = 0;

    public Congrats Congrat;

    public GameObject Shotball;

    public GameObject pickObject;

    public GameObject RayObject;

    public GameObject TargetObject;

    public GameObject TargetQuad;

    public List<GameObject> Walls;

    public List<GameObject> Obstacles;

    public List<GameObject> samples;

    public Button DeleteButton;

    public Button ResetButton;

    public Button ReadyButton;

    public GameState State = GameState.Place;

    public Text ComboText;

    public Text LevelText;

    public Text TargetComboText;

    public int ComboCount = 0;

    int TargetCombo = 0;

    public int TargetComboScale = 10;

    public int LevelWallCheckCollided = 3;

    public float WinAnimationTime = 3.0f;

    float curWinAnimationTime = 0.0f;

    // Start is called before the first frame updateS
    void Start()
    {
        Shotball = GameObject.FindGameObjectWithTag("ShotBall");
        CloseWallsCollision();
        RandomTargetPos();
        SetLevelConfig();
        curPictureNumText.SetNum(0);
        curMaxPictureNumText.SetNum(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (State == GameState.Place)
        {
            RayCheck();
        }
        else if (State == GameState.Over)
        {
            curWinAnimationTime += Time.deltaTime;
            if(curWinAnimationTime >= WinAnimationTime)
            {
                curWinAnimationTime = 0.0f;
                if (Congrat != null)
                {
                    Congrat.CloseLight();
                }
                SwitchPlaceState();
                if (Shotball != null)
                {
                    Shotball.SetActive(true);
                }
                ReadyButton.gameObject.SetActive(true);
                TargetObject.SetActive(true);
                for (int i = 0; i < Obstacles.Count; i++)
                {
                    Obstacles[i].SetActive(true);
                }
            }
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
                        SetPickObject(hitinfo.collider.gameObject);
                    }
                    else
                    {
                        ClearPickObject();
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
            Shotball.GetComponent<shot>().ArrowImage.SetActive(false);
        }
        ResetWallbCollided();
        CloseShotballTrail();
        CheckQuadColor();
        SetLevelConfig();
        for (int i = 0; i < samples.Count; i++)
        {
            samples[i].GetComponent<ObstacleSample>().SetGrayState();
        }

        if (TargetCombo == 0)
        {
            targetSlider.value = 1;
            curPictureNumText.gameObject.SetActive(false);
            curMaxPictureNumText.gameObject.SetActive(true);
            curPictureNumText.SetNum(0);
            curMaxPictureNumText.SetNum(0);
            TargetPictureNumText.SetNum(TargetCombo);
            starImage.texture = max;
        }
        else
        {
            targetSlider.value = 0;
            curPictureNumText.gameObject.SetActive(true);
            curMaxPictureNumText.gameObject.SetActive(false);
            curPictureNumText.SetNum(0);
            curMaxPictureNumText.SetNum(0);
            TargetPictureNumText.SetNum(TargetCombo);
            starImage.texture = Nomax;
        }
    }


    public void SetPickObject(GameObject obj)
    {
        ClearPickObject();
        pickObject = obj;
        var animator = pickObject.GetComponent<Animator>();
        animator.SetBool("Highlight", true);
    }

    void ClearPickObject()
    {
        if (pickObject != null)
        {
            var lastAnim = pickObject.GetComponent<Animator>();
            lastAnim.SetBool("Highlight", false);
        }
        pickObject = null;
    }

    public void SwitchReadyState()
    {
        ResetWallbCollided();
        SwitchCollider(true);
        State = GameState.Ready;
        ClearPickObject();
        ResetComboCount();
        CloseShotballTrail();
        if (Shotball != null)
        {
            Shotball.GetComponent<shot>().ResetPos();
            Shotball.GetComponent<shot>().ArrowImage.SetActive(false);
        }
        CheckQuadColor();
        SetLevelConfig();
        for (int i = 0; i < samples.Count; i++)
        {
            samples[i].GetComponent<ObstacleSample>().SetGrayState();
        }
        if (TargetCombo == 0)
        {
            targetSlider.value = 1;
            curPictureNumText.gameObject.SetActive(false);
            curMaxPictureNumText.gameObject.SetActive(true);
            curPictureNumText.SetNum(0);
            curMaxPictureNumText.SetNum(0);
            TargetPictureNumText.SetNum(TargetCombo);
            starImage.texture = max;
        }
        else
        {
            targetSlider.value = 0;
            curPictureNumText.gameObject.SetActive(true);
            curMaxPictureNumText.gameObject.SetActive(false);
            curPictureNumText.SetNum(0);
            curMaxPictureNumText.SetNum(0);
            TargetPictureNumText.SetNum(TargetCombo);
            starImage.texture = Nomax;
        }
    }

    public void DestroyPickObject()
    {
        if (pickObject != null)
        {
            Obstacles.Remove(pickObject);
            pickObject.GetComponent<Obstacle>().Sample.Count--;
            pickObject.GetComponent<Obstacle>().Sample.SetNormal();
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
        return true;
        //if (Level < LevelWallCheckCollided)
        //{
        //    return true;
        //}
        //return bAllWallbCollided();
    }

    public void AddComboCount() 
    {
        ComboCount++;
        ComboText.text = ComboCount.ToString();
        if (ComboCount >= TargetCombo)
        {
            curMaxPictureNumText.gameObject.SetActive(true);
            curPictureNumText.gameObject.SetActive(false);
            curMaxPictureNumText.SetNum(ComboCount);
            starImage.texture = max;
        }
        else 
        {
            curPictureNumText.gameObject.SetActive(true);
            curMaxPictureNumText.gameObject.SetActive(false);
            curPictureNumText.SetNum(ComboCount);
            starImage.texture = Nomax;
        }
       
        CheckQuadColor();
        targetSlider.value = (float)((float)ComboCount / (float)TargetCombo);
    }

    void ResetComboCount()
    {
        ComboCount = 0;
        ComboText.text = ComboCount.ToString();

        Target.Instance.SetTargetMaterial();
    }

    public void CloseShotballTrail() 
    {
        Shotball.GetComponent<shot>().SetTrailsEnabled(false);
    }
    
    public void SwitchOverState() 
    {
        State = GameState.Over;
        Level++;
        TargetCombo = Level * TargetComboScale;
        LevelText.text = Level.ToString();
        TargetComboText.text = TargetCombo.ToString();
        //RandomTargetPos();
        ResetWallbCollided();
        CheckQuadColor();
        ResetComboCount();
        //  SetLevelConfig();
        if (Shotball != null)
        {
            Shotball.SetActive(false);
        }
        if (Congrat != null)
        {
            Congrat.DisplayLight();
        }
        ReadyButton.gameObject.SetActive(false);
        TargetObject.SetActive(false);
        for (int i = 0; i < samples.Count; i++)
        {
            samples[i].SetActive(false);
        }
        DeleteButton.gameObject.SetActive(false);
        ResetButton.gameObject.SetActive(false);
        for (int i = 0; i < Obstacles.Count; i++)
        {
            Obstacles[i].SetActive(false);
        }
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

    public void CheckQuadColor() 
    {
        if (TargetQuad != null)
        {
            if (IsFinshedCombo() && CheckWallCollided())
            {
                TargetQuad.GetComponent<targetRota>().SetColor(true);
            }
            else 
            {
                TargetQuad.GetComponent<targetRota>().SetColor(false);
            }
        } 
    }

    public void SetLevelConfig()
    {
        switch (Level)
        {
            case 0:
                ReadyButton.gameObject.SetActive(true);
                DeleteButton.gameObject.SetActive(false);
                ResetButton.gameObject.SetActive(false);
                break;
            default:
                ReadyButton.gameObject.SetActive(true);
                DeleteButton.gameObject.SetActive(true);
                ResetButton.gameObject.SetActive(true);
                break;

        }

        for (int i = 0; i < samples.Count; i++) 
        {
            if (i + 1 <= Level)
            {
                samples[i].SetActive(true);
            }
            else
            {
                samples[i].SetActive(false);
            }
        }
    }

}
