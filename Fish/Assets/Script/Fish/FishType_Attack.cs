using UnityEngine;

public class AttackingFishTest : MonoBehaviour
{
    private AttackingFish attackingFish;

    [Header("攻撃タイマー設定")]
    [SerializeField] private float minAttackInterval = 1.0f;
    [SerializeField] private float maxAttackInterval = 2.0f;

    private float timer = 0f;
    private float nextAttackTime = 0f;

    private void Awake()
    {
        attackingFish = GetComponent<AttackingFish>();
    }

    private void Start()
    {
        SetNextAttackTime();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextAttackTime)
        {
            if (attackingFish != null)
            {
                attackingFish.JumpTowardsPlayer();
            }

            timer = 0f;
            SetNextAttackTime();
        }
    }

    private void SetNextAttackTime()
    {
        nextAttackTime = Random.Range(minAttackInterval, maxAttackInterval);
    }
}