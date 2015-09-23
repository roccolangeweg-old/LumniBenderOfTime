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

    private Vector3 basePosition;
    public float flyingSwing;

    private int currentHealth;
    private bool isKnockedBack;

    private float knockbackTime;
    public float knockbackLength;
    public float knockbackAmplifier;

    private Rigidbody2D myRigidbody;
    private Animator myAnimator;

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
        myAnimator = this.GetComponent<Animator>();
        basePosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        if (transform.position.x < destructionPoint.transform.position.x) {
            Destroy(gameObject);
        }

        if (isAlive) {

            /* check if knockback is done */
            knockbackTime -= Time.deltaTime;

            if (!isKnockedBack && !isAttacking) {
                myRigidbody.velocity = new Vector2(-moveSpeed, myRigidbody.velocity.y);
            } else if (isKnockedBack && knockbackTime <= 0) {      
                isKnockedBack = false;
            }

            /* let aerial enemies fly up & down */
            if (isAerialType) {
                if(player.transform.position.x + 6.5 >= transform.position.x && transform.position.x > player.transform.position.x + 0.5 && !isAttacking) {
					isAttacking = true;
					MoveTowardsPlayer();
				} else if (transform.position.x < player.transform.position.x + 1 && isAttacking) {
					isAttacking = false;
                    MoveBackToPosition();
				}
				
				if(transform.position.y < basePosition.y && !isAttacking) {
					myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, flyingSwing * 3);
				}
            }

            if (currentHealth <= 0) {

                /* set the enemy state to dead */
                isAlive = false;

                /* prepare enemy for becoming a projectile */
                gameObject.layer = LayerMask.NameToLayer("DeadEnemies");
                GetComponent<Collider2D>().sharedMaterial = deathMaterial;

                /* start using the enemy as projectile and destroy it afterwards */
                StartCoroutine(DestroyEnemyRoutine(1));

                /* create explosion on defeated enemy location */
                GameObject newExplosion = (GameObject) Instantiate(explosion, new Vector3(transform.position.x, transform.position.y, transform.position.z), Quaternion.identity);  
                newExplosion.gameObject.GetComponent<ExplosionController>().StartExplosion(isAerialType);

                gameManager.addOrbs(orbsRewarded);
                gameManager.addDefeatedEnemy();

                /* check if we need to bounce the enemy up (ground) or down (aerial) */
                if (isAerialType) {
                    this.myRigidbody.velocity = new Vector2(8, -3);      
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
               
            /* spin the enemy remains for FX */
            transform.Rotate (0,0,270 * Time.deltaTime);

        }
	}

    void OnTriggerEnter2D(Collider2D other) {

        /* check if we hit a player attack */
        if (other.gameObject.tag == "Attack") {
            currentHealth-=1;

            isKnockedBack = true;
            knockbackTime = knockbackLength;
            myRigidbody.velocity = new Vector2(4 * knockbackAmplifier * 1f, 5 * knockbackAmplifier * 0.3f);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {

        /* check if we hit an dead enemy projectile */
        if (other.gameObject.tag == "Enemy" && !other.gameObject.GetComponent<EnemyController>().isEnemyAlive()) {
            currentHealth-=1;
            isKnockedBack = true;
            knockbackTime = knockbackLength;
            myRigidbody.velocity = new Vector2(2,2);

            /* destroy the actual enemy - drop it off the screen */
            StartCoroutine(other.gameObject.GetComponent<EnemyController>().destroyRemains());
        }
    }

    public bool isEnemyAlive() {
        return isAlive;
    }

    public IEnumerator destroyRemains() {
        /* allow it to ignore collisions from now on */
        GetComponent<Collider2D>().isTrigger = true;

        /* put it in front of the forground so it doesn't dissapear behind it */
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 3);

        gameManager.getAnalytics().LogEvent("Statistics","Gameplay","Enemies Defeated",1);

        yield return new WaitForSeconds(3);
        Destroy(gameObject);

    }

    /* if enemy doesn't get hit anything within *time* - destroy it anyways */
    IEnumerator DestroyEnemyRoutine(float time) {
        yield return new WaitForSeconds(time);
        StartCoroutine(destroyRemains());
    }

	private void MoveTowardsPlayer() {
        Vector2 direction = new Vector2(player.transform.position.x + 2 - transform.position.x, -4.5f);
        myRigidbody.AddRelativeForce(direction.normalized * 5000, ForceMode2D.Force);

	}

    private void MoveBackToPosition() {
        Vector2 direction = new Vector2(transform.position.x - 5, basePosition.y);
        myRigidbody.AddForce(direction.normalized * 5000, ForceMode2D.Force);
    }


}
