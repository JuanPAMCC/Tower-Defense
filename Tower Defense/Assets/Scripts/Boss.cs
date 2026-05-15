public class Boss : EnemyBase
{
    protected override void RegisterDefeat()
    {
        if (gameManager != null)
        {
            gameManager.bossEnemiesDefeated++;
        }
    }
}