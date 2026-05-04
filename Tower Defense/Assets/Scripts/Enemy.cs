using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public GameObject target;
    public int health = 100;

    private NavMeshAgent agent;
    private Animator animator;
    private bool targetDestroyed;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        agent.SetDestination(target.transform.position);
        animator.SetBool("isMoving", true);
    }

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
            return;
        }

        if (target == null)
        {
            if (!targetDestroyed)
            {
                targetDestroyed = true;
                animator.SetBool("isMoving", false);
                animator.SetTrigger("onObjectiveDestroyed");
            }

            return;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (target != null && collision.gameObject == target)
        {
            animator.SetBool("isMoving", false);
            animator.SetTrigger("onObjectiveReached");
        }
    }

    public void DealDamage()
    {
        if (target == null)
        {
            return;
        }

        Target targetScript = target.GetComponent<Target>();

        if (targetScript != null)
        {
            targetScript.ReceiveDamage(40);
        }
    }

    public void ReceiveDamage(int damage = 20)
    {
        health -= damage;
    }
}