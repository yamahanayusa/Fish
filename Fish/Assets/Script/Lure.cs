using UnityEngine;
using System.Collections;

public class Lure : MonoBehaviour
{
    [Header("ヒット設定")]
    [Tooltip("魚を引き寄せる範囲（半径）")]
    [SerializeField] private float detectionRadius = 5f;

    [Tooltip("アタリが来てからヒットするまでの待ち時間（最小/最大）")]
    [SerializeField] private float minWaitTime = 2f;
    [SerializeField] private float maxWaitTime = 5f;

    private bool isWaterland = false;
    private Fish targetFish = null;

    /// <summary>
    /// 着水時に呼ばれる（FishingManagerから制御）
    /// </summary>
    public void OnLandOnWater()
    {
        if (isWaterland) return;
        isWaterland = true;

        Debug.Log("ルアーが着水しました。魚を探索中...");
        StartCoroutine(WaitForBiteRoutine());
    }

    /// <summary>
    /// アタリ待ちルーチン
    /// </summary>
    private IEnumerator WaitForBiteRoutine()
    {
        // ランダムな待ち時間
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        yield return new WaitForSeconds(waitTime);

        // 周囲の魚を検知
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        foreach (var col in hitColliders)
        {
            Fish fish = col.GetComponentInParent<Fish>();
            if (fish != null)
            {
                targetFish = fish;
                break;
            }
        }

        if (targetFish != null)
        {
            Debug.Log($"ヒット！ {targetFish.gameObject.name} が食いつきました！");
            // FishingManagerへヒット通知
            FishingManager.Instance?.OnFishHit(targetFish, this);
        }
        else
        {
            Debug.Log("アタリがありませんでした。もう一度キャストしてください。");
            FishingManager.Instance?.OnHitFailed();
        }
    }

    private void OnDrawGizmosSelected()
    {
        // ギズモで検知範囲を可視化
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}