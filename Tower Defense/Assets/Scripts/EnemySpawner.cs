using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public delegate void WaveEvent();
    public event WaveEvent OnWaveStarted;
    public event WaveEvent OnOneEnemyLeft;
    public event WaveEvent OnWaveWon;

    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<int> enemiesPerWave = new List<int>();
    public List<GameObject> spawnedEnemies = new List<GameObject>();
    public float spawnDelay = 1f;

    private int currentWave;
    private int enemiesDuringWave;
    private bool waveStarted;
    private bool oneEnemyLeftNotified;

    void Awake()
    {
        currentWave = 0;
    }

    void FixedUpdate()
    {
        if (!waveStarted)
        {
            return;
        }

        CleanupSpawnedEnemies();
        NotifyOneEnemyLeftIfNeeded();

        if (enemiesDuringWave <= 0 && spawnedEnemies.Count == 0)
        {
            WinWave();
        }
    }

    public void StartWave()
    {
        if (waveStarted)
        {
            return;
        }

        waveStarted = true;
        oneEnemyLeftNotified = false;

        ConfigureEnemiesDuringWave();

        if (OnWaveStarted != null)
        {
            OnWaveStarted();
        }

        SpawnEnemy();
    }

    private void ConfigureEnemiesDuringWave()
    {
        if (enemiesPerWave.Count == 0)
        {
            enemiesDuringWave = 0;
            return;
        }

        if (currentWave >= enemiesPerWave.Count)
        {
            currentWave = enemiesPerWave.Count - 1;
        }

        enemiesDuringWave = enemiesPerWave[currentWave];
    }

    private void SpawnEnemy()
    {
        if (!waveStarted)
        {
            return;
        }

        if (enemyPrefabs.Count == 0 || enemiesDuringWave <= 0)
        {
            NotifyOneEnemyLeftIfNeeded();
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject spawnedEnemy = Instantiate(enemyPrefabs[randomIndex], transform.position, Quaternion.identity);

        spawnedEnemies.Add(spawnedEnemy);
        enemiesDuringWave--;

        if (enemiesDuringWave > 0)
        {
            Invoke(nameof(SpawnEnemy), spawnDelay);
        }
        else
        {
            NotifyOneEnemyLeftIfNeeded();
        }
    }

    private void WinWave()
    {
        waveStarted = false;
        currentWave++;

        if (OnWaveWon != null)
        {
            OnWaveWon();
        }
    }

    private void CleanupSpawnedEnemies()
    {
        spawnedEnemies.RemoveAll(enemy => enemy == null);
    }

    private void NotifyOneEnemyLeftIfNeeded()
    {
        if (oneEnemyLeftNotified)
        {
            return;
        }

        if (enemiesDuringWave <= 0 && spawnedEnemies.Count == 1)
        {
            oneEnemyLeftNotified = true;

            if (OnOneEnemyLeft != null)
            {
                OnOneEnemyLeft();
            }
        }
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (spawnedEnemies.Contains(enemy))
        {
            spawnedEnemies.Remove(enemy);
        }

        NotifyOneEnemyLeftIfNeeded();
    }

    public bool HasWaveStarted()
    {
        return waveStarted;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }
}