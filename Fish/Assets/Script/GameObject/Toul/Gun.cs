using UnityEngine;

public class Gun : ToulBase
{
    [SerializeField] private GameObject firePos_;
    [SerializeField] private float recoilX_;
    [SerializeField] private float recoilY_;
    [SerializeField] private float rate_;
    [SerializeField] private int maxAmmo_;
    [SerializeField] private bool fireMode_;

    private int currentAmmo_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override bool Use()
    {
        bool isUse = false;
        if (fireMode_)
        {
            isUse = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            isUse = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if (isUse)
        {
            if (!CanFier())
            {
                return false;
            }

        }

        return false;
    }

    private bool CanFier()
    {


        return false;
    }

}
