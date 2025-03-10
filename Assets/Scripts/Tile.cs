using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tile : MonoBehaviour, IPointerClickHandler
{
    public bool isBomb = false;
    public bool isRevealed = false;
    bool isFlagged = false;
    public Vector2Int gridPosition;
    public TileGrid tileGrid;
    public Button button;

    public void OnClick()
    {
        if (isRevealed || isFlagged)
        {
            return;
        }
        tileGrid.RevealTile(gridPosition);
    }

    public void Flag()
    {
        isFlagged = !isFlagged;
        if(isFlagged)
        {
            button.image.sprite = Resources.Load<Sprite>("Sprites/TileFlag");
        }
        else
        {
            button.image.sprite = Resources.Load<Sprite>("Sprites/TileUnknown");
        }
        tileGrid.TileFlagged(isFlagged);
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(isRevealed)
        {
            return;
        }
        if (pointerEventData.button == PointerEventData.InputButton.Left)
        {
            OnClick();
        }
        else if (pointerEventData.button == PointerEventData.InputButton.Right)
        {
            Flag();
        }
    }

    public void Reveal(int bombNumber)
    {
        isRevealed = true;
        button.enabled = false;
        if (bombNumber == 0)
        {
            button.image.sprite = Resources.Load<Sprite>("Sprites/TileEmpty");
        }
        else
        {
            button.image.sprite = Resources.Load<Sprite>("Sprites/Tile" + bombNumber);
        }
    }

    public void Explode(bool wasClicked)
    {
        if (wasClicked)
        {
            button.image.sprite = Resources.Load<Sprite>("Sprites/TileExploded");
        }
        else
        {
            button.image.sprite = Resources.Load<Sprite>("Sprites/TileMine");
        }
    }
}
