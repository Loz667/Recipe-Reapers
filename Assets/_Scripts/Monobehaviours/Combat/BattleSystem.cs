using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Reapers.Core;
using Reapers.SceneManagement;
using Reapers.UI;

namespace Reapers.Combat
{
    public class BattleSystem : MonoBehaviour
    {
        [SerializeField] private enum BattleState { Start, Selection, Battle, Won, Lost, Run }

        [Header("Battle State")]
        [SerializeField] private BattleState state;

        [Header("Battlers")]
        [SerializeField] private List<BattleEntities> allBattlers = new List<BattleEntities>();
        [SerializeField] private List<BattleEntities> playerBattlers = new List<BattleEntities>();
        [SerializeField] private List<BattleEntities> enemyBattlers = new List<BattleEntities>();

        [Header("Spawn Points")]
        [SerializeField] private Transform[] partySpawnPoints;
        [SerializeField] private Transform[] enemySpawnPoints;

        [Header("UI")]
        [SerializeField] private GameObject battleMenu;
        [SerializeField] private TextMeshProUGUI actionText;
        [SerializeField] private GameObject enemySelectMenu;
        [SerializeField] private GameObject[] enemySelectButtons;
        [SerializeField] private GameObject battleInfoPopup;
        [SerializeField] private TextMeshProUGUI battleText;

        private PartyManager partyManager;
        private EnemyManager enemyManager;
        private int currentPlayer;

        private const int OVERWORLD_MEADOW_SCENE_INDEX = 2;
        private const int RUN_CHANCE = 50;
        private const int TURN_DURATION = 3;

        private const string ACTION_MSG = "'s Actions: ";
        private const string WIN_MSG = "All monsters hunger is sated";
        private const string LOSE_MSG = "All party members were vanquished";
        private const string LEAVE_SUCCESS_MSG = "The party escaped";
        private const string LEAVE_FAIL_MSG = "The party failed to escape";

        void Start()
        {
            partyManager = GameObject.FindFirstObjectByType<PartyManager>();
            enemyManager = GameObject.FindFirstObjectByType<EnemyManager>();

            CreatePartyEntities();
            CreateEnemyEntities();

            ShowBattleMenu();
            DetermineBattleOrder();
        }

        private IEnumerator BattleRoutine()
        {
            //Disable enemy selection menu
            enemySelectMenu.SetActive(false);
            //Change state to Battle
            state = BattleState.Battle;
            //Enable battle info popup
            battleInfoPopup.SetActive(true);

            //loop through battlers and do appropriate action
            for (int i = 0; i < allBattlers.Count; i++)
            {
                if (state == BattleState.Battle && allBattlers[i].CurrHealth > 0)
                {
                    switch (allBattlers[i].BattleAction)
                    {
                        case BattleEntities.Action.Feed:
                            //Do feed action
                            yield return StartCoroutine(FeedRoutine(i));
                            break;
                        case BattleEntities.Action.Leave:
                            yield return StartCoroutine(LeaveRoutine());
                            break;
                        default:
                            Debug.Log("Error -> no battle action available");
                            break;
                    }
                }

            }

            //If not won or lost, repeat loop
            if (state == BattleState.Battle)
            {
                battleInfoPopup.SetActive(false);
                currentPlayer = 0;
                ShowBattleMenu();
            }

            yield return null;
        }

        private IEnumerator FeedRoutine(int i)
        {
            //Player's turn
            if (allBattlers[i].IsPlayer == true)
            {
                //Feed selected enemy
                BattleEntities currAttacker = allBattlers[i];
                if (allBattlers[currAttacker.Target].CurrHealth <= 0)
                {
                    currAttacker.SetTarget(GetRandomEnemy());
                }
                BattleEntities currTarget = allBattlers[currAttacker.Target];
                FeedAction(currAttacker, currTarget);

                yield return new WaitForSeconds(TURN_DURATION);

                if (currTarget.CurrHealth >= currTarget.MaxHealth)
                {
                    battleText.text = string.Format("{0} reached full hunger", currTarget.Name);
                    yield return new WaitForSeconds(TURN_DURATION);
                    AwardExperience(currAttacker.Target);
                    currTarget.BattleVisuals.PlayDeathAnim();
                    enemyBattlers.Remove(currTarget);
                    allBattlers.Remove(currTarget);

                    if (enemyBattlers.Count <= 0)
                    {
                        state = BattleState.Won;
                        battleText.text = WIN_MSG;
                        yield return new WaitForSeconds(TURN_DURATION);
                        SceneTransition.TransitionToScene(OVERWORLD_MEADOW_SCENE_INDEX);
                        enabled = false;
                    }
                }
            }

            //Enemy's turn
            if (i < allBattlers.Count && allBattlers[i].IsPlayer == false)
            {
                //get random party member to become target
                BattleEntities currAttacker = allBattlers[i];
                currAttacker.SetTarget(GetRandomPartyMember());
                //attack selected party member (attack action)
                BattleEntities currTarget = allBattlers[currAttacker.Target];
                AttackAction(currAttacker, currTarget);
                //wait a few seconds
                yield return new WaitForSeconds(TURN_DURATION);

                if (currTarget.CurrHealth <= 0)
                {
                    //kill party member
                    battleText.text = string.Format("{0} defeated {1}", currAttacker.Name, currTarget.Name);
                    yield return new WaitForSeconds(TURN_DURATION);
                    playerBattlers.Remove(currTarget);
                    allBattlers.Remove(currTarget);
                }

                //if no party members remain, battle is lost
                if (playerBattlers.Count <= 0)
                {
                    state = BattleState.Lost;
                    battleText.text = LOSE_MSG;
                    yield return new WaitForSeconds(TURN_DURATION);
                    SceneTransition.TransitionToScene(OVERWORLD_MEADOW_SCENE_INDEX);
                    enabled = false;
                }
            }
        }

