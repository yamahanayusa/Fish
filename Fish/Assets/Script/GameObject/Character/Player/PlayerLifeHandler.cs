using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Health))]
public class PlayerLifeHandler : MonoBehaviour
{
    [Header("UI設定")]
    [SerializeField] private Image hpRingImage;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameOverVolume;

    [Header("復活設定")]
    [Tooltip("左クリック長押しで復活するまでの秒数")]
    [SerializeField] private float reviveHoldTime = 2f;

    private Health health;
    private bool isDead = false;
    private bool isPaused = false;
    private float currentHoldTime = 0f;

    // 外部（SystemMenuManagerなど）から死亡中か確認するためのプロパティ
    public bool IsDead => isDead;

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        health.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        health.OnHealthChanged -= HandleHealthChanged;
    }

    private void Start()
    {
        UpdateHPUI(health.GetHealth(), health.GetMaxHealth());

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameOverVolume != null) gameOverVolume.SetActive(false);
    }

    private void Update()
    {
        // ポーズ中は復活の長押しカウントも進めない
        if (!isDead || isPaused) return;

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

    // HealthコンポーネントのHPが変化したときに自動で呼ばれる
    private void HandleHealthChanged(int currentHealth, int maxHealth)
    {
        UpdateHPUI(currentHealth, maxHealth);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    private void UpdateHPUI(int currentHealth, int maxHealth)
    {
        if (hpRingImage != null && maxHealth > 0)
        {
            hpRingImage.fillAmount = (float)currentHealth / maxHealth;
        }
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("【死亡】YOU ARE DEAD");

        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverVolume != null) gameOverVolume.SetActive(true);

        // プレイヤー移動・カメラ・インベントリを強制停止
        UpdateControlsState();

        // 慣性で滑らないように速度を0にする
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
        }

        // ボスを消去
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

        // 最大HP分回復させる（OnHealthChangedが発火しHP_Ringも自動で満タンに戻る）
        health.AddHeel(health.GetMaxHealth());

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameOverVolume != null) gameOverVolume.SetActive(false);

        // 復活時：各スクリプトを再有効化
        UpdateControlsState();
    }

    /// <summary>
    /// ポーズ状態の切り替え（SystemMenuManagerから呼び出す）
    /// </summary>
    public void SetPaused(bool paused)
    {
        isPaused = paused;
        UpdateControlsState();
    }

    // 死亡中でもポーズ中でもない場合のみプレイヤー操作を有効化する
    private void UpdateControlsState()
    {
        bool canControl = !isDead && !isPaused;

        Player playerScript = GetComponent<Player>();
        if (playerScript != null) playerScript.enabled = canControl;

        CharactorCamera camScript = GetComponentInChildren<CharactorCamera>();
        if (camScript != null) camScript.enabled = canControl;

        Inventory inventoryScript = FindAnyObjectByType<Inventory>();
        if (inventoryScript != null) inventoryScript.enabled = canControl;
    }
}