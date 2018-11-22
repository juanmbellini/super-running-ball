using UnityEngine;

[CreateAssetMenu(fileName = "DifficultyProbability", menuName = "ScriptableObjects/DifficultyProbability", order = 1)]
public class DifficultyProbability : ScriptableObject {
    /// <summary>
    /// The chunk's difficulty.
    /// </summary>
    [SerializeField] private int _difficulty;

    /// <summary>
    /// The probability of occurance for this chunk.
    /// </summary>
    [SerializeField] private float _probability;

    /// <summary>
    /// The chunk's difficulty getter.
    /// </summary>
    public int Difficulty {
        get { return _difficulty; }
    }

    /// <summary>
    /// The probability's getter.
    /// </summary>
    public float Probability {
        get { return _probability; }
    }
}