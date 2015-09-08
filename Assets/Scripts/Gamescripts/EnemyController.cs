using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public int orbsRewarded;
    public int baseHealth;
    public bool isAerialType;
    public float moveSpeed;

    private bool isAlive;

    private Vector3 basePosition;
    public float flyingSwing;

    private int currentHealth;
    private bool isKnockedBack;

    private float knockbackTime;
    public float knockbackLength;
    public float knockbackAmplifier;

    private Rigidbody2D myRigidbody;
    private Animator myAnimator;

	// Use this for initialization
	void Start () {
        currentHealth = baseHealth;
        isAlive = true;
        myRigidbody = this.GetComponent<Rigidbody2D>();
        myAnimator = this.GetComponent<Animator>();
        basePosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        if (isAlive) {
        
            knockbackTime -= Time.deltaTime;

            if (!isKnockedBack) {
                myRigidbody.velocity = new Vector2(-moveSpeed, myRigidbody.velocity.y);
            } else if (isKnockedBack && knockbackTime <= 0) {      
                isKnockedBack = false;
            }

            if (isAerialType) {

                if (transform.position.y < basePosition.y - flyingSwing) {
                    myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, flyingSwing * 3);
                }

            }

            if (currentHealth <= 0) {

                myRigidbody.constraints = RigidbodyConstraints2D.None;
                isAlive = false;

                StartCoroutine(DestroyEnemyRoutine(3));

                int shootUp = Random.Range(0, 1);
                if (shootUp == 1) {
                    this.myRigidbody.velocity = new Vector2(10, 3);      
                } else {
                    this.myRigidbody.velocity = new Vector2(10, -3);
                }
            }

            myAnimator.SetBool("Alive", isAlive);
            myAnimator.SetFloat("Speed", Mathf.Abs(myRigidbody.velocity.x));
            myAnimator.SetBool("Knockback", isKnockedBack);

        } else {

            if(myRigidbody.velocity.x < 0) {
                myRigidbody.velocity = new Vector2(Mathf.Abs(myRigidbody.velocity.x), myRigidbody.velocity.y);
            }

        }



	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Attack") {
            currentHealth-=1;

            isKnockedBack = true;
            knockbackTime = knockbackLength;
            myRigidbody.velocity = new Vector2(6 * knockbackAmplifier * 1f, 5 * knockbackAmplifier * 0.3f);
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.tag == "Enemy" && !other.gameObject.GetComponent<EnemyController>().isEnemyAlive()) {
            // do nothing
        }
    }

    public bool isEnemyAlive() {
        return isAlive;
    }

    IEnumerator DestroyEnemyRoutine(float time) {
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

}
