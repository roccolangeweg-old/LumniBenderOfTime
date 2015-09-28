using UnityEngine;
using System.Collections;

using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

    private GameManager gameManager;

    public float baseHealth;
    private float currentHealth;
    private float totalHealth;

    public float baseSpeed;
    private float currentSpeed;
    private float attackSpeedMultiplier;

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


    /* TimeBend vars */
    private int defaultTBTargets;
    private int maxTBTargets;
    private int currentTBTargets;

	// Use this for initialization
	void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        currentSpeed = baseSpeed;
        totalHealth = currentHealth = baseHealth * gameManager.getHealthMultiplier();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myAnimator = GetComponent<Animator>();

        attackSpeedMultiplier = 1;

        playerDied = false;
	}

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Jump();
        }

        if (Input.GetMouseButtonDown(1)) {
            Attack();
        }
    }
	
	// Update is called once per frame
	void FixedUpdate() { 

        grounded = Physics2D.IsTouchingLayers(myCollider, groundLayer);
        knockbackTime -= Time.deltaTime;

        /* check if player is still alive */
        if (currentHealth <= 0) {

            if(!playerDied) {
                StartCoroutine(StartPlayerDeathAnimation());
            }

        } else {
            
            /* check if player is allowed to move */
            if (!isKnockedBack) {
                myRigidbody.velocity = new Vector2(currentSpeed * attackSpeedMultiplier * (1 + (0.10f * gameManager.RoundedCombo())), myRigidbody.velocity.y);
                gameManager.addScore(0.1f);
            } else if (isKnockedBack && grounded && knockbackTime <= 0) {      
                isKnockedBack = false;
                gameObject.layer = LayerMask.NameToLayer("Player");
            }
            
            /* check if animation for attack has finished */
            if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_BasicAttack") && myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
                isBasicAttacking = false;
                attackSpeedMultiplier = 1;
            }
    
            /* set animator values */
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
            gameManager.ResetCombo();
        }
        
    }

    /* CUSTOM PUBLIC FUNCTIONS */
    public float getTotalHealth() {
        return totalHealth;
    }

    public float getCurrentHealth() {
        return currentHealth;
    }

    public void Timebend() {
        gameManager.GetTBController().EnableTimebendMode();
    }

    public void TimebendAttack(List<GameObject> targets) {

        StartCoroutine(AttackTargets(targets));

    }

    public void UpdateCurrentSpeed(float value) {
        currentSpeed = currentSpeed * value;
    }

    public void Jump() {

        /* check if player is on the ground, then jump */
        if (grounded && !playerDied) {
            myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, jumpForce);
            gameManager.addJump();
        }

    }

    public void Attack() {

        /* check if attack is possible, then attack */
        if (!isBasicAttacking && !playerDied) {
            isBasicAttacking = true;
            GameObject loadedBasicAttack = (GameObject) Instantiate(basicAttack, new Vector3(transform.position.x + 0.5f, transform.position.y + 0.25f, transform.position.z + 1f), Quaternion.Euler(new Vector3(0, 0, 0)));
            attackSpeedMultiplier = 1.5f;
            
            /* update the attack scale */
            loadedBasicAttack.transform.parent = this.gameObject.transform;
            loadedBasicAttack.transform.localScale = new Vector3(2f, 2f, 1f);

        }

    }

    /* CUSTOM PRIVATE FUNCTIONS */

    private void knockPlayerBack() {
        isKnockedBack = true;
        knockbackTime = knockbackLength;
        myRigidbody.velocity = new Vector2(-3 * knockbackAmplifier * 0.75f, 5 * knockbackAmplifier / 2);
        gameObject.layer = LayerMask.NameToLayer("HurtPlayer");
           
    }

    private IEnumerator StartPlayerDeathAnimation() {
        playerDied = true;
        myAnimator.SetTrigger("Death");
        gameObject.layer = LayerMask.NameToLayer("DeathPlayer");
        myRigidbody.velocity = new Vector2(0,0);
        Time.timeScale = 0.5f;

        yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorClipInfo(0).Length);

        gameManager.playerDied();
    }

    private IEnumerator AttackTargets(List<GameObject> targets) {

        for (int i = 0; i < targets.Count; i++) {
            this.transform.position = new Vector3(targets[i].transform.position.x - 1, targets[i].transform.position.y, transform.position.z);
            yield return new WaitForSeconds(1 * Time.timeScale);
        }

        gameManager.GetTBController().DisableTimebendMode();
    }
    
}
