using UnityEngine;
using UnityEngine.UIElements;

public class Fist : MonoBehaviour
{
    [SerializeField] private GameObject fistPosition_;
    [SerializeField] private GameObject targetPosition_;
    private bool isUse_ = true;
    private bool moveState = false;
    private SphereCollider sphereCollider_;
    private int damege_ = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isUse_)
        {
            if (moveState)
            {
                Vector3 moveDirection = targetPosition_.transform.position - fistPosition_.transform.position;
                Vector3 moveVol = moveDirection.normalized * 0.07f;
                transform.position += moveVol;

                Vector3 length = transform.position - targetPosition_.transform.position;
                if(length.magnitude < 0.1f)
                {
                    moveState = false;

                    if(sphereCollider_)
                    {
                        Destroy(sphereCollider_);
                    }
                }
            }
            else
            {
                Vector3 moveDirection = fistPosition_.transform.position - targetPosition_.transform.position;
                Vector3 moveVol = moveDirection.normalized * 0.07f;
                transform.position += moveVol;

                Vector3 length = transform.position - fistPosition_.transform.position;
                if (length.magnitude < 0.1f)
                {
                    moveState = true;
                    transform.position = fistPosition_.transform.position;
                    isUse_ = false;
                }
            }
        }
    }

    void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.tag.Contains("Fish_"))
        {
            Health hp = collision.gameObject.GetComponent<Health>();

            if (hp)
            {
                hp.AddDamege(damege_);
            }
            moveState = false;
            Destroy(sphereCollider_);
        }
    }

    public bool IsUse()
    {
        return isUse_;
    }

    public void Use()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 3.0f))
        {
            targetPosition_.transform.position = hit.point;
        }
        else
        {
            Vector3 targetPosition = Camera.main.transform.position;
            targetPosition += Camera.main.transform.forward * 3.0f;

            targetPosition_.transform.position = targetPosition;
        }

        sphereCollider_ = gameObject.AddComponent<SphereCollider>();

        sphereCollider_.isTrigger = true;
        sphereCollider_.radius = 0.1f;
        isUse_ = true;
    }

    public void SetDamege(int damege)
    {
        damege_ = damege;
    }
}
