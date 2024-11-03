using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    public bool _isGameOver;

    private BoxCollider2D _boxCollider2D;
    void Awake()
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

        _isGameOver = false;
        _boxCollider2D = GetComponent<BoxCollider2D>();

        SetBounds();
        DisplayBounds();
    }

    private void SetBounds()
    {
        Vector2 offset = _boxCollider2D.offset;
        Vector2 size = _boxCollider2D.size;
        Vector2 pos = transform.position;

        Bounds.maxX = pos.x + (size.x / 2) + offset.x;
        Bounds.minX = pos.x - (size.x / 2) + offset.x;

        Bounds.maxY = pos.y + (size.y / 2) + offset.y;
        Bounds.minY = pos.y - (size.y / 2) + offset.y;
    }

    private void DisplayBounds()
    {
        Debug.Log("Bounds maxX : " + Bounds.maxX);
        Debug.Log("Bounds minX : " + Bounds.minX);
        Debug.Log("Bounds maxY : " + Bounds.maxY);
        Debug.Log("Bounds minY : " + Bounds.minY);
    }

    public void GameOver()
    {
        _isGameOver = true;
        ItemSpawner.Instance.GameOver();
        PowerUpManager.Instance.GameOver();
    }

    public void Draw()
    {
        _isGameOver = true;
    }
}
