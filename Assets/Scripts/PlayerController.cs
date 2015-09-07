using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float moveSpeed;
    public float jumpForce;

    private Rigidbody2D myRigidbody;
    private Collider2D myCollider;
    private Animator myAnimator;

    private bool grounded;
    private bool isBasicAttacking;
    public LayerMask groundLayer;

   

	// Use this for initialization
	void Start() {
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update() {
        grounded = Physics2D.IsTouchingLayers(myCollider, groundLayer);

        myRigidbody.velocity = new Vector2(moveSpeed, myRigidbody.velocity.y);
       
        if (Input.GetKeyDown(KeyCode.Space) && grounded) {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
        }

        if (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
            isBasicAttacking = false;
        }

        if (Input.GetMouseButtonDown(0) && !isBasicAttacking) {
            isBasicAttacking = true;
        } 



        myAnimator.SetFloat("Speed", myRigidbody.velocity.x);
        myAnimator.SetBool("BasicAttacking", isBasicAttacking);
        myAnimator.SetBool("Grounded", grounded);
	}

  
}
