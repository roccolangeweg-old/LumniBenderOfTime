using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public int orbsRewarded;
    public int baseHealth;
    public bool isAerialType;
    public float moveSpeed;

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
        myRigidbody = this.GetComponent<Rigidbody2D>();
        myAnimator = this.GetComponent<Animator>();
        basePosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

        knockbackTime -= Time.deltaTime;

        if (!isKnockedBack) {
            myRigidbody.velocity = new Vector2(-moveSpeed, myRigidbody.velocity.y);
        } else if (isKnockedBack && knockbackTime <= 0) {      
            isKnockedBack = false;
        }

        if (isAerialType) {

            if(transform.position.y < basePosition.y - flyingSwing) {
                myRigidbody.velocity = new Vector2(myRigidbody.velocity.x, flyingSwing * 3);
            }

        }

        myAnimator.SetFloat("Speed", Mathf.Abs(myRigidbody.velocity.x));
        myAnimator.SetBool("Knockback", isKnockedBack);

        if (currentHealth < 0) {
            myAnimator.SetBool("Alive", false);
            this.GetComponent<Collider2D>().isTrigger = false;
            this.myRigidbody.velocity = new Vector2(0,0);
        }

	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Attack") {
            //currentHealth-=1;

            isKnockedBack = true;
            knockbackTime = knockbackLength;
            myRigidbody.velocity = new Vector2(6 * knockbackAmplifier * 1f, 5 * knockbackAmplifier * 0.3f);
        }
    }
}
