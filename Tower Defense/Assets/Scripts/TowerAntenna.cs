using System.Collections.Generic;
using UnityEngine;

public class TowerAntenna : TowerBase
{
    public int rayDivisions = 10;
    public float rayOffset = 2f;
    public int rayDamage = 10;

    private LineRenderer lineRendererComponent;
    private List<Vector3> rayPoints = new List<Vector3>();

    void Start()
    {
        lineRendererComponent = GetComponent<LineRenderer>();
    }

    void FixedUpdate()
    {
        if (enemy != null && IsEnemyInRange(enemy))
        {
            Shoot();
        }
        else
        {
            ClearRay();
        }
    }

    public override void Shoot()
    {
        if (enemy == null || lineRendererComponent == null)
        {
            ClearRay();
            return;
        }

        EnemyBase enemyBase = enemy.GetComponent<EnemyBase>();

        if (enemyBase == null)
        {
            enemyBase = enemy.GetComponentInParent<EnemyBase>();
        }

        if (enemyBase == null)
        {
            ClearRay();
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

        enemyBase.ReceiveDamage(rayDamage);
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

        if (rayDivisions == 1)
        {
            points.Add(Vector3.Lerp(startPoint, endPoint, 0.5f));
            return points;
        }

        float step = 1f / rayDivisions;
        bool positive = true;

        for (int i = 1; i < rayDivisions; i++)
        {
            float lerpValue = step * i;
            Vector3 point = Vector3.Lerp(startPoint, endPoint, lerpValue);

            float offset = Random.value * rayOffset;

            if (positive)
            {
                point.x += offset;
            }
            else
            {
                point.x -= offset;
            }

            positive = !positive;
            points.Add(point);
        }

        return points;
    }

    private void ClearRay()
    {
        if (lineRendererComponent != null)
        {
            lineRendererComponent.positionCount = 0;
        }
    }
}