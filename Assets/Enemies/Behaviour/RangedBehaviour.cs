using UnityEngine;

[CreateAssetMenu(fileName = "ranged", menuName = "behaviour/ranged")]
public class RangedBehaviour : BasicBehaviour {

    public GameObject projectile;
    public float attackDamage;
    public float attackSpeed;

    public override void OnUpdate(GameObject self, Animator animator) {
        Debug.Log(self.name + " is trying to Shoot!"); // not fully implemented enemy shooting mechanic, remains as proof of concept for expandable behaviour system.
    }
}
