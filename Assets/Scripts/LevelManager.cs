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

    /// <summary>
    /// The amount of chunks added to the level at once.
    /// </summary>
    [SerializeField] private int _buildBatchSize;

    /// <summary>
    /// The amount of chunks that must exist till the end of the already created level.
    /// </summary>
    [SerializeField] private int _creationMarginSize;


    // ================================================================================================================
    // Internal state
    // ================================================================================================================

    /// <summary>
    /// The player that will walk the level.
    /// </summary>
    public Player Player { get; set; }

    /// <summary>
    /// Indicates how many chunks were already created so far.
    /// </summary>
    private int _amountOfCreatedChunks;

    /// <summary>
    /// Indicates the 'x' component of the starting position of the next chunk to be created.
    /// </summary>
    private float _nextStartingPosition;

    /// <summary>
    /// List holding the chunks that were created and still exist in the real world.
    /// </summary>
    private readonly List<Chunk> _createdChunks = new List<Chunk>();

    /// <summary>
    /// The length that must exist till the end of the already created level.
    /// </summary>
    private float _creationMarginLength;

    /// <summary>
    /// A Dictionary containing the chunks according to the defficulty.
    /// </summary>
    private Dictionary<int, List<Chunk>> _chunksByDifficulty;


    private void Start() {
        _nextStartingPosition = _levelStartingPosition.x;
        _amountOfCreatedChunks = 0;
        _chunksByDifficulty = SeparateByDifficulty();
    }

    private void Update() {
        CheckLevelExpansion();
    }


    /// <summary>
    /// Player starting position getter.
    /// </summary>
    public Vector3 PlayerStartingPosition {
        get { return _playerStartingPosition; }
    }

    /// <summary>
    /// Exppands the level if necessary.
    /// </summary>
    private void CheckLevelExpansion() {
        if (ShouldExpand()) {
            ExpandLevel();
        }
    }

    /// <summary>
    /// Indicates whether the level should be expanded, according to the actual built level, and the player's position.
    /// </summary>
    /// <returns>true if the level should be expanded, or false otherwise.</returns>
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


    /// <summary>
    /// Builds more level, according to the given amount.
    /// </summary>
    /// <param name="amount">The amount of chunks that must be added to the level.</param>
    private void BuildLevel(int amount) {
        var total = _amountOfCreatedChunks + amount; // Precalculates how many chunks will exist
        while (_amountOfCreatedChunks < total) {
            // Calculate a random chunk TODO: improve according to difficulty
            var selectedChunk = Random.Range(0, _chunksPrefabs.Count);
            var chunk = Instantiate(_chunksPrefabs[selectedChunk]);
            chunk.name = "Chunk" + _amountOfCreatedChunks;

            // The chunk must start in the next starting position 'x' component
            var chunkLength = chunk.ChunkLength;
            var chunkX = _nextStartingPosition + chunkLength / 2;
            chunk.transform.position = new Vector3(chunkX, _levelStartingPosition.y, _levelStartingPosition.z);

            // Update internal variables
            _nextStartingPosition += chunkLength;
            _amountOfCreatedChunks++;
            _createdChunks.Add(chunk);
        }
        RecalculateCreationMarginLength(); // Calculate the margin in 'x' at which the level should be expanded.
    }

    /// <summary>
    /// Recalculates the creation margin length (this is an idempotent operation).
    /// </summary>
    private void RecalculateCreationMarginLength() {
        _creationMarginLength = 0;
        for (var i = _createdChunks.Count - 1; i >= _createdChunks.Count - _creationMarginSize; i--) {
            _creationMarginLength += _createdChunks[i].ChunkLength;
        }
    }
}