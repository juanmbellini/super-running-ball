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
    /// The player that must be followed.
    /// </summary>
    public Player Player { get; set; }

    private void Start() {
        _initialPosition = transform.position;
    }

    private void Update() {
        transform.position = new Vector3(Player.transform.position.x, _initialPosition.y, _initialPosition.z);
    }
}
