using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;


public class shot : MonoBehaviour
{
    public GameObject gameMode;

    public float MaxForce = 4000;

    public float MaxSpeed;

    public float ShakeLimtedSpeed = 50;

    public float ForceScale = 500;

    public float maxLinearVelocity = 200;

    bool bReadlyShot = false;

    [ColorUsage(true,true)]
    public Color MaxSpeedColor1;

    [ColorUsage(true, true)]
    public Color MaxSpeedColor2;

    [ColorUsage(true, true)]
    public Color MaxSpeedColor3;

    [ColorUsage(true, true)]
    public Color MaxSpeedColor4;

    [ColorUsage(true, true)]
    public Color MaxSpeedColor5;

    [ColorUsage(true, true)]
    public Color MaxSpeedColor6;

    [ColorUsage(true, true)]
    public Color MinSpeedColor1;

    [ColorUsage(true, true)]
    public Color MinSpeedColor2;

    [ColorUsage(true, true)]
    public Color MinSpeedColor3;

    [ColorUsage(true, true)]
    public Color MinSpeedColor4;

    [ColorUsage(true, true)]
    public Color MinSpeedColor5;

    [ColorUsage(true, true)]
    public Color MinSpeedColor6;

    public GameObject ArrowImage;

    public AudioSource ColliedSound;

    Animator animator;

    Vector3 InitPos;

    // Start is called before the first frame update
    void Start()
    {
        ArrowImage.SetActive(false);
        InitPos = transform.position;
        Rigidbody rigBody = GetComponent<Rigidbody>();
        rigBody.maxAngularVelocity = MaxSpeed;
        animator = GetComponent<Animator>();
        rigBody.maxLinearVelocity = maxLinearVelocity;

        SetBallColor();
    }

    // Update is called once per frameS
    void Update()
    {
        SetBallColor();
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
                ArrowImage.SetActive(true);
                bReadlyShot = true;
                Vector3 scpos = Camera.main.WorldToScreenPoint(transform.position);
                ArrowImage.GetComponent<RectTransform>().position = scpos;
            }
            else if (Input.GetMouseButtonUp(0) && bReadlyShot)
            {
                ArrowImage.SetActive(false);
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
                SetTrailsEnabled(true);
            }
            else if (Input.GetMouseButton(0)) 
            {
                Vector3 scpos = Camera.main.WorldToScreenPoint(transform.position);
                float Angle = Vector3.Angle(Vector3.right, Input.mousePosition - scpos);
                if (Vector3.Cross(Vector3.right, Input.mousePosition - scpos).z < 0)
                {
                    Angle = -Angle;
                }
                ArrowImage.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, Angle);
                ArrowImage.GetComponent<RectTransform>().localScale = new Vector3(Mathf.Clamp(Vector3.Distance(Input.mousePosition, scpos) / 120, 0.5f, 3f), 1,1);
            }
        }
        else if (Mode.State == GameMode.GameState.Shoting)
        {
            ArrowImage.SetActive(false);
            Rigidbody rigBody = GetComponent<Rigidbody>();
            if (rigBody.velocity.Equals(Vector3.zero) && Mode.ComboCount >= 1) 
            {
                Mode.SwitchReadyState();
            }

        }
    }

    public void SetTrailsEnabled(bool enable)
    {
        var trails = GetComponentsInChildren<TrailRenderer>();
        foreach (var trail in trails)
        {
            trail.enabled = enable;
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

            if (animator)
            {
                animator.SetTrigger("Active");
            }

            Target.Instance.SetTargetMaterial();
            ColliedSound.Play();
        }    
    }

    void SetBallColor() 
    {
        Rigidbody rigBody = GetComponent<Rigidbody>();
        float Lerp = (rigBody.velocity.magnitude / MaxSpeed)*4;
        var trails = GetComponentsInChildren<TrailRenderer>();
        foreach (var trail in trails)
        {
            List<Material> Materials = new List<Material>();
            trail.GetMaterials(Materials);
            for (int i = 0; i < Materials.Count; i++) 
            {
                if (trail.gameObject.name == "Trail")
                {
                    Materials[i].SetColor("_Color01", Vector4.Lerp( MinSpeedColor1, MaxSpeedColor1, Lerp));
                    Materials[i].SetColor("_Color02", Vector4.Lerp(MinSpeedColor2, MaxSpeedColor2, Lerp));
                }
                else if (trail.gameObject.name == "Trail (1)")
                {
                    Materials[i].SetColor("_Color01", Vector4.Lerp(MinSpeedColor3, MaxSpeedColor3, Lerp));
                    Materials[i].SetColor("_Color02", Vector4.Lerp(MinSpeedColor4, MaxSpeedColor4, Lerp));
                }
            }
        }
        var Effects = GetComponentsInChildren<VisualEffect>();
        foreach (var Effect in Effects)
        {
            Effect.SetVector4("Color", Vector4.Lerp(MinSpeedColor5, MaxSpeedColor5, Lerp));
            Effect.SetVector4("ParticlesColor", Vector4.Lerp(MinSpeedColor6, MaxSpeedColor6, Lerp));
        }
    }
}
