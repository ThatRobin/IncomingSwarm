using UnityEngine;

[CreateAssetMenu(fileName = "melee", menuName = "behaviour/melee")]
public class MeleeBehaviour : BasicBehaviour {

    // Speicifies the attack speed, damage, and time between attacks
    public float attackSpeed;
    public float attackDamage;
    public float cooldownTime;

    public override void OnUpdate(GameObject self, Animator animator) {
        if (cooldownTime <= 0) { // if the cooldown is less than or equal to zero
            Ray ray = new Ray(self.transform.position + new Vector3(0, 1, 0), self.transform.forward); // create a ray facing forward from the enemy
            RaycastHit raycastHit;

            Physics.Raycast(ray, out raycastHit, 2); // raycast using the ray with a distance of 2
            if (raycastHit.transform != null) { // if the raycast hit something
                if (raycastHit.transform.CompareTag("Player")) { // if that hit is a Player
                    if (animator != null) { // if this enemies animator is not null
                        animator.SetBool("StartSwing", true); // start its attacking animation
                    }
                    raycastHit.collider.SendMessageUpwards("Hit", attackDamage, SendMessageOptions.DontRequireReceiver); // send the player a message saying they got hit.
                }
            }
            cooldownTime = attackSpeed; // set the cooldown equal to the attack speed.
        } else {
            cooldownTime -= Time.deltaTime;  // decrement the cooldown time
            if (animator != null) { // if the enemies animator is not null
                animator.SetBool("StartSwing", false); // make sure they arent attempting to swing.
            }
        }
    }

}
