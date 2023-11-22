using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageInfo {

    public float damage;
    public float knockbackStrength;
    public Vector2 knockbackDirection;
    
    public DamageInfo(float damage, float knockbackStrength, Vector2 knockbackDirection) {
        this.damage = damage;
        this.knockbackStrength = knockbackStrength;
        this.knockbackDirection = knockbackDirection;
    }
}
