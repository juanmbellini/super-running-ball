using System.Collections;
using Boo.Lang.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Class containing the game's main logic.
/// </summary>
public class GameController : MonoBehaviour {
    // ================================================================================================================
    // Constants
    // ================================================================================================================

    /// <summary>
    /// New gravity constant
    /// </summary>
    private const float Gravity = -60.0f;

    // ================================================================================================================
    // Prefabs
    // ================================================================================================================
    /// <summary>
    /// Prefab to be used to instantiate the player.
    /// </summary>
    [SerializeField] private Player _playerPrefab;

    /// <summary>
    /// Prefab to be used to instantiate the player's camera.
    /// </summary>
    [SerializeField] private PlayerCamera _playerCameraPrefab;


    // ================================================================================================================
    // Managers
    // ================================================================================================================

    /// <summary>
    /// The level manager.
    /// </summary>
    private LevelManager _levelManager;

    /// <summary>
    /// The player's camera.
    /// </summary>
    private PlayerCamera _playerCamera;

    /// <summary>
    /// The time manager.
    /// </summary>
    private TimeManager _timeManager;

    /// <summary>
    /// The UI controller.
    /// </summary>
    private UIController _uiController;


    // ================================================================================================================
    // Internal state
    // ================================================================================================================

    /// <summary>
    /// The player object.
    /// </summary>
    private Player _player;

    /// <summary>
    /// The 'y' at which the player loses.
    /// </summary>
    private float _losingHeight;

    /// <summary>
    /// Flag inidicating whether the player is alive.
    /// </summary>
    private bool _playerIsAlive;


    private void Awake() {
        _levelManager = FindObjectOfType<LevelManager>();
        _uiController = FindObjectOfType<UIController>();
        _timeManager = FindObjectOfType<TimeManager>();
    }

    private void Start() {
        ModifyGravity();
        SpawnPlayer();
        CreatePlayerCamera();
        _levelManager.Player = _player; // Sets the player in the level manager.
        _losingHeight = _levelManager.LosingHeight;
        _playerIsAlive = true;
    }

    private void Update() {
        CheckPlayerLose();
    }

    /// <summary>
    /// Spawns the player.
    /// </summary>
    private void SpawnPlayer() {
        _player = Instantiate(_playerPrefab);
        _player.name = "Player";
        _player.transform.position = _levelManager.PlayerStartingPosition;
    }

    /// <summary>
    /// Modifies global gravity
    /// </summary>
    private void ModifyGravity() {
        Physics.gravity = new Vector3(0f, Gravity, 0f);
    }

    /// <summary>
    /// Notifies this game controller that there is no more time.
    /// </summary>
    public void NotifyNoMoreTime() {
        if (_playerIsAlive) {
            _timeManager.StopTimer();
            StartCoroutine(TimeUpCoroutine());
        }
    }

    /// <summary>
    /// Returns the time remaining.
    /// </summary>
    /// <returns>The time remaining.</returns>
    public float GetTimeRemaining() {
        return _timeManager.TimeRemaining;
    }

    /// <summary>
    /// The amount of distance that was "walked" by the player.
    /// </summary>
    /// <returns>The amount of distance that was "walked" by the player.</returns>
    public float GetWalkedDistance() {
        return _player.transform.position.x;
    }

    /// <summary>
    /// Creates the player's camera.
    /// </summary>
    /// <exception cref="RuntimeException"></exception>
    private void CreatePlayerCamera() {
        if (_player == null) {
            throw new RuntimeException("Tried to create player camera without a player being created before.");
        }
        _playerCamera = Instantiate(_playerCameraPrefab);
        _playerCamera.name = "PlayerCamera";
        _playerCamera.Player = _player;
    }


    /// <summary>
    /// Checks whether the player has lost.
    /// </summary>
    private void CheckPlayerLose() {
        if (_player.transform.position.y < _losingHeight) {
            Lose();
        }
    }

    /// <summary>
    /// The lose action.
    /// </summary>
    private void Lose() {
        // Execute only if player is alive (might not be alive if already died and this is executed again).
        if (_playerIsAlive) {
            _playerIsAlive = false;
            _timeManager.StopTimer();
            StartCoroutine(LoseCorutine());
        }
    }

    /// <summary>
    /// The time's up corutine.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TimeUpCoroutine() {
        _playerIsAlive = false; // Sanity check (re set this value just in case).
        Debug.Log("Time is up!");
        _playerCamera.StopFollowingPlayer();
        _uiController.NotifyTimeUp();
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(LoseCorutine());
    }

    /// <summary>
    /// The Die corutine enumerator.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoseCorutine() {
        _playerIsAlive = false; // Sanity check (re set this value just in case).
        Debug.Log("The player has died. Game Over!");
        _playerCamera.StopFollowingPlayer();
        _uiController.NotifyGameOver();
        yield return new WaitForSeconds(2.0f);
        FinishGame();
    }

    /// <summary>
    /// Finishes the game.
    /// </summary>
    private static void FinishGame() {
        SceneManager.LoadScene("MainMenu");
    }
}