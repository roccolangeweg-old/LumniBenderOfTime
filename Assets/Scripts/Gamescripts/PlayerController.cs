using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float baseHealth;
    private float currentHealth;

    public float baseSpeed;
    private float currentSpeed;

    public float knockbackAmplifier;
    public float knockbackLength;
    private float knockbackTime;

    public float jumpForce;

    private Rigidbody2D myRigidbody;
    private Collider2D myCollider;
    private Animator myAnimator;

    private bool grounded;
    private bool isBasicAttacking;
    private bool isKnockedBack;

    public LayerMask groundLayer;
    public GameObject basicAttack;

   

	// Use this for initialization
	void Start() {
        currentSpeed = baseSpeed;
        currentHealth = baseHealth;
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update() {
        grounded = Physics2D.IsTouchingLayers(myCollider, groundLayer);

        knockbackTime -= Time.deltaTime;

        /* check if player is allowed to move */
        if (!isKnockedBack) {
            myRigidbody.velocity = new Vector2(currentSpeed, myRigidbody.velocity.y);
        } else if (isKnockedBack && grounded && knockbackTime <= 0) {      
            isKnockedBack = false;
        }

        /* jump when input it detected, and player is currently touching the ground */
        if ((Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).position.x < Screen.width/2)) && grounded) {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
        }

        /* check if animation for attack has finished */
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_BasicAttack") && myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
            isBasicAttacking = false;
            currentSpeed = this.baseSpeed;
        
        /* else check if attack input is pressed and isBasicAttacking is false */
        } else if ((Input.GetMouseButtonDown(1) || (Input.touchCount > 0 && Input.GetTouch(0).position.x > Screen.width/2)) && !isBasicAttacking) {

            isBasicAttacking = true;
            GameObject loadedBasicAttack = (GameObject) Instantiate(basicAttack, new Vector3(transform.position.x + 1.9f, transform.position.y, transform.position.z + 1f), Quaternion.Euler(new Vector3(0,0,35)));
            currentSpeed = currentSpeed * 1.5f;

            /* update the attack scale */
            loadedBasicAttack.transform.localScale = new Vector3(1.8f,1.8f,1f);
        } 

        myAnimator.SetFloat("Speed", myRigidbody.velocity.x);
        myAnimator.SetBool("BasicAttacking", isBasicAttacking);
        myAnimator.SetBool("Grounded", grounded);
	}

    void OnCollisionEnter2D(Collision2D other) {

        if (other.gameObject.tag == "Enemy" && !isBasicAttacking) {
            currentHealth-=0.5f;
            isKnockedBack = true;
            knockbackTime = knockbackLength;
            myRigidbody.velocity = new Vector2(-3 * knockbackAmplifier * 0.75f, 5 * knockbackAmplifier / 2);
        }
        
    }
    
}
