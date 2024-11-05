using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpwanner : MonoBehaviour
{
    private static ItemSpwanner instance;
    public static ItemSpwanner Instance { get { return instance; } }

    [Header("Fruit Iteam Details")]
    public Transform fruitPrefab;
    public int fruitValue;
    public float fruitSpawnInterval;
    public float fruitScore;

    [Header("Poison Iteam Details")]
    public Transform poisonPrefab;
    public int poisonValue;
    public float poisonSpawnInterval;
    public float poisonStayTime;
    public float poisonScore;

    private float[] _spawnTimer = new float[2];
    private bool _isPosionEnable = false;

    void Awake()
    {
        instance = this;
        
        IntitializeFruit();
    }

    void Update()
    {
        if (GameManager.Instance._isGameOver)
            return;

        Spawn();
    }

    private void IntitializeFruit()
    {
        fruitPrefab = Instantiate(fruitPrefab.gameObject).transform;
        fruitPrefab.GetComponent<BoxCollider2D>().enabled = true;
        fruitPrefab.parent = transform;
    }

    private void Spawn()
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

    public int SnakeAtePoison()
    {
        SpawnNextPoison();
        return poisonValue;
    }

    private void SpawnNextPoison()
    {
        Vector3 newSpawnPosition = GetRandomPosition();
        
        GameObject poisonInstance = Instantiate(poisonPrefab, newSpawnPosition, Quaternion.identity).gameObject;
        poisonInstance.transform.parent = transform;
        
        Destroy(poisonInstance, poisonStayTime);
        _spawnTimer[1] = 0;
    }

    public int SnakeAteFruit()
    {
        SpawnNextFruit();
        return fruitValue;
    }

    public void SpawnNextFruit()
    {
        Vector3 nextFruitSpawnPosition = GetRandomPosition();
        fruitPrefab.position = nextFruitSpawnPosition;
        _spawnTimer[0] = 0;
    }

    public void PoisonActivation(bool value)
    {
        poisonPrefab.gameObject.SetActive(value);
        _isPosionEnable = value;
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 position;
        position.x = Mathf.Round(Random.Range(Bounds.minX, Bounds.maxX));
        position.y = Mathf.Round(Random.Range(Bounds.minY, Bounds.maxY));
        position.z = 0;
        return position;
    }

    public void GameOver()
    {
        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);
    }
}