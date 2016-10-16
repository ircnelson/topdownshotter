using UnityEngine;
using System.Collections;
using System;

public class Spawner : MonoBehaviour
{
    public Wave[] Waves;
    public Enemy Enemy;

    private Wave _currentWave;
    private int _currentWaveNumber;
    private int _enemiesRemainingAlive;
    private int _enemiesRemainingToSpwan;
    private float _nextSpawnTime;
    
    void Start()
    {
        NextWave();
    }

    void Update()
    {
        if (_enemiesRemainingToSpwan > 0 && Time.time > _nextSpawnTime)
        {
            _enemiesRemainingToSpwan--;
            _nextSpawnTime = Time.time + _currentWave.TimeBetweenSpawns;

            Enemy spawnEnemy = Instantiate(Enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnEnemy.OnDeath += OnEnemyDeath;
        }
    }

    private void OnEnemyDeath()
    {
        _enemiesRemainingAlive--;

        if (_enemiesRemainingAlive == 0)
        {
            NextWave();
        }
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
