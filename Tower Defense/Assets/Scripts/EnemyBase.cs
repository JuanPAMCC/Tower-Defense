using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour, IAttackable, IAttacker
{
    public GameObject target;
    public int health = 100;
    public int damage = 5;
    public int resourceReward = 100;
    public float stopDistance = 2.4f;
    public float deathDelay = 3f;

    protected NavMeshAgent agent;
    protected Animator animator;
    protected Rigidbody rigidbodyComponent;
    protected Target targetScript;
    protected GameManager gameManager;
    protected EnemySpawner enemySpawner;
    protected bool targetDestroyed;
    protected bool reachedTarget;
    protected bool isDead;
    protected bool rewardGiven;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody>();
     
    }

    protected virtual void OnEnable()
    {
        if (target == null)
        {
            target = GameObject.FindWithTag("Objetivo");
        }

        GameObject gameManagerObject = GameObject.Find("Game Manager");

        if (gameManagerObject != null)
        {
            gameManager = gameManagerObject.GetComponent<GameManager>();
        }

        GameObject enemySpawnerObject = GameObject.Find("Enemy Spawner");

        if (enemySpawnerObject != null)
        {
            enemySpawner = enemySpawnerObject.GetComponent<EnemySpawner>();
        }

        if (target != null)
        {
            targetScript = target.GetComponent<Target>();

            if (targetScript != null)
            {
                targetScript.OnTargetDestroyed += HandleTargetDestroyed;
            }
        }
    }

    protected virtual void Start()
    {
        if (target != null && agent != null)
        {
            agent.stoppingDistance = stopDistance;
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", true);
        }
    }

    protected virtual void OnDisable()
    {
        if (targetScript != null)
        {
            targetScript.OnTargetDestroyed -= HandleTargetDestroyed;
        }
    }

    protected virtual void OnDestroy()
    {
        if (enemySpawner != null)
        {
            enemySpawner.RemoveEnemy(gameObject);
        }

        if (!isDead || rewardGiven)
        {
            return;
        }

        rewardGiven = true;

        if (gameManager != null)
        {
            gameManager.ModifyResources(resourceReward);
            RegisterDefeat();
        }
    }

    protected virtual void Update()
    {
        if (isDead)
        {
            return;
        }

        if (health <= 0)
        {
            Die();
            return;
        }

        if (!targetDestroyed && !reachedTarget && target != null)
        {
            if (HasReachedTarget())
            {
                reachedTarget = true;
                StopEnemy();

                if (animator != null)
                {
                    animator.SetTrigger("onObjectiveReached");
                }
            }
        }
    }

    protected virtual bool HasReachedTarget()
    {
        if (target == null)
        {
            return false;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance <= stopDistance)
        {
            return true;
        }

        if (agent != null && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            return true;
        }

        return false;
    }

    public void DealDamage()
    {
        DealDamage(damage);
    }

    public virtual void DealDamage(int attackDamage = 0)
    {
        if (targetDestroyed || targetScript == null)
        {
            HandleTargetDestroyed();
            return;
        }

        if (targetScript.health <= 0)
        {
            HandleTargetDestroyed();
            return;
        }

        int finalDamage = attackDamage == 0 ? damage : attackDamage;
        targetScript.ReceiveDamage(finalDamage);
    }

    public virtual void ReceiveDamage(int incomingDamage = 20)
    {
        if (isDead)
        {
            return;
        }

        health -= incomingDamage;

        if (health <= 0)
        {
            Die();
        }
    }

    protected virtual void HandleTargetDestroyed()
    {
        if (targetDestroyed)
        {
            return;
        }

        targetDestroyed = true;
        StopEnemy();

        if (animator != null)
        {
            animator.SetTrigger("onObjectiveDestroyed");
        }
    }

    protected virtual void StopEnemy()
    {
        if (agent != null)
        {
            agent.isStopped = true;
            agent.velocity = Vector3.zero;
            agent.ResetPath();
        }

        if (rigidbodyComponent != null)
        {
            rigidbodyComponent.linearVelocity = Vector3.zero;
            rigidbodyComponent.isKinematic = true;
        }

        if (animator != null)
        {
            animator.SetBool("isMoving", false);
        }
    }

    protected virtual void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        StopEnemy();

        if (animator != null)
        {
            animator.SetTrigger("onDeath");
        }

        Destroy(gameObject, deathDelay);
    }

    protected virtual void RegisterDefeat()
    {
    }
}