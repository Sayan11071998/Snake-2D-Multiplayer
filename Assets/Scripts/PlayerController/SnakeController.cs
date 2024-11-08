using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class SnakeController : MonoBehaviour
{
    [Range(0, 50)] public float _speed;
    public Players _player;
    public GameObject _playerUI;

    [Header("Body Configuration")]
    public GameObject _snakeSegments;
    public int _initialSegmentCount;

    [Header("Key Configuration")]
    public KeyCode upKey;
    public KeyCode downKey;
    public KeyCode leftKey;
    public KeyCode rightkey;

    private Vector3 _direction;
    private Rigidbody2D _rigidBody2D;
    private SpriteRenderer _headSprite;
    private List<Transform> _segments;
    private float _moveTimer = 0;
    private bool _isVertical, _isPaused, _isImmune;
    private bool[] _powerUps = new bool[3];
    private float[] _powerUpTimers = new float[3];
    private float _playerScore;

    private void Start()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
        _headSprite = GetComponent<SpriteRenderer>();

        _rigidBody2D.bodyType = RigidbodyType2D.Kinematic;
        _isPaused = false;

        InitializeBodySegments();
        initializePowerUp();
    }

    private void Update()
    {
        if (_isPaused || GameManager.Instance._isGameOver)
            return;

        GetSnakeDirection();
        MoveSnake();
        UpdatePowerUpTimer();
    }

    private void InitializeBodySegments()
    {
        _direction = Vector3.right;

        if (_player == Players.Beta)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            _direction = Vector3.left;
        }

        transform.position = transform.parent.position;
        _segments = new List<Transform>();

        StartCoroutine(SetImmunity(0.5f));
        _segments.Add(transform);

        for (int i = 0; i < _initialSegmentCount; i++)
            AddNewBodySegments();
    }

    private IEnumerator SetImmunity(float timer)
    {
        _isImmune = true;
        yield return new WaitForSeconds(timer);
        _isImmune = false;
    }

    private void AddNewBodySegments()
    {
        int segmentCount = _segments.Count;

        Vector3 snakeTail = _segments[segmentCount - 1].position;

        GameObject newSegment = Instantiate<GameObject>(_snakeSegments, snakeTail, Quaternion.identity);
        newSegment.name = string.Format("body {0}", segmentCount);
        _segments.Add(newSegment.transform);
        newSegment.transform.parent = transform.parent;

        if (_player == Players.Beta)
            newSegment.GetComponent<SpriteRenderer>().color = Color.red;

        SetScale();
    }

    private void SetScale()
    {
        int segmentCount = _segments.Count;
        float decrement = 0.1f;

        int limit = Mathf.Min(4, segmentCount - 1);

        for (int i = 1; i < limit; i++)
        {
            if (segmentCount - i == 1)
                return;

            Vector3 scale = _segments[segmentCount - i].localScale;
            scale.x = 0.7f - decrement;
            scale.y = 0.7f - decrement;
            _segments[segmentCount - i].localScale = scale;
            decrement -= 0.1f;
        }
    }

    private void initializePowerUp()
    {
        for (int i = 0; i < 3; i++)
        {
            _powerUps[i] = false;
            _powerUpTimers[i] = 0;
        }
    }

    private void GetSnakeDirection()
    {
        if (Input.GetKeyDown(upKey) && !_isVertical)
        {
            _direction = Vector3.up;
            FlipSnakeSprite();
        }
        else if (Input.GetKeyDown(downKey) && !_isVertical)
        {
            _direction = Vector3.down;
            FlipSnakeSprite();
        }
        else if (Input.GetKeyDown(leftKey) && _isVertical)
        {
            _direction = Vector3.left;
            FlipSnakeSprite();
        }
        else if (Input.GetKeyDown(rightkey) && _isVertical)
        {
            _direction = Vector3.right;
            FlipSnakeSprite();
        }
    }

    private void FlipSnakeSprite()
    {
        if (_direction == Vector3.left)
            _headSprite.transform.rotation = Quaternion.Euler(0, 0, 180);
        else if (_direction == Vector3.right)
            _headSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
        else if (_direction == Vector3.up)
            _headSprite.transform.rotation = Quaternion.Euler(0, 0, 90);
        else if (_direction == Vector3.down)
            _headSprite.transform.rotation = Quaternion.Euler(0, 0, 270);

        if (_player == Players.Beta)
        {
            if (_direction == Vector3.left)
                _headSprite.transform.rotation = Quaternion.Euler(0, 0, 180);
            else if (_direction == Vector3.right)
                _headSprite.transform.rotation = Quaternion.Euler(0, 0, 0);
            else if (_direction == Vector3.up)
                _headSprite.transform.rotation = Quaternion.Euler(0, 0, 90);
            else if (_direction == Vector3.down)
                _headSprite.transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }

    private void MoveSnake()
    {
        float effectiveSpeed = _speed * ((_powerUps[(int)PowerUpsItemTypes.speedUp]) ? 3 : 1);

        if (_moveTimer > 1 / effectiveSpeed)
        {
            MoveBody();
            MoveHead();
            _moveTimer = 0;
        }

        _moveTimer += Time.deltaTime;
    }

    private void MoveBody()
    {
        int segmentCount = _segments.Count;
        for (int i = segmentCount - 1; i > 0; i--)
            _segments[i].position = _segments[i - 1].position;
    }

    private void MoveHead()
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

    private void UpdatePowerUpTimer()
    {
        for (int i = 0; i < 3; i++)
        {
            if (_powerUps[i])
                _powerUpTimers[i] += Time.deltaTime;
            else
                continue;

            float timePeriod = PowerUpManager.powerUpInstance.GetPowerUpDuration((PowerUpsItemTypes)i);

            if (_powerUpTimers[i] > timePeriod)
            {
                _powerUps[i] = false;
                UIManager.Instance.PowerUp(_player, (PowerUpsItemTypes)i, false);
                _powerUpTimers[i] = 0;
            }
        }
    }

    private void CheckBoundary(ref Vector3 position)
    {
        if (position.x > Bounds.maxX || position.x < Bounds.minX)
            position.x = ((position.x > 0) ? Bounds.minX : Bounds.maxX);
        else if (position.y > Bounds.maxY || position.y < Bounds.minY)
            position.y = ((position.y > 0) ? Bounds.minY : Bounds.maxY);
    }

    private void DestoryLastBody()
    {
        Destroy(_segments[_segments.Count - 1].gameObject);
        _segments.RemoveAt(_segments.Count - 1);
        SetScale();
    }

    private void AteFruit()
    {
        AudioContoller.Instance.PlaySFX(AudioTypeList.FruitEat);
        int count = ItemSpwanner.Instance.SnakeAteFruit() * ((_powerUps[(int)PowerUpsItemTypes.scoreUp]) ? 2 : 1);

        for (int i = 0; i < count; i++)
            AddNewBodySegments();

        if (_segments.Count > 3)
            ItemSpwanner.Instance.PoisonActivation(true);

        UpdateScore(ItemSpwanner.Instance.fruitScore * ((_powerUps[(int)PowerUpsItemTypes.scoreUp]) ? 2 : 1));
    }

    private void UpdateScore(float fruitScore)
    {
        _playerScore += fruitScore;
        _playerScore = Mathf.Max(0, _playerScore);
        UIManager.Instance.SetScoreUI(_player, _playerScore);
    }

    private void AtePoison()
    {
        AudioContoller.Instance.PlaySFX(AudioTypeList.PoisonEat);
        int count = ItemSpwanner.Instance.SnakeAtePoison();

        if (_segments.Count < count + 1)
        {
            AudioContoller.Instance.PlaySFX(AudioTypeList.Death);
            GameManager.Instance.GameOver();
            return;
        }

        for (int i = 0; i < count; i++)
            DestoryLastBody();

        if (_segments.Count < 3)
            ItemSpwanner.Instance.PoisonActivation(false);

        UpdateScore(-Mathf.Min(ItemSpwanner.Instance.poisonScore, _playerScore));
    }

    private void AteBody()
    {
        if (_isImmune) return;

        if (_powerUps[(int)PowerUpsItemTypes.shield])
        {
            _powerUps[(int)PowerUpsItemTypes.shield] = false;
            UIManager.Instance.PowerUp(_player, PowerUpsItemTypes.shield, false);
            return;
        }

        AudioContoller.Instance.PlaySFX(AudioTypeList.Death);

        _playerScore = Mathf.Max(0, _playerScore - 10);
        UIManager.Instance.SetScoreUI(_player, _playerScore);

        CheckGameOver();
    }

    private void AteHead()
    {
        AudioContoller.Instance.PlaySFX(AudioTypeList.Death);

        _playerScore = Mathf.Max(0, _playerScore - 10);
        UIManager.Instance.SetScoreUI(_player, _playerScore);

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        float scorePlayer1, scorePlayer2;
        UIManager.Instance.GetScores(out scorePlayer1, out scorePlayer2);
        UIManager.Instance.CheckGameOver(scorePlayer1, scorePlayer2);
    }

    public void ActivatePowerUp(PowerUpsItemTypes power, GameObject powerObject)
    {
        AudioContoller.Instance.PlaySFX(AudioTypeList.PowerUpEat);

        Destroy(powerObject);
        UIManager.Instance.PowerUp(_player, power, true);

        _powerUps[(int)power] = true;
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

        if (other.CompareTag("SnakeHead"))
        {
            AteHead();
            return;
        }

        if (other.CompareTag("SnakeSegments"))
        {
            AteBody();
            return;
        }

        if (other.CompareTag("Shield"))
            ActivatePowerUp(PowerUpsItemTypes.shield, other.gameObject);
        else if (other.CompareTag("ScoreUp"))
            ActivatePowerUp(PowerUpsItemTypes.scoreUp, other.gameObject);
        else if (other.CompareTag("SpeedUp"))
            ActivatePowerUp(PowerUpsItemTypes.speedUp, other.gameObject);
    }
}