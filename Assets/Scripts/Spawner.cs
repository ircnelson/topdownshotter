using UnityEngine;
using System.Collections;
using System;

public class Spawner : MonoBehaviour
{
    public bool DevMode;

    public Wave[] Waves;
    public Enemy Enemy;

    public event Action<int> OnNewWave;

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

        if ((_enemiesRemainingToSpwan > 0 || _currentWave.Infinity) && Time.time > _nextSpawnTime)
        {
            _enemiesRemainingToSpwan--;
            _nextSpawnTime = Time.time + _currentWave.TimeBetweenSpawns;

            StartCoroutine(SpawnEnemy());
        }

        if (DevMode)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                foreach (var enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }

                StopCoroutine(SpawnEnemy());

                NextWave();
            }
        }
    }

    void ResetPlayerPosition()
    {
        _playerTransform.position = _map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 2f;
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
        Color initialColor = Color.white;
        Color flashColor = Color.red;
        
        while (spawnTimer < spawnDelay)
        {
            tileMaterial.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));

            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnEnemy = Instantiate(Enemy, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnEnemy.OnDeath += OnEnemyDeath;

        spawnEnemy.SetCharacteristics(_currentWave.MoveSpeed, _currentWave.HitsToKillPlayer, _currentWave.EnemyHealth, _currentWave.SkinColor);
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

            if (OnNewWave != null)
            {
                OnNewWave(_currentWaveNumber);
            }

            ResetPlayerPosition();
        }
    }
    
    [Serializable]
    public class Wave
    {
        public bool Infinity;

        public int EnemyCount;
        public float TimeBetweenSpawns;

        public float MoveSpeed;
        public int HitsToKillPlayer;
        public float EnemyHealth;

        public Color SkinColor;
    }
}
