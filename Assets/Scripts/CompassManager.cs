using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompassManager : MonoBehaviour
{

    void FixedUpdate() {
        List<GameObject> bosses = new List<GameObject>(); // create a list for bosses
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // find the player
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // find all enemies
        for (int i = 0; i < enemies.Length; i++) { // for each enemy
            if (enemies[i].GetComponent<EnemyHandler>().isQuestCounter) { // if the enemy counts towards the quest counter
                bosses.Add(enemies[i]); // add it to the boss list
            }
        }

        bosses.Sort((a, b) => CompareDistanceToMe(a, b, player)); // sort the bosses into a list of which is closest
        if (bosses.Count > 0 && player != null) { // if there is at least one boss in the list
            Vector3 bossPos = (bosses[0].transform.position - player.transform.position); // get the bosses position
            transform.rotation = Quaternion.Euler(0, 0, (bossPos.z/2f) + 90); // set the compasses rotation to the posses position.
        }
    }

    int CompareDistanceToMe(GameObject a, GameObject b, GameObject player) {
        float squaredRangeA = (a.transform.position - player.transform.position).sqrMagnitude; // get the distance to A
        float squaredRangeB = (b.transform.position - player.transform.position).sqrMagnitude; // get this distance to B
        return squaredRangeA.CompareTo(squaredRangeB); // compare the two distances
    }

}
