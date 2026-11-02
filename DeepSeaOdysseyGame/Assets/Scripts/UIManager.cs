using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject mainMenuPanel;
    public GameObject hudPanel;
    public GameObject pausePanel;

    // Кнопка PLAY: Открывает HUD и запускает игру
    public void PlayGame()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(true);
        Time.timeScale = 1f; // Время идет обычным шагом
    }

    // Кнопка PAUSE: Нажимаем || в HUD
    public void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f; // Полная остановка времени в игре!
    }

    // Кнопка RESUME: Возобновляем игру из паузы
    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f; // Время снова идет
    }

    // Кнопка RESTART: Перезагружаем текущую сцену
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Кнопка MAIN MENU: Возврат в главное меню
    public void LoadMenu()
    {
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (hudPanel != null) hudPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    // Кнопка QUIT: Выход из приложения
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit!");
    }
}