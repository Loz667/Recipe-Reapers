using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    private static GameObject instance;

    [SerializeField] private PartyMemberInfo[] allPartyMembers;
    [SerializeField] private List<PartyMember> currentParty;

    [SerializeField] private PartyMemberInfo defaultPartyMember;

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
            AddMemberToPartyByName(defaultPartyMember.MemberName);
            AddMemberToPartyByName(defaultPartyMember.MemberName);
        }

        DontDestroyOnLoad(gameObject);
    }

    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allPartyMembers.Length; i++)
        {
            if (allPartyMembers[i].MemberName == memberName)
            {
                PartyMember newPartyMember = new PartyMember();

                newPartyMember.MemberName = allPartyMembers[i].MemberName;
                newPartyMember.MemberOverworldVisualPrefab = allPartyMembers[i].MemberOverworldVisualPrefab;
                newPartyMember.MemberBattleVisualPrefab = allPartyMembers[i].MemberBattleVisualPrefab;
                newPartyMember.Level = allPartyMembers[i].StartLevel;
                newPartyMember.CurrHealth = allPartyMembers[i].BaseHealth;
                newPartyMember.MaxHealth = newPartyMember.CurrHealth;
                newPartyMember.Strength = allPartyMembers[i].BaseStrength;
                newPartyMember.Intelligence = allPartyMembers[i].BaseIntelligence;

                currentParty.Add(newPartyMember);
            }
        }
    }

    public List<PartyMember> GetCurrentParty()
    {
        List<PartyMember> partyMembers = new List<PartyMember>();
        partyMembers = currentParty;
        for (int i = 0; i < partyMembers.Count; i++)
        {
            if (partyMembers[i].CurrHealth <= 0)
            {
                partyMembers.RemoveAt(i);
            }
        }
        return partyMembers;
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
