using UnityEngine;
using TMPro;

public class EndRollController : MonoBehaviour
{
    [Header("シーン遷移設定")]
    [SerializeField] private SceneChanger sceneChanger;
    [SerializeField] private string titleSceneName = "Title";

    [Header("エンドロール設定")]
    [SerializeField] private RectTransform creditTextRect;
    [SerializeField] private float scrollSpeed = 80f;
    [Tooltip("このY座標まで上がったらスクロールを止める（テキスト量に合わせて調整）")]
    [SerializeField] private float endPosY = 1200f;
    [Tooltip("チェックを入れると、最後までスクロールし切ったときに自動でタイトルへ戻る")]
    [SerializeField] private bool autoReturnOnEnd = false;

    [Header("案内テキスト設定（任意）")]
    [SerializeField] private TextMeshProUGUI guideText;
    [SerializeField] private float blinkSpeed = 2f;

    private bool isTransitioning = false;

    private void Start()
    {
        // 万が一TimeScaleが0になっていた場合でも確実にスクロールさせる
        Time.timeScale = 1f;

        // 同じオブジェクトにSceneChangerがついていれば自動取得
        if (sceneChanger == null)
        {
            sceneChanger = GetComponent<SceneChanger>();
        }
    }

    private void Update()
    {
        // 1. クレジット文字を上にスクロールさせる
        if (creditTextRect != null)
        {
            Vector2 pos = creditTextRect.anchoredPosition;
            if (pos.y < endPosY)
            {
                pos.y += scrollSpeed * Time.deltaTime;
                creditTextRect.anchoredPosition = pos;
            }
            else if (autoReturnOnEnd)
            {
                ReturnToTitle();
            }
        }

        // 2. 案内テキストをゆっくり点滅させる
        if (guideText != null)
        {
            Color color = guideText.color;
            color.a = Mathf.PingPong(Time.time * blinkSpeed, 1f);
            guideText.color = color;
        }

        // 3. スペースキーでタイトルシーンへ遷移
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ReturnToTitle();
        }
    }

    private void ReturnToTitle()
    {
        if (isTransitioning) return;
        isTransitioning = true;

        if (sceneChanger != null)
        {
            sceneChanger.LoadTargetScene(titleSceneName);
        }
        else
        {
            Debug.LogWarning("SceneChangerが割り当てられていません。");
        }
    }
}