        private IEnumerator LeaveRoutine()
        {
            if (state == BattleState.Battle)
            {
                if (Random.Range(1, 101) >= RUN_CHANCE)
                {
                    //Tell player leave choice was successful
                    battleText.text = LEAVE_SUCCESS_MSG;
                    //Set battle state to Run
                    state = BattleState.Run;
                    //Clear the list of battlers
                    allBattlers.Clear();
                    //A short pause
                    yield return new WaitForSeconds(TURN_DURATION);
                    //Return to the Overworld
                    SceneTransition.TransitionToScene(OVERWORLD_MEADOW_SCENE_INDEX);
                    enabled = false;
                    yield break;
                }
                else
                {
                    //Tell player leaving was not an option
                    battleText.text = LEAVE_FAIL_MSG;
                    //A short pause
                    yield return new WaitForSeconds(TURN_DURATION);
                }
            }
        }

        private void CreatePartyEntities()
        {
            //get current party
            List<PartyMember> currentParty = new List<PartyMember>();
            currentParty = partyManager.GetAliveParty();

            //create battle entities from party members
            for (int i = 0; i < currentParty.Count; i++)
            {
                BattleEntities tempEntity = new BattleEntities();
                //assign values
                tempEntity.SetEntityValues(currentParty[i].MemberName, currentParty[i].CurrHealth, currentParty[i].MaxHealth,
                    currentParty[i].Intelligence, currentParty[i].Strength, currentParty[i].Level, true);

                //create visual for battler
                BattleVisuals tempBV = Instantiate(currentParty[i].MemberBattleVisualPrefab, partySpawnPoints[i].position,
                    Quaternion.identity).GetComponent<BattleVisuals>();
                //set starting values
                tempBV.SetStartingValues(currentParty[i].CurrHealth, currentParty[i].MaxHealth, currentParty[i].Level);
                //assign to battle entity
                tempEntity.BattleVisuals = tempBV;

                allBattlers.Add(tempEntity);
                playerBattlers.Add(tempEntity);
            }
        }

        private void CreateEnemyEntities()
        {
            List<Enemy> currentEnemies = new List<Enemy>();
            currentEnemies = enemyManager.GetCurrentEnemies();

            for (int i = 0; i < currentEnemies.Count; i++)
            {
                BattleEntities tempEntity = new BattleEntities();

                tempEntity.SetEntityValues(currentEnemies[i].EnemyName, currentEnemies[i].CurrHunger, currentEnemies[i].MaxHunger,
                    currentEnemies[i].Intelligence, currentEnemies[i].Anger, currentEnemies[i].Level, false);

                BattleVisuals tempBV = Instantiate(currentEnemies[i].EnemyVisualPrefab, enemySpawnPoints[i].position,
                    Quaternion.identity).GetComponent<BattleVisuals>();

                tempBV.SetStartingValues(currentEnemies[i].CurrHunger, currentEnemies[i].MaxHunger, currentEnemies[i].Level);

                tempEntity.BattleVisuals = tempBV;

                allBattlers.Add(tempEntity);
                enemyBattlers.Add(tempEntity);
            }
        }

        public void ShowBattleMenu()
        {
            actionText.text = playerBattlers[currentPlayer].Name + ACTION_MSG;
            battleMenu.SetActive(true);
        }

        public void ShowEnemySelectMenu()
        {
            battleMenu.SetActive(false);
            SetEnemySelectButtons();
            enemySelectMenu.SetActive(true);
        }

        private void SetEnemySelectButtons()
        {
            for (int i = 0; i < enemySelectButtons.Length; i++)
            {
                enemySelectButtons[i].SetActive(false);
            }

            for (int j = 0; j < enemyBattlers.Count; j++)
            {
                enemySelectButtons[j].SetActive(true);
                enemySelectButtons[j].GetComponentInChildren<TextMeshProUGUI>().text = enemyBattlers[j].Name;
            }
        }

