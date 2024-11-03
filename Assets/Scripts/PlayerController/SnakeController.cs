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
    private AudioController _audio;
    private List<Transform> _segments;
    private float _moveTimer = 0;
    private bool _isVertical, _isPaused, _isImmune;
    private bool[] _powerUp = new bool[3];
    private float[] _powerUpTimer = new float[3];
    private float _score;

    private void Start()
    {
        _rigidBody2d = GetComponent<Rigidbody2D>();
        _audio = GetComponent<AudioController>();
        _rigidBody2d.bodyType = RigidbodyType2D.Kinematic;
        _isPaused = false;

        InitializeBodySegments();
        InitializePowerUp();
    }

    private void Update()
    {
        if (_isPaused || GameManagerDependencyInfo.ManagerInstance.isGameOver)
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
        float effectiveSnakeSpeed = _snakeSpeed * ((_powerUp[(int)PowerUps.speedUp]) ? 3 : 1);

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

            float powerUpTimePeriod = PowerUpManager.powerUpInstance.getPowerUpPeriod((PowerUps)i);

            if (_powerUpTimer[i] > powerUpTimePeriod)
            {
                _powerUp[i] = false;
                UIManager.UiInstance.PowerUp(player, (PowerUps)i, false);
                _powerUpTimer[i] = 0;
            }
        }
    }

    // IEnumerator DeathAnimation()
    // {
    //     m_Paused = true;
    //     float waitTime = 0.1f;
    //     for (int i = m_body.Count - 1; i > 0; i--)
    //     {
    //         Destroy(m_body[i].gameObject, waitTime);
    //         waitTime += 0.05f;
    //     }
    //     yield return new WaitForSeconds(waitTime);
    //     m_body.Clear();
    //     UIManager.UiInstance.GameOver(player);
    //     Destroy(this.gameObject);
    // }

    // private void DestoryLastBody()
    // {
    //     Destroy(m_body[m_body.Count - 1].gameObject);
    //     m_body.RemoveAt(m_body.Count - 1);
    //     SetScale();
    // }

    // private void UpdateScore(float fruitScore)
    // {
    //     m_score += fruitScore;
    //     UIManager.UiInstance.SetScoreUI(player, m_score);
    // }

    // private void AteFruit()
    // {
    //     m_Audio.Play(Sounds.Eat);
    //     int count = FruitSpwanner.FruitInstance.SnakeAteFruit() * ((m_PowerUp[(int)PowerUps.scoreUp]) ? 2 : 1);
    //     for (int i = 0; i < count; i++)
    //     {
    //         AddNewBodyPart();
    //     }
    //     if (m_body.Count > 3)
    //         FruitSpwanner.FruitInstance.PoisonActivation(true);

    //     UpdateScore(FruitSpwanner.FruitInstance.fruitScore * ((m_PowerUp[(int)PowerUps.scoreUp]) ? 2 : 1));
    // }

    // private void AtePoison()
    // {
    //     m_Audio.Play(Sounds.Poison);
    //     int count = FruitSpwanner.FruitInstance.SnakeAtePoison();
    //     if (m_body.Count < count + 1)
    //     {
    //         m_Audio.Play(Sounds.Death);
    //         StartCoroutine(DeathAnimation());
    //         GameManager.ManagerInstance.GameOver();
    //     }
    //     for (int i = 0; i < count; i++)
    //     {
    //         DestoryLastBody();
    //     }
    //     if (m_body.Count < 3)
    //         FruitSpwanner.FruitInstance.PoisonActivation(false);

    //     UpdateScore(-FruitSpwanner.FruitInstance.poisonScore);
    // }

    // private void AteBody()
    // {
    //     if (m_immunity)
    //         return;

    //     if (m_PowerUp[(int)PowerUps.shield])
    //     {
    //         m_PowerUp[(int)PowerUps.shield] = false;
    //         UIManager.UiInstance.PowerUp(player, PowerUps.shield, false);
    //         StartCoroutine(SetImmunity(1));
    //         return;
    //     }
    //     m_Audio.Play(Sounds.Death);
    //     Debug.Log("Player Dead");
    //     StartCoroutine(DeathAnimation());
    //     GameManager.ManagerInstance.GameOver();
    // }

    // private void AteHead()
    // {
    //     GameManager.ManagerInstance.Draw();
    //     UIManager.UiInstance.Draw();
    // }

    // public void ActivatePowerUp(PowerUps power, GameObject powerObject)
    // {
    //     m_Audio.Play(Sounds.Eat);
    //     Destroy(powerObject);
    //     UIManager.UiInstance.PowerUp(player, power, true);
    //     m_PowerUp[(int)power] = true;
    // }

    // void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (other.CompareTag("Fruit"))
    //     {
    //         AteFruit();
    //         return;
    //     }

    //     if (other.CompareTag("Poison"))
    //     {
    //         Destroy(other.gameObject);
    //         AtePoison();
    //         return;
    //     }

    //     if (other.CompareTag("Head"))
    //     {
    //         AteHead();
    //         return;
    //     }

    //     if (other.CompareTag("Body"))
    //     {
    //         AteBody();
    //         return;
    //     }

    //     if (other.CompareTag("Shield"))
    //     {
    //         ActivatePowerUp(PowerUps.shield, other.gameObject);
    //     }
    //     else if (other.CompareTag("ScoreUp"))
    //     {
    //         ActivatePowerUp(PowerUps.scoreUp, other.gameObject);
    //     }
    //     else if (other.CompareTag("SpeedUp"))
    //     {
    //         ActivatePowerUp(PowerUps.speedUp, other.gameObject);
    //     }
    // }
}