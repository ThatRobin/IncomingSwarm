using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "weapon", menuName = "weapons/weapon")]
public class Weapon : ScriptableObject {

    public WeaponType weaponType;
    public float attackSpeed;
    public float bulletSpeed;
    public float attackDuration;
    public float damage;
    public float knockback;
    public int comboCount;
    public AudioClip clip;
    public Mesh weaponObject;
    public GameObject projectile;

    public enum WeaponType
    {
        MELEE,
        RANGED,
        MISC
    }
}
