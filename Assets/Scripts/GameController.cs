using Boo.Lang.Runtime;
using UnityEngine;

/// <summary>
/// Class containing the game's main logic.
/// </summary>
public class GameController : MonoBehaviour {
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


    // ================================================================================================================
    // Game configuration
    // ================================================================================================================


    /// <summary>
    /// The player object.
    /// </summary>
    private Player _player;

    /// <summary>
    /// New gravity constant
    /// </summary>
    private float gravity = -30.0f;

    private void Awake() {
        _levelManager = FindObjectOfType<LevelManager>();
    }

    private void Start() {
        ModifyGravity();
        SpawnPlayer();
        CreatePlayerCamera();
        _levelManager.Player = _player; // Sets the player in the level manager.
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
        Physics.gravity = new Vector3(0f, gravity, 0f);
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
}