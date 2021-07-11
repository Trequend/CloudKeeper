using UnityEngine;

public class EnemyDestroyer : MonoBehaviour
{
    public void DestroyEnemies()
    {
        Enemy.ForeachEnemy(enemy => enemy.Die());
    }
}
