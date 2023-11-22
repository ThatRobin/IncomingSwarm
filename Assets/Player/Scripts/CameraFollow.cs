using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
    public Transform target;

    public bool isCustomOffset;
    public Vector3 offset;

    public float smoothSpeed = 0.1f;

    public void InitializeCamera(GameObject player) {
        target = player.transform; // sets the player as the target
        if (!isCustomOffset) { // check if there is a custom offset
            offset = transform.position - target.position; // set the offset to the position minus the target position.
        }
    }

    private void FixedUpdate() {
        if(target != null) { // if the target exists
            SmoothFollow(); // follow the target
        }
    }

    public void SmoothFollow() {
        Vector3 targetPos = target.position + offset; // set the target position to the players position plus the offset.
        Vector3 smoothFollow = Vector3.Lerp(transform.position, targetPos, smoothSpeed); // linear interpolate a positon between its current positon, and its target position

        transform.position = smoothFollow; // set it to that position.
    }
}