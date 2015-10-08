using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {

    public static ObjectPooler instance;

    public GameObject[] objectPrefabs;
    public int[] amountToLoad;

    public int defaultLoadAmount = 3;

    private List<GameObject>[] pooledObjects;

    private List<GameObject> pooledSections; 
    private List<GameObject> pooledLayers;


    void Awake () {
        instance = this;
    }

	// Use this for initialization
	void Start () {

        /* create a list for each prefab */
        pooledObjects = new List<GameObject>[objectPrefabs.Length];

        int i = 0;
        foreach (GameObject objectPrefab in objectPrefabs) {
            pooledObjects[i] = new List<GameObject>();

            int loadAmount;
            if(i < amountToLoad.Length) {
                loadAmount = amountToLoad[i];
            } else {
                loadAmount = defaultLoadAmount;
            }

            for (int q = 0; q < loadAmount; q++) {
                GameObject loadedObj = (GameObject) Instantiate(objectPrefab);
                loadedObj.name = objectPrefab.name; 
                AddToPool(loadedObj);
            }

            i++;
        }


	}

    public GameObject GetObjectByName(string objectName, bool allowLoadNew) {
        for(int i=0; i < objectPrefabs.Length; i++) {
            GameObject prefab = objectPrefabs[i];

            if(prefab.name == objectName) {
                if(pooledObjects[i].Count > 0) {
                    GameObject pooledObject = pooledObjects[i][0];
                    pooledObjects[i].RemoveAt(0);
                    pooledObject.transform.parent = null;
                    pooledObject.SetActive(true);

                    return pooledObject;
                } else if(allowLoadNew) {
                    GameObject loadedObj = (GameObject) Instantiate(objectPrefabs[i]);
                    loadedObj.name = objectPrefabs[i].name;
                    AddToPool(loadedObj);
                    return GetObjectByName(objectName, allowLoadNew);
                }

                break;
            }
        }

        return null;
    }

    public void AddToPool (GameObject obj) {
        for (int i=0; i < objectPrefabs.Length; i++) {
            if (objectPrefabs [i].name == obj.name) {
                obj.SetActive(false);
                obj.transform.parent = this.transform;
                pooledObjects [i].Add(obj);
                return;
            }
        }
    }
  
}
