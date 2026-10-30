using System.Collections.Generic;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public enum FishingState
{
    Idle,           // 待機中
    Aiming,         // 右クリック構え中
    Casting,        // ルアー飛翔中
    WaitingInWater, // 着水～誘い
    ReelingFight,   // ヒット後の格闘
    Caught          // 陸揚げ完了
}

public class FishingManager : MonoBehaviour
{
    public static FishingManager Instance { get; private set; }

    [Header("現在の状態")]
    public FishingState CurrentState { get; private set; } = FishingState.Idle;

    [Header("ルアー設定")]
    [Tooltip("現在のルアーID")]
    public string currentlureID = "NormalLure";

    [Header("参照・プレハブ")]
    [SerializeField] private GameObject lurePrefab;
    [SerializeField] private Transform rodTipTransform;

    [Header("ルアー別ドロップテーブルリスト")]
    [SerializeField] private List<FishingLureTable> lureTables;

    [Header("パラメータ")]
    [SerializeField] private float castForce = 10f;
    [SerializeField] private float reelSpeed = 3f;
    [SerializeField] private float fastReelSpeed = 6f;
    [SerializeField] private float waterLevelY = -3f;

    private GameObject currentLureObj;
    private Lure currentLure;
    private GameObject activeFishObj;

    private float lastClickTime = 0f;
    private float multiClickThreshold = 0.3f;
    private float fastReelEndTime = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
        //ルアーの切り替え
        if(Input.GetKeyDown(KeyCode.B))
        {
            if(currentlureID=="NormalLure")
            {
                currentlureID= "BossLure";
                Debug.Log("[ルアー変更] ボス用ルアーに切り替えました");
            }
            else
            {
                currentlureID= "NormalLure";
                Debug.Log("[ルアー変更] 通常ルアーに切り替えました");
            }
        }

