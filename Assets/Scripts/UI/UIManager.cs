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

    [Header("GameOver Panel")]
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;

    private Color on = new Color(1f, 1f, 1f, 1f);
    private Color off = new Color(1f, 1f, 1f, 0.5f);

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        gameOverPanel.SetActive(false);

        InstantiatePlayerUI();
    }

    private void InstantiatePlayerUI()
    {
        playerScore[0].text = "Player 1 Score: " + 0;
        playerScore[1].text = "Player 2 Score: " + 0;

        DeactivateAllPowerUp();
    }

    public void SetScoreUI(Player player, float value)
    {
        int playerIndex = (int)player;

        if (playerIndex == 0)
            playerScore[playerIndex].text = "Player 1 Score: " + value;
        else
            playerScore[playerIndex].text = "Player 2 Score: " + value;
    }

    public void PowerUp(Player player, PowerUps power, bool active)
    {
        int playerIndex = (int)player;

        if (power == PowerUps.shield)
            shield[playerIndex].color = (active) ? on : off;
        else if (power == PowerUps.scoreUp)
            score[playerIndex].color = (active) ? on : off;
        else if (power == PowerUps.speedUp)
            speed[playerIndex].color = (active) ? on : off;
    }

    public void DeactivateAllPowerUp()
    {
        for (int i = 0; i < 2; i++)
        {
            shield[i].color = off;
            score[i].color = off;
            speed[i].color = off;
        }
    }

    public void GameOver(Player player)
    {
        gameOverPanel.SetActive(true);

        if (player == Player.Alpha)
        {
            gameOverPanel.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            gameOverText.text = "Player 2 Wins!!";
        }
        else
        {
            gameOverPanel.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.3f);
            gameOverText.text = "Player 1 Wins!!";
        }
    }

    public void Draw()
    {
        gameOverPanel.SetActive(true);
        gameOverPanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.3f);
        gameOverText.text = "DRAW!!";
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
