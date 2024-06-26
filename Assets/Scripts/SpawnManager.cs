using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    #region Instance

    public static SpawnManager Instance;

    #endregion

    #region Fields

    [Header("Hexagon Spawner Settings")]
    #region Serialized Fields

    [SerializeField] private GameObject _hexagonPrefab;

    [SerializeField][Range(0.1f, 10.0f)] private float _spawnRate = 1.2f;

    [SerializeField] private int _currentHexagonAmount = 0;
    [SerializeField] private int _maxHexagonAmount = 4;

    #endregion

    #region Private Fields

    private float _nextSpawnTime = 0.0f;

    #endregion

    #region Properties

    public float SpawnRate { get { return _spawnRate; } set { _spawnRate = value; } }

    #endregion

    #endregion


    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    private void Update() {
        SpawnHexagon();
    }

    private void SpawnHexagon() {
        if (_currentHexagonAmount < _maxHexagonAmount && Time.time >= _nextSpawnTime)
        {
            Instantiate(_hexagonPrefab, Vector3.zero, Quaternion.identity);

            _nextSpawnTime = Time.time + _spawnRate;
            _currentHexagonAmount++;

        }
        else if (_currentHexagonAmount == _maxHexagonAmount)
        {
            enabled = false;
        }
    }

}
