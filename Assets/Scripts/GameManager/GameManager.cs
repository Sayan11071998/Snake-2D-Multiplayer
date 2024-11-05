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

        StopPlayerMovements();
        DestroyAllSnakes();

        ItemSpwanner.Instance.GameOver();
        PowerUpManager.powerUpInstance.GameOver();
    }

    private void StopPlayerMovements()
    {
        SnakeController[] snakeControllers = FindObjectsOfType<SnakeController>();
        foreach (var snakeController in snakeControllers)
            snakeController.enabled = false;
    }

    private void DestroyAllSnakes()
    {
        SnakeController[] snakeControllers = FindObjectsOfType<SnakeController>();
        foreach (var snakeController in snakeControllers)
            Destroy(snakeController.gameObject);
    }

    public void Draw()
    {
        _isGameOver = true;
    }
}