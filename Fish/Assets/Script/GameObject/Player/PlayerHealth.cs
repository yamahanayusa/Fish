using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    private bool isDead = false;

    [Header("UI設定")]
    [SerializeField] private Image hpRingImage;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameOverVolume; 

    [Header("復活設定")]
    [Tooltip("左クリック長押しで復活するまでの秒数")]
    [SerializeField] private float reviveHoldTime = 2f;
    private float currentHoldTime = 0f;

    [Header("チーム開発用：死亡・復活時のイベント")]
    [Tooltip("死亡時に実行したい処理（移動スクリプトのオフなど）を登録")]
    [SerializeField] private UnityEvent onDeathEvent; 
    [Tooltip("復活時に実行したい処理（移動スクリプトのオンなど）を登録")]
    [SerializeField] private UnityEvent onReviveEvent;

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
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHPUI();

        if (currentHealth <= 0)
        {
            Die();
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

        // 釣りの操作を無効化
        if (FishingManager.Instance != null)
        {
            FishingManager.Instance.enabled = false;
        }

        // インスペクターで登録した「死亡時の処理」を実行
        onDeathEvent?.Invoke();

        // ReviveからDieへ移動。ゲームオーバーになった瞬間にボスを消す
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

        // 操作を再び有効化し、釣りの状態をリセット
        if (FishingManager.Instance != null)
        {
            FishingManager.Instance.enabled = true;
            FishingManager.Instance.ResetFishing();
        }

        // インスペクターで登録した「復活時の処理」を実行
        onReviveEvent?.Invoke();
    }
}