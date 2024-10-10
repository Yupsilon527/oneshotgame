using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public static EnemyController main;
    private void Awake()
    {
        main = this;
    }
    public float lastSpawn = 3f;
    public ObjectPool enemypool;

    public EnemyData[] enemiesSpawned;
    private void Update()
    {

        if (lastSpawn < Level.main.gameTime)
        {
            lastSpawn += .33f;
            SpawnWave(enemiesSpawned[Mathf.FloorToInt(Random.value * enemiesSpawned.Length)], 10);
            //Bullet.FireBullet(new Vector2(UnityEngine.Random.Range(CameraBounds.xMin, CameraBounds.xMax),CameraBounds.yMax), Vector2.down, 10, 4, .25f, 0);
        }
    }
    #region Spawn Enemies

    public void SpawnWave(EnemyData name, int amount)
    {
        for (int I = 0; I < amount; I++)
        {
            SpawnEnemy(name);
        }
    }

    public void SpawnWave(EnemyData[] wave)
    {
        foreach (EnemyData name in wave)
        {
            SpawnEnemy(name);
        }
    }
    public void SpawnEnemy(EnemyData name)
    {
        if (enemypool.activeObjs.childCount < 50)
            Enemy.FromData(name);
    }
    #endregion
}
