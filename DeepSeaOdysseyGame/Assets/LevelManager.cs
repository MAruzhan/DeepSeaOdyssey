using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [Header("Данные 3 уровней")]
    public LevelData[] allLevels; // Сюда перетащишь Level_01, Level_02, Level_03
    public LevelData currentLevel;

    [Header("UI элементы меню")]
    public Button[] levelButtons; // 3 кнопки уровней
    public GameObject confirmResetPanel; // Панель подтверждения сброса
    public TextMeshProUGUI bestScoreText;
    public Slider volumeSlider;

    [Header("Панели экранов")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;

    void Start()
    {
        if (SaveManager.Instance != null && volumeSlider != null)
        {
            volumeSlider.value = SaveManager.Instance.data.musicVolume;
        }
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (SaveManager.Instance == null) return;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int levelNum = i + 1;
            bool isUnlocked = SaveManager.Instance.data.unlockedLevels.Contains(levelNum);
            levelButtons[i].interactable = isUnlocked;
        }

        if (bestScoreText != null)
        {
            bestScoreText.text = "BEST SCORE: " + SaveManager.Instance.data.bestScore;
        }
    }

    // Привязать к кнопкам Level 1 (0), Level 2 (1), Level 3 (2)
    public void StartLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= allLevels.Length) return;

        currentLevel = allLevels[levelIndex];
        Debug.Log("Загружен уровень: " + currentLevel.levelName + " | Цель: " + currentLevel.targetScore);

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
    }

    // Вызывать при победе на уровне (можно повесить на тестовую кнопку "Win Level" в HUD для проверки)
    public void CompleteCurrentLevel()
    {
        if (currentLevel == null) currentLevel = allLevels[0];

        var save = SaveManager.Instance.data;
        if (currentLevel.targetScore > save.bestScore)
        {
            save.bestScore = currentLevel.targetScore;
        }

        int nextLevel = currentLevel.levelNumber + 1;
        if (nextLevel <= allLevels.Length && !save.unlockedLevels.Contains(nextLevel))
        {
            save.unlockedLevels.Add(nextLevel);
        }

        SaveManager.Instance.SaveGame(); // Сохраняем после прохождения уровня
        RefreshUI();

        if (hudPanel != null) hudPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void OnVolumeChanged(float val)
    {
        if (SaveManager.Instance == null) return;
        SaveManager.Instance.data.musicVolume = val;
        AudioListener.volume = val;
        SaveManager.Instance.SaveGame(); // Сохраняем при изменении настроек
    }

    public void OpenResetConfirm()
    {
        if (confirmResetPanel != null) confirmResetPanel.SetActive(true);
    }

    public void ConfirmResetProgress()
    {
        SaveManager.Instance.ResetToDefaults();
        RefreshUI();
        if (confirmResetPanel != null) confirmResetPanel.SetActive(false);
    }

    public void CancelReset()
    {
        if (confirmResetPanel != null) confirmResetPanel.SetActive(false);
    }
}