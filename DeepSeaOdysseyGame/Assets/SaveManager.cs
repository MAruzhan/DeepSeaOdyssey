using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameSaveData
{
    public int version = 2;
    public int bestScore = 0;
    public List<int> unlockedLevels = new List<int> { 1 };
    public float musicVolume = 0.8f;
    public float sfxVolume = 0.8f;
}

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    public GameSaveData data = new GameSaveData();
    private string savePath;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            savePath = Path.Combine(Application.persistentDataPath, "save.json");
            LoadGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveGame()
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);
            Debug.Log("Сохранено в: " + savePath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Ошибка сохранения: " + e.Message);
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(savePath))
        {
            ResetToDefaults();
            return;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            GameSaveData loaded = JsonUtility.FromJson<GameSaveData>(json);
            if (loaded == null || loaded.unlockedLevels == null || loaded.unlockedLevels.Count == 0)
            {
                throw new Exception("Повреждённый JSON");
            }
            data = loaded;
            Debug.Log("Сохранение успешно загружено!");
        }
        catch (Exception e)
        {
            Debug.LogWarning("Файл save.json сломан, сбрасываем на стандартные: " + e.Message);
            ResetToDefaults();
            SaveGame();
        }
    }

    public void ResetToDefaults()
    {
        data = new GameSaveData();
        SaveGame();
    }

    // Сохраняем автоматически при сворачивании приложения на телефоне
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveGame();
        }
    }
}