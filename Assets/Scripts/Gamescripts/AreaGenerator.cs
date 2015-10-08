using UnityEngine;
using System.Collections;

public class AreaGenerator : MonoBehaviour {

    public Transform generationPoint;

    public float currentSectionWidth;

    public ObjectPooler objectPool;

    public GameObject[] sections;
    private float[] sectionWidths;
    private int sectionSelector;

    public GameObject caveObject;

    public int levelLength;

    private bool spawnAllowed;

	// Use this for initialization
	void Start () {

        spawnAllowed = true;

        sectionWidths = new float[sections.Length];

        for (int i = 0; i < sections.Length; i++) {
            sectionWidths[i] = sections[i].GetComponent<SectionController>().getSectionWidth();
        } 

	}
	
	// Update is called once per frame
	void Update () {
	    
        if (this.transform.position.x < generationPoint.transform.position.x && spawnAllowed) {

                sectionSelector = Random.Range(0, sections.Length);

                this.transform.position = new Vector3(this.transform.position.x + currentSectionWidth, this.transform.position.y, this.transform.position.z);
                currentSectionWidth = sectionWidths[sectionSelector];

                GameObject newSection = ObjectPooler.instance.GetObjectByName(sections[sectionSelector].name, true);
                newSection.transform.position = this.transform.position;

        }
	}

    public void CreateCave() {
        /* generate a cave to transition to new level */
        Instantiate (caveObject, new Vector3(this.transform.position.x + currentSectionWidth, this.transform.position.y + 5, this.transform.position.z), this.transform.rotation);
        currentSectionWidth = 32;
    }

    public void SetSpawnAllowed(bool allowed) {
        spawnAllowed = allowed;
    }
}
