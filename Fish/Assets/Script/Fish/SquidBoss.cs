using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Health))]
public class SquidBoss : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] private string bossName = "巨大イカ";

    [Header("攻撃設定")]
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashAttackDuration = 1.0f; // 突進の攻撃判定が持続する秒数
    [SerializeField] private AttackHitbox dashHitbox;         // 突進用の攻撃判定

    [Header("索敵範囲")]
    [SerializeField] private float aggroiDistance = 30f;

    [Header("イカ墨設定")]
    [SerializeField] private GameObject inkPrefab;
    [SerializeField] private Transform inkSpawnPoint;
    [SerializeField] private float inkSpeed = 20f;

    private GameObject bossUIPanel;
    private Slider bossHpSlider;
    private TextMeshProUGUI bossNameText;

    private Rigidbody rb;
    private Health health;
    private Transform player;
    private bool isBattling = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        // HealthコンポーネントのHP変化イベントを購読する
        health.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= HandleHealthChanged;
    }

    private void Start()
    {
        // プレイヤーの取得
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // Canvasを探し、その中からUIを自動取得する
        GameObject canvasObj = GameObject.Find("Canvas");
        if (canvasObj != null)
        {
            Transform panelTransform = canvasObj.transform.Find("BossUIPanel");
            if (panelTransform != null)
            {
                bossUIPanel = panelTransform.gameObject;
                bossHpSlider = bossUIPanel.GetComponentInChildren<Slider>(true);
                bossNameText = bossUIPanel.GetComponentInChildren<TextMeshProUGUI>(true);
            }
            else
            {
                Debug.LogWarning("Canvasの中に 'BossUIPanel' が見つかりません。");
            }
        }
    }

    private void Update()
    {
        // テスト用ダメージ入力（Healthコンポーネント経由でダメージを与える）
        if (Input.GetKeyDown(KeyCode.K))
        {
            health.AddDamege(20);
        }

        if (!isBattling && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance <= aggroiDistance)
            {
                StartBossBattle();
            }
        }
    }

    public void StartBossBattle()
    {
        if (!isBattling)
        {
            isBattling = true;
            Debug.Log("【ボス戦開始】" + bossName + " が戦闘態勢に入った！");

            if (bossUIPanel != null) bossUIPanel.SetActive(true);

            if (bossNameText != null)
            {
                bossNameText.text = bossName;
            }

            if (bossHpSlider != null)
            {
                bossHpSlider.maxValue = health.GetMaxHealth();
                bossHpSlider.value = health.GetHealth();
            }

            StartCoroutine(BossAttackRoutine());
        }
    }

    private IEnumerator BossAttackRoutine()
    {
        while (health.GetHealth() > 0 && isBattling)
        {
            yield return new WaitForSeconds(attackInterval);

            int attackType = Random.Range(0, 2);
            if (attackType == 0)
            {
                StartCoroutine(DashAttackRoutine());
            }
            else
            {
                InkAttack();
            }
        }
    }

    private IEnumerator DashAttackRoutine()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
            rb.AddForce(direction * dashForce, ForceMode.Impulse);

            // 突進開始時に攻撃判定をオンにする（1回の突進につき1回だけヒットする）
            if (dashHitbox != null) dashHitbox.StartAttack();

            yield return new WaitForSeconds(dashAttackDuration);

            // 突進終了時に攻撃判定をオフにする
            if (dashHitbox != null) dashHitbox.EndAttack();
        }
    }

    private void InkAttack()
    {
        if (inkPrefab != null && inkSpawnPoint != null && player != null)
        {
            Vector3 targetPos = player.position + Vector3.up * 0.5f;
            Vector3 direction = (targetPos - inkSpawnPoint.position).normalized;

            transform.rotation = Quaternion.LookRotation(direction);

            GameObject ink = Instantiate(inkPrefab, inkSpawnPoint.position, Quaternion.identity);
            Rigidbody inkRb = ink.GetComponent<Rigidbody>();
            if (inkRb != null)
            {
                inkRb.linearVelocity = direction * inkSpeed;
            }
        }
    }

    // HealthコンポーネントのHPが変化したときに自動で呼ばれる
    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        if (bossHpSlider != null)
        {
            bossHpSlider.value = currentHealth;
        }

        if (currentHealth <= 0 && isBattling)
        {
            Die();
        }
    }

    private void Die()
    {
        isBattling = false;

        if (dashHitbox != null)
        {
            dashHitbox.EndAttack();
        }

        if (bossUIPanel != null)
        {
            bossUIPanel.SetActive(false);
        }

        Destroy(gameObject);
        SceneManager.LoadScene("Clear");
    }
}