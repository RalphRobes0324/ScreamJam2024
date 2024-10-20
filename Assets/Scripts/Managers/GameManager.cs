using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    private Transform player;
    [SerializeField] float spawnCooldown = 5f;
    [SerializeField] bool onCooldown = false;
    [SerializeField] float timeRemaining = 180f;
    [SerializeField] bool isTimeRunning = false;
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
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(SpawnMonstersTimer());
        Timer();
    }

    /// <summary>
    /// Controls the time when monster spawns
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnMonstersTimer()
    {
        while (true)
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

    /// <summary>
    /// Spawns Monsters in world
    /// </summary>
    void SpawnMonsters()
    {
        SpawnManager.instance.CanSpawn(player);
    }


    /// <summary>
    /// Controls the time of the game
    /// </summary>
    void Timer()
    {
        if (isTimeRunning)
        {
            if(timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                DisplayTime(timeRemaining);
                
            }
            else
            {
                Debug.Log("Time is up");
                timeRemaining = 0;
                isTimeRunning = false;
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
