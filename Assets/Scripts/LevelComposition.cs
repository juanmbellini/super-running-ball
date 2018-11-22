using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelComposition", menuName = "ScriptableObjects/LevelComposition", order = 2)]
public class LevelComposition : ScriptableObject {
    /// <summary>
    /// The level number.
    /// </summary>
    [SerializeField] private int _level;

    /// <summary>
    /// The List of difficulty probabilities for this level.
    /// </summary>
    [SerializeField] private List<DifficultyProbability> _probabilities;

    /// <summary>
    /// The duration of the level (i.e how many batches of this level must be created before finishin it).
    /// </summary>
    [SerializeField] private int _duration;


    /// <summary>
    /// The level number's getter.
    /// </summary>
    public int Level {
        get { return _level; }
    }

    /// <summary>
    /// The difficulty probabilities's getter.
    /// </summary>
    public List<DifficultyProbability> Probabilities {
        get { return _probabilities; }
    }

    /// <summary>
    /// The level duration's getter.
    /// </summary>
    public int? Duration {
        get {
            if (_duration > 0) {
                return _duration;
            }
            return null;
        }
    }
}