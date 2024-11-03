using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [Range(0, 50)] public float _snakeSpeed;


    [Header("Body Configuration")]
    public GameObject _snakeSegments;
    public int _initialSegmentCount;

    [Header("Player Movement Configuration")]
    [SerializeField] KeyCode upKey;
    [SerializeField] KeyCode downKey;
    [SerializeField] KeyCode leftKey;
    [SerializeField] KeyCode rightKey;
}