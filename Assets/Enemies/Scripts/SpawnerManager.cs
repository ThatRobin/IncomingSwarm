using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerManager : MonoBehaviour {

    public static bool shouldSpawn = true;
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    public int waveNumber = 0;
    private System.Random rnd = new();

    void Start() {
        StartCoroutine(SpawnMonsters());
        spawners.AddRange(this.GetComponentsInChildren<EnemySpawner>());
    }

    IEnumerator SpawnMonsters() {
        yield return new WaitForSeconds(1f);
        while (shouldSpawn) {
            waveNumber++;
            Debug.Log("Wave: " + waveNumber);
            foreach (EnemySpawner spawner in spawners.OrderBy(x => rnd.Next()).Take(4)) {
                spawner.spawnBoss = waveNumber % 3 == 0;
                spawner.SpawnEnemies();
            }
            yield return new WaitForSeconds(20f);
        }
    }
}
