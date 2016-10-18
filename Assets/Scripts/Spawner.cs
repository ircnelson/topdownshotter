using UnityEngine;
using System.Collections;
using System;

public class Spawner : MonoBehaviour
{
    public Wave[] Waves;
    public Enemy Enemy;

    private LivingEntity _playerEntity;
    private Transform _playerTransform;

    private Wave _currentWave;
    private int _currentWaveNumber;
    private int _enemiesRemainingAlive;
    private int _enemiesRemainingToSpwan;
    private float _nextSpawnTime;
    private MapGenerator _map;

    private float _timeBetweenCampingChecks = 2;
    private float _campThresholdDistance = 1.5f;
    private float _nextCampCheckTime;
    private Vector3 _campPositionOld;
    private bool _isCamping;
    private bool _isDisabled = false;

    void Start()
    {
        _playerEntity = FindObjectOfType<Player>();
        _playerEntity.OnDeath += OnPlayerDie;
        _playerTransform = _playerEntity.transform;

        _nextCampCheckTime = _timeBetweenCampingChecks + Time.time;
        _campPositionOld = _playerTransform.position;
        
        _map = FindObjectOfType<MapGenerator>();
        NextWave();
    }

    void Update()
    {
        if (_isDisabled) return;

        if (Time.time > _nextCampCheckTime)
        {
            _nextCampCheckTime = Time.time + _timeBetweenCampingChecks;
            _isCamping = (Vector3.Distance(_playerTransform.position, _campPositionOld) < _campThresholdDistance);

            _campPositionOld = _playerTransform.position;
        }

        if (_enemiesRemainingToSpwan > 0 && Time.time > _nextSpawnTime)
        {
            _enemiesRemainingToSpwan--;
            _nextSpawnTime = Time.time + _currentWave.TimeBetweenSpawns;

            StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        float spawnTimer = 0;

        Transform spawnTile = _map.GetRandomOpenTile();

        if (_isCamping)
        {
            spawnTile = _map.GetTileFromPosition(_playerTransform.position);
        }

        Material tileMaterial = spawnTile.GetComponent<Renderer>().material;
        Color initialColor = tileMaterial.color;
        Color flashColor = Color.red;
        
        while (spawnTimer < spawnDelay)
        {
            tileMaterial.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnEnemy = Instantiate(Enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnEnemy.OnDeath += OnEnemyDeath;
    }

    private void OnEnemyDeath()
    {
        _enemiesRemainingAlive--;

        if (_enemiesRemainingAlive == 0)
        {
            NextWave();
        }
    }

    private void OnPlayerDie()
    {
        _isDisabled = true;
    }

    void NextWave()
    {
        _currentWaveNumber++;
        if (_currentWaveNumber - 1 < Waves.Length)
        {
            _currentWave = Waves[_currentWaveNumber - 1];
            _enemiesRemainingToSpwan = _currentWave.EnemyCount;
            _enemiesRemainingAlive = _enemiesRemainingToSpwan;
        }
    }
    
    [Serializable]
    public class Wave
    {
        public int EnemyCount;
        public float TimeBetweenSpawns;
    }
}
