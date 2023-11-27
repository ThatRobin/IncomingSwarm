using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHandler : MonoBehaviour {

    public float maxHealth = 20;
    public float health = 20;
    public Slider healthBar;
    public float hurtTime = 2;
    public float hurtTimeMax = 2;
    public bool hurt;

    public GameObject hud;
    public GameObject deathScreen;
    public GameObject winScreen;
    public GameObject pauseScreen;

    private Material matInst;

    private void Awake() {
        ResetUI(); // reset the screens
        healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponentInChildren<Slider>(); // cache the health bar slider
    }

    public void SetScreens(GameObject hud, GameObject deathScreen, GameObject winScreen, GameObject pauseScreen) { // set the screens to the provided variables
        this.hud = hud;
        this.deathScreen = deathScreen;
        this.winScreen = winScreen;
        this.pauseScreen = pauseScreen;
    }

    public void SetMaterial(Material mat) {
        matInst = mat; // set the material instance to the provided material
    }

    private void Update() {
        healthBar.maxValue = maxHealth; // set the health bar max value to the max health
        healthBar.value = health; // set the health bar value to the current health

        if (hurt) { // if the player is currently being hurt
            if (hurtTime <= 0) { // if hurt time is less than zero
                hurt = false; // set the player to not being hurt
                hurtTime = hurtTimeMax; // set the hurt time to the max hurt time
            } else {
                if (this.matInst != null) {
                    this.matInst.color = Color.Lerp(Color.white, Color.red, hurtTime / hurtTimeMax); // lerp between the models base colour and red when hurt, so it fades away
                }
                hurtTime -= Time.deltaTime; // decrease the hurt time
            }
        }
    }

    void Hit(float rawDamage) {
        health -= rawDamage; // decrease the players health
        hurt = true; // set the player to be hurt
        hurtTime = hurtTimeMax; // set the hurt time to max
        if (health <= 0) { // if the health is less than 0
            Invoke("SelfTerminate", 0f); // kill the player
        }
    }

    public void ResetUI() {
        hud.SetActive(true); // set the hud to be active
        deathScreen.SetActive(false); // set the death screen to not be active
        winScreen.SetActive(false);  // set the win screen to not be active
        health = maxHealth; // set the health to the max health
    }

    void SelfTerminate() {
        if (!winScreen.activeSelf) { // if the win screen is not active
            hud.SetActive(false); // set the hud the be not active
            deathScreen.SetActive(true); // set the death screen to be active.
        }
    }
}
