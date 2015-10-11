using UnityEngine;
using System.Collections;

public class ExplosionController : MonoBehaviour {

    private Animator myAnimator;

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
       // myAnimator.SetBool("Aerial", false);
       // myAnimator.SetBool("Ground", false);
	}
	
	// Update is called once per frame
	void Update () {
        if (myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Explosion_Ground") || myAnimator.GetCurrentAnimatorStateInfo(0).IsName("Explosion_Aerial")) {

            if (myAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !myAnimator.IsInTransition(0)) {
                ObjectPooler.instance.AddToPool(gameObject);
            }
       
        }
	}

    public void StartExplosion(bool aerial) {
        if (aerial) {
            GetComponent<Animator>().SetTrigger("Aerial");
        } else {
            GetComponent<Animator>().SetTrigger("Ground");
        }
    }
}
