using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class SnakeController : MonoBehaviour
{ 
    [Range(0,50)] public float speed;
    public Players player;
    public GameObject UI;

    [Header("Body Config")]
    public GameObject bodyPart;
    public int initialBodyCount;

    [Header("Key Config")]
    public KeyCode upKey = KeyCode.W;
    public KeyCode downKey = KeyCode.A;
    public KeyCode leftKey = KeyCode.A;
    public KeyCode rightkey = KeyCode.D;

    private Vector3 m_Direction;
    private Rigidbody2D m_rigidBody;
    private AudioContoller m_Audio;
    private List<Transform> m_body;
    private float m_MoveTimer = 0;
    private bool m_IsVertical, m_Paused, m_immunity;
    private bool[] m_PowerUp = new bool[3];
    private float[] m_PowerUpTimer = new float[3];
    private float m_score;

    private void Start()
    {
        m_rigidBody = GetComponent<Rigidbody2D>();
        m_Audio = GetComponent<AudioContoller>();
        m_rigidBody.bodyType = RigidbodyType2D.Kinematic;
        m_Paused = false;
        InitializeBody();
        initializePowerUp();
    }

    private void initializePowerUp()
    {
        for(int i = 0; i < 3; i++) 
        {
            m_PowerUp[i] = false;
            m_PowerUpTimer[i] = 0;
        }
    }

    private void InitializeBody()
    {
        m_Direction = Vector3.right;
        if (player == Players.Beta)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
            m_Direction = Vector3.left;
        }
        transform.position = transform.parent.position;
        m_body = new List<Transform>();
        StartCoroutine(SetImmunity(0.5f));
        m_body.Add(this.transform);
        for (int i = 0; i < initialBodyCount; i++)
        {
            AddNewBodyPart();
        }
    }
    
    private IEnumerator SetImmunity(float timer)
    {
        m_immunity = true;
        yield return new WaitForSeconds(timer);
        m_immunity = false;
    }

    private void Update() 
    { 
        if(m_Paused || GameManager.ManagerInstance.isGameOver)
            return;    
        GetSnakeDirection();
        MoveSnake();  
        UpdatePowerUpTimer();
    }

    private void UpdatePowerUpTimer()
    {
        for (int i = 0; i < 3; i++)
        {
            if(m_PowerUp[i])
                m_PowerUpTimer[i] += Time.deltaTime;
            else    
                continue;
            
            float timePeriod = PowerUpManager.powerUpInstance.getPowerUpPeriod((PowerUps)i);

            if(m_PowerUpTimer[i] > timePeriod)
            {
                m_PowerUp[i] = false;
                UIManager.UiInstance.PowerUp(player,(PowerUps)i,false);
                m_PowerUpTimer[i] = 0;
            }
        }
    }

    private void GetSnakeDirection()
    {
        if(Input.GetKeyDown(upKey) && !m_IsVertical)
        {
            m_Direction = Vector3.up;
        }
        else if(Input.GetKeyDown(leftKey) && m_IsVertical)
        {   
             m_Direction = Vector3.left;
        }
        else  if(Input.GetKeyDown(rightkey) && m_IsVertical)
        {   
            m_Direction = Vector3.right;
        }
        else if(Input.GetKeyDown(downKey) && !m_IsVertical)
        {   
            m_Direction = Vector3.down;  
        }
    }

    private void MoveSnake()
    {
        float effectiveSpeed = speed * ((m_PowerUp[(int)PowerUps.speedUp])?3:1);
        if (m_MoveTimer > 1 / effectiveSpeed)
        {
            MoveBody();
            MoveHead();
            m_MoveTimer = 0;
        }
        m_MoveTimer += Time.deltaTime;
    }

    private void MoveBody()
    {
        int bodyCount = m_body.Count;
        for (int i = bodyCount - 1; i > 0; i--)
        {
            m_body[i].position = m_body[i-1].position;
        }
    }

    private void MoveHead()
    {
        Vector3 pos = transform.position;
        pos += m_Direction;
        CheckBoundary(ref pos);
        transform.position = pos;     

        if(m_Direction.x == 0)
            m_IsVertical = true;
        else
            m_IsVertical = false;
    }

    private void CheckBoundary(ref Vector3 pos)
    { 
        //Bounds Bounds = GameManager.managerInstance.GetBounds(); 
        if(pos.x > Bounds.maxX || pos.x < Bounds.minX)
            pos.x =((pos.x > 0)?Bounds.minX:Bounds.maxX);
        else if(pos.y > Bounds.maxY || pos.y < Bounds.minY)
            pos.y =((pos.y > 0)?Bounds.minY:Bounds.maxY);
    }

    private void AddNewBodyPart()
    {
        int bodyCount =m_body.Count;
        Vector3 lastPart ;
        lastPart = m_body[bodyCount-1].position; //- Vector3.one; 
        GameObject newPart = Instantiate<GameObject>(bodyPart,lastPart,Quaternion.identity);
        newPart.name = string.Format("body {0}",bodyCount);
        m_body.Add(newPart.transform);
        newPart.transform.parent = transform.parent;
        if(player == Players.Beta)
            newPart.GetComponent<SpriteRenderer>().color = Color.red;
        SetScale();
    }

    private void SetScale()
    {
        int bodyCount = m_body.Count;
        float decrement = 0.3f;
        for (int i = 1; i < 5; i++)
        {  
            if(bodyCount-i == 1)
                return;
            Vector3 scale = m_body[bodyCount - i].localScale;
            scale.x = 0.7f - decrement;
            scale.y = 0.7f - decrement;
            m_body[bodyCount - i].localScale = scale;
            decrement -= 0.1f;
        }
    }
    IEnumerator DeathAnimation()
    {
        m_Paused = true;
        float waitTime = 0.1f;
        for (int i = m_body.Count-1; i > 0; i--)
        {
            Destroy(m_body[i].gameObject,waitTime);
            waitTime += 0.05f;
        }
        yield return new WaitForSeconds(waitTime);
        m_body.Clear();
        UIManager.UiInstance.GameOver(player);
        Destroy(this.gameObject);
    }
    
    private void DestoryLastBody()
    {
        Destroy(m_body[m_body.Count- 1].gameObject);
        m_body.RemoveAt(m_body.Count- 1);
        SetScale();
    }
    
    private void UpdateScore(float fruitScore)
    { 
        m_score += fruitScore;
        UIManager.UiInstance.SetScoreUI(player,m_score);
    }

    private void AteFruit()
    {
        m_Audio.Play(Sounds.Eat);
        int count = ItemSpwanner.FruitInstance.SnakeAteFruit() * ((m_PowerUp[(int)PowerUps.scoreUp]) ? 2 : 1);
        for (int i = 0; i < count; i++)
        {
            AddNewBodyPart();
        }
        if (m_body.Count > 3)
            ItemSpwanner.FruitInstance.PoisonActivation(true);
        
        UpdateScore(ItemSpwanner.FruitInstance.fruitScore * ((m_PowerUp[(int)PowerUps.scoreUp]) ? 2 : 1));
    }

    private void AtePoison()
    {
        m_Audio.Play(Sounds.Poison);
        int count = ItemSpwanner.FruitInstance.SnakeAtePoison();
        if (m_body.Count < count + 1)
        {
            m_Audio.Play(Sounds.Death);
            StartCoroutine(DeathAnimation());
            GameManager.ManagerInstance.GameOver();
        }
        for (int i = 0; i < count; i++)
        {
            DestoryLastBody();
        }
        if (m_body.Count < 3)
            ItemSpwanner.FruitInstance.PoisonActivation(false);

        UpdateScore(-ItemSpwanner.FruitInstance.poisonScore);
    }

     private void AteBody()
    {
        if(m_immunity)
            return;
        
        if (m_PowerUp[(int)PowerUps.shield])
        {
            m_PowerUp[(int)PowerUps.shield] = false;
            UIManager.UiInstance.PowerUp(player, PowerUps.shield, false);
            StartCoroutine(SetImmunity(1));
            return;
        }
        m_Audio.Play(Sounds.Death);
        Debug.Log("Player Dead");
        StartCoroutine(DeathAnimation());
        GameManager.ManagerInstance.GameOver();
    }

    private void AteHead()
    {
        GameManager.ManagerInstance.Draw();
        UIManager.UiInstance.Draw();
    }

    public void ActivatePowerUp(PowerUps power,GameObject powerObject)
    {
        m_Audio.Play(Sounds.Eat);
        Destroy(powerObject);
        UIManager.UiInstance.PowerUp(player,power, true);
        m_PowerUp[(int)power] = true;
    }

    void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Fruit"))
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
        {
            ActivatePowerUp(PowerUps.shield,other.gameObject);
        }
        else if(other.CompareTag("ScoreUp"))
        {
            ActivatePowerUp(PowerUps.scoreUp,other.gameObject);
        }
        else if(other.CompareTag("SpeedUp"))
        {
            ActivatePowerUp(PowerUps.speedUp,other.gameObject);
        }
    }
}