using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PictureNumText : MonoBehaviour
{

    public List<Texture2D> Nums;

    public RawImage T1;

    public RawImage T2;

    public float Txpos = 43;

    public float Typos = -18;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetNum(int Num) 
    {
        Num = Mathf.Clamp(Num, 0, 99);

        if (Num < 10)
        {
            T1.gameObject.SetActive(true);
            T2.gameObject.SetActive(false);
            T1.GetComponent<RectTransform>().localPosition = new Vector3(0, Typos, 0);
            T1.texture = Nums[Num];
           // T1.
        }
        else 
        {
            T1.gameObject.SetActive(true);
            T2.gameObject.SetActive(true);
            T1.GetComponent<RectTransform>().localPosition = new Vector3(-Txpos, Typos, 0);
            T2.GetComponent<RectTransform>().localPosition = new Vector3(Txpos, Typos, 0);
            int T1Num = Num / 10;
            int T2Num = Num % 10;
            T1.texture = Nums[T1Num];
            T2.texture = Nums[T2Num];
        }
    }
}