        HandleInput();
    }

    private void HandleInput()
    {
        bool isRightPressed = false, isRightReleased = false, isRightDownThisFrame = false;
        bool isLeftPressed = false, isLeftDownThisFrame = false;

#if ENABLE_INPUT_SYSTEM
        if (Mouse.current != null)
        {
            isRightPressed = Mouse.current.rightButton.isPressed;
            isRightReleased = Mouse.current.rightButton.wasReleasedThisFrame;
            isRightDownThisFrame = Mouse.current.rightButton.wasPressedThisFrame;
            isLeftPressed = Mouse.current.leftButton.isPressed;
            isLeftDownThisFrame = Mouse.current.leftButton.wasPressedThisFrame;
        }
#else
        isRightPressed = Input.GetMouseButton(1);
        isRightReleased = Input.GetMouseButtonUp(1);
        isRightDownThisFrame = Input.GetMouseButtonDown(1);
        isLeftPressed = Input.GetMouseButton(0);
        isLeftDownThisFrame = Input.GetMouseButtonDown(0);
#endif

        // 1. まず陸揚げ完了後のリリース処理を判定
        if ((CurrentState == FishingState.Caught || CurrentState == FishingState.ReelingFight) && isRightDownThisFrame)
        {
            ReleaseFish();
            return; // 魚を離したフレームはここで処理を終了し、構えへの誤爆を防ぐ
        }

        // 2. 待機中からの構え判定
        if (CurrentState == FishingState.Idle && isRightDownThisFrame)
        {
            CurrentState = FishingState.Aiming;
            Debug.Log("【状態】竿を構えました");
        }

        // 3. 構え中 -> キャスト（離した瞬間）
        if (CurrentState == FishingState.Aiming && isRightReleased)
        {
            CastLure();
        }

        // 4. 着水後の引き寄せ
        if (CurrentState == FishingState.WaitingInWater && isLeftPressed)
        {
            if (currentLure != null && rodTipTransform != null)
            {
                currentLure.ReelTowards(rodTipTransform.position, reelSpeed * Time.deltaTime);
            }
        }

        // 5. ヒット後の格闘
        if (CurrentState == FishingState.ReelingFight)
        {
            // ① クリックされた瞬間の判定（連打チェック）
            if (isLeftDownThisFrame)
            {
                if (Time.time - lastClickTime < multiClickThreshold)
                {
                    // 連打成功！0.3秒間は高速巻き取りモードを維持する
                    fastReelEndTime = Time.time + 0.3f;
                }
                lastClickTime = Time.time;
            }

            // ② スピードの決定（余韻中は常に高速）
            float currentSpeed = reelSpeed;
            bool isFastReeling = (Time.time < fastReelEndTime);

            if (isFastReeling)
            {
                currentSpeed = fastReelSpeed;
            }

            // ③ 巻き取り実行（長押し中、または連打モード中は巻き取る）
            if (isLeftPressed || isFastReeling)
            {
                ReelFishIn(currentSpeed);
            }
        }
    }

    private void CastLure()
    {
        CurrentState = FishingState.Casting;
        Debug.Log("【状態】キャスト！");

        Vector3 spawnPos = rodTipTransform != null ? rodTipTransform.position : transform.position + Vector3.up;
        currentLureObj = Instantiate(lurePrefab, spawnPos, Quaternion.identity);
        currentLure = currentLureObj.GetComponent<Lure>();

        Rigidbody lureRb = currentLureObj.GetComponent<Rigidbody>();
        if (lureRb != null)
        {
            Vector3 playerForward = Vector3.forward;

            if (rodTipTransform != null)
            {
                playerForward = rodTipTransform.parent != null ? rodTipTransform.parent.forward : rodTipTransform.forward;
            }

            Vector3 throwDir = (playerForward + Vector3.up * 0.4f).normalized;
            lureRb.AddForce(throwDir * castForce, ForceMode.Impulse);
        }
    }

    public void OnLureLanded()
    {
        if (CurrentState != FishingState.Casting) return;
        CurrentState = FishingState.WaitingInWater;
        Debug.Log("【状態】着水。左長押しで巻き取ってください。");
    }

    public void OnFishHitAtPosition(Vector3 hitPosition, string lureID)
    {
        if (CurrentState != FishingState.WaitingInWater) return;

        CurrentState = FishingState.ReelingFight;

        FishingLureTable targetTable = lureTables.Find(t => t.lureID == lureID);
        if (targetTable != null)
        {
            activeFishObj = targetTable.SpawnRandomFish(hitPosition, Quaternion.identity);

            // Fish と AttackingFish の両方を探して、ついていればOFFにする
            var fishAI = activeFishObj.GetComponent<Fish>();
            if (fishAI != null) fishAI.enabled = false;

            var attackingAI = activeFishObj.GetComponent<AttackingFish>();
            if (attackingAI != null) attackingAI.enabled = false;

            var fishRb = activeFishObj.GetComponent<Rigidbody>();
            if (fishRb != null) fishRb.isKinematic = true;

            Debug.Log($"【ヒット！】{activeFishObj.name} が表示されました！");
        }
    }

    public void OnLureRetrieved()
    {
        if (CurrentState != FishingState.WaitingInWater) return;
        ResetFishing();
    }

    private void ReelFishIn(float speed)
    {
        if (activeFishObj == null || rodTipTransform == null) return;

        Vector3 targetPos = rodTipTransform.position;
        activeFishObj.transform.position = Vector3.MoveTowards(activeFishObj.transform.position, targetPos, speed * Time.deltaTime);

        if (currentLureObj != null)
        {
            currentLureObj.transform.position = activeFishObj.transform.position;
        }

        float distance = Vector3.Distance(activeFishObj.transform.position, targetPos);
        if (distance < 1.5f)
        {
            OnCatchSuccess();
        }
    }

    private void OnCatchSuccess()
    {
        CurrentState = FishingState.Caught;
        Debug.Log("【釣獲成功！】右クリックで魚を離します。");
        // 自動リセットを廃止
    }

    /// <summary>
    /// 魚をルアーから離してAIを起動する処理
    /// </summary>
    private void ReleaseFish()
    {
        if (activeFishObj != null)
        {
            if (activeFishObj.transform.position.y <= waterLevelY)
            {
                Debug.Log($"【リリース】{activeFishObj.name} は海へ逃げていきました。（モデル消去）");
                Destroy(activeFishObj);
            }
            else
            {
                Rigidbody fishRb = activeFishObj.GetComponent<Rigidbody>();
                if (fishRb != null) fishRb.isKinematic = false;

                // ★追加：ボスの場合はボス戦を開始する
                SquidBoss bossAI = activeFishObj.GetComponent<SquidBoss>();
                if (bossAI != null)
                {
                    bossAI.StartBossBattle();
                    Debug.Log("【リリース】ボス戦スタート！");
                }
                else
                {
                    // 通常の魚AIの起動
                    Fish fishAI = activeFishObj.GetComponent<Fish>();
                    if (fishAI != null) fishAI.enabled = true;

                    AttackingFish attackingAI = activeFishObj.GetComponent<AttackingFish>();
                    if (attackingAI != null) attackingAI.enabled = true;

                    Debug.Log($"【リリース】{activeFishObj.name} を地面に落とし、AIを起動しました！");
                }
            }
            activeFishObj = null;
        }
        ResetFishing();
    }

    public void ResetFishing()
    {
        if (currentLureObj != null) Destroy(currentLureObj);

        // 陸揚げ完了前（途中でキャンセルされた場合など）のみ魚モデルを消去
        if (activeFishObj != null) Destroy(activeFishObj);

        activeFishObj = null;
        CurrentState = FishingState.Idle;
        Debug.Log("【リセット】待機状態（Idle）に戻りました。");
    }

    public Transform GetRodTip() => rodTipTransform;
    public GameObject GetCurrentLure() => currentLureObj;
}