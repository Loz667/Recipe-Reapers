using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class PartyManager : MonoBehaviour
    {
        private static GameObject instance;

        [SerializeField] private PartyMemberInfo[] members;
        [SerializeField] private List<PartyMember> currentParty;

        [SerializeField] private PartyMemberInfo defaultMember;

        private Vector3 playerPosition;

        private void Awake()
        {
            if (instance != null)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this.gameObject;
                AddMemberToPartyByName(defaultMember.MemberName);
            }

            DontDestroyOnLoad(gameObject);
        }

        public void AddMemberToPartyByName(string memberName)
        {
            for (int i = 0; i < members.Length; i++)
            {
                if (members[i].MemberName == memberName)
                {
                    PartyMember newPartyMember = new PartyMember();

                    newPartyMember.MemberName = members[i].MemberName;
                    newPartyMember.MemberOverworldVisualPrefab = members[i].MemberOverworldVisualPrefab;
                    newPartyMember.MemberBattleVisualPrefab = members[i].MemberBattleVisualPrefab;
                    newPartyMember.Level = members[i].StartLevel;
                    newPartyMember.CurrHealth = members[i].BaseHealth;
                    newPartyMember.MaxHealth = newPartyMember.CurrHealth;
                    newPartyMember.Strength = members[i].BaseStrength;
                    newPartyMember.Intelligence = members[i].BaseIntelligence;

                    currentParty.Add(newPartyMember);
                }
            }
        }

        public List<PartyMember> GetAliveParty()
        {
            List<PartyMember> aliveParty = new List<PartyMember>();
            aliveParty = currentParty;
            for (int i = 0; i < aliveParty.Count; i++)
            {
                if (aliveParty[i].CurrHealth <= 0)
                {
                    aliveParty.RemoveAt(i);
                }
            }
            return aliveParty;
        }

        public List<PartyMember> GetCurrentParty()
        {
            return currentParty;
        }

        public void SaveHealth(int partyMember, int health)
        {
            currentParty[partyMember].CurrHealth = health;
        }

        public void SetPosition(Vector3 position)
        {
            playerPosition = position;
        }

        public Vector3 GetPosition()
        {
            return playerPosition;
        }
    }
}

[System.Serializable]
public class PartyMember
{
    public string MemberName;
    public GameObject MemberOverworldVisualPrefab;
    public GameObject MemberBattleVisualPrefab;
    public int Level;
    public int CurrHealth;
    public int MaxHealth;
    public int Strength;
    public int Intelligence;
    public int CurrExp;
    public int MaxExp;
}
