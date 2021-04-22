using UnityEngine;
using SWNetwork;
using UnityEngine.Events;
using System;

    [Serializable]
    public class GameDataEvent : UnityEvent<EncryptedData>
    {

    }

[Serializable]
public class MoveSelectedEvent : UnityEvent<string>
{

}

public class NetCode : MonoBehaviour
    {
        public GameDataEvent OnGameDataReadyEvent = new GameDataEvent();
        public GameDataEvent OnGameDataChangedEvent = new GameDataEvent();

        public UnityEvent OnGameStateChangedEvent = new UnityEvent();

        public MoveSelectedEvent OnMoveSelectedEvent = new MoveSelectedEvent();

        public UnityEvent OnLeftRoom = new UnityEvent();

        RoomPropertyAgent roomPropertyAgent;
        RoomRemoteEventAgent roomRemoteEventAgent;

        const string ENCRYPTED_DATA = "EncryptedData";
        const string GAME_STATE_CHANGED = "GameStateChanged";
        const string MOVE_SELECTED = "MoveSelected";

        public void ModifyGameData(EncryptedData encryptedData)
        {
            roomPropertyAgent.Modify(ENCRYPTED_DATA, encryptedData);
        }

        public void NotifyOtherPlayersGameStateChanged()
        {
            roomRemoteEventAgent.Invoke(GAME_STATE_CHANGED);
        }

        public void NotifyHostPlayerMoveSelected(string selectedMove)
        {
            SWNetworkMessage message = new SWNetworkMessage();
            message.PushUTF8ShortString(selectedMove);
            roomRemoteEventAgent.Invoke(MOVE_SELECTED, message);
        }

        public void EnableRoomPropertyAgent()
        {
            roomPropertyAgent.Initialize();
        }

        public void LeaveRoom()
        {
            NetworkClient.Instance.DisconnectFromRoom();
            NetworkClient.Lobby.LeaveRoom((successful, error) => {

                if (successful)
                {
                    Debug.Log("Left room");
                }
                else
                {
                    Debug.Log($"Failed to leave room {error}");
                }

                OnLeftRoom.Invoke();
            });
        }

        private void Awake()
        {
            roomPropertyAgent = FindObjectOfType<RoomPropertyAgent>();
            roomRemoteEventAgent = FindObjectOfType<RoomRemoteEventAgent>();
        }

        //****************** Room Property Events *********************//
        public void OnEncryptedDataReady()
        {
            Debug.Log("OnEncryptedDataReady");
            EncryptedData encryptedData = roomPropertyAgent.GetPropertyWithName(ENCRYPTED_DATA).GetValue<EncryptedData>();
            OnGameDataReadyEvent.Invoke(encryptedData);
        }

        public void OnEncryptedDataChanged()
        {
            Debug.Log("OnEncryptedDataChanged");
            EncryptedData encryptedData = roomPropertyAgent.GetPropertyWithName(ENCRYPTED_DATA).GetValue<EncryptedData>();
            OnGameDataChangedEvent.Invoke(encryptedData);
        }

        //****************** Room Remote Events *********************//
        public void OnGameStateChangedRemoteEvent()
        {
            OnGameStateChangedEvent.Invoke();
        }

        public void OnMoveSelectedRemoteEvent(SWNetworkMessage message)
        {
            string stringMove = message.PopUTF8ShortString();
            OnMoveSelectedEvent.Invoke(stringMove);
        }

    }