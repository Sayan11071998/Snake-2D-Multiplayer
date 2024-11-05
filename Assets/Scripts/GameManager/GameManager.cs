using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public bool _isGameOver;

    private BoxCollider2D m_collider;
    void Awake()
    {
        _isGameOver = false;
        instance = this;
        m_collider = GetComponent<BoxCollider2D>();

        SetBoundsFromCollider();
    }

    private void SetBoundsFromCollider()
    {
        Vector2 offset = m_collider.offset;
        Vector2 size = m_collider.size;
        Vector2 position = transform.position;

        Bounds.maxX = position.x + (size.x / 2) + offset.x;
        Bounds.minX = position.x - (size.x / 2) + offset.x;

        Bounds.maxY = position.y + (size.y / 2) + offset.y;
        Bounds.minY = position.y - (size.y / 2) + offset.y;
    }

    public void GameOver()
    {
        _isGameOver = true;

        ItemSpwanner.Instance.GameOver();
        PowerUpManager.powerUpInstance.GameOver();
    }

    // public void HandleGameOver(float player1Score, float player2Score)
    // {
    //     if (player1Score <= 0 && player2Score <= 0)
    //     {
    //         // Both players are out of score, handle draw
    //         UIManager.Instance.Draw();
    //         Debug.Log("It's a draw!");
    //     }
    //     else if (player1Score <= 0)
    //     {
    //         // Player 1 loses
    //         UIManager.Instance.GameOver(Players.Alpha); // Adjust as needed
    //         Debug.Log("Player 2 wins!");
    //     }
    //     else if (player2Score <= 0)
    //     {
    //         // Player 2 loses
    //         UIManager.Instance.GameOver(Players.Beta); // Adjust as needed
    //         Debug.Log("Player 1 wins!");
    //     }
    // }


    public void Draw()
    {
        _isGameOver = true;
    }
}