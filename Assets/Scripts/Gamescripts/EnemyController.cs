using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    private GameManager gameManager;
    private PlayerController player;

    private GameObject destructionPoint;

    public int orbsRewarded;
    public int baseHealth;
    public bool isAerialType;
    public float moveSpeed;

    private bool isAlive;
    private bool isAttacking;

    public float flyingSwing;

    private int currentHealth;
    private bool isKnockedBack;

    private float knockbackTime;
    public float knockbackLength;
    public float knockbackAmplifier;

    private Rigidbody2D myRigidbody;
    private Animator myAnimator;
    private Collider2D myCollider;

    public PhysicsMaterial2D deathMaterial;
    public GameObject explosion;

	// Use this for initialization
	void Start () {
        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        currentHealth = baseHealth;

        destructionPoint = GameObject.Find("DestructionPoint");

        player = FindObjectOfType<PlayerController>();

        isAlive = true;
        isAttacking = false;

        myRigidbody = this.GetComponent<Rigidbody2D>();
        myAnimator = this.GetComponentInChildren<Animator>();
        myCollider = this.GetComponentInChildren<Collider2D>();

        if (isAerialType) {
            myRigidbody.isKinematic = true;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        if (transform.position.x < destructionPoint.transform.position.x) {
            Destroy(gameObject);
        }

        if (isAlive) {

            /* check if knockback is done */
            knockbackTime -= Time.deltaTime;

            if(isAerialType) {
                if(player.transform.position.x + 5 >= transform.position.x && transform.position.x > player.transform.position.x && !isAttacking) {
                    isAttacking = true;
                }
            } else {
                if(player.transform.position.x + 3 >= transform.position.x && transform.position.x > player.transform.position.x && !isAttacking) {
                    isAttacking = true;
                }
            }

            
            if (!isKnockedBack && !isAttacking) {
                myRigidbody.velocity = new Vector2(-moveSpeed, myRigidbody.velocity.y);
            } else if (isKnockedBack && knockbackTime <= 0) {      
                isKnockedBack = false;
                myRigidbody.isKinematic = true;
            } else if (isAttacking && myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Attack") && myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
                transform.position = myAnimator.gameObject.transform.position;
                isAttacking = false;
            }

            if (currentHealth <= 0) {

                /* set the enemy state to dead */
                isAlive = false;

                /* prepare enemy for becoming a projectile */
                gameObject.layer = LayerMask.NameToLayer("DeadEnemies");
                transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("DeadEnemies");

                myCollider.sharedMaterial = deathMaterial;

                /* start using the enemy as projectile and destroy it afterwards */
                StartCoroutine(DestroyEnemyRoutine(1));

                /* create explosion on defeated enemy location */
                GameObject newExplosion = (GameObject) Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);  
                newExplosion.gameObject.GetComponent<ExplosionController>().StartExplosion(isAerialType);

                gameManager.addOrbs(Random.Range(Mathf.FloorToInt(orbsRewarded * 0.50f),orbsRewarded));
                gameManager.addDefeatedEnemy();

                /* check if we need to bounce the enemy up (ground) or down (aerial) */
                if (isAerialType) {
                    this.myRigidbody.velocity = new Vector2(10, -3);      
                } else {
                    this.myRigidbody.velocity = new Vector2(8, 6);
                }
            }

            /* set enemy animator states */
            myAnimator.SetBool("Alive", isAlive);
            myAnimator.SetFloat("Speed", Mathf.Abs(myRigidbody.velocity.x));
            myAnimator.SetBool("Knockback", isKnockedBack);
            myAnimator.SetBool("Attacking", isAttacking);

        } else {

            /* make sure the enemy is not going the wrong way */
            if(myRigidbody.velocity.x < 0) {
                myRigidbody.AddForce(new Vector2(0,0));
                myRigidbody.velocity = new Vector2(Mathf.Abs(myRigidbody.velocity.x), myRigidbody.velocity.y);
            }

            transform.Rotate (0,0,360 * Time.deltaTime);

        }
	}

    public bool isEnemyAlive() {
        return isAlive;
    }

    public IEnumerator destroyRemains() {
        /* allow it to ignore collisions from now on */
        myCollider.isTrigger = true;

        /* put it in front of the forground so it doesn't dissapear behind it */
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 3);


        yield return new WaitForSeconds(3);
        Destroy(gameObject);

    }

    /* if enemy doesn't get hit anything within *time* - destroy it anyways */
    IEnumerator DestroyEnemyRoutine(float time) {
        yield return new WaitForSeconds(time);
        StartCoroutine(destroyRemains());
    }

    public void TakeDamage(int damage) {
        transform.position = myCollider.gameObject.transform.position;
        myRigidbody.isKinematic = false;

        currentHealth -= damage;
        isKnockedBack = true;
        knockbackTime = knockbackLength;

        myRigidbody.velocity = new Vector2(8,2);


    }
    
    
}
