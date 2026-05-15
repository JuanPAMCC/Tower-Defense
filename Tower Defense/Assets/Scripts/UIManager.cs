using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject gameOverMenu;
    public GameObject waveWonMenu;
    public GameObject lastEnemyMessage;

    public TMP_Text resourcesText;
    public TMP_Text waveText;
    public TMP_Text baseEnemiesText;
    public TMP_Text bossEnemiesText;

    public GameManager gameManager;
    public EnemySpawner enemySpawner;
    public Target target;

    void OnEnable()
    {
        if (gameManager != null)
        {
            gameManager.OnResourcesChanged += UpdateResources;
        }

        if (enemySpawner != null)
        {
            enemySpawner.OnWaveStarted += UpdateWave;
            enemySpawner.OnWaveStarted += HideWaveWonMenu;
            enemySpawner.OnOneEnemyLeft += ShowLastEnemyMessage;
            enemySpawner.OnWaveWon += ShowWaveWonMenu;
        }

        if (target != null)
        {
            target.OnTargetDestroyed += ShowGameOverMenu;
        }
    }

    void Start()
    {
        HideGameOverMenu();
        HideWaveWonMenu();
        HideLastEnemyMessage();

        if (gameManager != null)
        {
            UpdateResources(gameManager.playerResources);
        }

        UpdateWave();
    }

    void OnDisable()
    {
        if (gameManager != null)
        {
            gameManager.OnResourcesChanged -= UpdateResources;
        }

        if (enemySpawner != null)
        {
            enemySpawner.OnWaveStarted -= UpdateWave;
            enemySpawner.OnWaveStarted -= HideWaveWonMenu;
            enemySpawner.OnOneEnemyLeft -= ShowLastEnemyMessage;
            enemySpawner.OnWaveWon -= ShowWaveWonMenu;
        }

        if (target != null)
        {
            target.OnTargetDestroyed -= ShowGameOverMenu;
        }
    }

    public void ShowWaveWonMenu()
    {
        if (waveWonMenu != null)
        {
            waveWonMenu.SetActive(true);
        }

        if (baseEnemiesText != null && gameManager != null)
        {
            baseEnemiesText.text = "Enemigos derrotados: " + gameManager.baseEnemiesDefeated;
        }

        if (bossEnemiesText != null && gameManager != null)
        {
            bossEnemiesText.text = "Jefes derrotados: " + gameManager.bossEnemiesDefeated;
        }
    }

    public void HideWaveWonMenu()
    {
        if (waveWonMenu != null)
        {
            waveWonMenu.SetActive(false);
        }
    }

    public void ShowLastEnemyMessage()
    {
        if (lastEnemyMessage != null)
        {
            lastEnemyMessage.SetActive(true);
            Invoke(nameof(HideLastEnemyMessage), 3f);
        }
    }

    public void HideLastEnemyMessage()
    {
        if (lastEnemyMessage != null)
        {
            lastEnemyMessage.SetActive(false);
        }
    }

    public void ShowGameOverMenu()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(true);
        }
    }

    public void HideGameOverMenu()
    {
        if (gameOverMenu != null)
        {
            gameOverMenu.SetActive(false);
        }
    }

    public void UpdateResources(int currentResources)
    {
        if (resourcesText != null)
        {
            resourcesText.text = "Recursos: " + currentResources;
        }
    }

    public void UpdateWave()
    {
        if (waveText != null && enemySpawner != null)
        {
            waveText.text = "Ola: " + (enemySpawner.GetCurrentWave() + 1);
        }
    }
}