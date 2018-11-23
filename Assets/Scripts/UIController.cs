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
    /// The time's up Textfield.
    /// </summary>
    [SerializeField] private TextMeshProUGUI _timeIsUpMessage;



    /// <summary>
    /// A reference to the game controller, in order to notify out of time.
    /// </summary>
    private GameController _gameController;

    // Use this for initialization
    private void Start() {
        _gameController = FindObjectOfType<GameController>();
        _timeIsUpMessage.gameObject.SetActive(false); // Disable the time's up message
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
        //var actualScore = _gameController.GetScore();
        //var livesRemaining = _gameController.GetLives();
        // Values can go below zero in special cases
        _timeText.SetText("Time\n" + (timeRemaining < 0 ? 0 : timeRemaining));
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

}