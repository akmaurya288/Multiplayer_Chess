using ChessDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SWNetwork;
using UnityEngine;

[Serializable]
public class ProtectedData
{
    [SerializeField]
    string player1Id;
    [SerializeField]
    string player2Id;
    [SerializeField]
    int Turn;
    [SerializeField]
    float WhiteTime;
    [SerializeField]
    float BlackTime;
    [SerializeField]
    string currentTurnPlayerId;
    [SerializeField]
    string fenCode;
    [SerializeField]
    string selectedMove;

    [SerializeField]
    int currentGameState;


    byte[] encryptionKey;
    byte[] safeData;

    public ProtectedData(string p1Id, string p2Id,string roomId)
    {
        player1Id = p1Id;
        player2Id = p2Id;
        Turn = 0;
        WhiteTime = 300;
        BlackTime = 300;
        currentTurnPlayerId = "";
        fenCode = "";
        selectedMove = "";
        CalculateKey(roomId);
        Encrypt();
    }

    public void SetFenData(string fen)
    {
        Decrypt();
        fenCode = fen;
        Encrypt();
    }
    public string GetFenData()
    {
        string result;
        Decrypt();
        result = fenCode;
        Encrypt();
        return result;
    }

    public void SetMove(string move)
    {
        Decrypt();
        selectedMove = move;
        Encrypt();
    }
    public string GetMove()
    {
        string result;
        Decrypt();
        result = selectedMove;
        Encrypt();
        return result;
    }
    public void SetGameState(int gameState)
    {
        Decrypt();
        currentGameState = gameState;
        Encrypt();
    }
    public int GetGameState()
    {
        int result;
        Decrypt();
        result = currentGameState;
        Encrypt();
        return result;
    }

    public void SetCurrentTurnPlayerId(string playerId)
    {
        Decrypt();
        currentTurnPlayerId = playerId;
        Encrypt();
    }

    public string GetCurrentTurnPlayerId()
    {
        string result;
        Decrypt();
        result = currentTurnPlayerId;
        Encrypt();
        return result;
    }

    public void SetTurn(int time)
    {
        Decrypt();
        Turn = time;
        Encrypt();
    }
    public int GetTurn()
    {
        int result;
        Decrypt();
        result = Turn;
        Encrypt();
        return result;
    }


    public void SetWhiteTime(float time)
    {
        Decrypt();
        WhiteTime = time;
        Encrypt();
    }
    public float GetWhiteTime()
    {
        float result;
        Decrypt();
        result = WhiteTime;
        Encrypt();
        return result;
    }

    public void SetBlackTime(float time)
    {
        Decrypt();
        BlackTime = time;
        Encrypt();
    }
    public float GetBlackTime()
    {
        float result;
        Decrypt();
        result = BlackTime;
        Encrypt();
        return result;
    }

    public bool GameFinished()
    {

        var game = new ChessGame(fenCode);
        if (game.IsCheckmated(0))
        {
            return true;
        }

        if (game.IsCheckmated(0))
        {
            return true;
        }

        return false;
    }


    public Byte[] ToArray()
    {
        return safeData;
    }

    public void ApplyByteArray(Byte[] byteArray)
    {
        safeData = byteArray;
    }

    void CalculateKey(string roomId)
    {
        string roomIdSubString = roomId.Substring(0, 16);
        encryptionKey = Encoding.UTF8.GetBytes(roomIdSubString);
    }

    void Encrypt()
    {
        SWNetworkMessage message = new SWNetworkMessage();

        message.PushUTF8ShortString(player1Id);
        message.PushUTF8ShortString(player2Id);
        message.PushUTF8ShortString(fenCode);
        message.PushUTF8ShortString(selectedMove);
        message.PushUTF8ShortString(currentTurnPlayerId);
        message.Push(currentGameState);
        message.Push(WhiteTime);
        message.Push(BlackTime);

        safeData = AES.EncryptAES128(message.ToArray(), encryptionKey);


        //player1Id = null;
        //player2Id = null;
        //fenCode = null;
        //selectedMove = null;
        //currentTurnPlayerId = null;
        //currentGameState = 0;
    }

    void Decrypt()
    {
        byte[] byteArray = AES.DecryptAES128(safeData, encryptionKey);

        SWNetworkMessage message = new SWNetworkMessage(byteArray);
       
        player1Id = message.PopUTF8ShortString();
        player2Id = message.PopUTF8ShortString();
        fenCode = message.PopUTF8ShortString();
        selectedMove = message.PopUTF8ShortString();

        currentTurnPlayerId = message.PopUTF8ShortString();
        currentGameState = message.PopInt32();
        WhiteTime = message.PopFloat();
        BlackTime = message.PopFloat();
    }

}
