using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour {

    public int spawnAmountMin;
    public int spawnAmountMax;
    public float spawnX;
    public float spawnZ;
    public GameObject[] enemyTypes;
    public GameObject[] bossTypes;
    private bool spawnBoss;

    private void Start() {
        SpawnEnemies();
    }

    public void SpawnEnemies() {
        int spawnAmount = Random.Range(spawnAmountMin, spawnAmountMax); // generates an amount of enemies to spawn
        for (int i = 0; i < spawnAmount; i++) { // itterate over the following for each enemy that should be spawnedf
            int index = Random.Range(0, enemyTypes.Length); // get a random enemy type from the array NOTE: ONLY ONE TYPE OF ENEMY IMPLEMENTED THUS FAR.
            float spawnDistanceX = Random.Range(-spawnX, spawnX); // create a random spawn offset on the X and Z axis
            float spawnDistanceZ = Random.Range(-spawnZ, spawnZ);
            var enemy = Instantiate(enemyTypes[index], transform.position + new Vector3(spawnDistanceX, 0, spawnDistanceZ), Quaternion.Euler(0f, 180f, 0f), transform);
            // create enemy at the given position. 
            enemy.name = "Knight"; // name the enemy "Knight"
            enemy.GetComponent<EnemyHandler>().OnStart(); // trigger the enemies OnStart method.
        }
        if(this.spawnBoss) { // if this room should spawn a boss
            int index = Random.Range(0, bossTypes.Length); // for each boss type NOTE: ONLY ONE TYPE OF BOSS IMPLEMENTED THUS FAR.
            float spawnDistanceX = Random.Range(-spawnX, spawnX); // create random spawn offset on the X and Z axis
            float spawnDistanceZ = Random.Range(-spawnZ, spawnZ);
            var enemy = Instantiate(bossTypes[index], transform.position + new Vector3(spawnDistanceX, 0, spawnDistanceZ), Quaternion.Euler(0f, 180f, 0f), transform);
            // create the enemy at the given position
            enemy.name = "Clock"; // name the enemy "Clock"
            enemy.GetComponent<EnemyHandler>().OnStart(); // trigger the enemies OnStart method.
        }
    }

    public void SetBossRoom(bool spawnBoss) {
        this.spawnBoss = spawnBoss; // sets this room to be a boss room
    }
}
