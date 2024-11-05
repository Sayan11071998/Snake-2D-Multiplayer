using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager instance;
    public static PowerUpManager powerUpInstance { get { return instance; } }

    [Header("Shield Details")]
    public GameObject shield;
    public float shieldDuration;

    [Header("Score UP Details")]
    public GameObject scoreUp;
    public float scoreUpDuration;

    [Header("Speed UP Details")]
    public GameObject speedUp;
    public float speedUpDuration;

    [Header("General Details")]
    public float spawnInterval;
    public float timeout;
    public float aliveTime;

    private float _timer;

    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (GameManager.Instance._isGameOver)
            return;

        if (_timer > spawnInterval)
            SpawnRandomPowerUpItems();

        _timer += Time.deltaTime;
    }

    private void SpawnRandomPowerUpItems()
    {
        int _powerItemIndex = Random.Range(0, 3);

        GameObject newPowerUpItem = (_powerItemIndex == 0) ? shield : (_powerItemIndex == 2) ? scoreUp : speedUp;
        newPowerUpItem = Instantiate<GameObject>(newPowerUpItem);
        newPowerUpItem.transform.position = GetRandomPosition();
        newPowerUpItem.transform.parent = transform;

        Destroy(newPowerUpItem, aliveTime);

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

    public float GetPowerUpDuration(PowerUpsItemTypes power)
    {
        if (PowerUpsItemTypes.shield == power)
            return shieldDuration;
        else if (PowerUpsItemTypes.scoreUp == power)
            return scoreUpDuration;
        else
            return speedUpDuration;
    }

    public void GameOver()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }
}

public enum PowerUpsItemTypes
{
    shield = 0,
    scoreUp = 1,
    speedUp = 2
}