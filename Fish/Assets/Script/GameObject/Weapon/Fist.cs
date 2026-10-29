using UnityEngine;
using UnityEngine.UIElements;

public class Fist : MonoBehaviour
{
    [SerializeField] private GameObject fistPosition_;
    [SerializeField] private GameObject targetPosition_;
    private bool isUse_ = false;
    private bool moveState = false;


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
                Vector3 moveVol = moveDirection.normalized * 0.1f;
                transform.position += moveVol;

                Vector3 length = transform.position - targetPosition_.transform.position;
                if(length.magnitude < 0.2f)
                {
                    moveState = false;
                }
            }
            else
            {
                Vector3 moveDirection = fistPosition_.transform.position - targetPosition_.transform.position;
                Vector3 moveVol = moveDirection.normalized * 0.1f;
                transform.position += moveVol;

                Vector3 length = transform.position - fistPosition_.transform.position;
                if (length.magnitude < 0.2f)
                {
                    moveState = true;
                    transform.position = fistPosition_.transform.position;
                    isUse_ = false;
                }
            }
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
        if (Physics.Raycast(ray, out hit, 10.0f))
        {
            targetPosition_.transform.position = hit.point;
        }
        else
        {
            Vector3 targetPosition = Camera.main.transform.position;
            targetPosition += Camera.main.transform.forward * 10.0f;

            targetPosition_.transform.position = targetPosition;
        }
        isUse_ = true;
    }

}
