using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class in charge of creating the level.
/// </summary>
public class LevelManager : MonoBehaviour {
    // ================================================================================================================
    // Prefabs
    // ================================================================================================================

    /// <summary>
    /// A List holding all the chunks prefabs that must be used to create the level.
    /// </summary>
    [SerializeField] private List<Chunk> _chunksPrefabs = new List<Chunk>();


    // ================================================================================================================
    // Variables
    // ================================================================================================================

    /// <summary>
    /// Position at which the player spawns.
    /// </summary>
    [SerializeField] private Vector3 _playerStartingPosition;

    /// <summary>
    /// Player starting position getter.
    /// </summary>
    public Vector3 PlayerStartingPosition {
        get { return _playerStartingPosition; }
    }

    // Use this for initialization
    private void Start() {
        BuildLevel(0, _chunksPrefabs.Count);
    }

    // Update is called once per frame
    private void Update() {
    }

    /// <summary>
    /// Creates some level.
    /// </summary>
    /// <param name="from">The first inclusive chunk number to be built (i.e the next to be added to the level).</param>
    /// <param name="to">The last exclusive chunk number to be built.</param>
    private void BuildLevel(int from, int to) {
        // TODO: make it random
        for (var i = 0; i < to - from; i++) {
            var chunk = Instantiate(_chunksPrefabs[i]);
            chunk.name = "Chunk" + i;
            chunk.transform.position = new Vector3(i * chunk.ChunkLength, 0, 0);
            // TODO: save it in a manager for reuse?
        }
    }
}
