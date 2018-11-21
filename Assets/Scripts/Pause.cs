using UnityEngine;

public class Pause : MonoBehaviour {
    /// <summary>
    /// Game object for the pause message.
    /// </summary>
    [SerializeField] private GameObject pausePanel;

    /// <summary>
    /// Flag indicating the game is paused.
    /// </summary>
    public static bool GamePaused;


    private void Start() {
        pausePanel.SetActive(false);
    }

    private void Update() {
        if (!Input.GetKeyDown(KeyCode.Escape)) {
            return;
        }
        if (GamePaused) {
            ContinueGame();
        }
        else {
            PauseGame();
        }
    }

    private void FixedUpdate() {
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    private void PauseGame() {
        // Disable scripts that still work while timescale is set to 0
        pausePanel.SetActive(true);
        Time.timeScale = 0;
        GamePaused = true;
    }

    /// <summary>
    /// Unpauses the game.
    /// </summary>
    private void ContinueGame() {
        // Enable the scripts again
        pausePanel.SetActive(false);
        Time.timeScale = 1;
        GamePaused = false;
    }
}