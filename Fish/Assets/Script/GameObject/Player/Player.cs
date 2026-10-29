using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    Rigidbody rb_;
    [SerializeField] private GameObject eye_;
    [SerializeField] private int hp_;
    [SerializeField] private float moveSpeed_;
    [SerializeField] private float jumpPower_;
    private bool isGround= false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb_ = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

    }


    void Move()
    {
        float x = 0;
        float y = 0;

        if (Input.GetKey(KeyCode.A))
        {
            x--;
        }
        if (Input.GetKey(KeyCode.D))
        {
            x++;
        }
        if (Input.GetKey(KeyCode.W))
        {
            y++;
        }
        if (Input.GetKey(KeyCode.S))
        {
            y--;
        }

        Vector3 eRot = eye_.transform.rotation.eulerAngles;
        Quaternion rotation = Quaternion.Euler(0, eRot.y, 0);

        Vector3 movePower = Vector3.zero;
        Vector3 gravity = Vector3.zero;
        Vector3 jumpPower = Vector3.zero;

        movePower.x = x;
        movePower.z = y;
        movePower = rotation * movePower;
        movePower = movePower.normalized;
        movePower *= moveSpeed_;

        movePower.y = rb_.linearVelocity.y;

        rb_.linearVelocity = movePower;

        if (!isGround)
        {
            float radius = transform.localScale.z / 2;

            Vector3 bottom = transform.position;
            bottom.y -= transform.localScale.y;
            bottom.y += radius;

            Vector3 top = transform.position;
            top.y += transform.localScale.y;
            top.y -= radius;

            bool isHit = Physics.CapsuleCast(bottom, top, radius, Vector3.down, 0.15f);
            if (isHit)
            {
                isGround = true;
                Debug.Log("着地");
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            jumpPower.y = jumpPower_;
            rb_.AddForce(jumpPower);
            isGround = false;
            Debug.Log("ジャンプ");
        }

        gravity.y = 9.81f * Time.deltaTime * 2;
        rb_.AddForce(gravity);

    }
}
