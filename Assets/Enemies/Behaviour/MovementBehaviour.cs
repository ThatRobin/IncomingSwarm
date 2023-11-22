using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "movement", menuName = "behaviour/movement")]
public class MovementBehaviour : BasicBehaviour {

    public float speed;
    public float senseRange;
    private NavMeshAgent navMeshAgent;
    public override void OnStart(GameObject self) {
        navMeshAgent = self.GetComponent<NavMeshAgent>(); // get the navmesh agent
        navMeshAgent.speed = speed; // set the navmesh agent speed to the enemy speed
        if (navMeshAgent == null) { // if the agent is null
            Debug.Log("The NavMeshAgent is not atached to " + self.name); // let the console know the enemy is missing a navmesh.
        }
    }

    public override void OnUpdate(GameObject self, Animator animator) {
        navMeshAgent = self.GetComponent<NavMeshAgent>(); // get the navmesh agent
        bool stunned = self.GetComponent<EnemyHandler>().IsStunned(); // if the enemy is null
        if (animator != null) { // if the animator is not null
            animator.SetBool("Walking", navMeshAgent.velocity.magnitude > 0); // set the Walking attribute to true, if the navmesh agent is moving at a speed greater than zero.
        }
        navMeshAgent.enabled = !stunned; // disable the agent if they are stunned
        if (!stunned) { // if the enemy is not stunned
            if (navMeshAgent != null) { // and the agent is not null
                SetDestination(navMeshAgent, self); // set the destination of the navmesh
            }
        }
    }

    private void SetDestination(NavMeshAgent navMeshAgent, GameObject self) {
        if (GameObject.FindGameObjectWithTag("Player") != null) { // If the player can be found
            Vector3 targetVector = GameObject.FindGameObjectWithTag("Player").transform.position; // cache the position of the player
            if (Vector3.Distance(targetVector, self.transform.position) < senseRange) { // if the distance is within sensing range
                navMeshAgent.SetDestination(targetVector); // let the enemy target the player
            }
            if (Vector3.Distance(targetVector, self.transform.position) < 2) { // if the enemy is within two blocks of the player
                targetVector.y = self.transform.position.y; //set the target heigh to be the same as your own height
                self.transform.LookAt(targetVector); // look at the target.
            }
        }
    }

}
