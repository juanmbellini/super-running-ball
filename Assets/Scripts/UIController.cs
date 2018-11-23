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
    /// Textfield for distance.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _distanceText;

    /// <summary>
    /// The time's up Textfield.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _timeIsUpMessage;

    /// <summary>
    /// The game over's Textfield.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _gameOverMessage;


    /// <summary>
    /// A reference to the game controller, in order to notify out of time.
    /// </summary>
    private GameController _gameController;

    private bool _gameIsRunning;

    // Use this for initialization
    private void Start() {
        _gameController = FindObjectOfType<GameController>();
        _timeIsUpMessage.gameObject.SetActive(false); // Disable the time's up message
        _gameOverMessage.gameObject.SetActive(false); // Disable the game over's message
        _gameIsRunning = true;
    }

    // Update is called once per frame
    private void Update() {
        UpdateUI();
    }

    /// <summary>
    /// Updates the UI.
    /// </summary>
    private void UpdateUI() {
        var timeRemaining = (int) _gameController.GetTimeRemaining();
        // Values can go below zero in special cases
        _timeText.SetText("Time Remaining: " + " " + (timeRemaining < 0 ? 0 : timeRemaining) + "s");
        if (_gameIsRunning) {
            var distance = _gameController.GetWalkedDistance();
            _distanceText.SetText("Distance: " + distance.ToString("F2") + "m");
        }
    }

    /// <summary>
    /// Notifies this controller that time is up, so the "Time's up!" message is displayed.
    /// </summary>
    public void NotifyTimeUp() {
        _timeIsUpMessage.gameObject.SetActive(true);
        _gameIsRunning = false;
    }

    /// <summary>
    /// Notifies this controller that the game is over, so the "Game Over!" message is displayed.
    /// </summary>
    public void NotifyGameOver() {
        _gameOverMessage.gameObject.SetActive(true);
        _gameIsRunning = false;
    }
}