using UnityEngine;

public class InkBullet : MonoBehaviour
{
    [Tooltip("画面を真っ黒にする秒数")]
    [SerializeField] private float inkDuration = 3f;

    [Tooltip("何かに当たらなかった場合に自動で消滅するまでの秒数")]
    [SerializeField] private float lifeTime = 5f;

    private void Start()
    {
        // 生成されてから lifeTime（5秒）後に自動で消滅させる（外れた弾の掃除）
        Destroy(gameObject, lifeTime);
    }

    // Is Trigger にチェックを入れた場合は OnTriggerEnter を使います
    private void OnTriggerEnter(Collider other)
    {
        // 撃ったボス自身（SquidBoss）に触れた場合は何もせずすり抜ける
        if (other.GetComponent<SquidBoss>() != null) return;

        // プレイヤーに当たった場合
        if (other.CompareTag("Player"))
        {
            if (InkScreen.Instance != null)
            {
                InkScreen.Instance.ShowInk(inkDuration);
            }
            Destroy(gameObject); // 当たったら消える
        }
        // プレイヤー以外（地面や木など）に当たった場合
        else
        {
            Destroy(gameObject); // 当たったら消える
        }
    }
}