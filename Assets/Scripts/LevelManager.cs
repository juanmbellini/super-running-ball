using System;
using System.Collections.Generic;
using System.Linq;
using Boo.Lang.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

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
    /// Seed to be used with the level's random numbers generator.
    /// </summary>
    [SerializeField] private int _randomSeed;

    /// <summary>
    /// Position at which the player spawns.
    /// </summary>
    [SerializeField] private Vector3 _playerStartingPosition;

    /// <summary>
    /// Position at which the level starts building.
    /// </summary>
    [SerializeField] private Vector3 _levelStartingPosition;

    /// <summary>
    /// The 'y' at which the player loses.
    /// </summary>
    [SerializeField] private float _losingHeight;

    /// <summary>
    /// The amount of chunks added to the level at once.
    /// </summary>
    [SerializeField] private int _buildBatchSize;

    /// <summary>
    /// The amount of chunks that must exist till the end of the already created level.
    /// </summary>
    [SerializeField] private int _creationMarginSize;

    /// <summary>
    /// Compositions of levels (i.e this variable is used for loading data from editor).
    /// </summary>
    [SerializeField] private List<LevelComposition> _levelsCompositions;


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
    private readonly IList<Chunk> _createdChunks = new List<Chunk>();

    /// <summary>
    /// The length that must exist till the end of the already created level.
    /// </summary>
    private float _creationMarginLength;

    /// <summary>
    /// The actual level.
    /// </summary>
    private int _actualLevel;

    /// <summary>
    /// The amount of batches created in the actual level.
    /// </summary>
    private int _actualLevelBatchsCount;

    /// <summary>
    /// Indicates how many batches were executed so far.
    /// </summary>
    private int _amountOfBatchesExecuted;

    /// <summary>
    /// A Dictionary containing the chunks according to the defficulty.
    /// </summary>
    private IDictionary<int, List<Chunk>> _chunksByDifficulty;

    /// <summary>
    /// A Dictionary containing each level, together with the level's definition.
    /// </summary>
    private IDictionary<int, LevelDefinition> _levels;

    /// <summary>
    /// Stores the random state to be used.
    /// </summary>
    private Random.State _randomState;

    
    private TimeManager _timeManager;

    private float timeToAdd = 20f;
    

    private bool _initialized = false;


    private void Start() {
        RemoveChunksFromScene(); // First clear the level
        InitializeLevelManager();
    }

    private void Update() {
        CheckLevelExpansion();
    }

    private void InitializeLevelManager() {
        Debug.Log("Initializing level manager");
        // If the seed is zero, then we use the "default" seed.
        if (_randomSeed != 0) {
            Random.InitState(_randomSeed); // Initialize random
        }

        _timeManager = FindObjectOfType<TimeManager>();
        _randomState = Random.state; // Save the random's state
        _amountOfCreatedChunks = 0;
        _nextStartingPosition = _levelStartingPosition.x;
        _createdChunks.Clear();
        _chunksByDifficulty = SeparateByDifficulty();
        _levels = ToLevelDefinition(_levelsCompositions);
        _actualLevel = _levels.Keys.Min();
        _actualLevelBatchsCount = 0;
        _actualLevelBatchsCount = 0;
        _initialized = true;
        Debug.Log("Finished initializing level manager");
    }


    /// <summary>
    /// Player starting position getter.
    /// </summary>
    public Vector3 PlayerStartingPosition {
        get { return _playerStartingPosition; }
    }

    /// <summary>
    /// Losing length's getter.
    /// </summary>
    public float LosingHeight {
        get { return _losingHeight; }
    }


    /// <summary>
    /// Creates a Dictionary containing the chunks prefabs grouped by their difficulty.
    /// </summary>
    /// <returns>A Dictionary containing the chunks prefabs grouped by their difficulty.</returns>
    private IDictionary<int, List<Chunk>> SeparateByDifficulty() {
        return _chunksPrefabs.ToList()
            .GroupBy(c => c.Difficulty)
            .ToDictionary(c => c.Key, c => c.ToList());
    }

    /// <summary>
    /// Exppands the level if necessary.
    /// </summary>
    private void CheckLevelExpansion() {
        if (ShouldExpand()) {
            ExpandLevel();
            DestoyLevel();
            _timeManager.addTime(timeToAdd);
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
        var levelDefinition = _levels[_actualLevel]; // Get the definition of the actual level
        _actualLevelBatchsCount++; // Increase the count of batches of the actual level
        BuildLevel(_buildBatchSize, levelDefinition); // Build level in batch
        // Check if a new level was achieved.
        // This can happen if not in last level (i.e duration is null)
        // and if the count of batches is equal to the duration
        if (levelDefinition.Duration == null || _actualLevelBatchsCount < levelDefinition.Duration) {
            return; // In case of true, then it's the last level or the count didn't reach the level's duration.
        }
        // Up to here we know that a new level was recahed
        _actualLevel++; // Increase level
        _actualLevelBatchsCount = 0; // Reset the count
        _amountOfBatchesExecuted++;
    }


    /// <summary>
    /// Builds more level, according to the given amount.
    /// </summary>
    /// <param name="amount">
    ///     The amount of chunks that must be added to the level.
    /// </param>
    /// <param name="levelDefinition">
    ///     The level's definition used to build more level
    ///     (i.e this is used to know which type of chunks should be added).
    /// </param>
    /// <exception cref="System.ArgumentException">If the given amount is not positive</exception>
    private void BuildLevel(int amount, LevelDefinition levelDefinition) {
        if (amount <= 0) {
            Debug.LogError("The amount of chunks to be added to the level must be positive.");
            throw new ArgumentException("The amount must be positive");
        }
        var total = _amountOfCreatedChunks + amount; // Precalculates how many chunks will exist
        while (_amountOfCreatedChunks < total) {
            var selectedChunkPrefab = GetAChunkPrefab(levelDefinition);
            var chunk = Instantiate(selectedChunkPrefab);
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


    /// <summary>
    /// Returns a random chunk prefab according to the given level definition.
    /// </summary>
    /// <param name="levelDefinition">The level definition used to know which chunk prefabs can be returned.</param>
    /// <returns></returns>
    private Chunk GetAChunkPrefab(LevelDefinition levelDefinition) {
        var chunkLelve = GetRandomWithProbability(levelDefinition.Probabilities);
        var chunks = _chunksByDifficulty[chunkLelve];
        Random.state = _randomState; // Set the state.
        var randomIndex = Random.Range(0, chunks.Count);
        _randomState = Random.state; // Save the state
        return chunks[randomIndex];
    }


    /// <summary>
    /// Returns a random value from the given probabilities dictionary, according to the given values.
    ///
    /// </summary>
    /// <param name="probabilities">
    ///     A dictionary containing as keys each integer that can be returned,
    ///     together with the probability of occcurance.
    /// </param>
    /// <returns>A random integer from the dictionary, according to the probabilities of occurance.</returns>
    /// <exception cref="System.ArgumentNullException">In case the dictionary is null</exception>
    /// <exception cref="RuntimeException">
    ///     Sanity Check: this should not happen. It will be thrown if the random value is more than 1.0
    /// </exception>
    private int GetRandomWithProbability(IDictionary<int, float> probabilities) {
        if (probabilities == null) {
            Debug.LogError("The given probabilities dictionary is null!");
            throw new ArgumentNullException("probabilities");
        }
        Random.state = _randomState; // Set the state.
        var random = Random.value;
        _randomState = Random.state; // Save the state

        var items = probabilities.ToList();
        var amount = items.Count;
        var accumulated = 0f;
        for (var i = 0; i < amount; i++) {
            var item = items[i];
            var probability = item.Value;
            accumulated += probability;
            if (random <= accumulated) {
                return item.Key;
            }
        }
        Debug.LogError("Something wrong happend as no random was selected");
        throw new RuntimeException("This should not happen");
    }

    private void DestoyLevel() {
        if (_amountOfBatchesExecuted <= 2) {
            return;
        }
        Debug.Log("Destoying some level");
        DestroyTheFirstChunks(_buildBatchSize);
    }


    /// <summary>
    /// Destroys the first "chunkNumber" chunks in the list.
    /// </summary>
    /// <param name="amount">The amount of chunks to be destroyed.</param>
    /// <exception cref="System.ArgumentException">If the chunk number is negative</exception>
    private void DestroyTheFirstChunks(int amount) {
        if (amount < 0) {
            Debug.LogError("The amount must not be negative");
            throw new ArgumentException("The amount must not be negative");
        }
        for (var i = 0; i < amount; i++) {
            var chunk = _createdChunks[0];
            _createdChunks.RemoveAt(0);
            Destroy(chunk.gameObject);
        }
    }

    /// <summary>
    /// Clears the level.
    /// </summary>
    private void RemoveChunksFromScene() {
        var chunks = FindObjectsOfType<Chunk>();
        if (chunks == null) {
            return;
        }
        foreach (var chunk in chunks) {
            Destroy(chunk.gameObject);
        }
        _createdChunks.Clear();
    }


    // ================================================================================================================
    // Editor stuff
    // ================================================================================================================

    /// <summary>
    /// Creates a level with the given number.
    /// </summary>
    /// <param name="levelNumber"></param>
    public void CreateLevel(int levelNumber) {
        if (!_initialized) {
            // First initialize the level manager
            InitializeLevelManager();
        }
        _actualLevel = levelNumber;
        ExpandLevel();
    }

    /// <summary>
    /// Clears the level.
    /// </summary>
    public void ClearLevel() {
        var chunks = FindObjectsOfType<Chunk>();
        if (chunks == null) {
            return;
        }
        foreach (var chunk in chunks) {
            DestroyImmediate(chunk.gameObject);
        }
        InitializeLevelManager();
    }


    // ================================================================================================================
    // Helpers
    // ================================================================================================================

    /// <summary>
    /// Converts an IEnumerable of difficulty probabilities into a Dictionary
    /// that has as keys the difficulties and the probabilities as values.
    /// </summary>
    /// <param name="levels">The list of difficulty probability to be transformed into a dictionary</param>
    /// <returns>The dictionary.</returns>
    /// <exception cref="System.ArgumentNullException">If the list is null</exception>
    /// <exception cref="System.ArgumentException">
    ///     If the list contains more than one difficulty probability with a given difficulty.
    /// </exception>
    private static IDictionary<int, LevelDefinition> ToLevelDefinition(IList<LevelComposition> levels) {
        if (levels == null) {
            Debug.LogError("The levels list is null");
            throw new ArgumentNullException("levels");
        }
        if (levels.GroupBy(dp => dp.Level).Any(dps => dps.Count() > 1)) {
            Debug.LogError("The levels composition list has more than one composition for a level number");
            throw new ArgumentException("More than one composition for a level number");
        }
        return levels.ToDictionary(lc => lc.Level, ToLevelDefinition);
    }

    private static LevelDefinition ToLevelDefinition(LevelComposition levelComposition) {
        if (levelComposition == null) {
            Debug.LogError("The level composition must not be null");
            throw new ArgumentNullException("levelComposition");
        }
        return new LevelDefinition(AsDictionary(levelComposition.Probabilities), levelComposition.Duration);
    }

    /// <summary>
    /// Converts an IEnumerable of difficulty probabilities into a Dictionary
    /// that has as keys the difficulties and the probabilities as values.
    /// </summary>
    /// <param name="difficulties">The list of difficulty probability to be transformed into a dictionary</param>
    /// <returns>The dictionary.</returns>
    /// <exception cref="System.ArgumentNullException">If the list is null</exception>
    /// <exception cref="System.ArgumentException">
    ///     If the list contains more than one difficulty probability with a given difficulty.
    /// </exception>
    private static IDictionary<int, float> AsDictionary(IList<DifficultyProbability> difficulties) {
        if (difficulties == null) {
            Debug.LogError("The difficulties list is null");
            throw new ArgumentNullException("difficulties");
        }
        if (difficulties.GroupBy(dp => dp.Difficulty).Any(probs => probs.Count() > 1)) {
            Debug.LogError("The difficulties list has more than one probability for a chunk difficulty");
            throw new ArgumentException("More than one probability for a chunk difficulty");
        }
        return difficulties.ToDictionary(dp => dp.Difficulty, dp => dp.Probability);
    }

    /// <summary>
    /// Struct used to define a level (i.e probability of chunks and duration).
    /// </summary>
    private struct LevelDefinition {
        /// <summary>
        /// Epsilon to be used for the probabilities sum check.
        /// </summary>
        private const float Tolerance = 0.000001f;


        /// <summary>
        /// Dictionary containing the probabilities of appearance for each difficulty level of a chunk.
        /// </summary>
        private readonly IDictionary<int, float> _probabilities;

        /// <summary>
        /// Amount of batches this level is composed of
        /// (i.e how many times the expand level method must be called till the level is ended).
        /// </summary>
        private readonly int? _duration;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="probabilities">
        ///     Dictionary containing the probabilities of appearance for each difficulty level of a chunk.
        /// </param>
        /// <param name="duration">
        ///     Amount of batches this level is composed of
        ///     (i.e how many times the expand level method must be called till the level is ended).
        ///     Can be set to null in order to declare the level as the final level.
        /// </param>
        /// <exception cref="System.ArgumentNullException">If the probabilities dictionary is null.</exception>
        /// <exception cref="System.ArgumentException">
        ///     If any probability is negative, if the sum of probabilities is not 1.0,
        ///     or if the duration is not positive when present.
        /// </exception>
        public LevelDefinition(IDictionary<int, float> probabilities, int? duration) {
            if (probabilities == null) {
                Debug.LogError("Probabilities dictionary must be present");
                throw new ArgumentNullException("probabilities");
            }
            if (probabilities.Values.Any(probability => probability < 0)) {
                Debug.LogError("Probabilities must not be negative");
                throw new ArgumentException("Probabilities must not be negative");
            }
            if (Math.Abs(probabilities.Values.Sum() - 1.0) > Tolerance) {
                Debug.LogError("Probabilities must sum 1.0");
                throw new ArgumentException("Probabilities must sum 1.0");
            }
            if (duration.HasValue && duration.Value <= 0) {
                Debug.LogError("If present, duration must be positive!");
                throw new ArgumentException("The duration must be positive when present");
            }

            _probabilities = probabilities;
            _duration = duration;
        }


        /// <summary>
        /// The probabilities getter.
        /// </summary>
        public IDictionary<int, float> Probabilities {
            get { return _probabilities; }
        }

        /// <summary>
        /// The duration getter.
        /// </summary>
        public int? Duration {
            get { return _duration; }
        }
    }
}