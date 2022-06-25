using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shot : MonoBehaviour
{
    public GameObject gameMode;

    public float MaxForce = 4000;

    public float MaxSpeed;

    public float ShakeLimtedSpeed = 50;

    public float ForceScale = 500;

    bool bReadlyShot = false;

    Animator animator;

    Vector3 InitPos;

    // Start is called before the first frame update
    void Start()
    {
        InitPos = transform.position;
        Rigidbody rigBody = GetComponent<Rigidbody>();
        rigBody.maxAngularVelocity = MaxSpeed;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frameS
    void Update()
    {
        GameMode Mode = gameMode.GetComponent<GameMode>();
        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z));
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        if (Mode.State == GameMode.GameState.Ready)
        {
            float ypos = Mathf.Clamp(mousePos.y, -14.0f, 17.0f);
            transform.position = new Vector3(InitPos.x, ypos, InitPos.z);
            if (Input.GetMouseButtonDown(0))
            {
                Mode.State = GameMode.GameState.Shot;
            }
        }
        else if (Mode.State == GameMode.GameState.Shot)
        {
            Vector3 ShotVec = mousePos - this.transform.position;
            ShotVec.z = 0;
            if (Input.GetMouseButtonDown(0))
            {
                bReadlyShot = true;
            }
            else if (Input.GetMouseButtonUp(0) && bReadlyShot)
            {
                Rigidbody rigBody = GetComponent<Rigidbody>();
                rigBody.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ;
                ShotVec = ShotVec * ForceScale;
                float ox = ShotVec.x;
                float yxScale = ShotVec.y / ShotVec.x;
                ShotVec.x = Mathf.Max(ShotVec.x, -MaxForce);
                ShotVec.x = Mathf.Min(ShotVec.x, MaxForce);
                if (ShotVec.x != ox)
                {
                    ShotVec.y = ShotVec.x * yxScale;
                }
                float oy = ShotVec.y;
                ShotVec.y = Mathf.Max(ShotVec.y, -MaxForce);
                ShotVec.y = Mathf.Min(ShotVec.y, MaxForce);

                if (oy != ShotVec.y)
                {
                    ShotVec.x = ShotVec.y / yxScale;
                }
                rigBody.AddForce(ShotVec);
                Mode.State = GameMode.GameState.Shoting;
                bReadlyShot = false;
                GetComponent<TrailRenderer>().enabled = true;
            }
        }
        else if (Mode.State == GameMode.GameState.Shoting)
        {
            Rigidbody rigBody = GetComponent<Rigidbody>();
            if (rigBody.velocity.Equals(Vector3.zero) && Mode.ComboCount >= 1) 
            {
                Mode.SwitchReadyState();
            }
        }
    }

    public void ResetPos()
    {
        Rigidbody rigBody = GetComponent<Rigidbody>();
        rigBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX;
        transform.position = InitPos;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameMode Mode = gameMode.GetComponent<GameMode>();
        if (Mode.State == GameMode.GameState.Shoting)
        {
            Rigidbody rigBody = GetComponent<Rigidbody>();
            if (rigBody.velocity.x>ShakeLimtedSpeed || rigBody.velocity.y> ShakeLimtedSpeed)
            {
                Camera.main.GetComponent<ShakeCamera>().enabled = true;
            }
           Mode.AddComboCount();

            animator.SetTrigger("Active");

            Target.Instance.SetTargetMaterial();
        }
    }
}
