using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoardMesh : MonoBehaviour
{
    List<Vector3> vertices;
    List<int> triangles;
    List<Vector2> uv;
    Mesh mesh;
    int vertexCount = 0;

    public float blockWidth = 1.0f;
    public float blockHeight = 1.0f;
    public Vector2 textureUnit = new Vector2(0.25f, 0.5f);

    public Vector2 I = new Vector2(0f, 0f);
    public Vector2 O = new Vector2(1f, 0f);
    public Vector2 T = new Vector2(2f, 0f);
    public Vector2 S = new Vector2(3f, 0f);
    public Vector2 Z = new Vector2(0f, 1f);
    public Vector2 J = new Vector2(1f, 1f);
    public Vector2 L = new Vector2(2f, 1f);
    public Vector2 empty = new Vector2(3f, 1f);

	// Use this for initialization
	void Awake ()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uv = new List<Vector2>();
    }

    public void GenerateBoardMesh(BoardManager.TetrominoType [] board, int width, int height, bool enableEmptyTexture = true)
    {
        vertices.Clear();
        triangles.Clear();
        uv.Clear();
        vertexCount = 0;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (board[x + y * width] != BoardManager.TetrominoType.Empty && !enableEmptyTexture)
                {
                    continue;
                }

                Vector2 texturePosition = GetTexturePosition(board[x + y * width]);
                GenerateBlock(new Vector3(
                    transform.position.x + blockWidth * x,
                    transform.position.y + blockHeight * y,
                    transform.position.z), texturePosition);
            }
        }

        SetGeneratedMesh();
    }

    public void GenerateBoardMesh(bool enableEmptyTexture = true)
    {
        BoardManager.TetrominoType[] board = BoardManager.instance.board;
        int width = BoardManager.instance.width;
        int height = BoardManager.instance.height;

        GenerateBoardMesh(board, width, height, enableEmptyTexture);
    }

    private Vector2 GetTexturePosition(BoardManager.TetrominoType tetromino)
    {
        switch(tetromino)
        {
            case BoardManager.TetrominoType.Empty:
                return empty;
            case BoardManager.TetrominoType.I:
                return I;
            case BoardManager.TetrominoType.O:
                return O;
            case BoardManager.TetrominoType.T:
                return T;
            case BoardManager.TetrominoType.S:
                return S;
            case BoardManager.TetrominoType.Z:
                return Z;
            case BoardManager.TetrominoType.J:
                return J;
            case BoardManager.TetrominoType.L:
                return L;

            default: return new Vector2(-1.0f, -1.0f);
        }
    }

    private void GenerateBlock(Vector3 position, Vector2 texturePosition)
    {
        vertices.Add(new Vector3(position.x, position.y, position.z));
        vertices.Add(new Vector3(position.x + blockWidth, position.y, position.z));
        vertices.Add(new Vector3(position.x + blockWidth, position.y + blockHeight, position.z));
        vertices.Add(new Vector3(position.x, position.y + blockHeight, position.z));

        triangles.Add(vertexCount + 0);
        triangles.Add(vertexCount + 3);
        triangles.Add(vertexCount + 1);
        triangles.Add(vertexCount + 3);
        triangles.Add(vertexCount + 2);
        triangles.Add(vertexCount + 1);

        uv.Add(new Vector2(texturePosition.x * textureUnit.x, texturePosition.y * textureUnit.y));
        uv.Add(new Vector2(texturePosition.x * textureUnit.x + textureUnit.x, texturePosition.y * textureUnit.y));
        uv.Add(new Vector2(texturePosition.x * textureUnit.x + textureUnit.x, texturePosition.y * textureUnit.y + textureUnit.y));
        uv.Add(new Vector2(texturePosition.x * textureUnit.x, texturePosition.y * textureUnit.y + textureUnit.y));

        vertexCount += 4;
    }

    private void SetGeneratedMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();

        mesh.Optimize();
        mesh.RecalculateNormals();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
