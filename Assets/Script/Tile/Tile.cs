using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public string SpecialEff;
    public Vector3Int POS;
    public bool blocked;
    public Tile parentNode;
    public int F => G + H;
    public int G = int.MaxValue;
    public int H = int.MaxValue;
    public GameObject obj;
    public SpriteRenderer checker;
    public SpriteRenderer tileSprite;
    public SpriteRenderer rangeSprite;
    public SpriteRenderer selectedUnitSprite;
    public SpriteRenderer stateSprite;
    public SpriteRenderer mark;
    public List<Character> CheckRange = new();
    public TileObject tileObject;
    public TileObject TempTileObject;
    public TileObject justTileObject;

    public void ResetPath()
    {
        G = int.MaxValue;
        H = int.MaxValue;
        parentNode = null;
    }
}
