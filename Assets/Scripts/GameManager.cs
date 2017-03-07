using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject menuObject;
    private Menu menu;
    public AudioSource music;
    public AudioSource sound;
    public AudioClip menuMusic;
    public AudioClip gameMusic;

    public bool menuFirst = true;

    void Awake()
    {
        if (GameManager.instance == null)
            GameManager.instance = this;
        else if (GameManager.instance != this)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(this);
    }
	// Use this for initialization
	void Start () {
        menu = menuObject.GetComponent<Menu>();
        PlayMusic(menuMusic);
	}
	
    public void PlaySound(AudioClip clip)
    {
        sound.clip = clip;
        sound.volume = 0.25f;
        sound.loop = false;
        sound.Play();
    }

    public void PlayMusic(AudioClip clip, float volume = 0.25f)
    {
        volume = Mathf.Clamp01(volume);
        music.clip = clip;
        music.loop = true;
        music.volume = volume;
        music.Play();
    }

	// Update is called once per frame
	void Update ()
    {
	    
	}

    public void PauseGame()
    {
        BoardManager.instance.gameRunning = false;
        menuObject.SetActive(true);
        menu.Pause();
    }

    public void ResumeGame()
    {
        BoardManager.instance.gameRunning = true;
        menuObject.SetActive(false);
    }

    public void StartGame()
    {
        menuFirst = false;
        SceneManager.LoadScene("main");
    }

    void OnLevelWasLoaded()
    {
        if (!menuFirst)
        {
            menuObject = GameObject.Find("Menu");
            menuObject.SetActive(false);
            BoardManager.instance.gameRunning = true;
            music = GameObject.Find("Main Camera").GetComponent<AudioSource>();
            sound = GameObject.Find("Board").GetComponent<AudioSource>();
        }
        menu = menuObject.GetComponent<Menu>();
        PlayMusic(gameMusic, 0.20f);
    }
}
