using System.Collections.Generic;
using UnityEngine;

public class TowerManager : MonoBehaviour
{
    public enum TowerType
    {
        TowerOne,
        TowerTwo,
        TowerThree,
        TowerFour,
        TowerFive
    }

    public TouchManager touchManager;
    public TowerType selectedTower;
    public List<GameObject> towerPrefabs = new List<GameObject>();

    void OnEnable()
    {
        if (touchManager != null)
        {
            touchManager.OnPlatformTouched += CreateTower;
        }
    }

    void OnDisable()
    {
        if (touchManager != null)
        {
            touchManager.OnPlatformTouched -= CreateTower;
        }
    }

    private void CreateTower(GameObject platform)
    {
        Debug.Log("Creating tower");

        if (platform.transform.childCount > 0)
        {
            return;
        }

        int towerIndex = (int)selectedTower;

        if (towerIndex < 0 || towerIndex >= towerPrefabs.Count)
        {
            Debug.LogError("Tower is not defined");
            return;
        }

        Vector3 spawnPosition = platform.transform.position;
        spawnPosition.y += 0.5f;

        GameObject tower = Instantiate(towerPrefabs[towerIndex], spawnPosition, Quaternion.identity);
        tower.transform.SetParent(platform.transform);
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
}