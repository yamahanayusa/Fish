using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackHitbox : MonoBehaviour
{
    [Header("攻撃設定")]
    [SerializeField] private int defaultDamage_ = 10;
    [SerializeField] private string targetTag_ = "Player";

    private Collider hitboxCollider_;
    private int currentDamage_ = 0;
    private bool isAttacking_ = false;
    private bool hasHitInAttack_ = false;

    private void Awake()
    {
        currentDamage_ = defaultDamage_;
        hitboxCollider_ = GetComponent<Collider>();

        if (hitboxCollider_ != null)
        {
            hitboxCollider_.isTrigger = true;
            hitboxCollider_.enabled = false; // 初期状態は判定をオフにしておく
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
        hasHitInAttack_ = false;

        if (hitboxCollider_ != null)
        {
            hitboxCollider_.enabled = true; // 攻撃時のみコライダーを出す
        }
    }

    /// <summary>
    /// 攻撃判定を無効化する
    /// </summary>
    public void EndAttack()
    {
        isAttacking_ = false;

        if (hitboxCollider_ != null)
        {
            hitboxCollider_.enabled = false; // 攻撃終了でコライダーを消す
        }
    }

    private void OnDisable()
    {
        EndAttack();
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
        if (!isAttacking_ || hasHitInAttack_) return;

        if (other.CompareTag(targetTag_))
        {
            Health targetHealth = other.GetComponentInParent<Health>();
            if (targetHealth != null)
            {
                targetHealth.AddDamege(currentDamage_);
                hasHitInAttack_ = true;
            }
        }
    }
}