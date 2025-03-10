using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TileGrid : MonoBehaviour
{
    public GridLayoutGroup gridLayoutGroup;
    public GameObject cell;

    public int size;
    
    Tile[,] cellTileBehaviours;

    public GameObject gameWonPanel;

    public Text flagNumberDisplay;
    int remainingFlagNumber;
    int bombNumber;

    public InputAction exitAction;

    private bool firstClick = true;

    void Start()
    {
        GenerateTiles();

        exitAction.Enable();
    }

    private void Update()
    {
        if (exitAction.WasPerformedThisFrame())
        {
            Exit();
        }
    }

    void GenerateTiles()
    {
        size = PlayerPrefs.GetInt("size", 10);

        gridLayoutGroup.constraintCount = size;
        cellTileBehaviours = new Tile[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                cellTileBehaviours[i, j] = Instantiate(cell, gameObject.transform).GetComponent<Tile>();
                cellTileBehaviours[i, j].gridPosition = new Vector2Int(i, j);
                cellTileBehaviours[i, j].tileGrid = this;
            }
        }

        bombNumber = BombNumber(size);
        remainingFlagNumber = bombNumber;
        FlagNumberChange();
    }

    int BombNumber(int size)
    {
        return (int)(size * size * 0.15);
    }

    List<Tile> bombTiles;
    void RandomizeBombs()
    {
        List<Tile> flattenedList = new List<Tile>();
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                cellTileBehaviours[i, j].isBomb = false;
                flattenedList.Add(cellTileBehaviours[i, j]);
            }
        }

        List<Tile> shuffledList = flattenedList.OrderBy(flattenedList => Random.value).ToList();

        bombTiles = shuffledList.Take(bombNumber).ToList();

        foreach (Tile tile in bombTiles)
        {
            tile.isBomb = true;
        }
    }

    void GameLost(Vector2Int tileClickedPosition)
    {
        Debug.Log("Game Lost");
        foreach (Tile tile in bombTiles)
        {
            tile.Explode(false);
        }
        cellTileBehaviours[tileClickedPosition.x, tileClickedPosition.y].Explode(true);
        DisableTiles();
    }

    void GameWon()
    {
        Debug.Log("Game Won");
        gameWonPanel.SetActive(true);
        DisableTiles();
    }

    void DisableTiles()
    {
        foreach (Tile tile in cellTileBehaviours)
        {
            tile.GetComponent<Tile>().enabled = false;
        }
    }

    private bool IsTileInGrid(int x, int y)
    {
        return x >= 0 && x < size && y >= 0 && y < size;
    }

    public void RevealTile(Vector2Int position)
    {
        if (cellTileBehaviours[position.x, position.y].isRevealed)
        {
            return;
        }

        int bombCount;
        Tile tile = cellTileBehaviours[position.x, position.y];
        if (firstClick)
        {
            firstClick = false;
            do
            {
                bombCount = 0;
                RandomizeBombs();
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (IsTileInGrid(position.x + i, position.y + j))
                        {
                            if (cellTileBehaviours[position.x + i, position.y + j].isBomb)
                            {
                                bombCount++;
                            }
                        }
                    }
                }
            }
            while (bombCount > 0);
        }

        if (tile.isBomb)
        {
            GameLost(position);
        }
        else
        {
            bombCount = 0;
            for (int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (IsTileInGrid(position.x + i, position.y + j))
                    {
                        if (cellTileBehaviours[position.x + i, position.y + j].isBomb)
                        {
                            bombCount++;
                        }
                    }
                }
            }

            cellTileBehaviours[position.x, position.y].Reveal(bombCount);
            //if no bombs are around, reveal all surrounding cells
            if (bombCount == 0)
            {
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (IsTileInGrid(position.x + i, position.y + j))
                        {
                            RevealTile(new Vector2Int(position.x + i, position.y + j));
                        }
                    }
                }
            }

            // Check if there are any unrevealed non-bomb cells left
            bool foundUnrevealed = false;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (!cellTileBehaviours[i, j].isBomb && !cellTileBehaviours[i, j].isRevealed)
                    {
                        foundUnrevealed = true;
                        break;
                    }
                    else if (i == size-1 && j == size-1)
                    {
                        GameWon();
                    }
                }
                if (foundUnrevealed)
                {
                    break;
                }
            }
        }
    }

    public void TileFlagged(bool wasAdded)
    {
        if (wasAdded)
        {
            remainingFlagNumber--;
        }
        else
        {
            remainingFlagNumber++;
        }
        FlagNumberChange();
    }

    void FlagNumberChange()
    {
        flagNumberDisplay.text = remainingFlagNumber.ToString("D3");
    }

    public void Exit()
    {
        SceneManager.LoadScene("Menu");
    }
}
