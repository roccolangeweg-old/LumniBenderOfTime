using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyGenerator : MonoBehaviour {

    public List<GameObject> enemyList;
    public float minSecondsBetweenEnemies;

    private int currentListLength;
    private float secondsSinceEnemy;

    public float distanceBetweenSpawn;
    private float lastSpawnPosition;

    private bool spawnAllowed;

	// Use this for initialization
	void Start () {
        spawnAllowed = true;
        InvokeRepeating("PrepareEnemySpawn", 2f, minSecondsBetweenEnemies);
	}
	
    private void PrepareEnemySpawn() {

        float timeTillSpawn = Random.Range(0f, 1f);
        GameObject enemy = enemyList[Random.Range(0, enemyList.Count)];
        if (transform.position.x - lastSpawnPosition > distanceBetweenSpawn && spawnAllowed) {
            StartCoroutine(spawnEnemy(enemy, timeTillSpawn));
        }
    }

    private IEnumerator spawnEnemy(GameObject enemy, float time) {

        yield return new WaitForSeconds(time);

        GameObject newEnemy = (GameObject) Instantiate(enemy);

        if (newEnemy.GetComponent<EnemyController>().isAerialType) {
            newEnemy.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
        } else {
            newEnemy.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        }
        lastSpawnPosition = transform.position.x;

    }

    public void loadEnemies(List<GameObject> enemies) {
        enemyList = enemies;
    }

    public void SetSpawnAllowed(bool allowed) {
        spawnAllowed = allowed;
    }


}
