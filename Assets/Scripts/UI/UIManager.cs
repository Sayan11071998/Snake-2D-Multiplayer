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

    private Color on = new Color(1f,1f,1f,1f);
    private Color off = new Color(1f,1f,1f,0.5f);

    void Awake()
    {
        instance = this;
        GameOverPanel.SetActive(false);
        InstantiatePlayerUI();
    }

    private void InstantiatePlayerUI()
    {
        playerScore[0].text = "Score : " + 0;
        playerScore[1].text = "Score : " + 0;
        DeactivateALlPowerUp();
    }

    public void SetScoreUI(Players player, float Value)
    {
        int index = (int)player;
        playerScore[index].text = "Score : " + Value;
    }

    public void PowerUp(Players player,PowerUpsItemTypes power, bool active)
    {
        int index = (int)player;
        if(power == PowerUpsItemTypes.shield)
            shield[index].color = (active)?on:off;
        else if(power == PowerUpsItemTypes.scoreUp)
            score[index].color = (active)?on:off;
        else if(power == PowerUpsItemTypes.speedUp)
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
            GameOverText.text = "Player 2 Wins";
		}
		else
		{
            GameOverPanel.GetComponent<Image>().color = new Color(0f, 1f, 0f, 0.3f);
            GameOverText.text = "Player 1 Wins";
        }
	}

    public void Draw()
	{
        GameOverPanel.SetActive(true);
        GameOverPanel.GetComponent<Image>().color = new Color(0f, 0f, 1f, 0.3f);
        GameOverText.text = "DRAW";
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