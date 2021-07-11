using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    private class Combination : IEnumerable
    {
        public Figure[] Figures;

        public IEnumerator GetEnumerator()
        {
            return Figures.GetEnumerator();
        }
    }

    [System.Serializable]
    private class EnemyInfo
    {
        public Enemy Prefab;

        public Combination[] Combinations;
    }

    [SerializeField] private Character _character;

    [SerializeField] private Transform _enemiesTarget;

    [SerializeField] private BoxCollider2D _spawnZone;

    [SerializeField] private float _spawnInterval = 0.3f;

    [SerializeField] private EnemyInfo[] _enemies;

    private Coroutine _spawn;

    public void StartSpawn()
    {
        if (_spawn != null)
        {
            return;
        }

        _spawn = StartCoroutine(Spawn());
    }

    public void StopSpawn()
    {
        if (_spawn == null)
        {
            return;
        }

        StopCoroutine(_spawn);
        _spawn = null;
    }

    private IEnumerator Spawn()
    {
        do
        {
            SpawnEnemy();
            yield return new WaitForSeconds(_spawnInterval);
        }
        while (true);
    }

    private void SpawnEnemy()
    {
        EnemyInfo info = _enemies[Random.Range(0, _enemies.Length)];

        Bounds spawnBounds = _spawnZone.bounds;
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnBounds.min.x, spawnBounds.max.x),
            Random.Range(spawnBounds.min.y, spawnBounds.max.y),
            0.0f
        );

        Enemy enemy = Instantiate(info.Prefab, spawnPosition, Quaternion.identity);

        enemy.gameObject.AddComponent<ParabolaMover>()
            .SetTargetPosition(_enemiesTarget.position)
            .SetSpeed(4.0f)
            .SetCallback(() => enemy.Attack(_character))
            .StartMoving();

        Combination combination = info.Combinations[Random.Range(0, info.Combinations.Length)];
        foreach (Figure figure in combination)
        {
            enemy.AddFigure(figure);
        }
    }
}
