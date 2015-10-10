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

        StartCoroutine(SpawnEnemy(0.5f, 1));
	}

    private IEnumerator SpawnEnemy(float timeTillSpawn, int amountOfEnemies) {

        yield return new WaitForSeconds(timeTillSpawn);

        while (transform.position.x < lastSpawnPosition + 2) {
            yield return new WaitForEndOfFrame();
        }

        for (int i = 0; i < amountOfEnemies; i++) {
            if (spawnAllowed) {

                GameObject newEnemy = ObjectPooler.instance.GetObjectByName(enemyList [Random.Range(0, enemyList.Count)].name, true);

                if (newEnemy.GetComponent<EnemyController>().isAerialType) {
                    newEnemy.transform.position = new Vector3(transform.position.x, transform.position.y + 2, -1);
                } else {
                    newEnemy.transform.position = new Vector3(transform.position.x, transform.position.y + 1, -1);
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        lastSpawnPosition = transform.position.x;

        int spawnMultiplier = 1;
        if (GameManager.instance.RoundedCombo() > 8) {
            spawnMultiplier = 4;
        } else if (GameManager.instance.RoundedCombo() > 6) {
            spawnMultiplier = 3;
        } else if (GameManager.instance.RoundedCombo() > 3) {
            spawnMultiplier = 2;
        }

        StartCoroutine(SpawnEnemy(Random.Range(1f,2f), 1 * spawnMultiplier));
    }

    public void loadEnemies(List<GameObject> enemies) {
        enemyList = enemies;
    }

    public void SetSpawnAllowed(bool allowed) {
        StartCoroutine(EnableSpawnAfterWait(allowed));
    }

    private IEnumerator EnableSpawnAfterWait(bool allowed) {
        yield return new WaitForSeconds(1f);
        spawnAllowed = allowed;
    }


}
