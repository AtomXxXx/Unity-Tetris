using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TetrominoController : MonoBehaviour
{
    enum RotationState
    {
        Spawn = 0,
        Right = 1,
        Twice = 2,
        Left = 3
    }
    /*90 left
    x = -y
    y = x

    90 right
    x = y
    y = -x*/
    public Vector2 position;
    private Vector2 [] shape;
    private BoardManager.TetrominoType type;
    private RotationState state;
    private BoardManager.TetrominoType[] board;
    private List<Vector2>[] offsetSRS;

	// Use this for initialization
	void Start ()
    {
        //shape = new Vector2[4];
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*if (Input.GetKeyDown(KeyCode.RightArrow))
            RotateRight();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            RotateLeft();*/
    }

    public bool Spawn(BoardManager.TetrominoType type)
    {
        shape = GetShape(type);
        state = RotationState.Spawn;
        this.type = type;
        offsetSRS = GetOffsetSRS();
        board = BoardManager.instance.board;

        int spawnX = (int)(BoardManager.instance.width / 2.0f) - 1;
        int spawnY = BoardManager.instance.height;
        //int spawnX = 1;
        //int spawnY = 0;

        position = new Vector2(spawnX, spawnY);
        if (Collision(shape, position))
            return false;

        SetOnBoard(spawnX, spawnY, type);
        BoardManager.instance.boardMesh.GenerateBoardMesh();
        return true;
    }

    public bool IsBlockAboveBoard()
    {
        for(int i = 0; i < 4; i++)
        {
            if ((position + shape[i]).y >= BoardManager.instance.height)
                return true;
        }
        return false;
    }

    public bool Move(Vector2 dir)
    {
        bool moved = false;
        SetOnBoard((int)(position.x + .5f), (int)(position.y + .5f), BoardManager.TetrominoType.Empty);

        if(!Collision(shape, position + dir))
        {
            position = position + dir;
            moved = true;
        }

        SetOnBoard((int)(position.x + .5f), (int)(position.y + .5f), type);
        BoardManager.instance.boardMesh.GenerateBoardMesh();
        return moved;
    }

    private void SetOnBoard(int posX, int posY, BoardManager.TetrominoType type)
    {
        board = BoardManager.instance.board;
        for (int i = 0; i < 4; i++)
        {
            int x = (int)(posX + shape[i].x);
            int y = (int)(posY + shape[i].y);
            board[x + y * BoardManager.instance.width] = type;
        }
    }

    private bool CollisionSRS(Vector2 [] newShape, RotationState newState, out Vector2 offset)
    {
        offset = new Vector2(0f, 0f);

        List<Vector2> newOffset = new List<Vector2>();

        for(int i = 0; i < offsetSRS[0].Count; i++)
        {
            Vector2 res = new Vector2();
            res = offsetSRS[(int)state][i] - offsetSRS[(int)newState][i];
            newOffset.Add(res);
        }

        for(int i = 0; i < newOffset.Count; i++)
        {
            if(!Collision(newShape, position + newOffset[i]))
            {
                offset = newOffset[i];
                return false;
            }
        }

        return true;
    }

    private bool Collision(Vector2 [] newShape, Vector2 pos)
    {
        for(int i = 0; i < newShape.Length; i++)
        {
            int x = (int)(pos.x + newShape[i].x);
            int y = (int)(pos.y + newShape[i].y);

            if (x < 0 || y < 0 || x >= BoardManager.instance.width || y >= BoardManager.instance.height + 2)
                return true;

            if (board[x + y * BoardManager.instance.width] != BoardManager.TetrominoType.Empty)
                return true;
        }

        return false;
    }

    public bool RotateRight()
    {
        bool rotated = false;
        board = BoardManager.instance.board;
        Vector2[] newShape = new Vector2[4];
        for(int i = 0; i < 4; i++)
        {
            newShape[i].x = shape[i].y;
            newShape[i].y = -shape[i].x;
        }

        RotationState newState = state;
        switch(newState)
        {
            case RotationState.Spawn:
                newState = RotationState.Right;
                break;
            case RotationState.Right:
                newState = RotationState.Twice;
                break;
            case RotationState.Twice:
                newState = RotationState.Left;
                break;
            case RotationState.Left:
                newState = RotationState.Spawn;
                break;
        }

        SetOnBoard((int)(position.x + .5f), (int)(position.y + .5f), BoardManager.TetrominoType.Empty);

        Vector2 offset;
        if(!CollisionSRS(newShape, newState, out offset))
        {
            position = position + offset;
            state = newState;
            shape = newShape;
            rotated = true;
        }

        SetOnBoard((int)(position.x + .5f), (int)(position.y + .5f), type);
        BoardManager.instance.boardMesh.GenerateBoardMesh();
        return rotated;
    }

    public bool RotateLeft()
    {
        bool rotated = false;
        board = BoardManager.instance.board;
        Vector2[] newShape = new Vector2[4];
        for (int i = 0; i < 4; i++)
        {
            newShape[i].x = -shape[i].y;
            newShape[i].y = shape[i].x;
        }

        RotationState newState = state;
        switch (newState)
        {
            case RotationState.Spawn:
                newState = RotationState.Left;
                break;
            case RotationState.Left:
                newState = RotationState.Twice;
                break;
            case RotationState.Twice:
                newState = RotationState.Right;
                break;
            case RotationState.Right:
                newState = RotationState.Spawn;
                break;
        }

        SetOnBoard((int)(position.x + .5f), (int)(position.y + .5f), BoardManager.TetrominoType.Empty);

        Vector2 offset;
        if (!CollisionSRS(newShape, newState, out offset))
        {
            position = position + offset;
            state = newState;
            shape = newShape;
            rotated = true;
        }

        SetOnBoard((int)(position.x + .5f), (int)(position.y + .5f), type);
        BoardManager.instance.boardMesh.GenerateBoardMesh();
        return rotated;
    }

    private List<Vector2>[] GetOffsetSRS()
    {
        // Wall kick offset values used to detect collision after a rotation
        // Check https://tetris.wiki/SRS for more info

        List<Vector2>[] result = new List<Vector2>[4];
        for (int i = 0; i < 4; i++)
            result[i] = new List<Vector2>();

        if(type != BoardManager.TetrominoType.I && type != BoardManager.TetrominoType.O)
        {
            result[(int)RotationState.Spawn].Add(new Vector2(0, 0));
            result[(int)RotationState.Spawn].Add(new Vector2(0, 0));
            result[(int)RotationState.Spawn].Add(new Vector2(0, 0));
            result[(int)RotationState.Spawn].Add(new Vector2(0, 0));
            result[(int)RotationState.Spawn].Add(new Vector2(0, 0));

            result[(int)RotationState.Right].Add(new Vector2(0, 0));
            result[(int)RotationState.Right].Add(new Vector2(1, 0));
            result[(int)RotationState.Right].Add(new Vector2(1, -1));
            result[(int)RotationState.Right].Add(new Vector2(0, 2));
            result[(int)RotationState.Right].Add(new Vector2(1, 2));

            result[(int)RotationState.Twice].Add(new Vector2(0, 0));
            result[(int)RotationState.Twice].Add(new Vector2(0, 0));
            result[(int)RotationState.Twice].Add(new Vector2(0, 0));
            result[(int)RotationState.Twice].Add(new Vector2(0, 0));
            result[(int)RotationState.Twice].Add(new Vector2(0, 0));

            result[(int)RotationState.Left].Add(new Vector2(0, 0));
            result[(int)RotationState.Left].Add(new Vector2(-1, 0));
            result[(int)RotationState.Left].Add(new Vector2(-1, -1));
            result[(int)RotationState.Left].Add(new Vector2(0, 2));
            result[(int)RotationState.Left].Add(new Vector2(-1, 2));
        }
        else if(type == BoardManager.TetrominoType.I)
        {
            result[(int)RotationState.Spawn].Add(new Vector2(0, 0));
            result[(int)RotationState.Spawn].Add(new Vector2(-1, 0));
            result[(int)RotationState.Spawn].Add(new Vector2(2, 0));
            result[(int)RotationState.Spawn].Add(new Vector2(-1, 0));
            result[(int)RotationState.Spawn].Add(new Vector2(2, 0));

            result[(int)RotationState.Right].Add(new Vector2(-1, 0));
            result[(int)RotationState.Right].Add(new Vector2(0, 0));
            result[(int)RotationState.Right].Add(new Vector2(0, 0));
            result[(int)RotationState.Right].Add(new Vector2(0, 1));
            result[(int)RotationState.Right].Add(new Vector2(0, -2));

            result[(int)RotationState.Twice].Add(new Vector2(-1, 1));
            result[(int)RotationState.Twice].Add(new Vector2(1, 1));
            result[(int)RotationState.Twice].Add(new Vector2(-2, 1));
            result[(int)RotationState.Twice].Add(new Vector2(1, 0));
            result[(int)RotationState.Twice].Add(new Vector2(-2, 0));

            result[(int)RotationState.Left].Add(new Vector2(0, 1));
            result[(int)RotationState.Left].Add(new Vector2(0, 1));
            result[(int)RotationState.Left].Add(new Vector2(0, 1));
            result[(int)RotationState.Left].Add(new Vector2(0, -1));
            result[(int)RotationState.Left].Add(new Vector2(0, 2));
        }
        else if(type == BoardManager.TetrominoType.O)
        {
            result[(int)RotationState.Spawn].Add(new Vector2(0, 0));
            result[(int)RotationState.Right].Add(new Vector2(0, -1));
            result[(int)RotationState.Twice].Add(new Vector2(-1, -1));
            result[(int)RotationState.Left].Add(new Vector2(-1, 0));
        }

        return result;
    }

    static public Vector2 [] GetShape(BoardManager.TetrominoType type)
    {
        Vector2[] result = new Vector2[4];
        switch(type)
        {
            case BoardManager.TetrominoType.I:
                {
                    /*result[0] = new Vector2(-1.5f, 0.5f);
                    result[1] = new Vector2(-0.5f, 0.5f);
                    result[2] = new Vector2(0.5f, 0.5f);
                    result[3] = new Vector2(1.5f, 0.5f);*/
                    result[0] = new Vector2(0, 0);
                    result[1] = new Vector2(-1, 0);
                    result[2] = new Vector2(1, 0);
                    result[3] = new Vector2(2, 0);
                    return result;
                }
            case BoardManager.TetrominoType.O:
                {
                    /*result[0] = new Vector2(0.5f, 0.5f);
                    result[1] = new Vector2(-0.5f, 0.5f);
                    result[2] = new Vector2(-0.5f, -0.5f);
                    result[3] = new Vector2(0.5f, -0.5f);*/
                    result[0] = new Vector2(0, 0);
                    result[1] = new Vector2(0, 1);
                    result[2] = new Vector2(1, 0);
                    result[3] = new Vector2(1, 1);
                    return result;
                }
            case BoardManager.TetrominoType.T:
                {
                    result[0] = new Vector2(0f, 0f);
                    result[1] = new Vector2(-1f, 0f);
                    result[2] = new Vector2(0f, 1f);
                    result[3] = new Vector2(1f, 0f);
                    return result;
                }
            case BoardManager.TetrominoType.S:
                {
                    result[0] = new Vector2(0f, 0f);
                    result[1] = new Vector2(-1f, 0f);
                    result[2] = new Vector2(0f, 1f);
                    result[3] = new Vector2(1f, 1f);
                    return result;
                }
            case BoardManager.TetrominoType.Z:
                {
                    result[0] = new Vector2(0f, 0f);
                    result[1] = new Vector2(-1f, 1f);
                    result[2] = new Vector2(0f, 1f);
                    result[3] = new Vector2(1f, 0f);
                    return result;
                }
            case BoardManager.TetrominoType.J:
                {
                    result[0] = new Vector2(0f, 0f);
                    result[1] = new Vector2(-1f, 0f);
                    result[2] = new Vector2(-1f, 1f);
                    result[3] = new Vector2(1f, 0f);
                    return result;
                }
            case BoardManager.TetrominoType.L:
                {
                    result[0] = new Vector2(0f, 0f);
                    result[1] = new Vector2(-1f, 0f);
                    result[2] = new Vector2(1f, 1f);
                    result[3] = new Vector2(1f, 0f);
                    return result;
                }
        }
        return result;
    }
}
