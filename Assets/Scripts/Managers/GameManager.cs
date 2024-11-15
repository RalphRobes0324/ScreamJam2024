using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Game Manager Info")]
    public GameState currentState;
    public static GameManager instance;
    public bool isPaused = false;
    public GameObject pauseMenu;

    [Header("Player Info")]
    [SerializeField] GameObject playerBody;
    public bool playerBodySpawned = false;
    public GameObject deathScene;
    public GameObject winScene;
    public GameObject innerVoices;
    private Transform player;
    bool onItemCooldown = false;
    [SerializeField] float spawnItemCooldown = 10f;


    [Header("Monster Spawning Info")]
    [SerializeField] float spawnCooldown = 5f;
    bool onCooldown = false;
    bool isSpawning = true;

    [Header("Clock Info")]
    [SerializeField] float timeRemaining = 180f;
    bool isTimeRunning = false;
    public TextMeshProUGUI timeText;

    private void Awake()
    {
        //Game Manager in world, destory
        if (instance != null)
        {
            Destroy(instance.gameObject);
        }
        else
        {
            instance = this;
        }

        //Disable the pause Menu
        pauseMenu.gameObject.SetActive(false);

    }

    // Start is called before the first frame update
    void Start()
    {
        //Get Player transform
        if (player != null)
        {
            player = PlayerManager.instance.player.transform;
        }

        isTimeRunning = true;

        currentState = GameState.Playing;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState == GameState.Playing)
        {
            Time.timeScale = 1f;
            Timer();
            StartCoroutine(SpawnMonstersTimer());
            StartCoroutine(SpawnItemsTimer());
        }

        //End the session
        else if (currentState == GameState.End)
        {
            isSpawning = false;
            StopCoroutine(SpawnMonstersTimer());
            StopCoroutine(SpawnItemsTimer());
            SpawnManager.instance.ClearMonsters();
        }
    }


    public void SwitchScenes(string _sceneName)
    {
        SceneManager.LoadScene(_sceneName);
    }

    /// <summary>
    /// Controls Pause system
    /// </summary>
    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        currentState = GameState.Paused;
        pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        pauseMenu.gameObject.SetActive(false);
    }

    public void LostGame()
    {
        deathScene.SetActive(true);
        Time.timeScale = 0;
        currentState = GameState.End;
        timeRemaining = 0;
    }

    public void WinGame()
    {
        winScene.SetActive(true);
        Time.timeScale = 0;
        currentState = GameState.End;
        timeRemaining = 0;
    }

    public void displayInnerVoices()
    {
        innerVoices.SetActive(true);
    }

    public void hideInnerVoices()
    {
        innerVoices.SetActive(false);
    }


    /// <summary>
    /// Controls the time when monster spawns
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnMonstersTimer()
    {
        while (isSpawning)
        {
            if (!onCooldown)
            {
                SpawnMonsters();
                onCooldown = true;
                yield return new WaitForSeconds(spawnCooldown);
                onCooldown = false;
            }
            yield return null;
        }
    }

    IEnumerator SpawnItemsTimer()
    {
        while (isSpawning)
        {
            if (!onItemCooldown)
            {
                onItemCooldown = true;
                yield return new WaitForSeconds(spawnItemCooldown);
                SpawnManager.instance.SpawnItem();
                onItemCooldown = false;
            }
            yield return null;
        }
    }




    /// <summary>
    /// Spawns Monsters in world
    /// </summary>
    void SpawnMonsters()
    {
        SpawnManager.instance.CanSpawn();
    }


    /// <summary>
    /// Controls the time of the game
    /// </summary>
    void Timer()
    {
        if (isTimeRunning)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);

            }
            else
            {
                //Time is up
                Debug.Log("Time is up");
                timeRemaining = 0;
                isTimeRunning = false;
                //Clear time
                timeText.text = "";
                LostGame();
            }

            if (!playerBodySpawned)
            //Spawn Player body
            {
                if (timeRemaining <= 60f)
                {
                    Debug.Log("Body has spawned");
                    //Spawn Body
                    SpawnManager.instance.CanSpawnPlayerBody(playerBody);
                    displayInnerVoices();
                }

            }
        }
    }

    /// <summary>
    /// Display/Update time to screen
    /// </summary>
    /// <param name="_timeRemaining"></param>
    void DisplayTime(float _timeRemaining)
    {
        _timeRemaining += 1;

        float min = Mathf.FloorToInt(_timeRemaining / 60);
        float sec = Mathf.FloorToInt(_timeRemaining % 60);

        //Check text is exist
        if (timeText)
        {
            //Setup text
            timeText.text = string.Format("{0:00}:{1:00}", min, sec);
        }

    }
}
