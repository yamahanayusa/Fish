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

    [Header("参照・プレハブ")]
    [SerializeField] private GameObject lurePrefab;
    [SerializeField] private Transform rodTipTransform;

    [Header("ルアー別ドロップテーブルリスト")]
    [SerializeField] private List<FishingLureTable> lureTables;

    [Header("パラメータ")]
    [SerializeField] private float castForce = 10f;
    [SerializeField] private float reelSpeed = 3f;
    [SerializeField] private float fastReelSpeed = 6f;

    private GameObject currentLureObj;
    private Lure currentLure;
    private GameObject activeFishObj;

    private float lastClickTime = 0f;
    private float multiClickThreshold = 0.25f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Update()
    {
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

        if (CurrentState == FishingState.Idle && isRightPressed)
        {
            CurrentState = FishingState.Aiming;
            Debug.Log("【状態】竿を構えました");
        }

        if (CurrentState == FishingState.Aiming && isRightReleased)
        {
            CastLure();
        }

        if (CurrentState == FishingState.WaitingInWater && isLeftPressed)
        {
            if (currentLure != null && rodTipTransform != null)
            {
                currentLure.ReelTowards(rodTipTransform.position, reelSpeed * Time.deltaTime);
            }
        }

        if (CurrentState == FishingState.ReelingFight)
        {
            float speed = reelSpeed;

            if (isLeftDownThisFrame)
            {
                if (Time.time - lastClickTime < multiClickThreshold) speed = fastReelSpeed;
                lastClickTime = Time.time;
            }
            else if (isLeftPressed)
            {
                speed = reelSpeed;
            }

            if (isLeftPressed || isLeftDownThisFrame)
            {
                ReelFishIn(speed);
            }
        }

        // ★ 追加：陸揚げ完了後に右クリックでリリース（AI起動）
        if (CurrentState == FishingState.Caught && isRightDownThisFrame)
        {
            ReleaseFish();
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

            // 釣られている間はAIと重力をオフにする
            var fishAI = activeFishObj.GetComponent<Fish>();
            if (fishAI != null) fishAI.enabled = false;

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
    /// ★ 追加：魚をルアーから離してAIを起動する処理
    /// </summary>
    private void ReleaseFish()
    {
        if (activeFishObj != null)
        {
            // 物理演算（重力）を再開
            Rigidbody fishRb = activeFishObj.GetComponent<Rigidbody>();
            if (fishRb != null) fishRb.isKinematic = false;

            // AI挙動（Fish.cs）をONにする
            Fish fishAI = activeFishObj.GetComponent<Fish>();
            if (fishAI != null) fishAI.enabled = true;

            Debug.Log($"【リリース】{activeFishObj.name} を地面に落とし、AIを起動しました！");

            // FishingManagerの管理から外す（Reset時に消去されないようにする）
            activeFishObj = null;
        }

        // ルアーだけをリセットして次の釣りに備える
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