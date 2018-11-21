using TMPro;
using UnityEngine;

/// <summary>
/// Controller class for the UI.
/// </summary>
public class UIController : MonoBehaviour {
    /// <summary>
    /// Textfield for time.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _timeText;

    /// <summary>
    /// Textfield for score.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _scoreText;

    /// <summary>
    /// Textfield for lives.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _livesText;

    /// <summary>
    /// The time's up Textfield.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _timeIsUpMessage;

    /// <summary>
    /// The game result Textfield.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _gameResultMessage;


    /// <summary>
    /// A reference to the game controller, in order to notify out of time.
    /// </summary>
    private GameController _gameController;

    // Use this for initialization
    private void Start() {
        _gameController = FindObjectOfType<GameController>();
        _timeIsUpMessage.gameObject.SetActive(false); // Disable the time's up message
        _gameResultMessage.gameObject.SetActive(false); // Disable the game result message
    }

    // Update is called once per frame
    private void Update() {
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI.
    /// </summary>
    private void UpdateUI() {
       // var timeRemaining = (int) _gameController.GetTimeRemaining();
        //var actualScore = _gameController.GetScore();
        //var livesRemaining = _gameController.GetLives();
        // Values can go below zero in special cases
        //_timeText.SetText("Time\n" + (timeRemaining < 0 ? 0 : timeRemaining));
        //_scoreText.SetText("Score\n" + actualScore);
        //_livesText.SetText("Lives " + (livesRemaining < 0 ? 0 : livesRemaining));
    }

    /// <summary>
    /// Notifies this controller that time is up, so the "Time's up!" message must be displayed.
    /// </summary>
    public void NotifyTimeUp() {
        _timeIsUpMessage.SetText("Time's Up!");
        _timeIsUpMessage.gameObject.SetActive(true);
    }

    /// <summary>
    /// Notifies this controller that the player has win, so the "You Win!" message must be displayed.
    /// </summary>
    public void NotifyWin() {
        _gameResultMessage.SetText("You Win!");
        GameHasFinished();
    }

    /// <summary>
    /// Notifies this controller that the player has reached game over, so the "Game Over!" message must be displayed.
    /// </summary>
    public void NotifyGameOver() {
        _gameResultMessage.SetText("Game Over");
        GameHasFinished();
    }

    /// <summary>
    /// Sets the game result message to active, as the game has finished (either win or game over).
    /// </summary>
    private void GameHasFinished() {
        _gameResultMessage.gameObject.SetActive(true);
    }
}