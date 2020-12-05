using System.Collections.Generic;
using UnityEngine;

public class LevelGeneratorScript : MonoBehaviour
{

    private const float PLAYER_DISTANCE_SPAWN_LEVEL_PART = 15f;

    [SerializeField] public Transform levelPartStart;
    [SerializeField] public List<Transform> levelPartList;
    [SerializeField] public Rigidbody2D player;

    private Vector3 lastEndPosition;
    private const int VERTICAL_OFFSET = 5;
    private void Awake()
    {
        // set lastEndPosition to the position of the EndPosition empty game object of level part start for initizialization
        lastEndPosition = levelPartStart.Find("EndPosition").position + new Vector3(0, VERTICAL_OFFSET, 0);
        

        int startingSpawnLevelParts = 2; // broj koliko ih zelis spawnat na pocetku odmah, default
        for (int i = 0; i < startingSpawnLevelParts; i++)
        {
            SpawnLevelPart();
        }
    }

    private void Update()
    {
        if (Vector3.Distance(player.transform.position, lastEndPosition) < PLAYER_DISTANCE_SPAWN_LEVEL_PART)
        {
            SpawnLevelPart();
        }
    }

    public void SpawnLevelPart()
    {
        // Select a random level part as a transform
        Transform chosenLevelPart = getRandomLevel();

        // Since the original method returns the end position of a newly instantiated level part, get its transform
        Transform lastLevelPartTransform = SpawnLevelPart(chosenLevelPart, lastEndPosition);

        lastEndPosition = lastLevelPartTransform.Find("EndPosition").position + new Vector3(0, VERTICAL_OFFSET, 0);
    }
    private Transform SpawnLevelPart(Transform levelPart, Vector3 spawnPosition)
    {

        // Create a new level part using Instantiate, return newly created clone's Transform and return it for further use
        Transform levelPartTransform = Instantiate(levelPart, spawnPosition, Quaternion.identity);
        return levelPartTransform;
    }

    public void RegenerateStartingLevels()
    {
        if (Player.hasDied)
        {
            lastEndPosition = new Vector3(0f, 4f, 0f);
            SpawnLevelPart(getRandomLevel(), lastEndPosition);
            SpawnLevelPart();
            Player.hasDied = false;
        }
    }

    private Transform getRandomLevel() {
        return levelPartList[Random.Range(0, levelPartList.Count)];
    }
}
