using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    private static ItemSpawner instance;
    public static ItemSpawner Instance { get { return instance; } }

    [Header("Fruit Item")]
    public Transform fruitPrefab;
    public int fruitValue;
    public float fruitSpawnInterval;
    public float fruitScore;

    [Header("Poison Iteam")]
    public Transform poisonPrefab;
    public int poisonValue;
    public float poisonSpawnInterval;
    public float poisonEffectTime;
    public float poisonScore;

    private float[] _spawnTimer = new float[2];
    private bool _isPosionEnable = false;

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

        IntitializeFruit();
    }

    void Update()
    {
        if (GameManager.Instance._isGameOver)
            return;

        SpawnItems();
    }

    private void IntitializeFruit()
    {
        fruitPrefab = Instantiate(fruitPrefab.gameObject).transform;
        fruitPrefab.GetComponent<BoxCollider2D>().enabled = true;
        fruitPrefab.parent = transform;
    }

    private void SpawnItems()
    {
        if (_spawnTimer[0] > fruitSpawnInterval)
            SpawnNextFruit();
        _spawnTimer[0] += Time.deltaTime;

        if (!_isPosionEnable)
            return;

        if (_spawnTimer[1] > poisonSpawnInterval)
            SpawnNextPoison();
        _spawnTimer[1] += Time.deltaTime;
    }

    public void SpawnNextFruit()
    {
        Vector3 newPos = GetRandomPosition();
        fruitPrefab.position = newPos;
        _spawnTimer[0] = 0;
    }

    private void SpawnNextPoison()
    {
        Vector3 newPosition = GetRandomPosition();
        GameObject poisonInstance = Instantiate(poisonPrefab, newPosition, Quaternion.identity).gameObject;
        poisonInstance.transform.parent = transform;
        Destroy(poisonInstance, poisonEffectTime);
        _spawnTimer[1] = 0;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 pos;
        pos.x = Mathf.Round(Random.Range(Bounds.minX, Bounds.maxX));
        pos.y = Mathf.Round(Random.Range(Bounds.minY, Bounds.maxY));
        pos.z = 0;
        return pos;
    }

    public int SnakeAteFruit()
    {
        SpawnNextFruit();
        return fruitValue;
    }

    public int SnakeAtePoison()
    {
        SpawnNextPoison();
        return poisonValue;
    }

    public void PoisonActivation(bool value)
    {
        poisonPrefab.gameObject.SetActive(value);
        _isPosionEnable = value;
    }

    public void GameOver()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
