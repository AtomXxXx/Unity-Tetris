using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Menu : MonoBehaviour
{
    public Button startGameButton;
    public Button controlsButton;
    public Button resumeButton;
    public Text title;
    public Text controls;
    public Button backButton;

    bool inControls = false;
    bool isPaused = false;

    void Awake()
    {
        resumeButton.gameObject.SetActive(false);
        backButton.gameObject.SetActive(false);
        controls.enabled = false;
        startGameButton.onClick.AddListener(delegate () { StartButtonClicked(); });
        resumeButton.onClick.AddListener(delegate () { ResumeButtonClicked(); });
        controlsButton.onClick.AddListener(delegate () { ControlsButtonClicked(); });
        backButton.onClick.AddListener(delegate () { BackButtonClicked(); });
    }

    void StartButtonClicked()
    {
        GameManager.instance.StartGame();
    }

    void ResumeButtonClicked()
    {
        GameManager.instance.ResumeGame();
    }

    void ControlsButtonClicked()
    {
        inControls = true;
        backButton.gameObject.SetActive(true);
        controls.enabled = true;

        title.enabled = false;
        startGameButton.gameObject.SetActive(false);
        controlsButton.gameObject.SetActive(false);
        if(isPaused)
            resumeButton.gameObject.SetActive(false);
    }

    void BackButtonClicked()
    {
        inControls = false;
        backButton.gameObject.SetActive(false);
        controls.enabled = false;

        title.enabled = true;
        startGameButton.gameObject.SetActive(true);
        controlsButton.gameObject.SetActive(true);
        if(isPaused)
            resumeButton.gameObject.SetActive(true);
    }

    public void Pause()
    {
        resumeButton.gameObject.SetActive(true);
        startGameButton.GetComponentInChildren<Text>().text = "Restart";
        isPaused = true;
    }

	// Use this for initialization
	void Start ()
    {
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            if (isPaused && !inControls)
                ResumeButtonClicked();
            else if (inControls)
                BackButtonClicked();
	}
}
