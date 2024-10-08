using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="PlayerData", fileName ="PlayerData")]
public class PlayerData : ScriptableObject
{
    public string playerName;
    public int highestScore;
    public int itemAddTimeCount;
    public int multiScoreCount;
}
    