using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int[,] grid = { { -1, -1, -1, -1, -1 } , { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }, { 0, 0, 0, 0, 0 }};
    private List<GameObject> SquareClones = new List<GameObject>();
    public GameObject[] Square = new GameObject[30];
    public GameObject[] NextSquares = new GameObject[5];
    public int[] NextSquares_int = new int[5];
    private int cursor;
    private int highest;
    private bool GameOn;
    private List<ulong> scores = new List<ulong>();
    private List<string> names = new List<string>();
    private System.Random rand = new System.Random();
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI RankingNameText;
    public TextMeshProUGUI RankingScoreText;
    public TextMeshProUGUI GameOverPanel_scoreText;
    public TextMeshProUGUI GameOverPanel_HighestBlockText;
    public TextMeshProUGUI GameClearPanel_FinalText;
    public GameObject GameOverPanel;
    public GameObject GameClearPanel;
    public TMP_InputField UserInputField;
    public ulong score = 0;

    private void Start()
    {
        GameOverPanel.SetActive(false);
        GameClearPanel.SetActive(false);
        GameOn = true;
        highest = 0;
        cursor = 2;
        score = 0;
        if (PlayerPrefs.GetString("score") == "")
        {
            scores = new List<ulong>();
            names = new List<string>();
        }
        else
        {
            scores = new List<ulong>(Array.ConvertAll(PlayerPrefs.GetString("score").Split("\n"), ulong.Parse));
            names = new List<string>(PlayerPrefs.GetString("username").Split("\n"));
        }
        RankingScoreText.text = PlayerPrefs.GetString("score");
        RankingNameText.text = PlayerPrefs.GetString("username");
        for (int i = 0; i < 30; i++)
        {
            Square[i] = GameObject.Find(Mathf.Pow(2, i+1).ToString());
        }
        Build();
        for (int i = 0; i < 5; i++)
        {
            NextSquares_int[i] = rand.Next(Mathf.Max(1, highest - 8), Mathf.Max(5, highest - 3));
            if (i == 0)
            {
                NextSquares[i] = Instantiate(Square[NextSquares_int[i] - 1], new Vector3(0, 4.5f, 0), Quaternion.identity);
            }
            else
            {
                NextSquares[i] = Instantiate(Square[NextSquares_int[i] - 1], new Vector3(3.5f, 3.5f - (i * 2.0f), 0), Quaternion.identity);
            }
            SquareClones.Add(NextSquares[i]);
        }
        InvokeRepeating("Down", 1.0f, 0.75f);
    }

    private void Update()
    {
        if (GameOn)
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
            {
                if (cursor > 0)
                {
                    if (NextSquares[0].transform.position.y != 4.5f)
                    {
                        if (grid[(int)(NextSquares[0].transform.position.y + 4.5f), (int)(NextSquares[0].transform.position.x + 1.0f)] == 0)
                        {
                            cursor -= 1;
                            NextSquares[0].transform.position += Vector3.left;
                        }
                    } else
                    {
                        cursor -= 1;
                        NextSquares[0].transform.position += Vector3.left;
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
            {
                if (cursor < 4)
                {
                    if (NextSquares[0].transform.position.y != 4.5f)
                    {
                        if (grid[(int)(NextSquares[0].transform.position.y + 4.5f), (int)(NextSquares[0].transform.position.x + 3.0f)] == 0)
                        {
                            cursor += 1;
                            NextSquares[0].transform.position += Vector3.right;
                        }
                    }
                    else
                    {
                        cursor += 1;
                        NextSquares[0].transform.position += Vector3.right;
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.Space))
            {
                PutSquareDown();
            }
            else
            {
                for (int i = 0; i < 5; i++)
                {
                    if (Input.GetKeyUp((KeyCode)(49 + i)))
                    {
                        cursor = i;
                        PutSquareDown();
                        break;
                    }
                }
            }
            if (NextSquares[0].transform.position.y != 4.5f)
            {
                if (grid[(int)(NextSquares[0].transform.position.y + 4.5f), (int)(NextSquares[0].transform.position.x + 2.0f)] != 0)
                {
                    PutSquareDown();
                }
            }
        }
    }

    private void NextSquare(int index)
    {
        for (int i = index; i < 5; i++)
        {
            if (i != 4)
            {
                NextSquares_int[i] = NextSquares_int[i + 1];
            }
            else
            {
                NextSquares_int[4] = rand.Next(Mathf.Max(1, highest - 8), Mathf.Max(5, highest - 3));
            }
            if (i == 0)
            {
                NextSquares[i] = Instantiate(Square[NextSquares_int[i] - 1], new Vector3(0, 4.5f, 0), Quaternion.identity);
            }
            else
            {
                NextSquares[i] = Instantiate(Square[NextSquares_int[i] - 1], new Vector3(3.5f, 3.5f - (i * 2.0f), 0), Quaternion.identity);
            }
            SquareClones.Add(NextSquares[i]);
        }
    }
    private void PutSquareDown()
    {
        Add(cursor, NextSquares_int[0]);
        scoreText.text = score.ToString();
        NextSquare(0);
        cursor = 2;
    }
    private void Down()
    {
        if (GameOn)
        {
            NextSquares[0].transform.position += Vector3.down;
        }
    }
    private void Add(int column, int type)
    {
        if (highest < type)
        {
            highest = type;
            FindObjectOfType<SoundManager>().Play("Merge_New");
            for (int i = 0; i < 5; i++)
            {
                if (NextSquares_int[i] < Mathf.Max(1, highest - 8))
                {
                    NextSquares_int[i] += 1;
                }
            }
            if (type == 30)
            {
                GameClear();
            }
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                for (int i = 1; i < grid.GetLength(0); i++)
                {
                    if (grid[i, j] == 0)
                    {
                        break;
                    }
                    if (grid[i, j] < Mathf.Max(1, highest - 8))
                    {
                        grid[i, j] = 0;
                        for (int k = i+1; k < grid.GetLength(0); k++)
                        {
                            if (grid[k, j] == 0)
                            {
                                break;
                            }
                            int temp = grid[k, j];
                            grid[k, j] = 0;
                            Add(j, temp);
                        }
                    }
                }
            }
        }
        else
        {
            FindObjectOfType<SoundManager>().Play("Merge");
        }
        int t = type;
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            if (grid[i, column] == 0)
            {
                if (column < grid.GetLength(1) - 1)
                {
                    if (grid[i, column + 1] == type)
                    {
                        t += 1;
                        grid[i, column + 1] = 0;
                        for (int j = 0; j < grid.GetLength(0); j++)
                        {
                            if ((grid[j, column + 1] != 0) && (grid[j, column + 1] != -1))
                            {
                                int temp = grid[j, column + 1];
                                grid[j, column + 1] = 0;
                                Add(column + 1, temp);
                            }
                        }
                    }
                }
                if (column > 0)
                {
                    if (grid[i, column - 1] == type)
                    {
                        t += 1;
                        grid[i, column - 1] = 0;
                        for (int j = 0; j < grid.GetLength(0); j++)
                        {
                            if ((grid[j, column - 1] != 0) && (grid[j, column - 1] != -1))
                            {
                                int temp = grid[j, column - 1];
                                grid[j, column - 1] = 0;
                                Add(column - 1, temp);
                            }
                        }
                    }
                }
                if (i > 1)
                {
                    if (grid[i - 1, column] == type)
                    {
                        t += 1;
                        grid[i - 1, column] = 0;
                    }
                }
                if (t == type)
                {
                    grid[i, column] = t;
                } else
                {
                    Add(column, t);
                }
                break;
            }
        }
        score += (ulong)(type);
        Build();
    }
    private void Build()
    {
        foreach (GameObject c in SquareClones)
        {
            Destroy(c);
        }
        for (int i = 0; i < grid.GetLength(0); i++)
        {
            for (int j = 0; j < grid.GetLength(1); j++)
            {
                if ((grid[i, j] != 0) && (grid[i, j] != -1))
                {
                    GameObject SquareClone = Instantiate(Square[grid[i, j] - 1], new Vector3(-2.0f + j, -5.5f + i, 0), Quaternion.identity);
                    SquareClones.Add(SquareClone);
                }
            }
        }
        for (int i = 0; i < grid.GetLength(1); i++)
        {
            if (grid[grid.GetLength(0)-1, i] != 0)
            {
                GameOver();
            }
        }
    }

    private void Restart()
    {
        GameOverPanel.SetActive(false);
        GameClearPanel.SetActive(false);
        GameOn = true;
        highest = 0;
        cursor = 2;
        score = 0;
        scoreText.text = score.ToString();
        for (int i = 0; i<grid.GetLength(0); i++)
        {
            if (i == 0)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = -1;
                }
            }
            else
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = 0;
                }
            }
        }
        Build();
        for (int i = 0; i < 5; i++)
        {
            NextSquares_int[i] = rand.Next(Mathf.Max(1, highest - 8), Mathf.Max(5, highest - 3));
            if (i == 0)
            {
                NextSquares[i] = Instantiate(Square[NextSquares_int[i] - 1], new Vector3(0, 4.5f, 0), Quaternion.identity);
            }
            else
            {
                NextSquares[i] = Instantiate(Square[NextSquares_int[i] - 1], new Vector3(3.5f, 3.5f - (i * 2.0f), 0), Quaternion.identity);
            }
            SquareClones.Add(NextSquares[i]);
        }
    }
    private void GameOver()
    {
        FindObjectOfType<SoundManager>().Play("GameOver");
        GameOn = false;
        GameOverPanel.SetActive(true);
        GameOverPanel_scoreText.text = "Your Score: " + score;
        GameOverPanel_HighestBlockText.text = "Highest Block: " + (int)(Mathf.Pow(2, highest));
    }
    private void GameClear()
    {
        FindObjectOfType<SoundManager>().Play("GameClear");
        GameOn = false;
        GameClearPanel.SetActive(true);
        GameClearPanel_FinalText.text = "Congratulations!\nYou Reached 1,072,741,824\nWith Score of " + score;
    }

    public void UserInput()
    {
        if (scores.Count == 14)
        {
            scores.RemoveAt(13);
            names.RemoveAt(13);
        }
        if (scores.Count == 0)
        {
            scores.Add(score);
            names.Add(UserInputField.text);
        } else if (score < scores[scores.Count - 1])
        {
            scores.Add(score);
            names.Add(UserInputField.text);
        }
        else
        {
            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i] <= score)
                {
                    scores.Insert(i, score);
                    names.Insert(i, UserInputField.text);
                    break;
                }
            }
        }
        PlayerPrefs.SetString("score", string.Join("\n", scores.ToArray()));
        PlayerPrefs.SetString("username", string.Join("\n", names.ToArray()));
        RankingScoreText.text = PlayerPrefs.GetString("score");
        RankingNameText.text = PlayerPrefs.GetString("username");
        UserInputField.text = "Unnamed";
        Restart();
    }
    public void ResetRanking()
    {
        if (GameOn)
        {
            names = new List<string>();
            scores = new List<ulong>();
            PlayerPrefs.SetString("username", "");
            RankingNameText.text = PlayerPrefs.GetString("username");
            PlayerPrefs.SetString("score", "");
            RankingScoreText.text = PlayerPrefs.GetString("score");
        }
    }
}