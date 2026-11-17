using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackHitbox : MonoBehaviour
{
    [Header("攻撃設定")]
    [SerializeField] private int defaultDamage_ = 10;
    [SerializeField] private string targetTag_ = "Player";

    private int currentDamage_ = 0;
    private bool isAttacking_ = false;
    private bool hasHitInAttack_ = false;

    private void Awake()
    {
        currentDamage_ = defaultDamage_;

        // アタッチされたColliderを確実にTrigger設定にする
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    /// <summary>
    /// 攻撃判定を有効化する（インスペクター設定のダメージを使用）
    /// </summary>
    public void StartAttack()
    {
        StartAttack(defaultDamage_);
    }

    /// <summary>
    /// 攻撃判定を有効化する（攻撃ごとにダメージを指定する場合）
    /// </summary>
    public void StartAttack(int damage)
    {
        currentDamage_ = damage;
        isAttacking_ = true;
        hasHitInAttack_ = false; // 今回の攻撃のヒット済みフラグをリセット
    }

    /// <summary>
    /// 攻撃判定を無効化する
    /// </summary>
    public void EndAttack()
    {
        isAttacking_ = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryHit(other);
    }

    private void TryHit(Collider other)
    {
        // 攻撃中でない、または今回の攻撃ですでにヒット済みなら処理しない
        if (!isAttacking_ || hasHitInAttack_) return;

        if (other.CompareTag(targetTag_))
        {
            // コライダーが子オブジェクトにある場合も考慮して親方向も検索
            Health targetHealth = other.GetComponentInParent<Health>();
            if (targetHealth != null)
            {
                targetHealth.AddDamege(currentDamage_);
                hasHitInAttack_ = true; // 1回ヒットしたらこの攻撃中は再ヒットさせない
            }
        }
    }
}