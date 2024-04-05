using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Party Member")]
public class PartyMemberInfo : ScriptableObject
{
    public string MemberName;
    public GameObject MemberOverworldVisualPrefab;
    public GameObject MemberBattleVisualPrefab;

    public int StartLevel;
    public int BaseHealth;
    public int BaseStrength;
    public int BaseIntelligence;
}
