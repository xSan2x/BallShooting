using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    Player player;
    Target target;

    public Text mainText;
    public CanvasGroup mainCanvasGroup;
    public GameObject wayTemplate;
    public GameObject blockTemplate;
    public GameObject shotTemplate;
    private GameObject wayObject;
    GameObject newShot;
    private GameObject[] blocks;
    Transform camTransform;
    public float timeToRestart = 3f;
    private enum State
    {
        Start,
        Win,
        Lose
    }
    State state = State.Start;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
       if (instance == this)
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        player = Player.instance;
        target = Target.instance;
        camTransform = Camera.main.transform;
        player.OnAimStarted += CreateShot;
        player.OnGameOver += GameOver;
        target.OnPlayerCollision += WinnerWinnerChickenDinner;
    }

    // Update is called once per frame
    void Update()
    {
        if (timeToRestart > 0f)
        {
            timeToRestart -= Time.deltaTime;
            if (timeToRestart > 0f)
            {
                int remaining = Mathf.CeilToInt(timeToRestart);
                switch (state)
                {
                    case State.Start:
                        mainText.text = "Starts in " + remaining + "s...";
                        break;
                    case State.Win:
                        mainText.text = "You won! Restart in " + remaining + "s...";
                        break;
                    case State.Lose:
                        mainText.text = "You lose! Restart in " + remaining + "s...";
                        break;
                    default:
                        mainText.text = "Starts in " + remaining + "s...";
                        break;
                }
            }
            else
            {
                Restart();
            }
        }
    }
    void Restart()
    {
        //Destroy olds objects
        if (newShot != null)
        {
            Destroy(newShot);
        }
        blocks = GameObject.FindGameObjectsWithTag("Block");
        foreach(GameObject block in blocks)
        {
            Destroy(block);
        }
        //Init new objects
        InitGame();
        //Off canvas
        mainCanvasGroup.alpha = 0f;
        mainCanvasGroup.blocksRaycasts = false;
        mainCanvasGroup.interactable = false;
        player.state = Player.State.Wait; //Restart player state
    }
    private void InitGame()
    {
        player.transform.position = new Vector3(Random.Range(-4.5f, 4.5f), 0.5f, -4f); // Player spawns at random position
        target.transform.position = new Vector3(Random.Range(-4.5f, 4.5f), 0.5f, 4.5f); // Target spawns at random position
        player.playerSize = Vector3.Distance(player.transform.position, target.transform.position) - 1f; //Start player size equals distance between player and target
        player.transform.localScale = new Vector3(player.playerSize / 10, player.playerSize / 10, player.playerSize / 10);
        camTransform.position = new Vector3(player.transform.position.x, Camera.main.transform.position.y, Camera.main.transform.position.z); // Setup camera
        camTransform.LookAt(target.transform);
        camTransform.rotation = new Quaternion(0.45f, camTransform.rotation.y, camTransform.rotation.z, 1f);
        wayObject = Instantiate(wayTemplate);
        wayObject.GetComponent<Way>().Init(player.transform.position, target.transform.position);
        SeedTheField();
    }
    private void SeedTheField()
    {
        bool setBlock;
        float posX;
        float posZ;

        //Split field on 25 rows
        for (int i = 0; i < 25; i++)
        {
            //Split field on 50 columns
            for (int j = 0; j < 50; j++)
            {
                setBlock = (Random.value > 0.5f); //Random decides, create a block or not
                if (setBlock)
                {
                    posX = j * 0.2f - 4.9f;
                    posZ = i * 0.2f - 1.1f;
                    Instantiate(blockTemplate, new Vector3(posX, 0.4f, posZ), Quaternion.identity);
                }
            }
        }
    }
    void CreateShot(Vector3 direction)
    {
        newShot = Instantiate(shotTemplate, player.transform.position + direction, Quaternion.identity);
    }
    void GameOver()
    {
        Destroy(wayObject);
        state = State.Lose;
        player.state = Player.State.Wait;
        timeToRestart = 3f;
        //Turn on canvas
        mainCanvasGroup.alpha = 1f;
        mainCanvasGroup.blocksRaycasts = true;
        mainCanvasGroup.interactable = true;
    }
    void WinnerWinnerChickenDinner()
    {
        Destroy(wayObject);
        state = State.Win;
        timeToRestart = 3f;
        //Turn on canvas
        mainCanvasGroup.alpha = 1f;
        mainCanvasGroup.blocksRaycasts = true;
        mainCanvasGroup.interactable = true;
    }
}
