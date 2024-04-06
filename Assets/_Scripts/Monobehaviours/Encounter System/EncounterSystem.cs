using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterSystem : MonoBehaviour
{
    [SerializeField] private Encounter[] enemiesInScene;
    [SerializeField] private int maxNumEnemies;

    private EnemyManager enemyManager;

    void Start()
    {
        enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();
        enemyManager.GenerateEnemiesByEncounter(enemiesInScene, maxNumEnemies);
    }
}

[System.Serializable]
public class Encounter
{
    public EnemyInfo EnemyType;
    public int minLevel;
    public int maxLevel;
}
