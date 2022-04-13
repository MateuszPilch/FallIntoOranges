using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    private int currentScore;
    public GameObject[] scoreTexts; //[0] - highScore, [1] - failScore, [2] - currentScore
    public int GetScore()
    {
        return currentScore;
    }
    public void SetScore(int newScoreValue)
    {
        currentScore = newScoreValue;
    }
    public void AddScore()
    {
        currentScore++;
        UpdateScoreText(2,currentScore);

        if(PlayerPrefs.GetInt("HighScore") < currentScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
        }
    }
    public void UpdateScoreState(int scoreID,bool scoreState)
    {
        scoreTexts[scoreID].SetActive(scoreState);
    }

    public void UpdateScoreText(int scoreID,int scoreValue)
    {
        scoreTexts[scoreID].GetComponent<Text>().text = scoreValue.ToString();
    }

    public void ClearScore()
    {
        currentScore = 0;
        UpdateScoreText(2, currentScore);

    }
}
