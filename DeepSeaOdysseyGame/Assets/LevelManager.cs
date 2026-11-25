using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour
{
    [Header("Данные 3 уровней")]
    public LevelData[] allLevels;
    public LevelData currentLevel;

    [Header("Префабы врагов для спавна")]
    public GameObject patrollerPrefab;
    public GameObject shooterPrefab;
    public GameObject chargerPrefab;

    [Header("UI элементы меню")]
    public Button[] levelButtons;
    public GameObject confirmResetPanel;
    public TextMeshProUGUI bestScoreText;
    public Slider volumeSlider;
    public Slider sfxSlider;

    [Header("Панели экранов")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Start()
    {
        if (SaveManager.Instance != null)
        {
            if (volumeSlider != null) volumeSlider.value = SaveManager.Instance.data.musicVolume;
            if (sfxSlider != null) sfxSlider.value = SaveManager.Instance.data.sfxVolume;
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

    public void StartLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= allLevels.Length) return;

        currentLevel = allLevels[levelIndex];
        Debug.Log("Загружен уровень: " + currentLevel.levelName + " | Цель: " + currentLevel.targetScore);

        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null) Destroy(enemy);
        }
        spawnedEnemies.Clear();

        SpawnEnemies(patrollerPrefab, currentLevel.patrollerCount, new Vector2(-2.2f, 2.8f), new Vector2(1.8f, 0f));
        SpawnEnemies(shooterPrefab, currentLevel.shooterCount, new Vector2(2.2f, 2.2f), new Vector2(0f, -1.8f));
        SpawnEnemies(chargerPrefab, currentLevel.chargerCount, new Vector2(2.4f, -1.2f), new Vector2(-1.8f, -1.2f));

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);

        if (AudioManager.Instance != null) AudioManager.Instance.SwitchToLevelMusic();
    }

    void SpawnEnemies(GameObject prefab, int count, Vector2 basePos, Vector2 offset)
    {
        if (prefab == null) return;
        for (int i = 0; i < count; i++)
        {
            Vector3 pos = new Vector3(basePos.x + (offset.x * i), basePos.y + (offset.y * i), 0f);
            GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
            spawnedEnemies.Add(obj);
        }
    }

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

        SaveManager.Instance.SaveGame();
        RefreshUI();

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWin();
            AudioManager.Instance.SwitchToMenuMusic();
        }

        if (hudPanel != null) hudPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void OnVolumeChanged(float val)
    {
        if (AudioManager.Instance != null) AudioManager.Instance.SetMusicVolume(val);
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