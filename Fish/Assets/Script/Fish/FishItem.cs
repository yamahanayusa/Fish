using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody))]
public class FishItem : MonoBehaviour
{
    [Header("データソース")]
    [SerializeField] private FishData fishData;

    [Header("拾う設定")]
    [Tooltip("拾えるプレイヤーとの最大距離")]
    [SerializeField] private float pickupDistance = 3.0f;

    [Header("手持ち時の位置オフセット")]
    [Tooltip("プレイヤーの基準位置からの相対オフセット（前・上など）")]
    [SerializeField] private Vector3 holdOffset = new Vector3(0f, 0.5f, 1.5f);

    public bool isHeld = false; // プレイヤーが持っているか
    private Transform holdParent;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // 拾われていない時のみ、拾う判定をチェック
        if (!isHeld && Input.GetMouseButtonDown(0))
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                float distance = Vector3.Distance(transform.position, playerObj.transform.position);
                if (distance <= pickupDistance)
                {
                    PickUp(playerObj.transform);
                }
            }
        }
        // 持っている時、Qボタンで捨てれるようにする
        else if(isHeld && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    private void LateUpdate()
    {
        // 持たれている最中は毎フレームプレイヤーの前方に追従
        if (isHeld && holdParent != null)
        {
            Vector3 targetPosition = holdParent.position
                                   + holdParent.forward * holdOffset.z
                                   + holdParent.up * holdOffset.y
                                   + holdParent.right * holdOffset.x;

            transform.position = targetPosition;
            transform.rotation = holdParent.rotation;
        }
    }

    /// <summary>
    /// 魚アイテムを拾う処理
    /// </summary>
    public void PickUp(Transform parent)
    {
        isHeld = true;
        holdParent = parent;

        // 物理演算と衝突判定をオフ
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        // 自身および子オブジェクトの全コライダーを無効化
        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            col.enabled = false;
        }

        string nameStr = fishData != null ? fishData.fishName : gameObject.name;
        Debug.Log($"【アイテム】{nameStr} を拾いました！");
    }

    /// <summary>
    /// 持っているアイテムを捨てる処理
    /// </summary>
    public void Drop()
    {
        isHeld = false;

        // プレイヤーの前方に少し話した位置に配置する
        if(holdParent != null)
        {
            transform.position = holdParent.position + holdParent.forward * 1.5f + Vector3.up * 0.5f;
        }

        holdParent = null;

        // コライダーを復活させる
        Collider[] cols = GetComponentsInChildren<Collider>();
        foreach (Collider col in cols)
        {
            col.enabled = true;
        }

        // 物理演算を復活させる
        if(rb != null)
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
        }

        Debug.Log("アイテムを捨てました!");
    }

    /// <summary>
    /// アイテムの初期化
    /// </summary>
    public void Initialize(FishData data)
    {
        fishData = data;
    }
}