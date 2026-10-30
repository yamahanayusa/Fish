using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InkScreen : MonoBehaviour
{
    // 他のスクリプトから簡単に呼び出せるようにする
    public static InkScreen Instance { get; private set; }

    private Image inkImage;

    private void Awake()
    {
        Instance = this;
        inkImage = GetComponent<Image>();
        // ゲーム開始時は必ず非表示にする
        inkImage.enabled = false;
    }

    // 墨を浴びた時に外部から呼ばれる関数
    public void ShowInk(float duration)
    {
        StopAllCoroutines(); // 連続で当たった場合、前のタイマーをキャンセル
        StartCoroutine(InkRoutine(duration));
    }

    private IEnumerator InkRoutine(float duration)
    {
        inkImage.enabled = true; // 画面を真っ黒にする
        Debug.Log("【視界ジャック】前が見えない！");

        yield return new WaitForSeconds(duration); // 指定秒数待機

        inkImage.enabled = false; // 元に戻す
        Debug.Log("【視界ジャック】視界がクリアになった");
    }
}