using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("被弾設定")]
    [SerializeField] private float invincibilityTime = 1.0f; // 無敵時間（秒）
    private bool isInvincible = false; // 無敵状態かどうかのフラグ

    [Header("UI設定")]
    [SerializeField] private Image hpRingImage;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameOverVolume; 

    [Header("復活設定")]
    [Tooltip("左クリック長押しで復活するまでの秒数")]
    [SerializeField] private float reviveHoldTime = 2f;
    private float currentHoldTime = 0f;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHPUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameOverVolume != null) gameOverVolume.SetActive(false);
    }

    private void Update()
    {
        if (isDead)
        {
            // 左クリック長押しで復活
            if (Input.GetMouseButton(0))
            {
                currentHoldTime += Time.deltaTime;
                if (currentHoldTime >= reviveHoldTime)
                {
                    Revive();
                }
            }
            else
            {
                currentHoldTime = 0f; // 離したらリセット
            }
        }
    }

    public void TakeDamage(float damage)
    {
        // すでに死んでいるか、無敵時間中ならここで処理をストップ
        if (isDead || isInvincible) return;

        currentHealth -= damage;
        UpdateHPUI(); // HPバー更新処理

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // 死ななかった場合は無敵時間を開始する
            StartCoroutine(InvincibilityRoutine());
        }
    }

    private void UpdateHPUI()
    {
        if (hpRingImage != null)
        {
            hpRingImage.fillAmount = currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("【死亡】YOU ARE DEAD");
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverVolume != null) gameOverVolume.SetActive(true);

        //if (FishingManager.Instance != null)
        //{
        //    FishingManager.Instance.enabled = false;
        //}

        // プレイヤーの移動を強制停止
        Player playerScript = GetComponent<Player>();
        if (playerScript != null)
        {
            playerScript.enabled = false;
        }

        // カメラ視点（CharactorCamera）を強制停止
        CharactorCamera camScript = GetComponentInChildren<CharactorCamera>();
        if (camScript != null)
        {
            camScript.enabled = false;
        }

        // インベントリを強制停止
        Inventory inventoryScript = FindAnyObjectByType<Inventory>();
        if (inventoryScript != null)
        {
            inventoryScript.enabled = false;
        }

        // 慣性で滑らないように速度を0にする
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }

        SquidBoss boss = FindAnyObjectByType<SquidBoss>();
        if (boss != null)
        {
            Destroy(boss.gameObject);
            Debug.Log("【死亡】ゲームオーバー処理：ボスを消去しました");
        }
    }

    private void Revive()
    {
        isDead = false;
        currentHoldTime = 0f;
        currentHealth = maxHealth;
        UpdateHPUI();
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameOverVolume != null) gameOverVolume.SetActive(false);

        //if (FishingManager.Instance != null)
        //{
        //    FishingManager.Instance.enabled = true;
        //    FishingManager.Instance.ResetFishing();
        //}

        // 復活時：各スクリプトを有効化
        Player playerScript = GetComponent<Player>();
        if (playerScript != null) playerScript.enabled = true;

        CharactorCamera camScript = GetComponentInChildren<CharactorCamera>();
        if (camScript != null) camScript.enabled = true;

        Inventory inventoryScript = FindAnyObjectByType<Inventory>();
        if (inventoryScript != null) inventoryScript.enabled = true;
    }

    private System.Collections.IEnumerator InvincibilityRoutine()
    {
        isInvincible = true;

        // 設定した時間（1秒）だけ待機
        yield return new WaitForSeconds(invincibilityTime);

        isInvincible = false;
    }
}