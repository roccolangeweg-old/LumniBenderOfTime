using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public float defaultSpeed;
    private float currentSpeed;

    public float jumpForce;

    private Rigidbody2D myRigidbody;
    private Collider2D myCollider;
    private Animator myAnimator;

    private bool grounded;
    private bool isBasicAttacking;
    public LayerMask groundLayer;

    public GameObject basicAttack;

   

	// Use this for initialization
	void Start() {
        currentSpeed = defaultSpeed;
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update() {
        grounded = Physics2D.IsTouchingLayers(myCollider, groundLayer);

        myRigidbody.velocity = new Vector2(currentSpeed, myRigidbody.velocity.y);
       
        if ((Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).position.x < Screen.width/2)) && grounded) {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
        }

        if (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
            isBasicAttacking = false;
            this.currentSpeed = this.defaultSpeed;

        }

        if ((Input.GetMouseButtonDown(1) || (Input.touchCount > 0 && Input.GetTouch(0).position.x > Screen.width/2)) && !isBasicAttacking) {

            isBasicAttacking = true;
            GameObject loadedBasicAttack = (GameObject) Instantiate(basicAttack, new Vector3(this.transform.position.x + 1.8f, this.transform.position.y, this.transform.position.z + 1f), Quaternion.Euler(new Vector3(0,0,35)));
            this.currentSpeed = this.currentSpeed * 1.5f;

            loadedBasicAttack.transform.localScale = new Vector3(2f,2f,1f);

        } 

        myAnimator.SetFloat("Speed", myRigidbody.velocity.x);
        myAnimator.SetBool("BasicAttacking", isBasicAttacking);
        myAnimator.SetBool("Grounded", grounded);
	}

  
}
