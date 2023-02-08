using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;



public class Main : MonoBehaviour
{
    private Player player;
    public Transform playerPos;
    public GameObject[] players;

    public Text coinText;
    public Image[] hearts;
    public Sprite isLife, nonLife;
    public GameObject PauseScreen;
    public GameObject WinScreen;
    public GameObject LoseScreen;
    float timer = 0f;
    public Text timeText;
    public TimeWork timeWork;
    public float countdown;
    public GameObject inventoryPan;
    public SoundDefector soundeffector;
    public AudioSource musicSource, soundSource;

   
    public void ReloadLvl()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    private void Awake()
    {
        player = Instantiate(players[PlayerPrefs.GetInt("Player")], playerPos.position, Quaternion.identity).GetComponent<Player>();
    }
    private void Start()
    {
        musicSource.volume = (float)PlayerPrefs.GetInt("MusicVolume")/9;
        soundSource.volume = (float)PlayerPrefs.GetInt("SoundVolume")/9;

        if ((int)timeWork == 2)
            timer = countdown;
        
    }
    public void Update()
    {
        coinText.text = player.GetCoins().ToString();

        for (int i = 0; i < hearts.Length; i++)
        {
            if (player.GetHP() > i)
                hearts[i].sprite = isLife;
            else
                hearts[i].sprite = nonLife;
        }
        if ((int)timeWork == 1)
        {
            timer += Time.deltaTime;
            timeText.text = timer.ToString("F2").Replace(",", ":");
        }
        else if ((int)timeWork == 2)
        {
            timer -= Time.deltaTime;
            // timeText.text = timer.ToString("F2").Replace(",", ":");
            timeText.text = ((int)timer / 60).ToString() + ":" + ((int)timer - ((int)timer / 60) * 60).ToString("D2");
            if (timer <= 0)
                Lose();
        }
        else
            timeText.gameObject.SetActive(false);
    }
    public void PauseOn()
    {
        Time.timeScale = 0f;
        player.enabled = false;
        PauseScreen.SetActive(true);
        inventoryPan.SetActive(false);
    }

    public void PauseOff()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        PauseScreen.SetActive(false);
    }

    public void Win()
    {
        soundeffector.PlayWinSound();
        Time.timeScale = 0f;
        player.enabled = false;
        WinScreen.SetActive(true);

        if (!PlayerPrefs.HasKey("Lvl") || PlayerPrefs.GetInt("Lvl") < SceneManager.GetActiveScene().buildIndex)
            PlayerPrefs.SetInt("Lvl", SceneManager.GetActiveScene().buildIndex);
        print(PlayerPrefs.GetInt("Lvl"));

        if (PlayerPrefs.HasKey("coins"))
            PlayerPrefs.SetInt("coins", PlayerPrefs.GetInt("coins") + player.GetCoins());
        else
            PlayerPrefs.SetInt("coins", player.GetCoins());
        print(PlayerPrefs.GetInt("coins"));
        inventoryPan.SetActive(false);
        GetComponent<Inventory>().RecountItems();
    }
    public void Lose()
    {
        soundeffector.PlayLoseSound();
        Time.timeScale = 0f;
        player.enabled = false;
        LoseScreen.SetActive(true);
        inventoryPan.SetActive(false);
        GetComponent<Inventory>().RecountItems();
        
        
    }

    public void MenuLvl()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene("Menu"); 
    }

    public void NextLvl()
    {
        Time.timeScale = 1f;
        player.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    public void JumpUp()
    {
        player.Jump();
    }
}
public enum TimeWork
  {
    None,
    Stopwatch,
    Timer
  }