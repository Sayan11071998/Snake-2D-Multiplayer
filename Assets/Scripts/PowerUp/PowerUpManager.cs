using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerUps
{
    shield = 0,
    scoreUp = 1,
    speedUp = 2
}

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager s_PowerUpInstance;
    public static PowerUpManager powerUpInstance { get { return s_PowerUpInstance; } }

    [Header("Shield")]
    public GameObject shield;
    public float shieldPeriod;

    [Header("Score UP")]
    public GameObject scoreUp;
    public float scorePeriod;

    [Header("Speed UP")]
    public GameObject speedUp;
    public float speedPeriod;

    [Header("Other")]
    public float SpawnInterval;
    public float timeout;
    public float aliveTime;

    private float m_Timer;

    void Awake()
    {
        s_PowerUpInstance = this;
    }

    void Update()
    {
        if (GameManager.ManagerInstance.isGameOver)
            return;

        if (m_Timer > SpawnInterval)
            SpawnRandomPowerUp();
        m_Timer += Time.deltaTime;
    }

    private void SpawnRandomPowerUp()
    {
        int index = Random.Range(0, 3);
        GameObject newPower = (index == 0) ? shield : (index == 2) ? scoreUp : speedUp;

        newPower = Instantiate<GameObject>(newPower);
        newPower.transform.position = GetRandomPos();
        newPower.transform.parent = transform;
        Destroy(newPower, aliveTime);
        m_Timer = 0;
    }

    private Vector3 GetRandomPos()
    {
        Vector3 pos;
        pos.x = Mathf.Round(Random.Range(Bounds.minX, Bounds.maxX));
        pos.y = Mathf.Round(Random.Range(Bounds.minY, Bounds.maxY));
        pos.z = 0;
        return pos;
    }

    public float getPowerUpPeriod(PowerUps power)
    {
        if (PowerUps.shield == power)
            return shieldPeriod;
        else if (PowerUps.scoreUp == power)
            return scorePeriod;
        else
            return speedPeriod;
    }

    public void GameOver()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}