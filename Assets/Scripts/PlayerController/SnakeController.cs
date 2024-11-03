using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class SnakeController : MonoBehaviour
{
    [Range(0, 50)] public float _snakeSpeed;
    public Player player;
    public GameObject UI;

    [Header("Body Configuration")]
    public GameObject _snakeSegment;
    public int _initialSegmentCount;

    [Header("Player Movement Configuration")]
    [SerializeField] KeyCode upKey;
    [SerializeField] KeyCode downKey;
    [SerializeField] KeyCode leftKey;
    [SerializeField] KeyCode rightKey;

    private Vector3 _direction;
    private Rigidbody2D _rigidBody2d;
    private AudioContoller _audio;
    private List<Transform> _segments;
    private float _moveTimer = 0;
    private bool _isVertical, _isPaused, _isImmune;
    private bool[] _powerUp = new bool[3];
    private float[] _powerUpTimer = new float[3];
    private float _score;

    private void Start()
    {
        _rigidBody2d = GetComponent<Rigidbody2D>();
        _audio = GetComponent<AudioContoller>();
        _rigidBody2d.bodyType = RigidbodyType2D.Kinematic;
        _isPaused = false;

        InitializeBodySegments();
        InitializePowerUp();
    }

    private void Update()
    {
        // if (_isPaused || GameManagerDependencyInfo.ManagerInstance.isGameOver)
        if (_isPaused || GameManager.Instance._isGameOver)
            return;

        GetSnakeDirection();
        MoveSnake();
        UpdatePowerUpTimer();
    }

    private void InitializeBodySegments()
    {
        _direction = Vector3.right;

        if (player == Player.Beta)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            _direction = Vector3.left;
        }

        transform.position = transform.parent.position;
        _segments = new List<Transform>();
        StartCoroutine(SetImmunity(0.5f));
        _segments.Add(transform);

        for (int i = 0; i < _initialSegmentCount; i++)
        {
            AddNewSegments();
        }
    }

    private IEnumerator SetImmunity(float timer)
    {
        _isImmune = true;
        yield return new WaitForSeconds(timer);
        _isImmune = false;
    }

    private void AddNewSegments()
    {
        int snakeSegmentCount = _segments.Count;

        Vector3 snakeTail = _segments[snakeSegmentCount - 1].position;
        GameObject newSnakePart = Instantiate<GameObject>(_snakeSegment, snakeTail, Quaternion.identity);
        newSnakePart.name = string.Format("body {0}", snakeSegmentCount);

        _segments.Add(newSnakePart.transform);
        newSnakePart.transform.parent = transform.parent;

        if (player == Player.Beta)
            newSnakePart.GetComponent<SpriteRenderer>().color = Color.red;

        SetScale();
    }

    private void SetScale()
    {
        int snakeSegmentCount = _segments.Count;
        float decrementAmout = 0.3f;

        for (int i = 1; i < 5; i++)
        {
            if (snakeSegmentCount - i == 1) return;

            Vector3 scale = _segments[snakeSegmentCount - 1].localScale;
            scale.x = 0.7f - decrementAmout;
            scale.y = 0.7f - decrementAmout;

            _segments[snakeSegmentCount - 1].localScale = scale;
            decrementAmout -= 0.1f;
        }
    }

    private void InitializePowerUp()
    {
        for (int i = 0; i < 3; i++)
        {
            _powerUp[i] = false;
            _powerUpTimer[i] = 0;
        }
    }

    private void GetSnakeDirection()
    {
        if (Input.GetKeyDown(upKey) && !_isVertical)
            _direction = Vector3.up;
        else if (Input.GetKeyDown(downKey) && !_isVertical)
            _direction = Vector3.down;
        else if (Input.GetKeyDown(leftKey) && _isVertical)
            _direction = Vector3.left;
        else if (Input.GetKeyDown(rightKey) && _isVertical)
            _direction = Vector3.right;
    }

    private void MoveSnake()
    {
        float effectiveSnakeSpeed = _snakeSpeed * ((_powerUp[(int)PowerUpTypes.speedUp]) ? 3 : 1);

        if (_moveTimer > 1 / effectiveSnakeSpeed)
        {
            MoveSnakeBody();
            MoveSnakeHead();
            _moveTimer = 0;
        }

        _moveTimer += Time.deltaTime;
    }

    private void MoveSnakeBody()
    {
        int snakeSegmentCount = _segments.Count;

        for (int i = snakeSegmentCount - 1; i > 0; i--)
        {
            _segments[i].position = _segments[i - 1].position;
        }
    }

    private void MoveSnakeHead()
    {
        Vector3 position = transform.position;
        position += _direction;

        CheckBoundary(ref position);

        transform.position = position;

        if (_direction.x == 0)
            _isVertical = true;
        else
            _isVertical = false;
    }

    private void CheckBoundary(ref Vector3 position)
    {
        if (position.x > Bounds.maxX || position.x < Bounds.minX)
            position.x = position.x > 0 ? Bounds.minX : Bounds.maxX;
        else if (position.y > Bounds.maxY || position.y > Bounds.minY)
            position.y = position.y > 0 ? Bounds.minY : Bounds.maxY;
    }

    private void UpdatePowerUpTimer()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_powerUp[i])
                _powerUpTimer[i] += Time.deltaTime;
            else
                continue;

            float powerUpTimePeriod = PowerUpManager.Instance.getPowerUpPeriod((PowerUpTypes)i);

            if (_powerUpTimer[i] > powerUpTimePeriod)
            {
                _powerUp[i] = false;
                UIManager.Instance.PowerUp(player, (PowerUpTypes)i, false);
                _powerUpTimer[i] = 0;
            }
        }
    }

    IEnumerator DeathAnimation()
    {
        _isPaused = true;
        float waitTime = 0.1f;

        for (int i = _segments.Count - 1; i > 0; i--)
        {
            Destroy(_segments[i].gameObject, waitTime);
            waitTime += 0.05f;
        }

        yield return new WaitForSeconds(waitTime);

        _segments.Clear();
        UIManager.Instance.GameOver(player);
        Destroy(gameObject);
    }

    private void DestoryLastBody()
    {
        Destroy(_segments[_segments.Count - 1].gameObject);
        _segments.RemoveAt(_segments.Count - 1);
        SetScale();
    }

    private void UpdateScore(float fruitScore)
    {
        _score += fruitScore;
        UIManager.Instance.SetScoreUI(player, _score);
    }

    private void AteFruit()
    {
        _audio.Play(Sounds.Eat);
        int count = ItemSpawner.Instance.SnakeAteFruit() * ((_powerUp[(int)PowerUpTypes.scoreUp]) ? 2 : 1);

        for (int i = 0; i < count; i++)
            AddNewSegments();

        if (_segments.Count > 3)
            ItemSpawner.Instance.PoisonActivation(true);

        UpdateScore(ItemSpawner.Instance.fruitScore * ((_powerUp[(int)PowerUpTypes.scoreUp]) ? 2 : 1));
    }

    private void AtePoison()
    {
        _audio.Play(Sounds.Poison);

        int count = ItemSpawner.Instance.SnakeAtePoison();

        if (_segments.Count < count + 1)
        {
            _audio.Play(Sounds.Death);
            StartCoroutine(DeathAnimation());
            GameManager.Instance.GameOver();
        }

        for (int i = 0; i < count; i++)
            DestoryLastBody();

        if (_segments.Count < 3)
            ItemSpawner.Instance.PoisonActivation(false);

        UpdateScore(-ItemSpawner.Instance.poisonScore);
    }

    private void AteBody()
    {
        if (_isImmune)
            return;

        if (_powerUp[(int)PowerUpTypes.shield])
        {
            _powerUp[(int)PowerUpTypes.shield] = false;
            UIManager.Instance.PowerUp(player, PowerUpTypes.shield, false);
            StartCoroutine(SetImmunity(1));
            return;
        }

        _audio.Play(Sounds.Death);
        Debug.Log("Player Dead");
        StartCoroutine(DeathAnimation());
        GameManager.Instance.GameOver();
    }

    private void AteHead()
    {
        GameManager.Instance.Draw();
        UIManager.Instance.Draw();
    }

    public void ActivatePowerUp(PowerUpTypes power, GameObject powerObject)
    {
        _audio.Play(Sounds.Eat);
        Destroy(powerObject);
        UIManager.Instance.PowerUp(player, power, true);
        _powerUp[(int)power] = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fruit"))
        {
            AteFruit();
            return;
        }

        if (other.CompareTag("Poison"))
        {
            Destroy(other.gameObject);
            AtePoison();
            return;
        }

        if (other.CompareTag("Head"))
        {
            AteHead();
            return;
        }

        if (other.CompareTag("Body"))
        {
            AteBody();
            return;
        }

        if (other.CompareTag("Shield"))
            ActivatePowerUp(PowerUpTypes.shield, other.gameObject);
        else if (other.CompareTag("ScoreUp"))
            ActivatePowerUp(PowerUpTypes.scoreUp, other.gameObject);
        else if (other.CompareTag("SpeedUp"))
            ActivatePowerUp(PowerUpTypes.speedUp, other.gameObject);
    }
}