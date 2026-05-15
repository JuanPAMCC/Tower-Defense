using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public delegate void EnemyUpdated(GameObject enemy);
    public event EnemyUpdated OnEnemyUpdated;

    public enum TowerType
    {
        TowerOne,
        TowerTwo,
        TowerThree,
        TowerFour,
        TowerFive
    }

    public TouchManager touchManager;
    public GameManager gameManager;
    public EnemySpawner enemySpawner;
    public GameObject target;

    public TowerType selectedTower;
    public List<GameObject> towerPrefabs = new List<GameObject>();

    public int towerOneCost = 400;
    public int towerTwoCost = 600;
    public int towerThreeCost = 800;
    public int towerFourCost = 1000;
    public int towerFiveCost = 1200;
    public float updateTargetInterval = 3f;

    private List<GameObject> instantiatedTowers = new List<GameObject>();

    void Awake()
    {
        instantiatedTowers = new List<GameObject>();
    }

    void OnEnable()
    {
        if (touchManager != null)
        {
            touchManager.OnPlatformTouched += CreateTower;
        }

        if (enemySpawner != null)
        {
            enemySpawner.OnWaveStarted += StartUpdatingTargets;
            enemySpawner.OnWaveWon += FinishWave;
        }
    }

    void OnDisable()
    {
        if (touchManager != null)
        {
            touchManager.OnPlatformTouched -= CreateTower;
        }

        if (enemySpawner != null)
        {
            enemySpawner.OnWaveStarted -= StartUpdatingTargets;
            enemySpawner.OnWaveWon -= FinishWave;
        }

        CancelInvoke(nameof(UpdateTargets));
    }

    private void CreateTower(GameObject platform)
    {
        Debug.Log("Creating tower");

        int towerIndex = (int)selectedTower;

        if (towerIndex < 0 || towerIndex >= towerPrefabs.Count)
        {
            Debug.LogError("Tower is not defined");
            return;
        }

        int towerCost = GetTowerCost();

        if (platform.transform.childCount > 0)
        {
            return;
        }

        if (gameManager != null && gameManager.playerResources < towerCost)
        {
            return;
        }

        Vector3 spawnPosition = platform.transform.position;
        spawnPosition.y += 0.5f;

        GameObject tower = Instantiate(towerPrefabs[towerIndex], spawnPosition, Quaternion.identity);
        tower.transform.SetParent(platform.transform);

        instantiatedTowers.Add(tower);

        if (gameManager != null)
        {
            gameManager.ModifyResources(-towerCost);
        }
    }

    public void SetTower(int towerIndex)
    {
        if (System.Enum.IsDefined(typeof(TowerType), towerIndex))
        {
            selectedTower = (TowerType)towerIndex;
        }
        else
        {
            Debug.LogError("Tower is not defined");
        }
    }

    private int GetTowerCost()
    {
        switch (selectedTower)
        {
            case TowerType.TowerOne:
                return towerOneCost;
            case TowerType.TowerTwo:
                return towerTwoCost;
            case TowerType.TowerThree:
                return towerThreeCost;
            case TowerType.TowerFour:
                return towerFourCost;
            case TowerType.TowerFive:
                return towerFiveCost;
            default:
                return 0;
        }
    }

    private void StartUpdatingTargets()
    {
        CancelInvoke(nameof(UpdateTargets));
        UpdateTargets();
    }

    private void FinishWave()
    {
        CancelInvoke(nameof(UpdateTargets));
        ClearTowerTargets();
        ClearTowers();
    }

    private void UpdateTargets()
    {
        for (int i = 0; i < instantiatedTowers.Count; i++)
        {
            if (instantiatedTowers[i] == null)
            {
                continue;
            }

            TowerBase towerBase = instantiatedTowers[i].GetComponent<TowerBase>();

            if (towerBase == null)
            {
                continue;
            }

            GameObject closestEnemy = GetClosestEnemyForTower(towerBase);

            if (closestEnemy != null)
            {
                towerBase.SetEnemy(closestEnemy);
                towerBase.Shoot();

                if (OnEnemyUpdated != null)
                {
                    OnEnemyUpdated(closestEnemy);
                }
            }
            else
            {
                towerBase.ClearEnemy();
            }
        }

        if (enemySpawner == null || enemySpawner.HasWaveStarted())
        {
            Invoke(nameof(UpdateTargets), updateTargetInterval);
        }
    }

    private GameObject GetClosestEnemyForTower(TowerBase towerBase)
    {
        if (target == null || towerBase == null)
        {
            return null;
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemigo");

        if (enemies.Length == 0)
        {
            return null;
        }

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
            {
                continue;
            }

            if (!towerBase.IsEnemyInRange(enemies[i]))
            {
                continue;
            }

            float distance = Vector3.Distance(target.transform.position, enemies[i].transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemies[i];
            }
        }

        return closestEnemy;
    }

    private void ClearTowerTargets()
    {
        for (int i = 0; i < instantiatedTowers.Count; i++)
        {
            if (instantiatedTowers[i] == null)
            {
                continue;
            }

            TowerBase towerBase = instantiatedTowers[i].GetComponent<TowerBase>();

            if (towerBase != null)
            {
                towerBase.ClearEnemy();
            }
        }
    }

    private void ClearTowers()
    {
        for (int i = 0; i < instantiatedTowers.Count; i++)
        {
            if (instantiatedTowers[i] != null)
            {
                Destroy(instantiatedTowers[i]);
            }
        }

        instantiatedTowers.Clear();
    }
}