using UnityEngine;
using UnityEngine.UI;

public class WorldUIHandler : MonoBehaviour {

    private Slider slider;
    private EnemyHandler enemyHandler;
    
    private void Start() {
        enemyHandler = this.transform.GetComponentInParent<EnemyHandler>(); // gets the enemy handler
        slider = this.GetComponentInChildren<Slider>(); // gets the enemies slider
    }

    void Update() {
        slider.maxValue = enemyHandler.maxHealth; // sets the slider max to the max health
        slider.value = enemyHandler.health; // set tge skuder value to the current value
        Vector3 position = new Vector3(transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z);
        // gets a position for the slider to face
        transform.LookAt(position); // makes the slider face towards to camera
    }

}
