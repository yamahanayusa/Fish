using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class SoundData
    {
        public string name;    // 呼び出し用の名前（例: "TitleBGM", "AttackSE"）
        public AudioClip clip; // 音声ファイル
    }

    [Header("ミキサー設定")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicMixerGroup;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    [Header("サウンドリスト（今後ここに＋で追加）")]
    [SerializeField] private List<SoundData> bgmList = new List<SoundData>();
    [SerializeField] private List<SoundData> seList = new List<SoundData>();

    [Header("動作確認用（最初の1音テスト）")]
    [SerializeField] private string playBgmOnStart = "";

    private AudioSource bgmSource;
    private AudioSource seSource;
    private Dictionary<string, AudioClip> bgmDict = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> seDict = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ゲーム起動時に保存済みの音量をAudioMixerへ即座に適用する
        SetMasterVolume(PlayerPrefs.GetFloat("MasterVolume", 1.0f));
        SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1.0f));
        SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1.0f));

        // テスト用BGMが指定されていれば再生
        if (!string.IsNullOrEmpty(playBgmOnStart))
        {
            PlayBGM(playBgmOnStart);
        }
    }

    private void Initialize()
    {
        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.outputAudioMixerGroup = musicMixerGroup;
        bgmSource.loop = true;

        seSource = gameObject.AddComponent<AudioSource>();
        seSource.outputAudioMixerGroup = sfxMixerGroup;

        foreach (var bgm in bgmList)
        {
            if (bgm.clip != null && !bgmDict.ContainsKey(bgm.name))
                bgmDict.Add(bgm.name, bgm.clip);
        }

        foreach (var se in seList)
        {
            if (se.clip != null && !seDict.ContainsKey(se.name))
                seDict.Add(se.name, se.clip);
        }
    }

    // ==========================================
    // 音量調整（0.0f ～ 1.0f をデシベルに変換して適用）
    // ==========================================
    public void SetMasterVolume(float value)
    {
        SetMixerVolume("MasterVolume", value);
    }

    public void SetMusicVolume(float value)
    {
        SetMixerVolume("MusicVolume", value);
    }

    public void SetSFXVolume(float value)
    {
        SetMixerVolume("SFXVolume", value);
    }

    private void SetMixerVolume(string parameterName, float linearValue)
    {
        if (audioMixer == null) return;

        // 0のときにLog10が無限小になるのを防ぎ、-80dB(無音)〜0dB(最大)に変換
        float clampedValue = Mathf.Clamp(linearValue, 0.0001f, 1.0f);
        float dB = Mathf.Log10(clampedValue) * 20f;
        audioMixer.SetFloat(parameterName, dB);
    }

    // ==========================================
    // 音の再生（今後スクリプトから呼ぶ窓口）
    // ==========================================
    public void PlayBGM(string name)
    {
        if (bgmDict.TryGetValue(name, out AudioClip clip))
        {
            if (bgmSource.clip == clip && bgmSource.isPlaying) return;
            bgmSource.clip = clip;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"BGM '{name}' がSoundManagerに登録されていません。");
        }
    }

    public void PlaySE(string name)
    {
        if (seDict.TryGetValue(name, out AudioClip clip))
        {
            seSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SE '{name}' がSoundManagerに登録されていません。");
        }
    }
}