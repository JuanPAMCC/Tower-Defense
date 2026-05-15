using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int baseEnemiesDefeated;
    public int bossEnemiesDefeated;
    public int playerResources = 800;

    public delegate void ResourcesChanged(int currentResources);
    public event ResourcesChanged OnResourcesChanged;

    void Start()
    {
        NotifyResourcesChanged();
    }

    public void ModifyResources(int amount)
    {
        playerResources += amount;

        if (playerResources < 0)
        {
            playerResources = 0;
        }

        NotifyResourcesChanged();
    }

    public void ResetValues()
    {
        baseEnemiesDefeated = 0;
        bossEnemiesDefeated = 0;

        NotifyResourcesChanged();
    }

    private void NotifyResourcesChanged()
    {
        if (OnResourcesChanged != null)
        {
            OnResourcesChanged(playerResources);
        }
    }
}