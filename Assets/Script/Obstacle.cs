using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Obstacle : MonoBehaviour
{
    public GameMode Mode;

    public float rotaScale = 0.5f;

    private bool bHole = false;

    private bool bRota = false;

    const float deflastMouseXpos = -1234565.0f;

    public ObstacleSample Sample;

    private float lastMouseXpos = -1234565.0f;
    // Start is called before the first frame update
    void Start()
    {
        lastMouseXpos = deflastMouseXpos;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mode.pickObject == gameObject)
        {
            Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z));
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            if (Input.GetMouseButton(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hitinfo;
                if (Physics.Raycast(ray, out hitinfo) || (bHole && !bRota))
                {
                    if (((hitinfo.collider != null && hitinfo.collider.gameObject == this.gameObject) || bHole) && !bRota)
                    {
                        this.transform.position = mousePos;
                        bHole = true;
                    }
                    else
                    {
                        SetRota(mousePos);
                    }
                }
                else
                {
                    SetRota(mousePos);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                bRota = false;
                bHole = false;
                transform.position = new Vector3(transform.position.x, transform.position.y, 0);
                lastMouseXpos = deflastMouseXpos;
            }
        }
    }

    void SetRota(Vector3 mousePos) 
    {
        bRota = true;
        if (lastMouseXpos == deflastMouseXpos)
        {
            lastMouseXpos = mousePos.x;
        }
        else
        {
            float offest = (mousePos.x - lastMouseXpos) * rotaScale;
            transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z + offest);
        }
    }

}
