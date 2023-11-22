using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyHandler : MonoBehaviour {

    public float maxHealth = 20;
    public float health = 20;
    public float stunTime = 2;
    public float stunTimeMax = 2;
    public Boolean stunned;
    public List<BasicBehaviour> behaviours = new List<BasicBehaviour>();
    public Material material;
    private Material matInst;
    public NavMeshAgent navMeshAgent;
    private Animator animator;
    public GameObject deathParticle;
    public bool isQuestCounter = false;

    AudioSource audioData;

    public void OnStart() {
        audioData = GetComponent<AudioSource>(); // cache the audio data
        this.navMeshAgent = this.GetComponent<NavMeshAgent>(); // cache the navmesh agent
        this.animator = this.GetComponent<Animator>(); // cache the animator
        this.matInst = new Material(material); // create a new instance of material based on this enemies default material
        foreach (MeshRenderer meshRenderer in this.GetComponentsInChildren<MeshRenderer>()) { // for each mesh in this objects heirarchy
            if(meshRenderer.gameObject.CompareTag("Reskin")) { // compare the object to a tag called "Reskin" to see if its model needs changing to the instance based one.
                meshRenderer.material = this.matInst; // change the model to the instance based material.
            }
        }
        foreach (BasicBehaviour behaviour in behaviours) { // for each behaviour
            behaviour.OnStart(this.gameObject); // run its OnStart Method
        }
        health = maxHealth; // set the current health to the max health
    }

    private void FixedUpdate() {
        if (!stunned) { // if the enemy is not stunned
            foreach (BasicBehaviour behaviour in behaviours) { // for each behaviour
                behaviour.OnUpdate(this.gameObject, animator); // run its OnUpdate method.
            }
        } else {
            if (stunTime <= 0) { // if the stun timer is less than zero
                this.navMeshAgent.angularSpeed = 360f; // set the angular speed to 360
                stunned = false; // set stunned to false
                stunTime = stunTimeMax; // set stun time to the max stun time
            } else {
                this.navMeshAgent.angularSpeed = 0f; //set the angular speed to 0
                this.matInst.color = Color.Lerp(Color.white, Color.red, stunTime / stunTimeMax); // lerp the colour of the material between white and red, so it fades as the enemy gets closer to being un-stunned
                stunTime -= Time.deltaTime; // decrease the stun time
            }
        }
    }

    public Boolean IsStunned() {
        return stunned; // returns in the enemy is stunned
    }

    void Hit(DamageInfo damageInfo) {
        if(!audioData.isPlaying) {
            audioData.Play(0);
        }
        this.navMeshAgent.velocity = new Vector3(damageInfo.knockbackDirection.x, 0f, damageInfo.knockbackDirection.y); // set the velocity of the navmeshagent using the damage info knockbace
        stunned = true; // set the enemy to be stunned
        stunTime = stunTimeMax; // set the stun time to be max
        health -= damageInfo.damage; // remove the damage dealt
        if (health <= 0) { // if the health is less than or equal to 0
            Invoke("SelfTerminate", 0f); // kill this object
            GameObject smokePuff = Instantiate(deathParticle, transform.position, transform.rotation) as GameObject; // create a death particle
            ParticleSystem parts = smokePuff.GetComponent<ParticleSystem>(); // get the parts of the particle
            float totalDuration = parts.duration + parts.startLifetime; // get the duration of the particle in total
            Destroy(smokePuff, totalDuration); // destroy the particle after its duration is complete.
        }
    }

    void SelfTerminate() {
        if (isQuestCounter) {  // if this death should count towards the quest counter
            QuestManager.IncrementKillCount(); // increment the counter
        }
        Destroy(gameObject); // destroy this object.
    }
}
