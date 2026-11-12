using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioOptionController : MonoBehaviour
{
    [Header("スライダー設定")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("数値テキスト設定")]
    [SerializeField] private TextMeshProUGUI masterValueText;
    [SerializeField] private TextMeshProUGUI musicValueText;
    [SerializeField] private TextMeshProUGUI sfxValueText;

    private void Start()
    {
        // 起動時にスライダーの初期位置に合わせてテキストを「1.00」などに更新する
        if (masterSlider != null) OnMasterVolumeChanged(masterSlider.value);
        if (musicSlider != null) OnMusicVolumeChanged(musicSlider.value);
        if (sfxSlider != null) OnSFXVolumeChanged(sfxSlider.value);
    }

    // マスター音量スライダーが動いたときに呼ばれる関数
    public void OnMasterVolumeChanged(float value)
    {
        if (masterValueText != null) masterValueText.text = value.ToString("F2");
        Debug.Log($"【オーディオ設定】マスター音量を変更: {value:F2}");
    }

    // ミュージック音量スライダーが動いたときに呼ばれる関数
    public void OnMusicVolumeChanged(float value)
    {
        if (musicValueText != null) musicValueText.text = value.ToString("F2");
        Debug.Log($"【オーディオ設定】ミュージック音量を変更: {value:F2}");
    }

    // 効果音音量スライダーが動いたときに呼ばれる関数
    public void OnSFXVolumeChanged(float value)
    {
        if (sfxValueText != null) sfxValueText.text = value.ToString("F2");
        Debug.Log($"【オーディオ設定】効果音音量を変更: {value:F2}");
    }
}