using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance { get { return instance; } }


    [Header("Player Details")]
    public Image[] shield;
    public Image[] score;
    public Image[] speed;
    public TMP_Text[] playerScore;

    [Space]
    public GameObject GameOverPanel;
    public TMP_Text GameOverText;

    private Color on = new Color(1f, 1f, 1f, 1f);
    private Color off = new Color(1f, 1f, 1f, 0.5f);

    void Awake()
    {
        instance = this;
        GameOverPanel.SetActive(false);
        InstantiatePlayerUI();
    }

    private void InstantiatePlayerUI()
    {
        playerScore[0].text = "Score: " + 0;
        playerScore[1].text = "Score: " + 0;

        DeactivateALlPowerUp();
    }

    public void SetScoreUI(Players player, float Value)
    {
        int index = (int)player;
        float newScore = Mathf.Max(0, Value);
        playerScore[index].text = "Score : " + newScore;
    }

    public void PowerUp(Players player, PowerUpsItemTypes power, bool active)
    {
        int index = (int)player;
        if (power == PowerUpsItemTypes.shield)
            shield[index].color = (active) ? on : off;
        else if (power == PowerUpsItemTypes.scoreUp)
            score[index].color = (active) ? on : off;
        else if (power == PowerUpsItemTypes.speedUp)
            speed[index].color = (active) ? on : off;
    }

    public void DeactivateALlPowerUp()
    {
        for (int i = 0; i < 2; i++)
        {
            shield[i].color = off;
            score[i].color = off;
            speed[i].color = off;
        }
    }

    public void GameOver(Players winningPlayer)
    {
        GameOverPanel.SetActive(true);

        if (winningPlayer == Players.Alpha)
        {
            GameManager.Instance.GameOver();
            GameOverPanel.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            GameOverText.text = "Player 1 Wins!";
        }
        else
        {
            GameManager.Instance.GameOver();
            GameOverPanel.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.3f);
            GameOverText.text = "Player 2 Wins!";
        }
    }

    public void Draw()
    {
        GameManager.Instance.GameOver();
        GameOverPanel.SetActive(true);
        GameOverPanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.3f);
        GameOverText.text = "It's a DRAW!";
    }

    public void CheckGameOver(float scorePlayer1, float scorePlayer2)
    {
        if (scorePlayer1 == 0 || scorePlayer2 == 0)
        {
            if (scorePlayer1 > scorePlayer2)
                GameOver(Players.Alpha);
            else if (scorePlayer2 > scorePlayer1)
                GameOver(Players.Beta);
            else
                Draw();
        }
    }

    public void GetScores(out float scorePlayer1, out float scorePlayer2)
    {
        scorePlayer1 = float.Parse(playerScore[0].text.Split(':')[1].Trim());
        scorePlayer2 = float.Parse(playerScore[1].text.Split(':')[1].Trim());
    }

    public void RestartGame()
    {
        // SceneManager.LoadScene(1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}