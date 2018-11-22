using System.Collections.Generic;
using System.Linq;
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
    /// Position at which the level starts building.
    /// </summary>
    [SerializeField] private Vector3 _levelStartingPosition;

//    private int _startingAmount;

    /// <summary>
    /// The amount of chunks added to the level at once.
    /// </summary>
    [SerializeField] private int _buildBatchSize;

    /// <summary>
    /// The min. margin that must exists between the player's position and the end of the level.
    /// </summary>
    [SerializeField] private int _creationMarginSize;


    // ================================================================================================================
    // Internal state
    // ================================================================================================================

    /// <summary>
    /// The player that will walk the level.
    /// </summary>
    public Player Player { get; set; }

    private int _amountOfCreatedChunks;
    private float _nextStartingPosition;

    /// <summary>
    /// A Dictionary containing the chunks according to the defficulty.
    /// </summary>
    private Dictionary<int, List<Chunk>> _chunksByDifficulty;

    /// <summary>
    /// Player starting position getter.
    /// </summary>
    public Vector3 PlayerStartingPosition {
        get { return _playerStartingPosition; }
    }

    // Use this for initialization
    private void Start() {
        _nextStartingPosition = _levelStartingPosition.x;
        _amountOfCreatedChunks = 0;
        _chunksByDifficulty = SeparateByDifficulty();

//        BuildLevel(_chunksPrefabs.Count);
    }

    // Update is called once per frame
    private void Update() {
        CheckLevelExpansion();
    }

    private void CheckLevelExpansion() {
        if (ShouldExpand()) {
            ExpandLevel();
        }
    }

    private bool ShouldExpand() {
        return _nextStartingPosition - Player.transform.position.x < _creationMarginLength;
    }


    /// <summary>
    /// Expands the level.
    /// </summary>
    private void ExpandLevel() {
        BuildLevel(_buildBatchSize);
    }


    /// <summary>
    /// Creates a Dictionary containing the chunks prefabs grouped by their difficulty.
    /// </summary>
    /// <returns>A Dictionary containing the chunks prefabs grouped by their difficulty.</returns>
    private Dictionary<int, List<Chunk>> SeparateByDifficulty() {
        return _chunksPrefabs.ToList()
            .GroupBy(c => c.Difficulty)
            .ToDictionary(c => c.Key, c => c.ToList());
    }


    private readonly List<Chunk> _createdChunks = new List<Chunk>();

    private float _creationMarginLength;

    private void BuildLevel(int amount) {
        var total = _amountOfCreatedChunks + amount;
        while (_amountOfCreatedChunks < total) {
            // Calculate a random chunk TODO: improve according to difficulty
            var selectedChunk = Random.Range(0, _chunksPrefabs.Count);
            var chunk = Instantiate(_chunksPrefabs[selectedChunk]);
            chunk.name = "Chunk" + _amountOfCreatedChunks;

            // The chunk must start in the next starting position 'x' value
            var chunkLength = chunk.ChunkLength;
            var chunkX = _nextStartingPosition + chunkLength / 2;
            chunk.transform.position = new Vector3(chunkX, _levelStartingPosition.y, _levelStartingPosition.z);

            // Update internal variables
            _nextStartingPosition += chunkLength;
            _amountOfCreatedChunks++;
            _createdChunks.Add(chunk);
        }

        // Calculate the margin in 'x' at which the level should be expanded.
        _creationMarginLength = 0;
        for (var i = _createdChunks.Count - 1; i >= _createdChunks.Count - _creationMarginSize; i--) {
            _creationMarginLength += _createdChunks[i].ChunkLength;
        }
    }
}