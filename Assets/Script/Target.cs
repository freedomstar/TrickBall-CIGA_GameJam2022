using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public GameMode Mode;
    public static Target Instance;

    public Material normalMat;
    public Material disbaledMat;

    public MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        SetTargetMaterial();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject && other.gameObject.tag == "ShotBall" && Mode && Mode.IsFinshedCombo() && Mode.CheckWallCollided())
        {
            Mode.SwitchOverState();

        }
    }

    public void SetTargetMaterial()
    {
        if (Mode.IsFinshedCombo())
        {
            mr.sharedMaterial = normalMat;
        }
        else
        {
            mr.sharedMaterial = disbaledMat;
        }
    }
}
