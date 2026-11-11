using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Fists : ToulBase
{
    [SerializeField] private GameObject rHandObject_;
    [SerializeField] private GameObject lHandObject_;
    [SerializeField] private int damege_ = 0;

    private Fist[] fists_ = new Fist[2];
    private bool nextFists_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toulType_ = ToulType.Fist;
        fists_[0] = rHand_.GetComponent<Fist>();
        fists_[0].SetDamege(damege_);
        fists_[1] = lHand_.GetComponent<Fist>();
        fists_[1].SetDamege(damege_);
    }

    // Update is called once per frame
    void Update()
    { 
    }


    public override bool Use()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            if (!fists_[nextFists_ ? 1 : 0].IsUse())
            {
                fists_[nextFists_ ? 1 : 0].Use();
                nextFists_ = !nextFists_;
                Debug.Log("パンチした");
                return true;
            }
        }
        return false; 
    }

    public GameObject GetRHandObject() { return rHandObject_; }
    public GameObject GetLHandObject() { return lHandObject_; }

}
