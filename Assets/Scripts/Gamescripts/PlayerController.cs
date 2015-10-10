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
    private float lastPositionX;

    public float knockbackAmplifier;
    public float knockbackLength;
    private float knockbackTime;

    public float jumpForce;

    private Rigidbody2D myRigidbody;
    private Collider2D myCollider;
    private Animator myAnimator;
    private AudioSource myAudioSource;

    private bool grounded;
    private bool isBasicAttacking;
    private bool isKnockedBack;

    private bool playerDied;

    public LayerMask groundLayer;
    public GameObject basicAttack;

    public AudioClip soundJump;
    public AudioClip soundAttack;
    public AudioClip soundCharge;
    public AudioClip soundWarp;

    /* TimeBend vars */
    private bool cameraFollow;
    private Vector3 timeBendPosition;
    private float timebendCharge;

	// Use this for initialization
	void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        currentSpeed = baseSpeed;
        totalHealth = currentHealth = baseHealth * gameManager.getHealthMultiplier();
        myRigidbody = GetComponent<Rigidbody2D>();
        myCollider = GetComponent<Collider2D>();
        myAnimator = GetComponent<Animator>();
        myAudioSource = GetComponent<AudioSource>();

        timebendCharge = 0;

        attackSpeedMultiplier = 1;

        cameraFollow = true;
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
                if(transform.position.x > lastPositionX) {
                    gameManager.addScore(0.1f);
                }
                ChargeTimebend(0.03f * gameManager.RoundedCombo() * 2);
            } else if (isKnockedBack && grounded && knockbackTime <= 0) {      
                isKnockedBack = false;
            }
            
            /* check if animation for attack has finished */
            if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Player_BasicAttack") && myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
                isBasicAttacking = false;
                attackSpeedMultiplier = 1;
            }
    
            lastPositionX = transform.position.x;

            /* set animator values */
            myAnimator.SetFloat("Speed", myRigidbody.velocity.x);
            myAnimator.SetBool("BasicAttacking", isBasicAttacking);
            myAnimator.SetBool("Grounded", grounded);
            myAnimator.SetBool("Knockback", isKnockedBack);
        }
       
	}

    void OnCollisionEnter2D(Collision2D other) {

        if (other.gameObject.tag == "Enemy" && !isBasicAttacking && cameraFollow && gameObject.layer == LayerMask.NameToLayer("Player")) {
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
        if(!isKnockedBack && timebendCharge == 100) {
            gameManager.GetTBController().EnableTimebendMode();
            myAnimator.SetBool("TimebendActive", true);
            myAudioSource.PlayOneShot(soundCharge);
        }
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
            myAudioSource.PlayOneShot(soundJump);
            gameManager.addJump();
        }

    }

    public void Attack() {

        /* check if attack is possible, then attack */
        if (!isBasicAttacking && !playerDied) {
            isBasicAttacking = true;

            basicAttack.SetActive(true);
            basicAttack.GetComponent<Animator>().SetTrigger("PlayAttack");

            attackSpeedMultiplier = 1.1f;

            myAudioSource.PlayOneShot(soundAttack);
            
            /* update the attack scale */
            basicAttack.transform.localScale = new Vector3(2f, 2f, 1f);

        }

    }

    public bool GetCameraFollow() {
        return cameraFollow;
    }

    public float GetTimebendCharge() {
        return timebendCharge;
    }

    public bool IsHurt() {
        return isKnockedBack;
    }

    /* CUSTOM PRIVATE FUNCTIONS */

    private void knockPlayerBack() {
        isKnockedBack = true;
        knockbackTime = knockbackLength;
        myRigidbody.velocity = new Vector2(-3 * knockbackAmplifier * 0.75f, 5 * knockbackAmplifier / 2);
        StartCoroutine(Invincible(1));
           
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
        cameraFollow = false;

        timeBendPosition = transform.position;

        if (targets.Count > 0) {
            myAnimator.SetTrigger("TimebendWarpStart");
            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorStateInfo(0).length * Time.timeScale);
        }

        for (int i = 0; i < targets.Count; i++) {
            EnemyController targetController = targets[i].GetComponentInParent<EnemyController>();

            if(targetController.IsAerial()) {
                this.transform.position = new Vector3(targets[i].transform.position.x - 1, targets[i].transform.position.y, transform.position.z - 1);
            } else {
                this.transform.position = new Vector3(targets[i].transform.position.x - 1, targets[i].transform.position.y + 0.5f, transform.position.z - 1);
            }

            myAnimator.SetTrigger("TB_Attack");
            myAudioSource.PlayOneShot(soundWarp);

            yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(myAnimator.GetCurrentAnimatorStateInfo(0).length * Time.timeScale);

        }

        for (int i = 0; i < targets.Count; i++) {
            EnemyController targetController = targets[i].GetComponentInParent<EnemyController>();
            targetController.HitByTimebend();
        }

        cameraFollow = true;
        transform.position = timeBendPosition;

        timebendCharge = 0;

        gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");
        StartCoroutine(Invincible(1));

        gameManager.GetTBController().DisableTimebendMode();
        myAnimator.SetBool("TimebendActive", false);;
        myAnimator.SetTrigger("StopTimebend");
    }

    private IEnumerator Invincible(float seconds) {

        gameObject.layer = LayerMask.NameToLayer("PlayerInvincible");

        float elapsedTime = 0f;
        bool increase = false;
        Color currentColor = GetComponent<SpriteRenderer>().color;

        while (elapsedTime < seconds || currentColor.a != 1) {

            if(currentColor.a >= 1) {
                increase = false;
            } else if (currentColor.a <= 0.3f) {
                increase = true;
            }

            if (increase) {
                currentColor.a += 0.05f;
            } else {
                currentColor.a -= 0.05f;
            }

            GetComponent<SpriteRenderer>().color = currentColor;

            yield return new WaitForEndOfFrame();
            elapsedTime += Time.deltaTime;

        }

        gameObject.layer = LayerMask.NameToLayer("Player");

    }

    private void ChargeTimebend(float amount) {

        timebendCharge += amount;

        if (timebendCharge > 100) {
            timebendCharge = 100;
        }




    }
    
}
