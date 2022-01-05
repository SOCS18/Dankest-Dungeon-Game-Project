using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public List<Transform> spawnpoints = new List<Transform>();
    public GameObject[] Enemies;
    public GameObject[] Bosses;
    public int spawnsLeft = 5;
    public int maxSpawns = 2;
    public int currentSpawns = 0;
    public float EnemySpawnTime;
    public float EnemySpawnTimeInterval = 5f;
    public float currentSpawnTime = 0;
    private int offset = 0;
    public bool bossRoom = false;

    // Start is called before the first frame update
    public void Start()
    {
        spawnpoints.Clear();
        foreach(GameObject go in GameObject.FindGameObjectsWithTag("SpawnPoint"))
        {
            spawnpoints.Add(go.transform);
        }

        //Code starts by spawning 2 enemies
        SpawnNewEnemies();
        EnemySpawnTime = EnemySpawnTimeInterval;
    }

    private void Update(){
        //Will spawn a new enemy every 5 seconds. The conditions to do so can be changed.
        if(currentSpawnTime <= 0 && spawnsLeft > 0 && currentSpawns < maxSpawns){
            SpawnNewEnemies();
            currentSpawnTime = EnemySpawnTime;
        }
        else
        {
            currentSpawnTime -= Time.deltaTime;
        }
    }

    public void SpawnNewEnemies(){
        spawnsLeft--;
        currentSpawns++;
        while (EnemyNearSpawn())
        {
            offset++;
            if (offset > spawnpoints.Count - 1)
                offset = 0;
        }

        if (spawnpoints.Count == 0 || Enemies.Length == 0)
            return;

        Vector3 x = new Vector3(1f, 1f, 1f);
        GameObject NME;
        if (!bossRoom)
            NME = Enemies[Random.Range(0, Enemies.Length)];
        else
            NME = Bosses[Random.Range(0, Bosses.Length)];

        x = spawnpoints[offset].position;
        Quaternion r = Quaternion.identity;

        var newenemy = Instantiate(NME, x, r);
        newenemy.GetComponent<Enemy>().spawner = this;

        offset++;
        if (offset > spawnpoints.Count - 1)
            offset = 0;
    }

    // Removes congestion
    private bool EnemyNearSpawn()
    {
        Transform spawn = spawnpoints[offset];
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy == null)
                continue;
            if (Vector3.Distance(spawn.position, enemy.transform.position) <= 1f) return true;
        }
        return false;
    }
}
