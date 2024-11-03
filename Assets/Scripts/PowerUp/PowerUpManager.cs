using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager instance;
    public static PowerUpManager Instance { get { return instance; } }

    [Header("Shield")]
    public GameObject shield;
    public float shieldDuration;

    [Header("Score Boost")]
    public GameObject scoreBoost;
    public float scoreDuration;

    [Header("Speed UP")]
    public GameObject speedUp;
    public float speedUpDuration;

    [Header("Power Ups Details")]
    public float spawnInterval;
    public float timeout;
    public float activeTime;

    private float _timer;

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
    }

    void Update()
    {
        if (GameManager.Instance._isGameOver)
            return;

        if (_timer > spawnInterval)
            SpawnRandomPowerUps();
        _timer += Time.deltaTime;
    }

    private void SpawnRandomPowerUps()
    {
        int index = Random.Range(0, 3);
        GameObject newPower = (index == 0) ? shield : (index == 2) ? scoreBoost : speedUp;

        newPower = Instantiate<GameObject>(newPower);
        newPower.transform.position = GetRandomPosition();
        newPower.transform.parent = transform;
        Destroy(newPower, activeTime);
        _timer = 0;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 position;
        position.x = Mathf.Round(Random.Range(Bounds.minX, Bounds.maxX));
        position.y = Mathf.Round(Random.Range(Bounds.minY, Bounds.maxY));
        position.z = 0;
        return position;
    }

    public float getPowerUpPeriod(PowerUpTypes power)
    {
        if (PowerUpTypes.shield == power)
            return shieldDuration;
        else if (PowerUpTypes.scoreUp == power)
            return scoreDuration;
        else
            return speedUpDuration;
    }

    public void GameOver()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}

public enum PowerUpTypes
{
    shield,
    scoreUp,
    speedUp
}