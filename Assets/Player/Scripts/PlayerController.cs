using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    CharacterController characterController;
    public float rotationSpeed = 1f;
    public float speed = 3f;
    public float gravity = 20f;

    public float attackDuration;
    public float attackSpeed;
    public float cooldownTime;
    public float swingTime;
    public bool finishedAttacking;
    public bool isOnCooldown;

    public float swapSpeed;
    public float swapCooldown;

    public InputActionAsset asset;
    public PlayerInput playerInput;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction attackAction;
    private InputAction swapWeaponAction;
    private InputAction pauseAction;
    private Animator animator;
    private WeaponHandler weaponHandler;
    public GameObject weapon;

    AudioSource attackAudio;

    void Start() {
        attackAudio = GetComponent<AudioSource>(); // cache the audio data
        animator = GetComponentInChildren<Animator>(); // Cache the animator
        characterController = GetComponent<CharacterController>(); // Cache the character controller
        playerInput = GetComponent<PlayerInput>(); // Cache the player input
        moveAction = asset.FindAction("Move"); // Cache the movement action from the controls
        lookAction = asset.FindAction("Look"); // Cache the look action from the controls
        swapWeaponAction = asset.FindAction("SwapWeapon"); // Cache the swap weapon action from the controls
        pauseAction = asset.FindAction("Pause"); // Cache the swap weapon action from the controls
        attackAction = asset.FindAction("Fire"); // cache the attack action from the controls
        weaponHandler = weapon.GetComponent<WeaponHandler>(); // cache the weapon handler
        weaponHandler.activeWeapon = weaponHandler.primaryWeapon; // set the active weapon to the primary weapon
    }

    void FixedUpdate() {
        if(pauseAction.ReadValue<float>() > 0) {
            GameObject.Find("GameManager").GetComponent<GameManager>().PauseGame();
        }
        if (!GetComponent<PlayerHandler>().deathScreen.activeSelf || QuestManager.IsQuestComplete()) { // if the death screen is not active, or the quest is complete
            if (moveAction.ReadValue<Vector2>().Equals(Vector2.zero) && lookAction.ReadValue<Vector2>().Equals(Vector2.zero) && attackAction.ReadValue<float>() == 0) { // if the user has not entered an input
                animator.SetFloat("IdleTime", animator.GetFloat("IdleTime") + Time.deltaTime); // increment the idle time
                animator.SetBool("Idling", true); // set idling to true.
            } else {
                animator.SetBool("Idling", false); // set idling to false
                animator.SetFloat("IdleTime", 0f); // reset the idle time to 0
            }
            if (swapCooldown <= 0) { // if the player is able to swap
                if (swapWeaponAction.ReadValue<float>() > 0) { // if they have pressed the swap key
                    weaponHandler.activeWeapon = weaponHandler.activeWeapon == weaponHandler.primaryWeapon ? weaponHandler.secondaryWeapon : weaponHandler.primaryWeapon; // swaps the weapons
                    swapCooldown = swapSpeed;
                }
            } else {
                swapCooldown -= Time.deltaTime; // decrease the cooldown timer
            }
            
            Move();
            Attack();
        }
    }

    void Move() {
        Vector3 movementVector = new Vector3(moveAction.ReadValue<Vector2>().x, 0, moveAction.ReadValue<Vector2>().y); // gets the movement input as a Vector3
        if(movementVector.x != 0 || movementVector.z != 0) { // if either are not zero
            animator.SetBool("Idling", false); // set idling to false
            if (finishedAttacking) { // if they are finished attacking
                animator.SetBool("Walking", true); // set walking to true
                if(!attackAudio.isPlaying) { // if the step sound is not playing
                    attackAudio.Play(0); // play the step sound
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(movementVector), 0.05f); // set their rotation to face where they are looking
            } else {
                animator.SetBool("Walking", false); // set walking to false
                movementVector.x = 0; // set the x and y movements to zero.
                movementVector.z = 0;
            }
        } else {
            animator.SetBool("Walking", false); // set walking to false.
        }
        movementVector *= speed; //  multiply the movement vector by the players speed
        movementVector.y -= gravity; // remove gravity from the y value of the movement vector
        characterController.Move(movementVector * Time.deltaTime); // move to the movement positon
    }

    void Attack() {
        attackDuration = weaponHandler.GetAttackDuration(); // get the weapons attack duration
        attackSpeed = weaponHandler.GetAttackSpeed(); // get the weapons attack speed
        if (attackAction.ReadValue<float>() > 0) { // if the user attempts to attack
            Vector3 hitpos = Vector3.zero; // set the hitpos to the world origin.
            animator.SetBool("Idling", false); // set idling to false
            if (playerInput.currentControlScheme.Equals("Keyboard&Mouse")) { // if the user is using a keyboard and mouse
                Ray ray = Camera.main.ScreenPointToRay(lookAction.ReadValue<Vector2>()); // get the Camera to ray value of the input
                if (Physics.Raycast(ray, out RaycastHit hit, 50f)) { // cast the ray
                    if (!isOnCooldown) { // if the player is not on cooldown
                        hitpos = new Vector3(hit.point.x, transform.position.y, hit.point.z); // create a Vector3 for the player to face
                        transform.LookAt(hitpos); // look at that position
                    }
                }
            } else {
                if (!isOnCooldown) { // if the user is not on cooldown
                    Vector2 point = lookAction.ReadValue<Vector2>().normalized; // get the normalized value of the look position
                    hitpos = new Vector3(transform.position.x + point.x, transform.position.y, transform.position.z + point.y); // create a Vector3 for the player to face
                    transform.LookAt(hitpos); // look at that positon.
                }
            }
            if (!isOnCooldown) { // if the user is not on cooldown
                int combo = animator.GetInteger("Combo") + 1; // increment the combo counter
                combo = combo == weaponHandler.activeWeapon.comboCount ? 0 : combo; // reset the combo counter if it reaches that weapons combo count limit
                isOnCooldown = true; // set the cooldown
                finishedAttacking = false; // set the attack to unfinished
                switch (weaponHandler.activeWeapon.weaponType) { // check the weapon type
                    case Weapon.WeaponType.MELEE: // if it is a melee weapon
                        animator.SetBool("StartSwing", true); // start the swing animation
                        animator.SetInteger("Combo", combo); // set the combo up
                        weapon.GetComponent<BoxCollider>().enabled = true; // enable the collider for damage
                        weaponHandler.PlayClip();
                        cooldownTime = attackSpeed; // set the cooldown to the attack speed
                        swingTime = attackDuration; // set the swing time to the attack duration
                        break;
                    case Weapon.WeaponType.RANGED: // if it is a ranged weapon
                        animator.SetBool("StartShoot", true); // start the shoot animation
                        animator.SetInteger("Combo", combo); // set the combo up
                        weaponHandler.RangedAttack(hitpos); // do a ranged attack
                        weaponHandler.PlayClip();
                        cooldownTime = attackSpeed; // set the cooldown to the atack speed
                        swingTime = attackDuration; // set the swing time to the attack duration
                        break;
                    default:
                        break;
                }
            }
        } else {
            switch (weaponHandler.activeWeapon.weaponType) { // check the weapon type
                case Weapon.WeaponType.MELEE: // if it is a melee weapon
                    animator.SetBool("StartSwing", false); // stop swinging
                    break;
                case Weapon.WeaponType.RANGED:
                    break;
                default:
                    break;
            }           
        }
        if(finishedAttacking) { // if the player is finished attacking
            if (cooldownTime <= 0) { // if the cooldown time is less than or equal to zero
                isOnCooldown = false; // specify that it is no longer on cooldown
                switch (weaponHandler.activeWeapon.weaponType) { // check the weapon type
                    case Weapon.WeaponType.MELEE: // if it is a melee weapon
                        weapon.GetComponent<BoxCollider>().enabled = false; // uncheck the box collider to disable damage
                        break;
                    case Weapon.WeaponType.RANGED: // if it is a ranged weapon
                        animator.SetBool("StartShoot", false); // stop the shooting animation
                        break;
                    default:
                        break;
                }
            } else {
                cooldownTime -= Time.deltaTime; // decrease the cooldown time
            }
        } else {
            if (swingTime <= 0) {
                finishedAttacking = true; // set it so the player has finished attacking
            } else {
                swingTime -= Time.deltaTime; // decrease the swing time.
            }
        }
    }
    
    private void OnControllerColliderHit(ControllerColliderHit hit) {
        if (characterController.isGrounded) { // if the character is on the ground
            if(hit.point.y > transform.position.y) { // if the collision point is higher than the origin position of the player
                if (hit.gameObject.CompareTag("World")) { // if the collided object is the world
                    characterController.stepOffset = 1; // set the step offset to 1
                } else {
                    characterController.stepOffset = 0.3f; // set the step offset to 0.3
                }
            }
        }
    }
    
}
