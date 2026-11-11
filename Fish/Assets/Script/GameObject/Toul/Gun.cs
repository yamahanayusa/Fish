using System;
using UnityEngine;

public class Gun : ToulBase
{
    [SerializeField] private GameObject ammoObject_;
    [SerializeField] private GameObject firePos_;
    [SerializeField] private float recoilX_ = 0.0f;
    [SerializeField] private float recoilY_ = 0.0f;
    [SerializeField] private float rate_ = 0.0f;
    [SerializeField] private int maxAmmo_ = 0;
    [SerializeField] private int power_ = 0;
    [SerializeField] private bool fireMode_ = false;

    private int currentAmmo_;
    private float coolDown_;
    private CharactorCamera camera_;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camera_ = GameObject.Find("eye").GetComponent<CharactorCamera>();
        currentAmmo_ = maxAmmo_;
        coolDown_ = 0.0f;
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

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentAmmo_ = maxAmmo_;
        }

        if (isUse)
        {
            if (CanFier())
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;
                Vector3 targetPos = Vector3.zero;

                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    targetPos = hit.point;
                }
                else
                {
                    targetPos = Camera.main.transform.position + (Camera.main.transform.forward * 100.0f);
                }

                GameObject ammoObject = Instantiate(ammoObject_);
                ammoObject.transform.position = firePos_.transform.position;
                Vector3 moveDirection = targetPos - ammoObject.transform.position;
                Rigidbody rb = ammoObject.GetComponent<Rigidbody>();
                rb.linearVelocity = moveDirection.normalized * 5.0f;
                coolDown_ = rate_;
                currentAmmo_--;
                Ammo ammo = ammoObject.GetComponent<Ammo>();
                ammo.SetAmmoDamege(power_);

                float recoily = UnityEngine.Random.Range(-recoilY_, recoilY_);
                camera_.AddCameraAngle(recoily, recoilX_);

                return true;
            }

        }

        coolDown_ -= Time.deltaTime;
        coolDown_ = Math.Max(-1.0f, coolDown_);

        return false;
    }

    private bool CanFier()
    {
        if (currentAmmo_ == 0) return false;

        if (coolDown_ > 0) return false;

        return true;
    }

}
