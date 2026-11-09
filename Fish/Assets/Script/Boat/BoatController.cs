using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float maxSpeed = 8.0f;     // 最高スピード
    public float acceleration = 5.0f; // 加速・減速の鋭さ
    public float turnSpeed = 100.0f;  // 旋回スピード

    private Rigidbody rb;
    private float currentSpeed = 0.0f; // 現在の実際のスピード

    [Header("乗り降りの設定")]
    public bool isPiloting = false;         // ボートに乗って操縦しているかどうか
    public Transform playerTransform;       // プレイヤーのオブジェクト
    public FPSController fpsController;     // プレイヤーの移動スクリプト

    [Header("乗船判定の設定")]
    [Tooltip("何メートル以内でEキーを押せば乗れるか")]
    [SerializeField] private float rideableDistance = 3.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearDamping = 1.0f;
            rb.angularDamping = 2.0f;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 乗っていないとき
            if (!isPiloting)
            {
                if (playerTransform != null)
                {
                    // プレイヤーとの距離を毎フレームチェックして範囲内なら乗る
                    float distance = Vector3.Distance(transform.position, playerTransform.position);
                    if (distance <= rideableDistance)
                    {
                        isPiloting = true;
                        Debug.Log("ボートに乗りました！操作できます。");

                        playerTransform.SetParent(transform);

                        if (fpsController != null)
                        {
                            fpsController.isPilotingBoat = true;
                        }
                    }
                    else
                    {
                        Debug.Log("ボートから離れすぎています。");
                    }
                }
            }
            // すでに乗っているとき
            else
            {
                isPiloting = false;
                Debug.Log("ボートから降りました。");

                if (playerTransform != null)
                {
                    playerTransform.SetParent(null);
                    // 降りたときに少しボートの横に押し出す
                    playerTransform.position = transform.position + transform.right * 2f + Vector3.up * 0.5f;
                }

                if (fpsController != null)
                {
                    fpsController.isPilotingBoat = false;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // 乗っていないときは、ボートの移動入力を受け付けない
        if (!isPiloting)
        {
            // 乗っていないときはスピードを0にする
            currentSpeed = Mathf.Lerp(currentSpeed, 0.0f, Time.fixedDeltaTime * acceleration);
            return;
        }

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
            if (currentSpeed < 0.0) turn = -turn; // 後退時は逆向き

            Quaternion turnRotation = Quaternion.Euler(0.0f, turn, 0.0f);
            rb.MoveRotation(rb.rotation * turnRotation);
        }
    }
}