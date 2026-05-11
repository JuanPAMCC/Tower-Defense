using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public delegate void WaveFinished();
    public event WaveFinished OnWaveFinished;

    public List<GameObject> enemyPrefabs = new List<GameObject>();
    public List<int> enemiesPerWave = new List<int>();
    public float spawnDelay = 1f;

    private int currentWave;
    private int enemiesDuringWave;

    void Start()
    {
        currentWave = 0;
        ConfigureEnemiesDuringWave();
        SpawnEnemy();
    }

    private void FinishWave()
    {
        if (OnWaveFinished != null)
        {
            OnWaveFinished();
        }
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

    public void SpawnEnemy()
    {
        if (enemyPrefabs.Count == 0 || enemiesDuringWave <= 0)
        {
            FinishWave();
            return;
        }

        int randomIndex = Random.Range(0, enemyPrefabs.Count);

        Instantiate(enemyPrefabs[randomIndex], transform.position, Quaternion.identity);

        enemiesDuringWave--;

        if (enemiesDuringWave > 0)
        {
            Invoke(nameof(SpawnEnemy), spawnDelay);
        }
        else
        {
            currentWave++;
            ConfigureEnemiesDuringWave();
            FinishWave();
        }
    }
}