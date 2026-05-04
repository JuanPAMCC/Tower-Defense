using UnityEngine;

public class Target : MonoBehaviour
{
    public int health = 100;

    void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ReceiveDamage(int damage = 20)
    {
        health -= damage;
    }
}