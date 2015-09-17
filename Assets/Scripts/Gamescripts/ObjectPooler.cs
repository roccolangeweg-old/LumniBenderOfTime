using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectPooler : MonoBehaviour {

    public GameObject pooledObject;
    public int pooledAmount;

    private List<GameObject> pooledSections; 
    private List<GameObject> pooledLayers;

	// Use this for initialization
	void Start () {
        pooledSections = new List<GameObject>();

        for (int i = 0; i < pooledAmount; i++) {
            createNewObject();

        }
	}
	
    public GameObject getPooledObject() {

        for (int i = 0; i < pooledSections.Count; i++) {

            if (!pooledSections[i].activeInHierarchy) {
                return pooledSections[i];
            }

        }

        GameObject obj = createNewObject();
        return obj;

    }

    private GameObject createNewObject() {
        GameObject obj = (GameObject) Instantiate(pooledObject);
        obj.SetActive(false);
        pooledSections.Add(obj);

        return obj;

    }
  
}
