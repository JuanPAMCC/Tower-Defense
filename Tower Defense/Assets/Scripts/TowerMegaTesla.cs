using System.Collections.Generic;
using UnityEngine;

public class TowerMegaTesla : TowerBase
{
    public int startDamage = 25;
    public int damageLossPerJump = 5;
    public int maxTargets = 5;
    public float chainRange = 8f;
    public float shootCooldown = 9f;
    public float rayDuration = 0.2f;
    public float targetHeightOffset = 1f;

    private LineRenderer lineRendererComponent;
    private float nextShootTime;
    private List<GameObject> hitEnemies = new List<GameObject>();
    private List<Vector3> rayPoints = new List<Vector3>();

    void Start()
    {
        lineRendererComponent = GetComponent<LineRenderer>();
        nextShootTime = -999f;
        ClearRay();
    }

    public override void Shoot()
    {
        if (Time.time < nextShootTime)
        {
            return;
        }

        if (enemy == null || !IsEnemyInRange(enemy))
        {
            ClearRay();
            return;
        }

        nextShootTime = Time.time + shootCooldown;
        ChainAttack();
    }

    private void ChainAttack()
    {
        hitEnemies.Clear();
        rayPoints.Clear();

        GameObject currentEnemy = enemy;
        int currentDamage = startDamage;

        rayPoints.Add(GetRayStartPoint());

        while (currentEnemy != null && currentDamage > 0 && hitEnemies.Count < maxTargets)
        {
            EnemyBase enemyBase = GetEnemyBase(currentEnemy);

            if (enemyBase == null || enemyBase.health <= 0)
            {
                break;
            }

            hitEnemies.Add(currentEnemy);
            enemyBase.ReceiveDamage(currentDamage);
            rayPoints.Add(GetEnemyPoint(currentEnemy));

            currentDamage -= damageLossPerJump;
            currentEnemy = GetNextEnemy(currentEnemy, hitEnemies);
        }

        DrawRay();
    }

    private GameObject GetNextEnemy(GameObject currentEnemy, List<GameObject> ignoredEnemies)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemigo");

        GameObject closestEnemy = null;
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null || ignoredEnemies.Contains(enemies[i]))
            {
                continue;
            }

            EnemyBase enemyBase = GetEnemyBase(enemies[i]);

            if (enemyBase == null || enemyBase.health <= 0)
            {
                continue;
            }

            float distance = Vector3.Distance(currentEnemy.transform.position, enemies[i].transform.position);

            if (distance <= chainRange && distance < closestDistance)
            {
                closestDistance = distance;
                closestEnemy = enemies[i];
            }
        }

        return closestEnemy;
    }

    private EnemyBase GetEnemyBase(GameObject targetEnemy)
    {
        EnemyBase enemyBase = targetEnemy.GetComponent<EnemyBase>();

        if (enemyBase == null)
        {
            enemyBase = targetEnemy.GetComponentInParent<EnemyBase>();
        }

        return enemyBase;
    }

    private Vector3 GetRayStartPoint()
    {
        if (cannonTips.Count > 0 && cannonTips[0] != null)
        {
            return cannonTips[0].transform.position;
        }

        return transform.position;
    }

    private Vector3 GetEnemyPoint(GameObject targetEnemy)
    {
        Vector3 point = targetEnemy.transform.position;
        point.y += targetHeightOffset;
        return point;
    }

    private void DrawRay()
    {
        if (lineRendererComponent == null || rayPoints.Count <= 1)
        {
            ClearRay();
            return;
        }

        CancelInvoke(nameof(ClearRay));

        lineRendererComponent.positionCount = rayPoints.Count;
        lineRendererComponent.SetPositions(rayPoints.ToArray());

        Invoke(nameof(ClearRay), rayDuration);
    }

    private void ClearRay()
    {
        if (lineRendererComponent != null)
        {
            lineRendererComponent.positionCount = 0;
        }
    }
}