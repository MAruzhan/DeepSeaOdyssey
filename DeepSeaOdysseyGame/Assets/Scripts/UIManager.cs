using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject pausePanel;

    // Метод Start вызывается автоматически в первую секунду запуска игры
    void Start()
    {
        ShowMainMenu();
    }

    // Включаем только Главное Меню
    public void ShowMainMenu()
    {
        Time.timeScale = 1f; // Время идет обычным шагом
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);  // Показываем Главное Меню
        if (hudPanel != null) hudPanel.SetActive(false);           // Скрываем HUD
        if (pausePanel != null) pausePanel.SetActive(false);       // Скрываем Паузу
    }

    // Кнопка PLAY: Открывает HUD и выключает Главное меню
    public void PlayGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        Time.timeScale = 1f;
    }

    // Кнопка PAUSE: Нажимаем || в HUD
    public void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f; // Остановка времени
    }

    // Кнопка RESUME: Продолжить игру
    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    // Кнопка RESTART: Перезагрузка сцены
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Кнопка MAIN MENU: Возврат в главное меню
    public void LoadMenu()
    {
        ShowMainMenu();
    }

    // Кнопка QUIT: Выход
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}