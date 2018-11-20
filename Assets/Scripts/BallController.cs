using System;
using UnityEngine;

/// <summary>
/// Controller for the ball (i.e the player).
/// </summary>
public class BallController : MonoBehaviour {
    /// <summary>
    /// Tolerance for float comparinsons.
    /// </summary>
    private const float Tolerance = 0.0001f;

    /// <summary>
    /// The speed of the ball.
    /// </summary>
    [SerializeField] private float _horizontalSpeed;

    /// <summary>
    /// Indicates how much force the player applies to the gound, making it jump.
    /// </summary>
    [SerializeField] private float _jumpingForceModule;

    /// <summary>
    /// Stores the rigid body for the player, in order to avoid searching for it in all update calls.
    /// </summary>
    private Rigidbody _rigidBody;

    /// <summary>
    /// The jumping force vector (saved to avoid allocating a new vector each time a jump is triggered).
    /// </summary>
    private Vector3 _jumpingForce;

    /// <summary>
    /// Flag indicating the player is jumping (to avoid a jump in the air).
    /// </summary>
    private bool _isJumping;


    private void Start() {
        _rigidBody = GetComponent<Rigidbody>();
        _jumpingForce = new Vector3(0, _jumpingForceModule, 0);
        _isJumping = false;
        _rigidBody.velocity = new Vector3(_horizontalSpeed, 0, 0);
    }


    private void Update() {
        CheckJump();
        UpdateParentPosition();
        CheckValuesChanges(); // TODO: remove (this is testing stuff)
    }


    private void OnCollisionStay() {
        StopJumping();
    }

    /// <summary>
    /// Checks whether a jumping action was triggered.
    /// </summary>
    private void CheckJump() {
        //Maybe we should check also for collision for the player to jump
        if (Input.GetKey(KeyCode.Space) && !_isJumping) {
            Jump();
        }
    }

    /// <summary>
    /// Moves the parent so that the player as a whole is moved.
    /// </summary>
    private void UpdateParentPosition() {
        transform.parent.position = transform.position;
        transform.localPosition = Vector3.zero;
    }

    /// <summary>
    /// Performs the jumping action.
    /// </summary>
    private void Jump() {
        _rigidBody.AddForce(_jumpingForce, ForceMode.Impulse);
        _isJumping = true;
    }

    /// <summary>
    /// Finishes the jumping action (i.e sets _isJumping to false).
    /// </summary>
    private void StopJumping() {
        _isJumping = false;
    }

    // TODO: remove
    /// <summary>
    /// Checks whether a property value was changed, changing the game object values.
    /// This is a debugging function.
    /// </summary>
    private void CheckValuesChanges() {
        if (Math.Abs(_jumpingForce.y - _jumpingForceModule) > Tolerance) {
            _jumpingForce = new Vector3(0, _jumpingForceModule, 0);
        }
        if (Math.Abs(_rigidBody.velocity.x - _horizontalSpeed) > Tolerance) {
            _rigidBody.velocity = new Vector3(_horizontalSpeed, 0, 0);
        }
    }
}