using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{

    static public BoardManager instance = null;

    public enum TetrominoType
    {
        //The letters are the tetromino shapes
        Empty = 0,
        I = 1,
        O = 2,
        T = 3,
        J = 4,
        L = 5,
        S = 6,
        Z = 7
    }

    private int level = 0;
    public Text levelText;
    public float[] levels =
        {
            53, 49, 45, 41, 37, 33, 28, 22, 17, 11, 10, 9, 8, 7, 6, 6, 5, 5, 4, 4, 3
        };


    private int score = 0;
    public Text scoreText;

    private int lines = 0;
    public Text linesText;
    public Text gameOverText;
    public Button restartButton;
    public int[] scores;

    public bool enableEmptyTexture = true;
    public bool gameRunning = false;
    bool gameOver = false;

    public int width = 10;
    public int height = 20;
    [HideInInspector]
    public TetrominoType[] board;
    [HideInInspector]
    public TetrominoType[] nextComingTetrominos;
    public int numNextComingTetrominos = 4;
    private int offScreenHeight = 2;
    private float timer = 0f;

    struct ButtonTimer
    {
        public float holdButtonTimer;
        public KeyCode keyCode;
    }
    private ButtonTimer buttonTimer;
    public float moveInterval = 0.25f;

    [HideInInspector]
    public BoardMesh boardMesh;
    TetrominoController tetrominoController;

    public AudioClip moveSound;
    public AudioClip gameOverMusic;

    void Awake()
    {
        if (BoardManager.instance == null)
            BoardManager.instance = this;
        else if (BoardManager.instance != this)
        {
            Destroy(gameObject);
            return;
        }

        boardMesh = GetComponent<BoardMesh>();
        scores = new int[4];
        scores[0] = 40;
        scores[1] = 100;
        scores[2] = 300;
        scores[3] = 1200;
        
        
        for (int i = 0; i < levels.Length; i++)
            levels[i] = (levels[i] / 60f) * 1000f;
    }

	// Use this for initialization
	void Start ()
    {
        tetrominoController = GetComponent<TetrominoController>();
        gameOverText.enabled = false;
        restartButton.gameObject.SetActive(false);
         
        nextComingTetrominos = new TetrominoType[numNextComingTetrominos];
        for(int i = 0; i < numNextComingTetrominos; i++)
        {
            nextComingTetrominos[i] = TetrominoType.Empty;
        }

        int numberOfBlocks = width * (height + offScreenHeight);
        board = new TetrominoType[numberOfBlocks];
        for (int i = 0; i < numberOfBlocks; i++)
            board[i] = TetrominoType.Empty;

        for(int i = 0; i < numNextComingTetrominos; i++)
            nextComingTetrominos[i] = (TetrominoType)((int)(Random.Range(1.0f, 7.0f) + 0.5f));

        UpdateTexts();

        tetrominoController.Spawn(TetrominoType.L);
        boardMesh.GenerateBoardMesh(enableEmptyTexture);

        buttonTimer.keyCode = 0;
        buttonTimer.holdButtonTimer = 0f;
	}
	
    public void GameOver()
    {
        if(!gameOver)
            GameManager.instance.PlayMusic(gameOverMusic);
        gameRunning = false;
        gameOver = true;
        gameOverText.enabled = true;
        restartButton.gameObject.SetActive(true);
        restartButton.onClick.AddListener(delegate () { RestartButtonClicked(); });
    }

    private void RestartButtonClicked()
    {
        GameManager.instance.StartGame();
    }

    private void DoActionForButton(KeyCode keyCode)
    {
        switch(keyCode)
        {
            case KeyCode.RightArrow:
                if (tetrominoController.Move(new Vector2(1, 0)))
                    GameManager.instance.PlaySound(moveSound);
                break;

            case KeyCode.LeftArrow:
                if (tetrominoController.Move(new Vector2(-1, 0)))
                    GameManager.instance.PlaySound(moveSound);
                break;

            case KeyCode.DownArrow:
                MoveDown();
                GameManager.instance.PlaySound(moveSound);
                break;

            case KeyCode.LeftShift:
            case KeyCode.RightShift:
                if (tetrominoController.RotateRight())
                    GameManager.instance.PlaySound(moveSound);
                break;

            case KeyCode.LeftControl:
            case KeyCode.RightControl:
                if (tetrominoController.RotateLeft())
                    GameManager.instance.PlaySound(moveSound);
                break;

            case KeyCode.Escape:
                GameManager.instance.PauseGame();
                break;

            case KeyCode.UpArrow:
                HardDrop();
                break;

            default:
                print("Invalid Keycode in DoActionForButton()");
                break;
        }
    }

    private void PlayerInput(float dt)
    {
        KeyCode[] keyCodes = { KeyCode.RightArrow, KeyCode.LeftArrow, KeyCode.DownArrow,
                            KeyCode.LeftShift, KeyCode.RightShift, KeyCode.LeftControl, KeyCode.RightControl };

        bool keyDown = false;
        foreach (KeyCode keyCode in keyCodes)
        {
            if (Input.GetKey(keyCode))
            {
                if (buttonTimer.keyCode == keyCode)
                {
                    buttonTimer.holdButtonTimer += dt;
                    if (buttonTimer.holdButtonTimer > moveInterval)
                    {
                        DoActionForButton(keyCode);
                        buttonTimer.holdButtonTimer = 0f;
                    }
                }
                else
                {
                    buttonTimer.keyCode = keyCode;
                    buttonTimer.holdButtonTimer = 0f;
                    DoActionForButton(keyCode);
                }
                keyDown = true;
                break;
            }
        }

        if (!keyDown)
        {
            buttonTimer.keyCode = 0;
            buttonTimer.holdButtonTimer = 0f;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
            DoActionForButton(KeyCode.Escape);
        if (Input.GetKeyDown(KeyCode.UpArrow))
            DoActionForButton(KeyCode.UpArrow);

        /*if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
            GameManager.instance.PlaySound(moveSound);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
            HardDrop();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            if(tetrominoController.Move(new Vector2(-1, 0)))
                GameManager.instance.PlaySound(moveSound);
        if(Input.GetKeyDown(KeyCode.RightArrow))
            if(tetrominoController.Move(new Vector2(1, 0)))
                GameManager.instance.PlaySound(moveSound);

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
            if(tetrominoController.RotateRight())
                GameManager.instance.PlaySound(moveSound);
        if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
           if( tetrominoController.RotateLeft())
                GameManager.instance.PlaySound(moveSound);

        if (Input.GetKeyDown(KeyCode.Escape))
            GameManager.instance.PauseGame();*/
    }

    private void HardDrop()
    {
        timer = 0;
        gameRunning = false;
        StartCoroutine("HardDropCoroutine");
    }

    IEnumerator HardDropCoroutine()
    {
        bool dropping = true;
        while (dropping)
        {
            if (!tetrominoController.Move(new Vector2(0, -1)))
            {
                EndTurn();
                dropping = false;
            }
            yield return new WaitForSeconds(0.008f);
        }
        gameRunning = true;
    }

    private void MoveDown()
    {
        timer = 0;
        if (!tetrominoController.Move(new Vector2(0, -1)))
        {
            EndTurn();
        }
    }
    
    private void ClearFullRows()
    {
        List<int> rowsToClear = new List<int>();
        bool rowFull = true;

        for(int y = 0; y < height; y++)
        {
            rowFull = true;
            for(int x = 0; x < width; x++)
            {
                if(board[x + y * width] == TetrominoType.Empty)
                {
                    rowFull = false;
                    break;
                }
            }
            if(rowFull)
            {
                rowsToClear.Add(y);
            }
        }

        if(rowsToClear.Count != 0)
        {
            gameRunning = false;
            lines += rowsToClear.Count;
            score += scores[rowsToClear.Count - 1] * (level + 1);
            if (level < (levels.Length - 1) && (lines-rowsToClear.Count) % 10 > lines % 10)
                level++;
            StartCoroutine(ClearFullRowsCoroutine(rowsToClear));
        }
    }

    IEnumerator ClearFullRowsCoroutine(List<int> rowsToClear)
    {
        int clearedRows = 0;

        foreach (int row in rowsToClear)
        {
            for(int y = (row - clearedRows) + 1; y < height; y++)
                for(int x = 0; x < width; x++)
                    board[x + (y - 1) * width] = board[x + y * width];

            for (int x = 0; x < width; x++)
                board[x + (height - 1) * width] = TetrominoType.Empty;

            clearedRows++;
        }
        UpdateTexts();
        gameRunning = true;
        yield return null;
    }

    private void UpdateTexts()
    {
        scoreText.text = "Score: " + score;
        levelText.text = "Level: " + level;
        linesText.text = "Lines: " + lines;
    }
    
    public void EndTurn()
    {
        if(tetrominoController.IsBlockAboveBoard())
        {
            GameOver();
            return;
        }

        ClearFullRows();

        timer = 0f;
        tetrominoController.Spawn(nextComingTetrominos[0]);

        for(int i = 0; i < numNextComingTetrominos - 1; i++)
        {
            nextComingTetrominos[i] = nextComingTetrominos[i + 1];
        }

        nextComingTetrominos[numNextComingTetrominos - 1] = (TetrominoType)((int)(Random.Range(1.0f, 7.0f) + 0.5f));
    }

	// Update is called once per frame
	void Update ()
    {
        if (gameRunning)
        {
            timer += Time.deltaTime;
            if (timer * 1000f > levels[level])
                MoveDown();

            PlayerInput(Time.deltaTime);
        }
        else if (gameOver)
            if (Input.GetKeyDown(KeyCode.Escape))
                GameManager.instance.PauseGame();
	}
}
