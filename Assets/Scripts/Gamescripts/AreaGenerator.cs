using UnityEngine;
using System.Collections;

public class AreaGenerator : MonoBehaviour {

    public GameManager gameManager;

    public Transform generationPoint;

    public float currentSectionWidth;

    public ObjectPooler objectPool;

    public GameObject[] sections;
    private float[] sectionWidths;
    private int sectionSelector;

    public GameObject caveObject;

    private float levelPoint;
    public int levelLength;

    private bool caveActive;

	// Use this for initialization
	void Start () {

        sectionWidths = new float[sections.Length];
        levelPoint = 0;
        caveActive = false;

        for (int i = 0; i < sections.Length; i++) {
            sectionWidths[i] = sections[i].GetComponent<SectionController>().getSectionWidth();
        } 

	}
	
	// Update is called once per frame
	void Update () {
	    
        if (this.transform.position.x < generationPoint.transform.position.x) {

            if (levelPoint < levelLength) {

                sectionSelector = Random.Range(0, sections.Length);

                this.transform.position = new Vector3(this.transform.position.x + currentSectionWidth, this.transform.position.y, this.transform.position.z);
                currentSectionWidth = sectionWidths[sectionSelector];

                Instantiate (sections[sectionSelector], this.transform.position, this.transform.rotation);

                levelPoint += currentSectionWidth; 
                /* GameObject newPlatform = objectPool.getPooledObject();
                newPlatform.transform.position = this.transform.position;
                newPlatform.transform.rotation = this.transform.rotation;
                newPlatform.SetActive(true); */
            } else if( !caveActive ) {
                /* generate a cave to transition to new level */
                Instantiate (caveObject, new Vector3(this.transform.position.x + currentSectionWidth, this.transform.position.y + 5, this.transform.position.z), this.transform.rotation);
                caveActive = true;
            }
        }
	}
}
