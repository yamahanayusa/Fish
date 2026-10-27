using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float maxSpeed = 8f;     // 最高スピード
    public float acceleration = 5f; // 加速・減速の鋭さ
    public float turnSpeed = 100f;  // 旋回スピード

    private Rigidbody rb;
    private float currentSpeed = 0f; // 現在の実際のスピード

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearDamping = 1f;
            rb.angularDamping = 2f;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // 前後入力
        float moveInput = Input.GetAxis("Vertical");

        // 目標とするスピードを計算
        float targetSpeed = moveInput * maxSpeed;

        // 現在のスピードから目標のスピードへ、滑らかに近づける
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.fixedDeltaTime * acceleration);

        // 前方向へ移動を適用
        Vector3 moveVelocity = transform.forward * currentSpeed;
        rb.linearVelocity = new Vector3(moveVelocity.x, rb.linearVelocity.y, moveVelocity.z);

        // 旋回
        float turnInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(currentSpeed) > 0.1f)
        {
            float turn = turnInput * turnSpeed * Time.fixedDeltaTime;
            if (currentSpeed < 0) turn = -turn; // 後退時は逆向き

            Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}