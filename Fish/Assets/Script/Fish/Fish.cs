using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
    [Header("データソース")]
    [SerializeField] private FishData fishData;

    [Header("ドロップ設定")]
    [Tooltip("死亡時に生成する魚アイテムのPrefab")]
    [SerializeField] private GameObject fishItemPrefab;

    [Header("環境設定")]
    [Tooltip("この高さ以下に落ちたら海に逃げた判定にして消去する")]
    [SerializeField] private float waterLevelY = -1f;

    [Header("逃走・暴れパラメータ")]
    [Tooltip("一度のジャンプで横方向に進む推進力")]
    [SerializeField] private float forwardJumpForce = 3f;

    [Tooltip("外側へ逃げる際の左右のブレ角度（最大値/度）")]
    [Range(0f, 60f)]
    [SerializeField] private float randomAngleOffset = 30f;

    [Header("状態フラグ")]
    public bool isDead = false; // 死亡したかの判定
    public bool isHeld = false; // プレイヤーが持っているか

    private Rigidbody rb;
    private Transform centerTransform; // 島の中心

    public float CurrentHealth { get; private set; }
    public MoveDirection CurrentMoveDirection { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (fishData != null)
        {
            Initialize(fishData);
        }

        GameObject islandObj = GameObject.Find("Island_3 Variant");
        if (islandObj != null)
        {
            centerTransform = islandObj.transform;
        }
    }

    private void Update()
    {
        // 海（水面）に落ちたかどうかの判定
        if (transform.position.y <= waterLevelY)
        {
            Debug.Log($"【AI魚】{gameObject.name} は海へ逃げ込みました。（モデル消去）");
            Destroy(gameObject);
        }

        // *テスト用*スペースキーを押したら50ダメージ
        if (Input.GetKeyDown(KeyCode.V) && !isDead)
        {
            TakeDamage(CurrentHealth);
        }
    }

    public void Initialize(FishData data)
    {
        fishData = data;
        CurrentHealth = fishData.maxHealth;
        CurrentMoveDirection = fishData.defaultMoveDirection;
    }

    /// <summary>
    /// ダメージの受ける処理
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (isDead) return; // 死んでたら何もしない

        CurrentHealth -= amount;
        Debug.Log($"魚に {amount} のダメージ! 残りHP: {CurrentHealth} / {fishData.maxHealth}");

        if(CurrentHealth <= 0.0f)
        {
            Die();
        }
    }

    /// <summary>
    /// HPが0になったときの死亡処理
    /// </summary>
    private void Die()
    {
        isDead = true;
        Debug.Log("魚のHPが0になりました！アイテムをドロップして消去");

        // ドロップ用Prefabが設定されていれば、現在の位置・回転で生成
        if (fishItemPrefab != null)
        {
            Instantiate(fishItemPrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning("FishItemPrefabが設定されていません！");
        }

        // 生きている魚本体は消去する
        Destroy(gameObject);
    }

    /// <summary>
    /// 島の中心から外側へ向けて跳ねる（跳ねる瞬間だけ向きを切り替える）
    /// </summary>
    public void JumpAwayFromCenter()
    {
        if (fishData == null || isDead) return;

        Vector3 jumpDirection = Vector3.up * fishData.jumpPower;

        if (centerTransform != null)
        {
            // 島の中心から魚への基本方向
            Vector3 baseAwayDirection = (transform.position - centerTransform.position);
            baseAwayDirection.y = 0;
            baseAwayDirection.Normalize();

            if (baseAwayDirection == Vector3.zero)
            {
                baseAwayDirection = Vector3.forward;
            }

            // 左右にランダムな角度ブレを追加
            float angle = Random.Range(-randomAngleOffset, randomAngleOffset);
            Vector3 finalDirection = Quaternion.Euler(0, angle, 0) * baseAwayDirection;

            // 跳ねる「瞬間」にだけ進行方向を向かせる
            transform.rotation = Quaternion.LookRotation(finalDirection);

            // 上方向への力 + ブレを含んだ外側への推進力
            jumpDirection += finalDirection * forwardJumpForce;
        }

        // 瞬時に力を加えて跳ねさせる
        rb.AddForce(jumpDirection, ForceMode.Impulse);
    }

    public void SetIslandCenter(Transform island)
    {
        centerTransform = island;
    }
}