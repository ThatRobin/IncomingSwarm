using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomBehaviour : MonoBehaviour {

    public GameObject[] walls;

    public void UpdateRoom(bool[] status) {
        for(int i = 0; i < status.Length; i++) { // for each wall in the room's status
            walls[i].SetActive(!status[i]); // set it active if it should be.
        }
    }
}
