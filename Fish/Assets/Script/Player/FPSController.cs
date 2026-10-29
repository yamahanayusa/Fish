using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FPSController : MonoBehaviour
{
    [Header("視点移動（マウス）")]
    [SerializeField] private float mouseSensitivity = 2f;
    [SerializeField] private Transform playerCamera;
    private float xRotation = 0f;

    [Header("移動・ジャンプ")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    private Rigidbody rb;
    private bool isGrounded;

    // ボートに乗っているかどうかのフラグ
    [HideInInspector] public bool isPilotingBoat = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        // 物理演算による勝手な回転（転がり）を完全にロック
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // ★追加：カメラのガタつき（ジッター）を防ぐ滑らか設定
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        // マウスカーソルをロック
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // 1. マウスによる視点移動
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // 上下90度制限

        if (playerCamera != null)
        {
            playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // プレイヤーの左右回転
        transform.Rotate(Vector3.up * mouseX);

        // ボートに乗っているときは、プレイヤー自身のジャンプ入力を受け付けない
        if (isPilotingBoat) return;

        // 2. ジャンプ（Update内で入力を取るのが最も確実）
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // 連続ジャンプ防止
        }
    }

    private void FixedUpdate()
    {
        // ボートに乗っているときは移動入力を受け付けない
        if (isPilotingBoat) return;

        // 3. WASDキーによる移動
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        Vector3 targetVelocity = move * moveSpeed;

        // Y軸（重力・ジャンプ）の速度はそのまま維持
        targetVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = targetVelocity;
    }

    // ★追加：確実な地面判定（コライダー同士の接触でチェック）
    private void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // 接触点の法線（向き）が上向きなら「地面」とみなす
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}