using UnityEngine;

public class Bullet : MonoBehaviour, IAttacker
{
    public Vector3 destination;
    public float speed = 20f;
    public int damage = 10;
    public float hitDistance = 0.6f;

    private EnemyBase enemy;

    void Update()
    {
        if (enemy != null)
        {
            destination = enemy.transform.position;
            destination.y += 1f;
        }

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, destination, step);

        if (enemy != null && Vector3.Distance(transform.position, destination) <= hitDistance)
        {
            DealDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (enemy == null && Vector3.Distance(transform.position, destination) <= 0.05f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyBase hitEnemy = other.GetComponent<EnemyBase>();

        if (hitEnemy == null)
        {
            hitEnemy = other.GetComponentInParent<EnemyBase>();
        }

        if (hitEnemy == null)
        {
            return;
        }

        enemy = hitEnemy;
        DealDamage(damage);
        Destroy(gameObject);
    }

    public void SetTarget(GameObject targetEnemy, int attackDamage)
    {
        if (targetEnemy != null)
        {
            enemy = targetEnemy.GetComponent<EnemyBase>();

            if (enemy == null)
            {
                enemy = targetEnemy.GetComponentInParent<EnemyBase>();
            }

            destination = targetEnemy.transform.position;
            destination.y += 1f;
        }

        damage = attackDamage;
    }

    public void DealDamage(int attackDamage = 0)
    {
        if (enemy == null)
        {
            return;
        }

        int finalDamage = attackDamage == 0 ? damage : attackDamage;
        enemy.ReceiveDamage(finalDamage);
    }
}