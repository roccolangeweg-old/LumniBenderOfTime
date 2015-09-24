using UnityEngine;
using System.Collections;

public class AttackController : MonoBehaviour {

    private Animator myAnimator;

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
            Destroy(this.gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D other) {

        if (other.gameObject.tag == "Enemy") {
            other.GetComponentInParent<EnemyController>().TakeDamage(1);
        }
    }
}
