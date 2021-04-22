using ChessDotNet;
using ChessDotNet.Pieces;
using JetBrains.Annotations;
using SWNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerChess : MonoBehaviour
{
    NetCode netCode;

    [SerializeField]
    protected PlayerS localPlayer;
    [SerializeField]
    protected PlayerS remotePlayer;
    [SerializeField]
    protected PlayerS currentPlayer;
    [SerializeField]
    protected int turn;
    public enum GameState
    {
        Idle,GameStarted,TurnStarted,TurnSelectingMove,TurnConfirmedSelectedMove,CheckForCases, GameFinished
    };
    public GameState gameState = GameState.Idle;

    public enum CheckState
    {
        None, Check, CheckMate, Draw, Stalemate, WinnerW, WinnerB
    }
    public CheckState checkState = CheckState.None;

    public GameDataManager gameDataManager;

    public GameObject whiteTile;
    public GameObject blackTile;
    public GameObject king;
    public GameObject queen;
    public GameObject rook;
    public GameObject bishop;
    public GameObject knight;
    public GameObject pawn;
    public Material white;
    public Material black;
    public Material green;
    public Material brown;
    public Material gray;

    public ChessGame game;
    public Text textBox;
    public Text localTimer;
    public Text remoteTimer;
    public Text localName;
    public Text remoteName;
    float localTime;
    float remoteTime;
    public Text p1;
    public Text p2;
    public GameObject promotionPanel;
    GameObject[,] boardPosition;

    Piece chessPiece;
    IEnumerable<Move> validMoves;
    bool isPieceSelected = false;
    public GameObject selectedPiece;
    GameObject newPiece;
    bool promotion = false;
    string selectedMove;
    bool movemade = false;
    bool firstMove = false;
    private void Awake()
    {
        netCode = FindObjectOfType<NetCode>();

        NetworkClient.Lobby.GetPlayersInRoom((successful, reply, error) =>
        {
            if (successful)
            {

                foreach (SWPlayer swPlayer in reply.players)
                {
                    string playerName = swPlayer.GetCustomDataString();
                    string playerId = swPlayer.id;
                  
                    if (playerId.Equals(NetworkClient.Instance.PlayerId))
                    {
                        Debug.LogWarning("local "+playerId);
                        localPlayer.PlayerId = playerId;
                        localPlayer.PlayerName = playerName;
                        localTime = 300;
                    }
                    else
                    {
                        Debug.LogWarning("remote " + playerId);
                        remotePlayer.PlayerId = playerId;
                        remotePlayer.PlayerName = playerName;
                        remoteTime = 300;
                    }
                }
                gameDataManager = new GameDataManager(localPlayer, remotePlayer, NetworkClient.Lobby.RoomId);
                netCode.EnableRoomPropertyAgent();
            }
            else
            {
                Debug.Log("Failed to get players in room.");
            }

        });
    }

    private void Start()
    {
        game = new ChessGame();
        boardPosition = new GameObject[8, 8];

        textBox.text = "";
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {

                GameObject tile;
                Vector3 position = new Vector3(i - 3.5f, 0, j - 3.5f);
                if (i % 2 == 0)
                {
                    if (j % 2 == 0)
                        tile = Instantiate(blackTile, position, Quaternion.identity, transform);
                    else
                        tile = Instantiate(whiteTile, position, Quaternion.identity, transform);

                }
                else
                {
                    if (j % 2 == 0)
                        tile = Instantiate(whiteTile, position, Quaternion.identity, transform);
                    else
                        tile = Instantiate(blackTile, position, Quaternion.identity, transform);
                }
                boardPosition[i, j] = tile;
                tile.gameObject.name = getTilePositionToString(i, j);
                tile.GetComponent<TileScript>().setTilePosition(getTilePositionToString(i, j));
            }
        }
        PlacePieces();

    }
   
    private string getTilePositionToString(int i, int j)
    {
        string str = (char)(i + 65) + "" + (j + 1);
        return str;

    }
   
    public void RotateBoardToBlack()
    {
        transform.localRotation = Quaternion.Euler(0, 180, 0);
    }
    public void RotateBoardToWhite()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
    }
    private void SetTileToGray(string str)
    {
        GameObject gameObject = GameObject.Find(str);

        gameObject.transform.GetComponent<MeshRenderer>().material = gray;
    }
    private void SetTileToNormal()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {

                if (i % 2 == 0)
                {
                    if (j % 2 == 0)
                        boardPosition[i, j].transform.GetComponent<MeshRenderer>().material = green;
                    else
                        boardPosition[i, j].transform.GetComponent<MeshRenderer>().material = brown;

                }
                else
                {
                    if (j % 2 == 0)
                        boardPosition[i, j].transform.GetComponent<MeshRenderer>().material = brown;
                    else
                        boardPosition[i, j].transform.GetComponent<MeshRenderer>().material = green;
                }
            }
        }
    }
    public void PlacePieces()
    {

        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                chessPiece = game.GetPieceAt(new Position(boardPosition[i, j].GetComponent<TileScript>().getTilePosition()));

                foreach (Transform child in boardPosition[i, j].transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                if (chessPiece != null)
                {
                    boardPosition[i, j].GetComponent<TileScript>().setPiece(chessPiece.GetFenCharacter().ToString());


                    // boardPosition[i, j].GetComponent<TileScript>().setPiece( parser.BoardStateData.Ranks[i][j]);

                    GameObject piece;

                    if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "p")
                    {
                        piece = Instantiate(pawn, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = black;

                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "r")
                    {
                        piece = Instantiate(rook, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = black;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "n")
                    {
                        piece = Instantiate(knight, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = black;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "b")
                    {
                        piece = Instantiate(bishop, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = black;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "q")
                    {
                        piece = Instantiate(queen, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = black;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "k")
                    {
                        piece = Instantiate(king, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = black;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "P")
                    {
                        piece = Instantiate(pawn, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = white;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "R")
                    {
                        piece = Instantiate(rook, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = white;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "N")
                    {
                        piece = Instantiate(knight, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = white;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "B")
                    {
                        piece = Instantiate(bishop, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = white;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "Q")
                    {
                        piece = Instantiate(queen, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = white;
                    }
                    else if (boardPosition[i, j].gameObject.GetComponent<TileScript>().getPiece() == "K")
                    {
                        piece = Instantiate(king, boardPosition[i, j].transform);
                        piece.transform.GetChild(0).GetComponent<MeshRenderer>().material = white;
                    }
                }

            }
        }
    }

    public void MakePromotionMove(string pro)
    {
        string promotionValue = "";
        if(game.WhoseTurn == Player.White)
        {
            promotionValue = pro;
        }else if(game.WhoseTurn == Player.Black)
        {
            promotionValue = (char)((int)pro[0] + 32)+"" ;
            Debug.LogError(promotionValue);
        }
        promotion = false;
        promotionPanel.SetActive(false);

        Move move = new Move(selectedPiece.GetComponent<TileScript>().getTilePosition(), newPiece.GetComponent<TileScript>().getTilePosition(), game.WhoseTurn,promotionValue[0]);
        //Debug.Log(game.IsValidMove(move));
        if (game.IsValidMove(move))
        {
            Debug.Log(game.MakeMove(move, true));
            movemade = true;
            selectedMove = selectedPiece.GetComponent<TileScript>().getTilePosition() + newPiece.GetComponent<TileScript>().getTilePosition()+promotionValue;
        }
        PlacePieces();
        textBox.text = "";
    }
    public void UndoMove()
    {
        if (movemade)
        {
            game.Undo();
            PlacePieces();
            movemade = false;
        }
    }
    public string TimeInString(float value)
    {
        int min = (int)value / 60;
        int sec = (int)value % 60;
        return min+":"+sec;
    }
    private void Update()
    {
        remoteName.text = remotePlayer.PlayerName;
        localName.text = localPlayer.PlayerName;

        if (localPlayer == currentPlayer)
        {
            localTime -= Time.deltaTime;
        }
        if(remotePlayer == currentPlayer)
        {
            remoteTime -= Time.deltaTime;
        }
        remoteTimer.text = TimeInString(remoteTime);
        localTimer.text = TimeInString(localTime);

        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("pressed" + NetworkClient.Instance.IsHost);
            if (localPlayer ==currentPlayer)
            {
                Debug.Log("Your Turn");
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit rayHit;
                if (Physics.Raycast(ray, out rayHit, 100.0f))
                {
                    Debug.Log("Touched title "+ rayHit.collider.tag);
                    if (rayHit.collider.tag == "tile")
                    {

                        Debug.Log("Piece Selected");
                        SetTileToNormal();
                        chessPiece = game.GetPieceAt(new Position(rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition()));
                        if (isPieceSelected)
                        {
                            SetTileToNormal();
                            isPieceSelected = false;
                            if (game.GetPieceAt(new Position(selectedPiece.GetComponent<TileScript>().getTilePosition())) is Pawn)
                            {
                                //Debug.Log("position " + rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition()[1]);

                                if (rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition()[1] == '8' && game.WhoseTurn == Player.White)
                                {
                                    promotion = true;
                                }
                                else if (rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition()[1] == '1' && game.WhoseTurn == Player.Black)
                                {
                                    promotion = true;
                                }
                            }
                            Move move = null;
                            string prmotionValue = "";
                            if (promotion)
                            {
                                newPiece = rayHit.collider.gameObject;
                                promotionPanel.SetActive(true);

                            }
                            else
                            {
                                move = new Move(selectedPiece.GetComponent<TileScript>().getTilePosition(), rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition(), game.WhoseTurn);
                                //Debug.Log(game.IsValidMove(move));
                                
                                if (game.IsValidMove(move))
                                {
                                    Debug.Log(game.MakeMove(move, true));
                                    movemade = true;
                                    selectedMove = selectedPiece.GetComponent<TileScript>().getTilePosition() + rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition();
                                }
                                PlacePieces();
                                textBox.text = "";
                            }
                           
                            
                        }
                        else
                        {
                            if (chessPiece != null)
                            {
                                isPieceSelected = true;
                                selectedPiece = rayHit.collider.gameObject;
                                validMoves = game.GetValidMoves(new Position(rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition()));
                                foreach (Move a in validMoves)
                                {
                                    SetTileToGray(a.NewPosition.ToString());
                                }
                            }
                        }

                    }
                }
            }
            else
            {
                Debug.Log("Not Your Turn");
            }
        }
    }

    public void GameFlow()
    {
        switch (gameState)
        {
            case GameState.Idle:
                {
                    Debug.Log("IDEL");
                    break;
                }
            case GameState.GameStarted:
                {
                    Debug.Log("GameStarted");
                    OnGameStarted();
                    break;
                }
            case GameState.TurnStarted:
                {
                    Debug.Log("TurnStarted");
                    OnTurnStarted();
                    break;
                }
            case GameState.TurnSelectingMove:
                {
                    Debug.Log("TurnSelectingMove");
                    OnTurnSelectingMove();
                    break;
                }
            case GameState.TurnConfirmedSelectedMove:
                {
                    Debug.Log("TurnConfirmedSelectedMove");
                    OnTurnConfirmedSelectedMove();
                    break;
                }
            case GameState.CheckForCases:
                {
                    Debug.Log("CheckForCases");
                     CheckForCases();
                    break;
                }
            case GameState.GameFinished:
                {
                    Debug.Log("GameFinished");
                    //  OnGameFinished();
                    break;
                }
        }

    }
    void OnGameStarted()
    {
        if (NetworkClient.Instance.IsHost)
        {
            localPlayer.Pcolor = Player.White;
            remotePlayer.Pcolor = Player.Black;
        }
        else
        {
            localPlayer.Pcolor = Player.Black;
            remotePlayer.Pcolor = Player.White;
            RotateBoardToBlack();
        }
        

        if (NetworkClient.Instance.IsHost)
        {
            gameState = GameState.TurnStarted;

            gameDataManager.SetGameState(gameState);

            netCode.NotifyOtherPlayersGameStateChanged();
        }
    }
    void OnTurnStarted()
    {
        Debug.LogError("TurnStarted");
        if (NetworkClient.Instance.IsHost)
        {
            SwitchTurn();
            gameState = GameState.TurnSelectingMove;

            gameDataManager.SetCurrentTurnPlayer(currentPlayer);
            gameDataManager.SetGameState(gameState);

            netCode.ModifyGameData(gameDataManager.EncryptedData());
            netCode.NotifyOtherPlayersGameStateChanged();
        }

    }

    public void OnTurnSelectingMove()
    {
    }

    public void OnTurnConfirmedSelectedMove()
    {
        if (currentPlayer == remotePlayer)
        {
            Debug.Log("Remote Get Move" + gameDataManager.GetSelectedMove());
            string sMove = gameDataManager.GetSelectedMove();
            Move move = null;
            if (sMove.Length == 4)
             move = new Move(sMove.Substring(0, 2), sMove.Substring(2, 2), game.WhoseTurn);
            else if(sMove.Length ==5)
                move = new Move(sMove.Substring(0, 2), sMove.Substring(2, 2), game.WhoseTurn,sMove[4]);

            Debug.LogWarning(game.MakeMove(move, true));
            PlacePieces();
            if(remotePlayer.Pcolor == Player.Black)
            remoteTime=gameDataManager.GetBlackTime();
            else
                remoteTime = gameDataManager.GetWhiteTime();
        }

        if (NetworkClient.Instance.IsHost)
        {
            gameState = GameState.CheckForCases;
            gameDataManager.SetGameState(gameState);
            gameDataManager.SetFENCode(game.GetFen());
            netCode.ModifyGameData(gameDataManager.EncryptedData());
            netCode.NotifyOtherPlayersGameStateChanged();
        }
    }
    public void CheckForCases()
    {
        if (game.IsInCheck(Player.White))
        {
            Debug.LogWarning("Check " + game.WhoseTurn);
            textBox.text = "Check " + game.WhoseTurn;
            checkState = CheckState.Check;
            gameState = GameState.TurnStarted;
        }
        if (game.IsInCheck(Player.Black))
        {
            Debug.LogWarning("Check " + game.WhoseTurn);
            textBox.text = "Check " + game.WhoseTurn;
            checkState = CheckState.Check;
            gameState = GameState.TurnStarted;
        }
        if (game.IsCheckmated(game.WhoseTurn))
        {
            Debug.LogWarning("Check Mate " + game.WhoseTurn);
            textBox.text = "Check Mate " + game.WhoseTurn;
            checkState = CheckState.CheckMate;
            gameState = GameState.GameFinished;
        }
        if (game.IsDraw())
        {
            Debug.LogWarning("Draw " + game.WhoseTurn);
            textBox.text = "Draw " + game.WhoseTurn;
            checkState = CheckState.Draw;
            gameState = GameState.GameFinished;
        }

        if (game.IsStalemated(game.WhoseTurn))
        {
            Debug.LogWarning("Stalemated " + game.WhoseTurn);
            textBox.text = "Stalemated " + game.WhoseTurn;
            checkState = CheckState.Stalemate;
            gameState = GameState.GameFinished;
        }
        if (game.IsWinner(Player.White))
        {

            Debug.LogWarning("Winner " + "white");
            textBox.text = "Winner " + "white";
            checkState = CheckState.WinnerW;
            gameState = GameState.GameFinished;
        }
        if (game.IsWinner(Player.Black))
        {
            Debug.LogWarning("Winner " + "black");
            textBox.text = "Winner " + "black";
            checkState = CheckState.WinnerB;
            gameState = GameState.GameFinished;
        }

        if (NetworkClient.Instance.IsHost)
        {

            if (checkState == CheckState.None || checkState == CheckState.Check)
            {
                gameDataManager.SetTurn(gameDataManager.GetTurn()+1);
                gameState = GameState.TurnStarted;
            }

            gameDataManager.SetGameState(gameState);
            netCode.ModifyGameData(gameDataManager.EncryptedData());
            netCode.NotifyOtherPlayersGameStateChanged();
        }


    }

    public void SwitchTurn()
    {
        if (currentPlayer == null) {
            if (localPlayer.Pcolor == Player.White)
                currentPlayer = localPlayer;
            else
                currentPlayer = remotePlayer;
            Debug.LogError("Switched Turn first time to " + currentPlayer.PlayerName);
            return;
        }
        
        if (currentPlayer == localPlayer)
        {
            currentPlayer = remotePlayer;
            Debug.LogError("Switched Turn to " + currentPlayer.PlayerName);
        }
        else
        {
            currentPlayer = localPlayer;
            Debug.LogError("Switched Turn to " + currentPlayer.PlayerName);
        }
    }

    //****************** User Interaction *********************//
   
    public void confirmSelectionBtnClick()
    {
        if (gameState == GameState.TurnSelectingMove && localPlayer == currentPlayer)
        {
            
            Debug.Log("Confirm Button");
            if (selectedMove != null)
            {
                Debug.Log("Confirm Button Move "+ selectedMove);
                movemade = false;
                if(localPlayer.Pcolor == Player.Black)
                {
                    gameDataManager.SetBlackTime(localTime);
                }
                else
                {
                    gameDataManager.SetWhiteTime(localTime);
                }
                netCode.ModifyGameData(gameDataManager.EncryptedData());
                netCode.NotifyHostPlayerMoveSelected(selectedMove);
                selectedMove = null; 
            }
        }
        else if (gameState == GameState.GameFinished)
        {
            netCode.LeaveRoom();
        }
    }


    //****************** NetCode Events *********************//
    public void OnGameDataReady(EncryptedData encryptedData)
    {
        Debug.Log("You were in ReadyMode");
        if (encryptedData == null)
        {
            Debug.Log("New game");
            if (NetworkClient.Instance.IsHost)
            {
                gameState = GameState.GameStarted;
                gameDataManager.SetGameState(gameState);
                gameDataManager.SetFENCode(game.GetFen());
                gameDataManager.SetWhiteTime(300);
                gameDataManager.SetBlackTime(300);                
                localTime= gameDataManager.GetBlackTime();
                remoteTime= gameDataManager.GetWhiteTime();


                netCode.ModifyGameData(gameDataManager.EncryptedData());

                netCode.NotifyOtherPlayersGameStateChanged();
            }
        }
        else
        {
            gameDataManager.ApplyEncrptedData(encryptedData);
            gameState = gameDataManager.GetGameState();
            currentPlayer = gameDataManager.GetCurrentTurnPlayer();
            selectedMove = gameDataManager.GetSelectedMove();

            if (gameState > GameState.GameStarted)
            {
                Debug.Log("Restore the game state");
                game = new ChessGame(gameDataManager.GetFENCode());
                PlacePieces();
            }
            if (NetworkClient.Instance.PlayerId == currentPlayer.PlayerId)
            {
                if (game.WhoseTurn == Player.White) {
                    localPlayer.Pcolor = Player.White;
                    remotePlayer.Pcolor = Player.Black;
                }
                else
                {
                    localPlayer.Pcolor = Player.Black;
                    remotePlayer.Pcolor = Player.White;
                }
            }else
            {
                if (game.WhoseTurn == Player.White)
                {
                    localPlayer.Pcolor = Player.Black;
                    remotePlayer.Pcolor = Player.White;
                }
                else
                {
                    localPlayer.Pcolor = Player.White;
                    remotePlayer.Pcolor = Player.Black;
                }
            }
            if (localPlayer.Pcolor == Player.Black)
                RotateBoardToBlack();
            else
                RotateBoardToWhite();

            if (gameState > GameState.GameStarted)
            {
                GameFlow();
            }
        }
    }

    public void OnGameDataChanged(EncryptedData encryptedData)
    {
        Debug.LogWarning("OnGameDataChanged");
        gameDataManager.ApplyEncrptedData(encryptedData);
        gameState = gameDataManager.GetGameState();
        if (localPlayer.Pcolor == Player.Black)
        {
            localTime = gameDataManager.GetBlackTime();
            remoteTime = gameDataManager.GetWhiteTime();
        }
        else
        {
            remoteTime = gameDataManager.GetBlackTime();
            localTime = gameDataManager.GetWhiteTime();
        }
        currentPlayer = gameDataManager.GetCurrentTurnPlayer();
        //selectedMove = gameDataManager.GetSelectedMove();
    }

    public void OnGameStateChanged()
    {
        Debug.LogWarning("Game State changed");
        GameFlow();
    }

    public void OnMoveSelected(string move)
    {
        gameDataManager.SetSelectedMove(move);
        Debug.LogWarning("Move "+NetworkClient.Instance.IsHost+" |"+move);
        //selectedMove = move;
        gameState = GameState.TurnConfirmedSelectedMove;
        gameDataManager.SetGameState(gameState);

        netCode.ModifyGameData(gameDataManager.EncryptedData());
        netCode.NotifyOtherPlayersGameStateChanged();
    }

    public void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }
}
