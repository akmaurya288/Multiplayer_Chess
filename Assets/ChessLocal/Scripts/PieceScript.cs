using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceScript : MonoBehaviour
{
    public string pieceName;

    public string getPieceName()
    {
        return (pieceName);
    }
    public void setPieceName(string str)
    {
        pieceName = str;
    }
}
