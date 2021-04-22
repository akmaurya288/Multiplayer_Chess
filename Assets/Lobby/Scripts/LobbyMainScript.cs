using SWNetwork;
using System;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMainScript : MonoBehaviour
{
    public enum LobbyState
    {
        Default,
        JoinedRoom,
    }
    public LobbyState State = LobbyState.Default;
    public GameObject onlinePopUp;
    public GameObject localPopUp;
    public InputField NicknameInputField;
    public Text roomecode;
    public string nickname;


    private void Start()
    {
        nickname = SaveSystem.LoadNickname();
        NicknameInputField.text = nickname;
        onlinePopUp.SetActive(false);
        localPopUp.SetActive(false);
        NetworkClient.Lobby.OnLobbyConnectedEvent += OnLobbyConnected;
        NetworkClient.Lobby.OnNewPlayerJoinRoomEvent += OnNewPlayerJoinRoomEvent;
        NetworkClient.Lobby.OnPlayerLeaveRoomEvent += OnPlayerLeaveRoomEvent;
        NetworkClient.Lobby.OnRoomReadyEvent += OnRoomReadyEvent;
    }

    private void OnDestroy()
    {
        NetworkClient.Lobby.OnLobbyConnectedEvent -= OnLobbyConnected;
        NetworkClient.Lobby.OnNewPlayerJoinRoomEvent -= OnNewPlayerJoinRoomEvent;
        NetworkClient.Lobby.OnPlayerLeaveRoomEvent -= OnPlayerLeaveRoomEvent;
        NetworkClient.Lobby.OnRoomReadyEvent -= OnRoomReadyEvent;
    }
    public void OnlineBtnClick()
    {
        onlinePopUp.SetActive(true);
        onlinePopUp.transform.GetChild(5).gameObject.SetActive(true);
        onlinePopUp.transform.GetChild(0).gameObject.SetActive(false);
        onlinePopUp.transform.GetChild(1).gameObject.SetActive(false);
        onlinePopUp.transform.GetChild(2).gameObject.SetActive(true);

    }
    public void OnlineCancleBtnClick()
    {
        onlinePopUp.SetActive(false);
    }
    public void LocalBtnClick()
    {
        localPopUp.SetActive(true);
    }
    public void LocalCancleBtnClick()
    {
        localPopUp.SetActive(false);
    }
    public void LocalPlayChess()
    {
        SceneManager.LoadScene("Local");
    }

    //########## Matchmaking ###########//
    void CheckIn()
    {
        NetworkClient.Instance.CheckIn(nickname, (bool successful, string error) =>
        {
            if (!successful)
            {
                Debug.LogError(error);
            }
        });
    }

    void RegisterToLobbyServer()
    {
        NetworkClient.Lobby.Register(nickname, (successful, reply, error) => {
            if (successful)
            {
                Debug.Log("Lobby registered " + reply);
                if (string.IsNullOrEmpty(reply.roomId))
                {
                    JoinOrCreateRoom();
                }else if (reply.started)
                {
                    State = LobbyState.JoinedRoom;
                    ConnectToRoom();
                }
                else
                {
                    State = LobbyState.JoinedRoom;
                    GetPlayersInTheRoom();
                }
            }
            else
            {
                Debug.Log("Lobby failed to register " + reply);
            }
        });
    }
    void ConnectToRoom()
    {
        NetworkClient.Instance.ConnectToRoom((connected) =>
        {
            if (connected)
            {
                SceneManager.LoadScene(2);
            }
        });
    }

    void JoinOrCreateRoom()
    {
        //NetworkClient.Lobby.CreateRoom(false, 2, (successful, reply, error) => {
        //    if (successful)
        //    {
        //        Debug.Log("Created room " + reply);
        //        roomecode.text = reply;
        //        State = LobbyState.JoinedRoom;
        //        GetPlayersInTheRoom();
        //    }
        //    else
        //    {
        //        Debug.Log("Failed to join or create room " + error);
        //    }
        //});
        NetworkClient.Lobby.JoinOrCreateRoom(false, 2, 60, (successful, reply, error) =>
        {
            if (successful)
            {
                Debug.Log("Joined or created room " + reply);
                roomecode.text = reply.roomId;
                State = LobbyState.JoinedRoom;
                GetPlayersInTheRoom();
            }
            else
            {
                Debug.Log("Failed to join or create room " + error);
            }
        });
    }

    void GetPlayersInTheRoom()
    {
        NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) => {
            if (successful)
            {
                Debug.Log("Got players " + reply);
                if (reply.players.Count == 1)
                {
                    onlinePopUp.transform.GetChild(1).gameObject.SetActive(false);
                    onlinePopUp.transform.GetChild(0).gameObject.SetActive(true);
                    onlinePopUp.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = reply.players[0].data;
                }else
                {
                    onlinePopUp.transform.GetChild(0).gameObject.SetActive(true);
                    onlinePopUp.transform.GetChild(0).GetChild(1).GetComponent<Text>().text = reply.players[0].data;
                    onlinePopUp.transform.GetChild(1).gameObject.SetActive(true);
                    onlinePopUp.transform.GetChild(1).GetChild(1).GetComponent<Text>().text = reply.players[1].data;
                    if (NetworkClient.Lobby.IsOwner)
                    {
                        onlinePopUp.transform.GetChild(4).gameObject.SetActive(true);
                        //StartRoom();
                    }
                }

            }

            else
            {
                Debug.Log("Failed to get players " + error);
            }
        });
    }

    void LeaveRoom()
    {
        NetworkClient.Lobby.LeaveRoom((successful, error) => {
            if (successful)
            {
                Debug.Log("Left room");
                State = LobbyState.Default;
            }
            else
            {
                Debug.Log("Failed to leave room " + error);
            }
        });
    }
    //########## Lobby ###########//

    void OnNewPlayerJoinRoomEvent(SWJoinRoomEventData eventData)
    {
        if (NetworkClient.Lobby.IsOwner)
        {
            onlinePopUp.transform.GetChild(4).gameObject.SetActive(true);
        }
        GetPlayersInTheRoom();
    }
    private void OnPlayerLeaveRoomEvent(SWLeaveRoomEventData eventData)
    {
        GetPlayersInTheRoom();
    }


    private void OnRoomReadyEvent(SWRoomReadyEventData eventData)
    {
        ConnectToRoom();
    }



    void OnLobbyConnected() {
        RegisterToLobbyServer();
    }


    public void OnConfirmNickNameClicked()
    {
        nickname = NicknameInputField.text;
        onlinePopUp.transform.GetChild(5).gameObject.SetActive(false);
        onlinePopUp.transform.GetChild(2).gameObject.SetActive(false);
        CheckIn();
    }
    public void OnCancleBtnClicked()
    {
        if(State == LobbyState.JoinedRoom)
        {
            LeaveRoom();
        }
    }

    public void StartBtnClicked() {
        StartRoom();
    }

    void StartRoom()
    {
        NetworkClient.Lobby.StartRoom((successful, error) => {
            if (successful)
            {
                Debug.Log("Started room.");
            }
            else
            {
                Debug.Log("Failed to start room " + error);
            }
        });
    }

    public void changeNickname()
    {
        SaveSystem.SaveNickname("");
        SceneManager.LoadScene(0);
    }






    /////////////////// GET HASH CODE ///////////////////
    public static byte[] GetHash(string inputString)
    {
        using (HashAlgorithm algorithm = SHA256.Create())
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(inputString));
    }
    public static string GetHashString(string inputString)
    {
        System.Text.StringBuilder sb = new StringBuilder();
        foreach (byte b in GetHash(inputString))
            sb.Append(b.ToString("X2"));

        return sb.ToString();
    }

}
