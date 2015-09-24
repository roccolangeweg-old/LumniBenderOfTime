using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

    private EnemyController enemyController;

	// Use this for initialization
	void Start () {
        enemyController = GetComponentInParent<EnemyController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    void OnCollisionEnter2D(Collision2D other) {
        
        /* check if we hit an dead enemy projectile */
        if (other.gameObject.tag == "Enemy" && !other.gameObject.GetComponent<EnemyController>().isEnemyAlive()) {
            enemyController.TakeDamage(1);
            
            /* destroy the actual enemy - drop it off the screen */
            StartCoroutine(other.gameObject.GetComponent<EnemyController>().destroyRemains());
        }
    }
}
