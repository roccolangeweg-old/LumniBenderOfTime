using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public int orbsRewarded;
    public int baseHealth;
    public bool isAerialType;
    public float moveSpeed;

    private int currentHealth;

    private Rigidbody2D myRigidbody;
    private Animator myAnimator;

	// Use this for initialization
	void Start () {
        currentHealth = baseHealth;
        myRigidbody = this.GetComponent<Rigidbody2D>();
        myAnimator = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        myRigidbody.velocity = new Vector2(-moveSpeed, myRigidbody.velocity.y);
        myAnimator.SetFloat("Speed", Mathf.Abs(myRigidbody.velocity.x));

        if (currentHealth < 0) {
            myAnimator.SetBool("Alive", false);
            this.GetComponent<Collider2D>().isTrigger = false;
            this.myRigidbody.velocity = new Vector2(0,0);
        }
	}

    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.tag == "Attack") {
            currentHealth-=1;
        }
    }
}
