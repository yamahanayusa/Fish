using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody))]
public class SquidBoss : MonoBehaviour
{
    [Header("ステータス")]
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;

    [Header("攻撃設定")]
    [SerializeField] private float attackInterval = 3f; // 攻撃の頻度
    [SerializeField] private float dashForce = 15f;     // 突進の力

    [Header("イカ墨設定")]
    [SerializeField] private GameObject inkPrefab;      // 発射する墨のプレハブ
    [SerializeField] private Transform inkSpawnPoint;   // 墨を発射する位置（口元など）
    [SerializeField] private float inkSpeed = 20f;

    private Rigidbody rb;
    private Transform player;
    private bool isBattling = false;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();

        // プレイヤーを取得
        GameObject playerObj = GameObject.Find("PlayerCube");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        // プレイヤーの攻撃未実装のため、仮で「Kキー」を押すとダメージを受ける
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(20f);
        }
    }

    // 釣り上げられた（リリースされた）時に外部（FishingManager等）から呼ぶ
    public void StartBossBattle()
    {
        if (!isBattling)
        {
            isBattling = true;
            Debug.Log("【ボス戦開始】イカが戦闘態勢に入った！");
            StartCoroutine(BossAttackRoutine());
        }
    }

    private IEnumerator BossAttackRoutine()
    {
        while (currentHealth > 0 && isBattling)
        {
            yield return new WaitForSeconds(attackInterval);

            // 0か1をランダムに出して攻撃パターンを分岐
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
        Debug.Log("【ボス攻撃】突進！");
        if (player != null)
        {
            // プレイヤーの方向を向いて突っ込む
            Vector3 direction = (player.position - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);
            rb.AddForce(direction * dashForce, ForceMode.Impulse);
        }
    }

    private void InkAttack()
    {
        Debug.Log("【ボス攻撃】イカ墨！");
        if (inkPrefab != null && inkSpawnPoint != null && player != null)
        {
            // 修正：プレイヤーの足元ではなく、高さ(Y)を少し上（胸のあたり）にずらして狙う
            Vector3 targetPos = player.position + Vector3.up * 0.5f;
            Vector3 direction = (targetPos - inkSpawnPoint.position).normalized;

            transform.rotation = Quaternion.LookRotation(direction);

            // イカ墨の弾を生成して飛ばす
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
        Debug.Log($"【ボス】ダメージを受けた！ 残りHP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isBattling = false;
        Debug.Log("【ボス討伐】イカを倒した！");

        // ボス自身を消去
        Destroy(gameObject);

        // Clearシーンへ移動する
        SceneManager.LoadScene("Clear");
    }
}