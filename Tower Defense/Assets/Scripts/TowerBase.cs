using System.Collections.Generic;
using UnityEngine;

public class TowerBase : MonoBehaviour
{
    public GameObject enemy;
    public GameObject bulletPrefab;
    public Transform rotatingPart;
    public List<GameObject> cannonTips = new List<GameObject>();
    public int projectileDamage = 10;
    public float attackRange = 15f;

    void Update()
    {
        if (enemy != null && IsEnemyInRange(enemy))
        {
            Aim();
        }
    }

    protected virtual void Aim()
    {
        Transform partToRotate = rotatingPart != null ? rotatingPart : transform;

        Vector3 enemyPosition = enemy.transform.position;
        enemyPosition.y = partToRotate.position.y;

        partToRotate.LookAt(enemyPosition);
    }

    public virtual void Shoot()
    {
        if (enemy == null || bulletPrefab == null)
        {
            return;
        }

        if (!IsEnemyInRange(enemy))
        {
            return;
        }

        for (int i = 0; i < cannonTips.Count; i++)
        {
            if (cannonTips[i] == null)
            {
                continue;
            }

            GameObject bulletObject = Instantiate(bulletPrefab, cannonTips[i].transform.position, Quaternion.identity);
            Bullet bullet = bulletObject.GetComponent<Bullet>();

            if (bullet != null)
            {
                bullet.SetTarget(enemy, projectileDamage);
            }
        }
    }

    public bool IsEnemyInRange(GameObject targetEnemy)
    {
        if (targetEnemy == null)
        {
            return false;
        }

        Vector3 origin = rotatingPart != null ? rotatingPart.position : transform.position;
        float distance = Vector3.Distance(origin, targetEnemy.transform.position);

        return distance <= attackRange;
    }

    public void SetEnemy(GameObject newEnemy)
    {
        enemy = newEnemy;
    }

    public void ClearEnemy()
    {
        enemy = null;
    }
}