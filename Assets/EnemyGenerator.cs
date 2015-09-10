using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGenerator : MonoBehaviour {

    public List<GameObject> enemyList;
    public float minSecondsBetweenEnemies;

    private int currentListLength;
    private float secondsSinceEnemy;
    private float distanceSinceLastSpawn;

	// Use this for initialization
	void Start () {
        InvokeRepeating("PrepareEnemySpawn", 2f, minSecondsBetweenEnemies);
	}
	
    void PrepareEnemySpawn() {

        float timeTillSpawn = Random.Range(0f, 2f);
        GameObject enemy = enemyList[Random.Range(0, enemyList.Count)];

        StartCoroutine(spawnEnemy(enemy, timeTillSpawn));
    }

    IEnumerator spawnEnemy(GameObject enemy, float time) {

        yield return new WaitForSeconds(time);

        GameObject newEnemy = (GameObject) Instantiate(enemy);
        newEnemy.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

    }


}
