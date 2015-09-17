using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private GameManager gameManager;

    public float baseHealth;
    private float currentHealth;
    private float totalHealth;

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

    private bool playerDied;

    public LayerMask groundLayer;
    public GameObject basicAttack;

    private bool timebendActive;
    private bool timebendReady;
    private int timebendCharge;

	// Use this for initialization
	void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        currentSpeed = baseSpeed;
        totalHealth = currentHealth = baseHealth * gameManager.getHealthMultiplier();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myAnimator = GetComponent<Animator>();

        playerDied = false;

        InvokeRepeating("ChargeTimebend", 0f, 0.25f);
	}
	
	// Update is called once per frame
	void Update() { 
        grounded = Physics2D.IsTouchingLayers(myCollider, groundLayer);
        knockbackTime -= Time.deltaTime;

        /* check if player is still alive */
        if (currentHealth <= 0) {
            if(!playerDied) {
                StartCoroutine(startPlayerDeathAnimation());
            }
        } else {
            
            /* check if player is allowed to move */
            if (!isKnockedBack) {
                myRigidbody.velocity = new Vector2(currentSpeed, myRigidbody.velocity.y);
            } else if (isKnockedBack && grounded && knockbackTime <= 0) {      
                isKnockedBack = false;
            }
            
            /* jump when input it detected, and player is currently touching the ground */
            if ((Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).position.x < Screen.width/2)) && grounded) {
                gameManager.getAnalytics().LogEvent("Statistics","Gameplay","Jump",1);
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
            }
            
            /* check if animation for attack has finished */
            if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_BasicAttack") && myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
                isBasicAttacking = false;
                currentSpeed = this.baseSpeed;
                
            /* else check if attack input is pressed and isBasicAttacking is false */
            } else if ((Input.GetMouseButtonDown(1) || (Input.touchCount > 0 && Input.GetTouch(0).position.x > Screen.width/2)) && !isBasicAttacking) {

                gameManager.getAnalytics().LogEvent("Statistics","Gameplay","Attack",1);

                isBasicAttacking = true;
                GameObject loadedBasicAttack = (GameObject) Instantiate(basicAttack, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.5f, transform.position.z + 1f), Quaternion.Euler(new Vector3(0,0,0)));
                currentSpeed = currentSpeed * 1.5f;
                
                /* update the attack scale */
                loadedBasicAttack.transform.parent = this.gameObject.transform;
                loadedBasicAttack.transform.localScale = new Vector3(2f,2f,1f);
            } 

            myAnimator.SetFloat("Speed", myRigidbody.velocity.x);
            myAnimator.SetBool("BasicAttacking", isBasicAttacking);
            myAnimator.SetBool("Grounded", grounded);
            myAnimator.SetBool("Knockback", isKnockedBack);
        }
       
	}

    void OnCollisionEnter2D(Collision2D other) {

        if (other.gameObject.tag == "Enemy" && !isBasicAttacking) {
            currentHealth-=0.5f;
            knockPlayerBack();
        }
        
    }

    /* CUSTOM PUBLIC FUNCTIONS */
    public float getTotalHealth() {
        return totalHealth;
    }

    public float getCurrentHealth() {
        return currentHealth;
    }

    public void ActivateTimebend() {

        if (timebendReady && !timebendActive && !playerDied) {
            timebendActive = true;
            timebendReady = false;

            StartCoroutine(gameManager.updateTimescale(Time.timeScale, 0.5f));
        }
    }

    /* CUSTOM PRIVATE FUNCTIONS */
    private void ChargeTimebend() {
        if (!timebendReady && !timebendActive) {
            timebendCharge++;
            FindObjectOfType<TimebendImageScript>().GetComponent<TimebendImageScript>().setPercentage(timebendCharge);
            
            if (timebendCharge >= 100) {
                timebendReady = true;
            }
        } else if (timebendActive) {
            
            timebendCharge-=5;
            FindObjectOfType<TimebendImageScript>().GetComponent<TimebendImageScript>().setPercentage(timebendCharge);
            
            if (timebendCharge < 0) {
                timebendActive = false;
                StartCoroutine(gameManager.updateTimescale(Time.timeScale, 1));
                timebendCharge = 0;
            }  
        }
    }

    private void knockPlayerBack() {
        isKnockedBack = true;
        knockbackTime = knockbackLength;
        myRigidbody.velocity = new Vector2(-3 * knockbackAmplifier * 0.75f, 5 * knockbackAmplifier / 2);
    }

    private IEnumerator startPlayerDeathAnimation() {
        playerDied = true;
        myAnimator.SetTrigger("Death");
        gameObject.layer = LayerMask.NameToLayer("DeathPlayer");
        myRigidbody.velocity = new Vector2(0,0);
        Time.timeScale = 0.5f;

        yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorClipInfo(0).Length);

        gameManager.playerDied();
    }
    
}
