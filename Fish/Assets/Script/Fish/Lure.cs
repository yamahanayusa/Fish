using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Lure : MonoBehaviour
{
    [Header("ルアー設定")]
    [SerializeField] private string lureID = "LureA";

    [Header("ヒット確率設定")]
    [Tooltip("巻き取り中にフレームごとにヒットする確率（%）")]
    [Range(0.1f, 10f)]
    [SerializeField] private float biteChancePercent = 1.5f;

    [Header("回収判定距離")]
    [SerializeField] private float retrieveDistance = 1.0f;

    private bool isGroundedOrWater = false;
    private bool hasHit = false;
    private Rigidbody rb;

    public string LureID => lureID;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isGroundedOrWater)
        {
            isGroundedOrWater = true;
            rb.isKinematic = true;
            FishingManager.Instance?.OnLureLanded();
        }
    }

    /// <summary>
    /// マウス左長押しでプレイヤー方向へ手繰り寄せられる処理
    /// </summary>
    public void ReelTowards(Vector3 targetPosition, float step)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // 1. まだヒットしていない場合、一定確率でヒット判定
        if (!hasHit)
        {
            TryRollForBite();
        }

        // 2. 魚がヒットしていない状態で手元まで戻ったかチェック
        if (!hasHit)
        {
            float distance = Vector3.Distance(transform.position, targetPosition);
            if (distance <= retrieveDistance)
            {
                Debug.Log("【ルアー回収】ヒットせずに手元まで戻りました。");
                FishingManager.Instance?.OnLureRetrieved();
            }
        }
    }

    /// <summary>
    /// アタリの確率抽選
    /// </summary>
    private void TryRollForBite()
    {
        float roll = Random.Range(0f, 100f);
        if (roll < biteChancePercent)
        {
            hasHit = true;
            Debug.Log("【アタリ！】魚が食いつきました！");
            // ヒット位置（ルアーの現在地）とルアーIDを伝える
            FishingManager.Instance?.OnFishHitAtPosition(transform.position, FishingManager.Instance.currentlureID);
        }
    }
}