using System;
using System.Collections;
using System.Collections.Generic;
using Alteruna;
using Alteruna.Scoreboard;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBoardRow : MonoBehaviour
{
    public ushort ID { get; private set; }
    public string Name { get; private set; }
    public int Score { get; private set; }
    public int Kills { get; private set; }
    public int Deaths { get; private set; }


    [Tooltip("Reference to the name text-object")]
    public TMP_Text nameText;
    [Tooltip("Reference to the score text-object")]
    public TMP_Text scoreText;
    [Tooltip("Reference to the kills text-object")]
    public TMP_Text killsText;
    [Tooltip("Reference to the deaths text-object")]
    public TMP_Text deathsText;


    public void Initialize(ushort userID, string userName, bool resetStats = true)
    {
        ID = userID;
        Name = userName;
        nameText.text = Name;

        if (resetStats)
        {
            Score = 0;
            Kills = 0;
            Deaths = 0;
        }
    }

    public void UpdateScore(IScoreObject scoreObject)
    {
        object scoreObjValue = scoreObject.Get(ID);

        switch (scoreObject.Key)
        {
            case "Score":
                Score = (int)scoreObjValue;
                scoreText.text = scoreObjValue.ToString();
                break;
            case "Kills":
                Kills = (int)scoreObjValue;
                killsText.text = scoreObjValue.ToString();
                break;
            case "Deaths":
                Deaths = (int)scoreObjValue;
                deathsText.text = scoreObjValue.ToString();
                break;
            default:
                break;
        }
    }
}
