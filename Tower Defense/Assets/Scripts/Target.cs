using UnityEngine;

public class Target : MonoBehaviour, IAttackable
{
    public delegate void TargetDestroyed();
    public event TargetDestroyed OnTargetDestroyed;

    public int health = 100;

    private bool isDestroyed;

    public void ReceiveDamage(int damage = 20)
    {
        if (isDestroyed)
        {
            return;
        }

        health -= damage;

        if (health <= 0)
        {
            DestroyTarget();
        }
    }

    private void DestroyTarget()
    {
        if (isDestroyed)
        {
            return;
        }

        isDestroyed = true;

        if (OnTargetDestroyed != null)
        {
            OnTargetDestroyed();
        }

        Destroy(gameObject);
    }
}