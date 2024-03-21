using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] static int score;

    [SerializeField] TMP_Text scoreTxt;



    public static void IncreaseScore(int totalGold)
    {
        score += totalGold;
        
    }

    void Update()
    {
        scoreTxt.text = "Score: " + score;

    }
}
