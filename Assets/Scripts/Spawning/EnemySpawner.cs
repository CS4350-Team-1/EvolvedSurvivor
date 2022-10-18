using MoreMountains.Tools;
using System.Collections.Generic;
using System.Linq;
using TeamOne.EvolvedSurvivor;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class EnemySpawnData
{
    public GameObject enemyObject;

    public int currentCount;
    public float spawnLimit;
    public int cost;

    public float limitScalingFactor;

    public void UpdateLimit(float deltaTime)
    {
        spawnLimit += limitScalingFactor * deltaTime;
    }

    public bool CanSpawn(float pointsLeft)
    {
        return Mathf.FloorToInt(pointsLeft) >= cost && currentCount < Mathf.FloorToInt(spawnLimit);
    }

    public string GetEnemyName()
    {
        currentCount++;
        return enemyObject.name;
    }
}

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private SpawnManagerScriptableObject spawnManager;
    [SerializeField] private List<EnemySpawnData> enemies;
    [SerializeField] private NonRandomObjectPooler objectPooler;

    [SerializeField] private float spendingPointsGainRate = 0.1f;
    [SerializeField] private float minSpawnFreq = 0.2f;
    [SerializeField] private float maxSpawnFreq = 1f;
    [SerializeField] private float spawnRadius = 20f;

    private List<string> enemiesToSpawn = new List<string>();

    // The spawner uses a point based system to pick enemies to spawn.
    private float spendingPoints = 10f;
    private float timePassed = 0f;
    private float nextSpawnTimestamp = 0f;

    private void Start()
    {
        enemies = enemies.OrderBy(e => e.cost).ToList();
        enemies.Reverse();

        foreach (EnemySpawnData enemy in enemies)
        {
            MMMultipleObjectPoolerObject poolerObject = new MMMultipleObjectPoolerObject();
            poolerObject.GameObjectToPool = enemy.enemyObject;
            poolerObject.PoolSize = 10;

            objectPooler.Pool.Add(poolerObject);
        }
        objectPooler.FillObjectPool();
    }

    private void Update()
    {
        if (timePassed >= nextSpawnTimestamp)
        {
            Spawn();
        }
    }

    protected void FixedUpdate()
    {
        spendingPoints += Time.deltaTime * spendingPointsGainRate;
        timePassed += Time.deltaTime;

        foreach (EnemySpawnData enemy in enemies)
        {
            enemy.UpdateLimit(Time.deltaTime);
        }
    }

    private void OnEnable()
    {
        spawnManager.PlayerSpawnEvent.AddListener(AttachToPlayer);
        spawnManager.EnemyDespawnEvent.AddListener(RefundEnemyCost);
    }

    private void OnDisable()
    {
        spawnManager.PlayerSpawnEvent.RemoveListener(AttachToPlayer);
        spawnManager.EnemyDespawnEvent.RemoveListener(RefundEnemyCost);

        objectPooler.DestroyObjectPool();
    }

    protected void Spawn()
    {
        foreach (EnemySpawnData enemy in enemies)
        {
            while (enemy.CanSpawn(spendingPoints))
            {
                enemiesToSpawn.Add(enemy.GetEnemyName());
                spendingPoints -= enemy.cost;
            }
        }

        SpawnEnemiesInCircle();
        enemiesToSpawn.Clear();

        nextSpawnTimestamp = timePassed + Random.Range(minSpawnFreq, maxSpawnFreq);
    }

    private void AttachToPlayer(PlayerController player)
    {
        gameObject.transform.position = player.transform.position;
        gameObject.transform.parent = player.transform;
    }

    private void SpawnEnemiesInCircle()
    {
        int enemyCount = enemiesToSpawn.Count;
        float angle = 360f / enemyCount;
        Vector3 startVector = Quaternion.Euler(0f, 0f, Random.Range(0f, 180f)) * new Vector3(1f, 0f, 0f) * spawnRadius;
        Vector3 playerPosition = this.transform.position;

        for (int i = 0; i < enemyCount; i++)
        {
            GameObject enemyToSpawn = objectPooler.GetPooledObjectByName(enemiesToSpawn[i]);
            Vector3 enemySpawnPosition = (Quaternion.Euler(0f, 0f, angle * i) * startVector) + playerPosition;
            enemyToSpawn.transform.position = enemySpawnPosition;
            enemyToSpawn.SetActive(true);

            enemyToSpawn.MMGetComponentNoAlloc<MMPoolableObject>().TriggerOnSpawnComplete();
            enemyToSpawn.GetComponent<Enemy>().ScaleStats(timePassed);
        }
    }

    private void RefundEnemyCost(GameObject enemyObject)
    {
        EnemySpawnData enemy = enemies.Find(e => e.enemyObject.name == enemyObject.name);
        spendingPoints += enemy.cost;
        enemy.currentCount--;
    }
}
