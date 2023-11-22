using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour {

    public static int TOTAL_ENEMIES_KILLED;
    public static int ENEMIES_KILLED;
    public TMP_Text questText;
    public static void IncrementKillCount() {
        ENEMIES_KILLED++; // adds 1 to the ENEMIES_KILLED variable
    }

    public static void ResetQuest() {
        ENEMIES_KILLED = 0; // sets ENEMIES_KILLED to zero
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy"); // Gets all enemies in the game.
        for (int i = 0; i < enemies.Length; i++) { // for each enemy
            if (enemies[i].GetComponent<EnemyHandler>().isQuestCounter) { // If the enemy should be considered for the quest counter
                TOTAL_ENEMIES_KILLED++; // increment the amount of total enemies that need to be killed
            }
        }
    }

    public static bool IsQuestComplete() {
        return TOTAL_ENEMIES_KILLED == ENEMIES_KILLED && TOTAL_ENEMIES_KILLED != 0; // if the total amount of enemies that needs to be killed is equal to the amount of enemies that have been killed, return true.
    }

    private void Update() {
        questText.text = TOTAL_ENEMIES_KILLED != 0 ? "Clocks Killed: " + ENEMIES_KILLED + "/" + TOTAL_ENEMIES_KILLED : ""; // update the quest text to make sure it is accurate to the values.
    }
}
