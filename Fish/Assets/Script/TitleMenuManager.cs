using UnityEngine;

public class TitleMenuManager : MonoBehaviour
{
    [Header("UIパネル設定")]
    [SerializeField] private GameObject optionsPanel;

    [Header("タイトルメインUI設定")]
    [SerializeField] private GameObject titleText;   // 左上のタイトルロゴ
    [SerializeField] private GameObject buttonGroup; // 左下のボタン群 (START/OPTION/EXIT)

    [Header("オプションサブパネル設定")]
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject gamePlayPanel;
    [SerializeField] private GameObject graphicPanel;
    [SerializeField] private GameObject languagePanel;

    // オプションを開く（OPTIONボタン用）
    public void OpenOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);

        // タイトルロゴとボタン群を非表示にする
        if (titleText != null) titleText.SetActive(false);
        if (buttonGroup != null) buttonGroup.SetActive(false);

        // オプションを開いた直後はサブパネルをすべて非表示にする
        HideAllOptionPanels();
    }

    // オプションを閉じる（Closeボタン用）
    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);

        // タイトルロゴとボタン群を再表示する
        if (titleText != null) titleText.SetActive(true);
        if (buttonGroup != null) buttonGroup.SetActive(true);

        // サブパネルもすべて閉じておく
        HideAllOptionPanels();
    }

    // 「オーディオ」ボタンが押されたとき
    public void OnClickAudioTab()
    {
        HideAllOptionPanels();
        if (audioPanel != null) audioPanel.SetActive(true);
    }

    // 「ゲームプレイ」ボタンが押されたとき（将来用）
    public void OnClickGamePlayTab()
    {
        HideAllOptionPanels();
        if (gamePlayPanel != null) gamePlayPanel.SetActive(true);
    }

    // 「グラフィック」ボタンが押されたとき（将来用）
    public void OnClickGraphicTab()
    {
        HideAllOptionPanels();
        if (graphicPanel != null) graphicPanel.SetActive(true);
    }

    // 「言語」ボタンが押されたとき（将来用）
    public void OnClickLanguageTab()
    {
        HideAllOptionPanels();
        if (languagePanel != null) languagePanel.SetActive(true);
    }

    // すべてのオプションサブパネルを非表示にする補助処理
    private void HideAllOptionPanels()
    {
        if (audioPanel != null) audioPanel.SetActive(false);
        if (gamePlayPanel != null) gamePlayPanel.SetActive(false);
        if (graphicPanel != null) graphicPanel.SetActive(false);
        if (languagePanel != null) languagePanel.SetActive(false);
    }

    // ゲーム終了（EXITボタン用）
    public void QuitGame()
    {
        Debug.Log("ゲームを終了します");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}