using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    public int health = 100;
    public int damage = 5;
    public float stopDistance = 2.4f;

    private NavMeshAgent agent;
    private Animator animator;
    private Rigidbody rigidbodyComponent;
    private Target targetScript;
    private bool targetDestroyed;
    private bool reachedTarget;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rigidbodyComponent = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        if (target == null)
        {
            target = GameObject.FindWithTag("Objetivo");
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

    void Start()
    {
        if (target != null)
        {
            agent.stoppingDistance = stopDistance;
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
            animator.SetBool("isMoving", true);
        }
    }

    void OnDisable()
    {
        if (targetScript != null)
        {
            targetScript.OnTargetDestroyed -= HandleTargetDestroyed;
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (!targetDestroyed && !reachedTarget && target != null)
        {
            float distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance <= stopDistance)
            {
                reachedTarget = true;
                StopEnemy();
                animator.SetTrigger("onObjectiveReached");
            }
        }
    }

    public void DealDamage()
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
        targetScript.ReceiveDamage(damage);
    }

    public void ReceiveDamage(int damage = 20)
    {
        health -= damage;
    }

    private void HandleTargetDestroyed()
    {
        if (targetDestroyed) return;
        targetDestroyed = true;
        StopEnemy();
        animator.SetTrigger("onObjectiveDestroyed");
    }

    private void StopEnemy()
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
}