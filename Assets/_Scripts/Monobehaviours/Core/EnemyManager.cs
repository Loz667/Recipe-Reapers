using System.Collections.Generic;
using UnityEngine;

namespace Reapers.Core
{
    public class EnemyManager : MonoBehaviour
    {
        private static GameObject instance;

        [SerializeField] private EnemyInfo[] allEnemies;
        [SerializeField] private List<Enemy> currentEnemies;

        private const float LEVEL_MODIFIER = 0.5f;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this.gameObject;
            }

            DontDestroyOnLoad(gameObject);
        }

        public void GenerateEnemiesByEncounter(Encounter[] encounters, int maxNumEnemies)
        {
            currentEnemies.Clear();
            int numEnemies = Random.Range(1, maxNumEnemies + 1);
            for (int i = 0; i < numEnemies; i++)
            {
                Encounter tempEnc = encounters[Random.Range(0, encounters.Length)];
                int level = Random.Range(tempEnc.minLevel, tempEnc.maxLevel + 1);
                GenerateEnemyByName(tempEnc.EnemyType.EnemyName, level);
            }
        }

        private void GenerateEnemyByName(string enemyName, int level)
        {
            for (int i = 0; i < allEnemies.Length; i++)
            {
                if (enemyName == allEnemies[i].EnemyName)
                {
                    Enemy newEnemy = new Enemy();

                    newEnemy.EnemyName = allEnemies[i].EnemyName;
                    newEnemy.EnemyVisualPrefab = allEnemies[i].EnemyVisualPrefab;
                    newEnemy.Level = level;
                    float levelModifier = (LEVEL_MODIFIER * newEnemy.Level);

                    newEnemy.MaxHunger = Mathf.RoundToInt(allEnemies[i].BaseHunger + (allEnemies[i].BaseHunger * levelModifier));
                    newEnemy.CurrHunger = 1;
                    newEnemy.Anger = Mathf.RoundToInt(allEnemies[i].BaseAnger + (allEnemies[i].BaseAnger * levelModifier));
                    newEnemy.Intelligence = Mathf.RoundToInt(allEnemies[i].BaseIntelligence +
                        (allEnemies[i].BaseIntelligence * levelModifier));
                    newEnemy.ExperienceReward = Mathf.RoundToInt(allEnemies[i].BaseExperienceReward +
                        (allEnemies[i].BaseExperienceReward * levelModifier));

                    currentEnemies.Add(newEnemy);
                }
            }
        }

        public List<Enemy> GetCurrentEnemies()
        {
            return currentEnemies;
        }

        public int GetExperienceReward(int reward)
        {
            foreach (Enemy enemy in currentEnemies)
            {
                reward = enemy.ExperienceReward;
            }
            return reward;
        }
    }

    [System.Serializable]
    public class Enemy
    {
        public string EnemyName;
        public GameObject EnemyVisualPrefab;
        public int Level;
        public int CurrHunger;
        public int MaxHunger;
        public int Anger;
        public int Intelligence;
        public int ExperienceReward;
    }

}