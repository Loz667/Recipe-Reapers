using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy")]
public class EnemyInfo : ScriptableObject
{
    public string EnemyName;
    public GameObject EnemyVisualPrefab;
    public int BaseHunger;
    public int BaseAnger;
    public int BaseIntelligence;
}
