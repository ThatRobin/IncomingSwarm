using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour {

    public WeaponHandler weaponHandler;
    public Weapon weapon;
    public Vector3 forward;
    public Vector3 target;
    public Vector3 hitPos;

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Enemy")) { // if the object is an enemy
            WeaponHandler.HandleAttack(this.gameObject, collider.gameObject, weaponHandler); // handle the attack as the weapon
            Destroy(this.gameObject); // destroy the projectile
        }
    }

    void Update() {
        transform.Translate(-transform.forward * weapon.bulletSpeed * Time.deltaTime); // move forward
    }

    public void SetHitPos(Vector3 hitPos) {
        this.hitPos = hitPos;
    }
}
