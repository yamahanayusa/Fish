using UnityEngine;

public enum FishingState
{
    Idle,       // 待機
    Casting,    // キャスト中（ルアー飛翔中）
    Waiting,    // アタリ待ち（着水後）
    Reeling,    // 格闘・巻き取り中
    Caught,     // 釣り上げ成功
    Failed      // 失敗・糸切れ
}

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance { get; private set; }

    [Header("現在の状態")]
    public FishingState CurrentState { get; private set; } = FishingState.Idle;

    [Header("参照・プレハブ")]
    [SerializeField] private GameObject lurePrefab;
    [SerializeField] private Transform rodTipTransform; // 竿の先端位置（仮）

    [Header("キャスト設定")]
    [SerializeField] private float castForce = 12f;

    private GameObject currentLureObj;
    private Fish activeFish;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        // デバッグテスト用：スペースキーでキャスト実行
        if (Input.GetKeyDown(KeyCode.Space) && CurrentState == FishingState.Idle)
        {
            CastLure(rodTipTransform != null ? rodTipTransform.position : transform.position + Vector3.up);
        }
    }

    /// <summary>
    /// キャスト処理（別メンバーのプレイヤーから呼ぶことも可能）
    /// </summary>
    public void CastLure(Vector3 spawnPosition)
    {
        if (CurrentState != FishingState.Idle) return;

        CurrentState = FishingState.Casting;
        Debug.Log("キャスト！ルアーを投げます。");

        // ルアーの生成
        currentLureObj = Instantiate(lurePrefab, spawnPosition, Quaternion.identity);
        Rigidbody lureRb = currentLureObj.GetComponent<Rigidbody>();

        if (lureRb != null)
        {
            // 斜め前上へ飛ばす
            Vector3 throwDirection = (transform.forward + Vector3.up * 0.5f).normalized;
            lureRb.AddForce(throwDirection * castForce, ForceMode.Impulse);
        }

        // 仮で「1秒後に着水した」とする（実際は水面ColliderのTrigger検知等で行う）
        Invoke(nameof(SimulateWaterLanding), 1.2f);
    }

    private void SimulateWaterLanding()
    {
        if (CurrentState != FishingState.Casting) return;

        CurrentState = FishingState.Waiting;
        Lure lure = currentLureObj.GetComponent<Lure>();
        if (lure != null)
        {
            lure.OnLandOnWater();
        }
    }

    /// <summary>
    /// 魚が食いついた時（Lure.cs から通知される）
    /// </summary>
    public void OnFishHit(Fish fish, Lure lure)
    {
        CurrentState = FishingState.Reeling;
        activeFish = fish;

        Debug.Log($"ファイト開始！ ターゲット: {fish.name}");
        // ここから巻き取りミニゲーム・テンション計算へ遷移
    }

    /// <summary>
    /// ヒットしなかった場合
    /// </summary>
    public void OnHitFailed()
    {
        CurrentState = FishingState.Failed;
        ResetFishing();
    }

    /// <summary>
    /// リセット処理
    /// </summary>
    public void ResetFishing()
    {
        if (currentLureObj != null) Destroy(currentLureObj);
        activeFish = null;
        CurrentState = FishingState.Idle;
        Debug.Log("釣りをリセットし、待機状態に戻りました。");
    }
}