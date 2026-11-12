using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro; 

[RequireComponent(typeof(Rigidbody))]
public class SquidBoss : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] private string bossName = "巨大イカ"; // インスペクターで名前を自由に変更可能
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("攻撃設定")]
    [SerializeField] private float attackInterval = 3f;
    [SerializeField] private float dashForce = 15f;

    [Header("索敵範囲")]
    [SerializeField] private float aggroiDistance = 30f;

    [Header("イカ墨設定")]
    [SerializeField] private GameObject inkPrefab;
    [SerializeField] private Transform inkSpawnPoint;
    [SerializeField] private float inkSpeed = 20f;

    private GameObject bossUIPanel;
    private Slider bossHpSlider;
    private TextMeshProUGUI bossNameText; // 名前を書き換えるためのテキスト部品

    private Rigidbody rb;
    private Transform player;
    private bool isBattling = false;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();

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

                // パネルの中からTextMeshProコンポーネントを自動取得
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
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(20f);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(35f);
            }
        }
    }

    public void StartBossBattle()
    {
        if (!isBattling)
        {
            isBattling = true;
            Debug.Log("【ボス戦開始】" + bossName + " が戦闘態勢に入った！"); // ログも名前に対応

            if (bossUIPanel != null) bossUIPanel.SetActive(true);

            // 戦闘開始時にUIのテキストをこのボスの名前に書き換える
            if (bossNameText != null)
            {
                bossNameText.text = bossName;
            }

            if (bossHpSlider != null)
            {
                bossHpSlider.maxValue = maxHealth;
                bossHpSlider.value = currentHealth;
            }

            StartCoroutine(BossAttackRoutine());
        }
    }

    private IEnumerator BossAttackRoutine()
    {
        while (currentHealth > 0 && isBattling)
        {
            yield return new WaitForSeconds(attackInterval);

            int attackType = Random.Range(0, 2);
            if (attackType == 0)
            {
                DashAttack();
            }
            else
            {
                InkAttack();
            }
        }
    }

    private void DashAttack()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
            rb.AddForce(direction * dashForce, ForceMode.Impulse);
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

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (bossHpSlider != null)
        {
            bossHpSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isBattling = false;

        if (bossUIPanel != null)
        {
            bossUIPanel.SetActive(false);
        }

        Destroy(gameObject);
        SceneManager.LoadScene("Clear");
    }
}