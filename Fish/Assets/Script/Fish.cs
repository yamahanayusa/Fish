using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Fish : MonoBehaviour
{
    [Header("データソース")]
    [SerializeField] private FishData fishData;

    [Header("逃走・暴れパラメータ")]
    [Tooltip("一度のジャンプで横方向に進む推進力")]
    [SerializeField] private float forwardJumpForce = 3f;

    [Tooltip("外側へ逃げる際の左右のブレ角度（最大値/度）")]
    [Range(0f, 60f)]
    [SerializeField] private float randomAngleOffset = 30f;

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

        GameObject islandObj = GameObject.Find("Cube");
        if (islandObj != null)
        {
            centerTransform = islandObj.transform;
        }
    }

    // ★ Update() 内での毎フレーム回転処理は削除しました（落下中の回転を防ぐため）

    public void Initialize(FishData data)
    {
        fishData = data;
        CurrentHealth = fishData.maxHealth;
        CurrentMoveDirection = fishData.defaultMoveDirection;
    }

    /// <summary>
    /// 島の中心から外側へ向けて跳ねる（跳ねる瞬間だけ向きを切り替える）
    /// </summary>
    public void JumpAwayFromCenter()
    {
        if (fishData == null) return;

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

            // ★【ポイント】跳ねる「瞬間」にだけ進行方向を向かせる
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