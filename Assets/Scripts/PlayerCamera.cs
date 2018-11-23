using UnityEngine;

/// <summary>
/// Class implementing logic for a camera to follow a player.
/// </summary>
public class PlayerCamera : MonoBehaviour {
    /// <summary>
    /// The camera's initial position (used to store the 'y' and 'z' components, which don't change for the camera).
    /// </summary>
    private Vector3 _initialPosition;

    /// <summary>
    /// Flag indicating whether the camera must follow the player.
    /// </summary>
    private bool _mustFollowPlayer;

    /// <summary>
    /// The player that must be followed.
    /// </summary>
    public Player Player { get; set; }

    private void Start() {
        _initialPosition = transform.position;
        _mustFollowPlayer = true;
    }

    private void Update() {
        if (_mustFollowPlayer) {
            transform.position = new Vector3(Player.transform.position.x, _initialPosition.y, _initialPosition.z);
        }
    }

    public void StopFollowingPlayer() {
        _mustFollowPlayer = false;
    }
}