        public void SelectEnemy(int currentEnemy)
        {
            //set current party member's target
            BattleEntities currentPlayerEntitiy = playerBattlers[currentPlayer];
            currentPlayerEntitiy.SetTarget(allBattlers.IndexOf(enemyBattlers[currentEnemy]));
            //tell battle system current member intends to feed
            currentPlayerEntitiy.BattleAction = BattleEntities.Action.Feed;
            //increment through party members
            currentPlayer++;

            if (currentPlayer >= playerBattlers.Count)
            {
                //Start battle
                StartCoroutine(BattleRoutine());
            }
            else
            {
                //Show battle menu for next member
                enemySelectMenu.SetActive(false);
                ShowBattleMenu();
            }
        }

        public void LeaveBattle()
        {
            state = BattleState.Selection;
            //set current party member's target
            BattleEntities currentPlayerEntitiy = playerBattlers[currentPlayer];

            //tell battle system current member intends to leave
            currentPlayerEntitiy.BattleAction = BattleEntities.Action.Leave;

            battleMenu.SetActive(false);
            //increment through party members
            currentPlayer++;

            if (currentPlayer >= playerBattlers.Count)
            {
                //Start battle
                StartCoroutine(BattleRoutine());
            }
            else
            {
                //Show battle menu for next member
                enemySelectMenu.SetActive(false);
                ShowBattleMenu();
            }
        }

        private void FeedAction(BattleEntities currAttacker, BattleEntities currTarget)
        {
            int heal = currAttacker.Strength; //get damage (could use an algorithm here)
            currAttacker.BattleVisuals.PlayAttackAnim(); //play animation
            currTarget.CurrHealth += heal; //deal damage
            currTarget.BattleVisuals.PlayHitAnim(); //play animation
            currTarget.UpdateUI(); //update UI
            battleText.text = string.Format("{0} feeds {1}. Its hunger decreases by {2}", currAttacker.Name, currTarget.Name, heal);
        }

        private void AttackAction(BattleEntities currAttacker, BattleEntities currTarget)
        {
            int damage = currAttacker.Strength;
            currAttacker.BattleVisuals.PlayAttackAnim();
            currTarget.CurrHealth -= damage;
            currTarget.BattleVisuals.PlayHitAnim();
            currTarget.UpdateUI();
            battleText.text = string.Format("{0} attacks {1} for {2} damage", currAttacker.Name, currTarget.Name, damage);
            SaveHealth();
        }

        private int GetRandomPartyMember()
        {
            //Create a temporary list
            List<int> partyMembers = new List<int>();
            //find all party members and add to list
            for (int i = 0; i < allBattlers.Count; i++)
            {
                if (allBattlers[i].IsPlayer == true)
                {
                    partyMembers.Add(i);
                }
            }
            //Return random party member
            return partyMembers[Random.Range(0, partyMembers.Count)];
        }

        private int GetRandomEnemy()
        {
            List<int> enemies = new List<int>();

            for (int i = 0; i < allBattlers.Count; i++)
            {
                if (allBattlers[i].IsPlayer == false)
                {
                    enemies.Add(i);
                }
            }

            return enemies[Random.Range(0, enemies.Count)];
        }

        private void AwardExperience(int target)
        {
            for (int i = 0; i < playerBattlers.Count; i++)
            {
                partyManager.GainExperience(i, enemyManager.GetExperienceReward(target));
            }
        }

        private void SaveHealth()
        {
            for (int i = 0; i < playerBattlers.Count; i++)
            {
                partyManager.SaveHealth(i, playerBattlers[i].CurrHealth);
            }
        }

        private void DetermineBattleOrder()
        {
            //Sorts battlers by intelligence level in ascending order
            allBattlers.Sort((bi1, bi2) => -bi1.Intelligence.CompareTo(bi2.Intelligence));
        }
    }

    [System.Serializable]
    public class BattleEntities
    {
        public enum Action { Feed, Leave }
        public Action BattleAction;

        public string Name;
        public BattleVisuals BattleVisuals;
        public int CurrHealth;
        public int MaxHealth;
        public int Intelligence;
        public int Strength;
        public int Level;
        public bool IsPlayer;
        public int Target;

        public void SetEntityValues(string name, int currHealth, int maxHealth, int intelligence, int strength, int level, bool isPlayer)
        {
            Name = name;
            CurrHealth = currHealth;
            MaxHealth = maxHealth;
            Intelligence = intelligence;
            Strength = strength;
            Level = level;
            IsPlayer = isPlayer;
        }

        public void SetTarget(int target)
        {
            Target = target;
        }

        public void UpdateUI()
        {
            BattleVisuals.ChangeHealthValue(CurrHealth);
        }
    }

}