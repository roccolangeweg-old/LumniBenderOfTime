using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    public List<LevelSet> levels;

    private EnemyGenerator enemyGenerator;
    private AreaGenerator areaGenerator;
    private ParallaxController backgroundGenerator;

    public int minLevelDistance, maxLevelDistance;

    public int defaultLevelNo;
    private LevelSet currentLevel;

	// Use this for initialization
    void Start() {

        enemyGenerator = FindObjectOfType<EnemyGenerator>();
        areaGenerator = FindObjectOfType<AreaGenerator>();
        backgroundGenerator = FindObjectOfType<ParallaxController>();

        Load(levels[defaultLevelNo]);
    }

    private void Load(LevelSet level) {
        currentLevel = level;
        enemyGenerator.loadEnemies(level.GetEnemies());
        backgroundGenerator.loadBackgrounds(level.GetBackgrounds());
    }

    public void StopSpawns() {
        SetSpawns(false);
        areaGenerator.CreateCave();
    }

    public void LoadNewLevel() {

        LevelSet nextLevel = levels [Random.Range(0, levels.Count)];

        while(nextLevel == currentLevel) {
            nextLevel = levels [Random.Range(0, levels.Count)];
        }

        Load(levels[Random.Range(0, levels.Count)]);
        SetSpawns(true);
    }

    private void SetSpawns(bool spawnAllowed) {
        enemyGenerator.SetSpawnAllowed(spawnAllowed);
        areaGenerator.SetSpawnAllowed(spawnAllowed);
    }

    public int RandomLevelDistance() {
        return Random.Range(minLevelDistance, maxLevelDistance) * 32;
    }
}

[System.Serializable]
public class LevelSet {

    public string name;
    public List<GameObject> backgrounds;
    public List<GameObject> enemies;
    public List<GameObject> sections;

    public List<GameObject> GetBackgrounds() {
        return backgrounds;
    }

    public List<GameObject> GetEnemies() {
        return enemies;
    }

    public List<GameObject> GetSections() {
        return sections;
    }

}
