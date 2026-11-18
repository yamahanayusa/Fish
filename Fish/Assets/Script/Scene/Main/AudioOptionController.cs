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

    private void OnEnable()
    {
        // オプション画面が開かれるたびに保存値を読み込んでUIに反映する
        float savedMaster = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 1.0f);
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 1.0f);

        if (masterSlider != null) masterSlider.value = savedMaster;
        if (musicSlider != null) musicSlider.value = savedMusic;
        if (sfxSlider != null) sfxSlider.value = savedSFX;

        if (masterValueText != null) masterValueText.text = savedMaster.ToString("F2");
        if (musicValueText != null) musicValueText.text = savedMusic.ToString("F2");
        if (sfxValueText != null) sfxValueText.text = savedSFX.ToString("F2");
    }

    public void OnMasterVolumeChanged(float value)
    {
        if (masterValueText != null) masterValueText.text = value.ToString("F2");

        // SoundManager経由で実際の音量を変更
        if (SoundManager.Instance != null) SoundManager.Instance.SetMasterVolume(value);

        PlayerPrefs.SetFloat("MasterVolume", value);
        PlayerPrefs.Save();
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (musicValueText != null) musicValueText.text = value.ToString("F2");

        // SoundManager経由で実際の音量を変更
        if (SoundManager.Instance != null) SoundManager.Instance.SetMusicVolume(value);

        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (sfxValueText != null) sfxValueText.text = value.ToString("F2");

        // SoundManager経由で実際の音量を変更
        if (SoundManager.Instance != null) SoundManager.Instance.SetSFXVolume(value);

        PlayerPrefs.SetFloat("SFXVolume", value);
        PlayerPrefs.Save();
    }
}