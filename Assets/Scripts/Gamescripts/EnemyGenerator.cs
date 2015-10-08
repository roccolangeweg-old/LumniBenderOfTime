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



        for (int i = 0; i < amountOfEnemies; i++) {
            if (spawnAllowed) {
                GameObject newEnemy = (GameObject)Instantiate(enemyList [Random.Range(0, enemyList.Count)]);
                if (newEnemy.GetComponent<EnemyController>().isAerialType) {
                    newEnemy.transform.position = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
                } else {
                    newEnemy.transform.position = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        StartCoroutine(SpawnEnemy(Random.Range(0.5f,1.5f),Mathf.RoundToInt(Random.Range(1,3 * GameManager.instance.RoundedCombo() / 2))));
    }

    public void loadEnemies(List<GameObject> enemies) {
        enemyList = enemies;
    }

    public void SetSpawnAllowed(bool allowed) {
        spawnAllowed = allowed;
    }


}
