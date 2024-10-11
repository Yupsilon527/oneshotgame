using System.Collections.Generic;
using UnityEngine;

public class PupController : MonoBehaviour
{
    public static PupController main;

    public GameObject powerupPrefab;
    public ObjectPool puppool;
    public List<Powerup> powerups = new();

    public int totalActivePowerups = 2;
    public Transform[] SpawnPoints;

    public PowerupData[] bonusesSpawned;

    private void Awake()
    {
        main = this;
    }
    private void Start()
    {
        InitPowerups();
    }
    void InitPowerups()
    {
        for (int i = 0; i < totalActivePowerups; i++)
        {
            SpawnPowerup();
        }
    }
    public void ReplenishPowerup()
    {
        if (powerups.Count <= totalActivePowerups)
            SpawnPowerup();
    }
    void SpawnPowerup()
    {
        List<Transform> availableSpawnPoints = new List<Transform>();

        foreach (var spawnPoint in SpawnPoints)
        {
            availableSpawnPoints.Add(spawnPoint.transform);
        }
        availableSpawnPoints.RemoveAll((Transform selectedSpawnPoint) =>
        {
            foreach (var existingPowerup in powerups)
            {
                float distance = (existingPowerup.transform.position- selectedSpawnPoint.position).sqrMagnitude;
                if  (distance < 5)return true;
            }
            return false;
        });


        if (availableSpawnPoints.Count > 0)
        {
            int randomIndex = Random.Range(0, availableSpawnPoints.Count);
            var selectedSpawnPoint = availableSpawnPoints[randomIndex];

            var spawnedPup = puppool.PoolItem(powerupPrefab);
            spawnedPup.transform.position = selectedSpawnPoint.position;
            if (spawnedPup.TryGetComponent(out Powerup pup))
            {
                randomIndex = Random.Range(0, bonusesSpawned.Length);
                pup.FromPowerupDta(bonusesSpawned[randomIndex]);
                powerups.Add(pup);
            }
        }
    }
}
