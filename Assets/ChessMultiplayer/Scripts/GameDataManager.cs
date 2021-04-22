using System;
using UnityEngine;

[Serializable]
public class EncryptedData
{
    public byte[] data;
}

[Serializable]
public class GameDataManager 
{

    PlayerS localPlayer;
    PlayerS remotePlayer;

    [SerializeField]
    ProtectedData protectedData;

    public GameDataManager(PlayerS local, PlayerS remote , string roomId = "1234567890123456")
    {
        localPlayer = local;
        remotePlayer = remote;
        protectedData = new ProtectedData(localPlayer.PlayerId, remotePlayer.PlayerId,roomId);
    }
    public void InitializeBoard(string fen)
    {
        protectedData.SetFenData(fen);
    }

    public void SetGameState(MultiplayerChess.GameState gameState)
    {
        protectedData.SetGameState((int)gameState);
    }


    public void SetCurrentTurnPlayer(PlayerS player)
    {
        protectedData.SetCurrentTurnPlayerId(player.PlayerId);
    }

    public PlayerS GetCurrentTurnPlayer()
    {
        string playerId = protectedData.GetCurrentTurnPlayerId();
        if (localPlayer.PlayerId.Equals(playerId))
        {
            return localPlayer;
        }
        else
        {
            return remotePlayer;
        }
    }
    public MultiplayerChess.GameState GetGameState()
    {
        return (MultiplayerChess.GameState)protectedData.GetGameState();
    }

    public bool GameFinished()
    {
        return protectedData.GameFinished();
    }

    public void SetFENCode(string move)
    {
        protectedData.SetFenData(move);
    }

    public string GetFENCode()
    {
        return protectedData.GetFenData();
    }

    public void SetTurn(int time)
    {
        protectedData.SetTurn(time);
    }
    public int GetTurn()
    {
        return protectedData.GetTurn();
    }
    public void SetWhiteTime(float time)
    {
        protectedData.SetWhiteTime(time);
    }
    public float GetWhiteTime()
    {
        return protectedData.GetWhiteTime();
    }
    public void SetBlackTime(float time)
    {
        protectedData.SetWhiteTime(time);
    }
    public float GetBlackTime()
    {
        return protectedData.GetBlackTime();
    }

    public void SetSelectedMove(string move)
    {
        protectedData.SetMove(move);
    }

    public string GetSelectedMove()
    {
        return protectedData.GetMove();
    }


    public EncryptedData EncryptedData()
    {
        Byte[] data = protectedData.ToArray();

        EncryptedData encryptedData = new EncryptedData();
        encryptedData.data = data;

        return encryptedData;
    }

    public void ApplyEncrptedData(EncryptedData encryptedData)
    {
        if (encryptedData == null)
        {
            return;
        }

        protectedData.ApplyByteArray(encryptedData.data);
    }
}
