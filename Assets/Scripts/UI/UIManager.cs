using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    private static UIManager s_UiInstance;
    public static UIManager UiInstance { get { return s_UiInstance; } }


    [Header("Player")]
    public Image[] shield;
    public Image[] score;
    public Image[] speed;
    public TMP_Text[] player1Score;
    //public Text player2Score;

    [Space]
    public GameObject GameOverPanel;
    public TMP_Text gameOverText;

    private Color on = new Color(1f,1f,1f,1f);
    private Color off = new Color(1f,1f,1f,0.5f);

    void Awake()
    {
        s_UiInstance = this;
        GameOverPanel.SetActive(false);
        InstantiatePlayerUI();
    }

    private void InstantiatePlayerUI()
    {
        player1Score[0].text = "Score : " + 0;
        player1Score[1].text = "Score : " + 0;
        DeactivateALlPowerUp();
    }

    public void SetScoreUI(Players player, float Value)
    {
        int index = (int)player;
        player1Score[index].text = "Score : " + Value;
    }

    public void PowerUp(Players player,PowerUps power, bool active)
    {
        int index = (int)player;
        if(power == PowerUps.shield)
            shield[index].color = (active)?on:off;
        else if(power == PowerUps.scoreUp)
            score[index].color = (active)?on:off;
        else if(power == PowerUps.speedUp)
            speed[index].color = (active)?on:off;
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

    public void GameOver(Players player)
	{
        GameOverPanel.SetActive(true);
        if(player == Players.Alpha)
		{
            GameOverPanel.GetComponent<Image>().color = new Color(1f, 0f, 0f, 0.3f);
            gameOverText.text = "Player 2 Wins";
		}
		else
		{
            GameOverPanel.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.3f);
            gameOverText.text = "Player 1 Wins";
        }
	}

    public void Draw()
	{
        GameOverPanel.SetActive(true);
        GameOverPanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.3f);
        gameOverText.text = "DRAW";
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