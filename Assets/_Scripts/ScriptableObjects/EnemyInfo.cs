using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Enemy")]
public class EnemyInfo : ScriptableObject
{
    public string EnemyName;
    public GameObject EnemyVisualPrefab;

    public int BaseHealth;
    public int BaseStrength;
    public int BaseIntelligence;
}