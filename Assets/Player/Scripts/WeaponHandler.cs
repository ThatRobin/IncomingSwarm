using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponHandler : MonoBehaviour {

    public Weapon primaryWeapon;
    public Weapon secondaryWeapon;
    public Weapon activeWeapon;
    public Transform spawnLoc;
    public Transform player;
    private TMP_Text controlText;
    public InputActionAsset asset;
    private InputAction swapWeaponAction;
    private InputAction attackAction;
    public PlayerInput playerInput;
    public AudioSource audioSource;

    public void PlayClip() {
        if (!audioSource.isPlaying) {
            audioSource.clip = activeWeapon.clip; // set the audio source clip to this clip
            audioSource.Play(0);
        }
    }

    public float GetDamage() { 
        return activeWeapon.damage; // returns the active weapons damage
    }

    public float GetAttackSpeed() {
        return activeWeapon.attackSpeed; // returns the active weapons attack speed
    }

    public float GetAttackDuration() {
        return activeWeapon.attackDuration; // return the active weapons duration
    }

    private void Awake() {
        controlText = GameObject.FindGameObjectWithTag("Controls").GetComponentInChildren<TMP_Text>(); // get the control text
        playerInput = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInput>(); // get the player input
        swapWeaponAction = asset.FindAction("SwapWeapon"); // cache the swap weapon action
        attackAction = asset.FindAction("Fire"); // cache the attack action
    }

    private void Update() {
        string text = "";
        if (swapWeaponAction != null && activeWeapon != null) { // if the swap weapon and active weapon both exist
            text += attackAction.GetBindingDisplayString() + ": Attack\n"; // display the binding for attacking
            text += swapWeaponAction.GetBindingDisplayString() + ": Swap Weapon\n"; // display the binding for swapping weapons
            text += "Active weapon: " + activeWeapon.name; // display the active weapon
        }
        controlText.text = text; // set the control text
    }

    void OnTriggerEnter(Collider collider) {
        WeaponHandler.HandleAttack(this.gameObject, collider.gameObject, this); // handle the attack as the weapon
    }

    public void RangedAttack(Vector3 hitPos) {
        GameObject localProjectile = Instantiate(activeWeapon.projectile, spawnLoc.position, player.rotation) as GameObject; // create the projectiles
        localProjectile.GetComponent<ProjectileManager>().weaponHandler = this; // set the projectiles weapon manager to this script
        localProjectile.GetComponent<ProjectileManager>().weapon = this.activeWeapon; // set the projectiles weapon to the currently active weapon
        localProjectile.GetComponent<ProjectileManager>().SetHitPos(hitPos);
        Destroy(localProjectile, 5f); // destroy the projectile after 5 seconds.
    }

    public static void HandleAttack(GameObject self, GameObject other, WeaponHandler weaponHandler) {
        if (other.CompareTag("Enemy")) { // if the hit object is an enemy
            Vector2 direction = (other.transform.position - self.transform.root.position).normalized * weaponHandler.activeWeapon.knockback; // get the direction for knockback
            DamageInfo damage = new DamageInfo(weaponHandler.GetDamage(), 2f, direction); // package the damage info into an object
            other.SendMessageUpwards("Hit", damage, SendMessageOptions.DontRequireReceiver); // set the damage info to the hit method
        }
    }

}
