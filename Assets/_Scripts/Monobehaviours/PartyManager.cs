using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyManager : MonoBehaviour
{
    [SerializeField] private PartyMemberInfo[] allMembers;
    [SerializeField] private List<PartyMember> currentParty;

    [SerializeField] private PartyMemberInfo defaultPartyMember;

    private void Awake()
    {
        AddMemberToPartyByName(defaultPartyMember.MemberName);
    }

    public void AddMemberToPartyByName(string memberName)
    {
        for (int i = 0; i < allMembers.Length; i++)
        {
            if (allMembers[i].MemberName == memberName)
            {
                PartyMember newPartyMember = new PartyMember();

                newPartyMember.MemberName = allMembers[i].MemberName;
                newPartyMember.MemberOverworldVisualPrefab = allMembers[i].MemberOverworldVisualPrefab;
                newPartyMember.MemberBattleVisualPrefab = allMembers[i].MemberBattleVisualPrefab;
                newPartyMember.Level = allMembers[i].StartLevel;
                newPartyMember.CurrHealth = allMembers[i].BaseHealth;
                newPartyMember.MaxHealth = newPartyMember.CurrHealth;
                newPartyMember.Strength = allMembers[i].BaseStrength;
                newPartyMember.Intelligence = allMembers[i].BaseIntelligence;

                currentParty.Add(newPartyMember);
            }
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
