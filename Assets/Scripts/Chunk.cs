using Boo.Lang.Runtime;
using UnityEngine;

/// <summary>
/// The logic stuff of a chunk.
/// </summary>
[System.Serializable]
public class Chunk : MonoBehaviour {
    /// <summary>
    /// The chunk's length.
    /// </summary>
    [SerializeField] private float _chunkLength;

    /// <summary>
    /// Chunk's length getter.
    /// </summary>
    public float ChunkLength {
        get { return _chunkLength; }
    }

    // Use this for initialization
    private void Start() {
        if (_chunkLength <= 0) {
            throw new RuntimeException("Chunk length must be positive.");
        }
    }

    // Update is called once per frame
    private void Update() {
    }
}
