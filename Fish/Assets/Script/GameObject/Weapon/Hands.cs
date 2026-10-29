using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Hands : MonoBehaviour
{
    [SerializeField] private GameObject RFist_;
    [SerializeField] private GameObject LFist_;

    private Fist[] fists_ = new Fist[2];
    private bool nextFists_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fists_[0] = RFist_.GetComponent<Fist>();
        fists_[1] = LFist_.GetComponent<Fist>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Punch();
        }
    }


    bool Punch()
    {
        if (!fists_[nextFists_ ? 1 : 0].IsUse())
        {
            fists_[nextFists_ ? 1 : 0].Use();
            nextFists_ = !nextFists_;
            Debug.Log("パンチした");
            return true;
        }
        Debug.Log("パンチできなかった");
        return false; 
    }
}
