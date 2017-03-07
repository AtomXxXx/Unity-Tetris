using UnityEngine;
using System.Collections;

public class TetrominoDisplay : MonoBehaviour
{
    public int displayIndex = 0;

    public int width = 5;
    public int height = 3;
    BoardManager.TetrominoType[] board;
    BoardManager.TetrominoType type;

    BoardMesh mesh;

	// Use this for initialization
	void Awake ()
    {
        board = new BoardManager.TetrominoType[width * height];
        for (int y = 0; y < height; y++)
            for (int x = 0; x < width; x++)
                board[x + y * width] = BoardManager.TetrominoType.Empty;

        mesh = GetComponent<BoardMesh>();
        type = BoardManager.TetrominoType.Empty;
	}
    	
	// Update is called once per frame
	void Update ()
    {
	    if(BoardManager.instance.nextComingTetrominos[displayIndex] != type)
        {
            type = BoardManager.instance.nextComingTetrominos[displayIndex];
            Vector2[] shape = new Vector2[4];
            
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    board[x + y * width] = BoardManager.TetrominoType.Empty;

            shape = TetrominoController.GetShape(type);

            for (int i = 0; i < 4; i++)
            {
                Vector2 position = new Vector2(2, 1);
                int x = (int)(shape[i].x + position.x + 0.5f);
                int y = (int)(shape[i].y + position.y + 0.5f);

                board[x + y * width] = type;
            }

            mesh.GenerateBoardMesh(board, width, height);
        }
	}
}
