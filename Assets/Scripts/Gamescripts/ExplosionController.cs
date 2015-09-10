using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {

    private Animator myAnimator;

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Explosion_Ground") || myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Explosion_Air")) {

            if (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
                Destroy(gameObject);
            }
       
        }
	}

    public void StartExplosion(bool aerial) {
        if (aerial) {
            GetComponent<Animator>().SetBool("Aerial", true);
        } else {
            GetComponent<Animator>().SetBool("Ground", true);
        }
    }
}
