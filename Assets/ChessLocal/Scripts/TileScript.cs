using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
   private  string tilePosition;
   private  string tileColor;
    private string piece;
    private void Awake()
    {
        gameObject.tag = "tile";
    }
    public string getTilePosition()
    {
        return (tilePosition);
    }
    public void setTilePosition(string str)
    {
        tilePosition = str;
    }
    public string getPiece()
    {
        return (piece);
    }
    public void setPiece(string str)
    {
        piece = str;
    }
}
