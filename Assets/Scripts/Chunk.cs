using System;
using Boo.Lang.Runtime;
using UnityEngine;

/// <summary>
/// The logic stuff of a chunk.
/// </summary>
[Serializable]
public class Chunk : MonoBehaviour {
    /// <summary>
    /// The chunk's length.
    /// </summary>
    [SerializeField] private float _chunkLength;

    /// <summary>
    /// The chunks difficulty.
    /// </summary>
    [SerializeField] private int _difficulty;


    /// <summary>
    /// Chunk's length getter.
    /// </summary>
    public float ChunkLength {
        get { return _chunkLength; }
    }

    /// <summary>
    /// The chunks difficulty. getter.
    /// </summary>
    public int Difficulty {
        get { return _difficulty; }
    }

    // Use this for initialization
    private void Start() {
        if (_chunkLength <= 0) {
            throw new RuntimeException("Chunk length must be positive.");
        }
        if (_difficulty < 0) {
            throw new RuntimeException("Chunk difficulty cannot be negative.");
        }
    }

    // Update is called once per frame
    private void Update() {
    }
}