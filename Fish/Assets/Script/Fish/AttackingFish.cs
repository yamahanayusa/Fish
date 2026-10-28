using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class AttackingFish : MonoBehaviour
{
    [Header("データソース")]
    [SerializeField] private FishData fishData;

    [Header("環境設定")]
    [Tooltip("この高さ以下に落ちたら海に逃げた判定にして消去する")]
    [SerializeField] private float waterLevelY = -1f; // FishingManager等と同じ値にします

    [Header("攻撃跳ねパラメータ")]
    [Tooltip("プレイヤーへ向かって進む推進力")]
    [SerializeField] private float attackJumpForce = 5f;

    [Tooltip("攻撃時にプレイヤーへ向かう際のブレ角度（ホーミング精度）")]
    [Range(0f, 45f)]
    [SerializeField] private float attackAngleOffset = 10f;

    private Rigidbody rb;
    private Transform playerTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // "Player" タグがついたオブジェクトを検索
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerTransform = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Playerタグがついたオブジェクトが見つかりません。");
        }
    }

    private void Update()
    {
        // 水面より下に落ちたかどうかの判定
        if (transform.position.y <= waterLevelY)
        {
            Debug.Log($"【AI魚(攻撃)】{gameObject.name} は海へ逃げ込みました。（モデル消去）");
            Destroy(gameObject);
            return; // 消去後はこれ以降の処理を行わない
        }

        // ---------------------------------------------------
        // これより下は、既存のAI処理（プレイヤー追従など）があればそのまま残す
        // ---------------------------------------------------
    }

    /// <summary>
    /// プレイヤーに向かって攻撃的に跳ねる
    /// </summary>
    public void JumpTowardsPlayer()
    {
        if (fishData == null || playerTransform == null) return;

        rb.linearVelocity = Vector3.zero; // 現在の速度をリセット

        // 基本の跳ね上げ（上方向）
        Vector3 jumpDirection = Vector3.up * fishData.jumpPower;

        // 自身からプレイヤーへの方向（水平ベクトル）
        Vector3 directionToPlayer = (playerTransform.position - transform.position);
        directionToPlayer.y = 0; // 水平方向のみ抽出
        directionToPlayer.Normalize();

        if (directionToPlayer != Vector3.zero)
        {
            // 多少のランダムブレを加えて、機械的すぎる追従を緩和
            float angle = Random.Range(-attackAngleOffset, attackAngleOffset);
            Vector3 finalDirection = Quaternion.Euler(0, angle, 0) * directionToPlayer;

            // 進行方向（Y軸回転）をプレイヤーに向かせる
            //transform.rotation = Quaternion.LookRotation(finalDirection);

            // 推進力を付与
            jumpDirection += finalDirection * attackJumpForce;
        }

        // 力（Impulse）を加えて跳ねさせる
        rb.AddForce(jumpDirection, ForceMode.Impulse);
    }
}