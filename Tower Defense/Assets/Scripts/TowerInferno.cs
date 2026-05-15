using System.Collections.Generic;
using UnityEngine;

public class TowerInferno : TowerBase
{
    public int startDamage = 2;
    public int maxDamage = 40;
    public float damageGrowthPerSecond = 4f;
    public float damageTickInterval = 0.5f;
    public int rayDivisions = 6;
    public float rayOffset = 0.4f;

    private LineRenderer lineRendererComponent;
    private GameObject currentEnemy;
    private float lockTime;
    private float nextDamageTime;
    private List<Vector3> rayPoints = new List<Vector3>();

    void Start()
    {
        lineRendererComponent = GetComponent<LineRenderer>();
        ClearRay();
    }

    void FixedUpdate()
    {
        if (enemy == null || !IsEnemyInRange(enemy))
        {
            ResetLock();
            ClearRay();
            return;
        }

        if (currentEnemy != enemy)
        {
            currentEnemy = enemy;
            lockTime = 0f;
            nextDamageTime = 0f;
        }

        lockTime += Time.fixedDeltaTime;
        DrawRay();

        if (Time.time >= nextDamageTime)
        {
            ApplyDamage();
            nextDamageTime = Time.time + damageTickInterval;
        }
    }

    public override void Shoot()
    {
    }

    private void ApplyDamage()
    {
        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();

        if (enemyBase == null)
        {
            enemyBase = enemy.GetComponentInParent<EnemyBase>();
        }

        if (enemyBase == null)
        {
            return;
        }

        int currentDamage = Mathf.Clamp(startDamage + Mathf.FloorToInt(lockTime * damageGrowthPerSecond), startDamage, maxDamage);
        enemyBase.ReceiveDamage(currentDamage);
    }

    private void DrawRay()
    {
        if (lineRendererComponent == null || enemy == null)
        {
            return;
        }

        Vector3 startPoint = GetRayStartPoint();
        Vector3 endPoint = enemy.transform.position;
        endPoint.y += 1f;

        rayPoints = GetRayPoints(startPoint, endPoint);
        rayPoints.Insert(0, startPoint);
        rayPoints.Add(endPoint);

        lineRendererComponent.positionCount = rayPoints.Count;
        lineRendererComponent.SetPositions(rayPoints.ToArray());
    }

    private Vector3 GetRayStartPoint()
    {
        if (cannonTips.Count > 0 && cannonTips[0] != null)
        {
            return cannonTips[0].transform.position;
        }

        return transform.position;
    }

    private List<Vector3> GetRayPoints(Vector3 startPoint, Vector3 endPoint)
    {
        List<Vector3> points = new List<Vector3>();

        if (rayDivisions <= 0)
        {
            return points;
        }

        float step = 1f / rayDivisions;

        for (int i = 1; i < rayDivisions; i++)
        {
            float lerpValue = step * i;
            Vector3 point = Vector3.Lerp(startPoint, endPoint, lerpValue);
            point += Random.insideUnitSphere * rayOffset;
            points.Add(point);
        }

        return points;
    }

    private void ResetLock()
    {
        currentEnemy = null;
        lockTime = 0f;
        nextDamageTime = 0f;
    }

    private void ClearRay()
    {
        if (lineRendererComponent != null)
        {
            lineRendererComponent.positionCount = 0;
        }
    }
}