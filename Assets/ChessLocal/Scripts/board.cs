using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ChessDotNet;
using System.Security.Cryptography;
using ChessDotNet.Pieces;
using UnityEngine.UI;

public class board : MonoBehaviour
{
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
    public InputField fenCode;
    public Text TextBox;
    public GameObject[,] boardPosition;
    public ChessGame game;
    public Piece chessPiece; 
    IEnumerable<Move> validMoves;
    public bool isPieceSelected = false;
    public GameObject selectedPiece;
    public bool  promotion= false;
    public GameObject gameEngine;
    // public FenParser.FenParser parser;
    public void Start()
    {

        game = new ChessGame();
        Debug.Log(game.GetValidMoves(Player.Black));
        

        boardPosition = new GameObject[8,8];
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i=0; i < 8; i++)
        {
            for(int j = 0; j < 8; j++)
            {
                
                GameObject tile;
                Vector3 position = new Vector3(i-3.5f,0,j-3.5f);
                if (i % 2 == 0)
                {
                    if(j % 2 == 0)
                    tile = Instantiate(blackTile, position,Quaternion.identity,transform);
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
                tile.GetComponent<TileScript>().setTilePosition(getTilePositionToString(i,j));
            }
        }
       PlacePieces();
        UpdateFenCode();
    }

    public void RotateBoard() {
        transform.Rotate(0,180,0,Space.Self);
    }
    private string getTilePositionToString(int i,int j)
    {
        string str = (char)(i + 65) + "" + (j + 1);
        return str;

    }

    public void SetFenCode()
    {
        
        game = new ChessGame(fenCode.text);
        PlacePieces();
        UpdateFenCode();
    }
    public void NewMatch()
    {
        game = new ChessGame();
        PlacePieces();
        UpdateFenCode();
    }

    public void GetFenCode()
    {
        Debug.Log(game.GetFen());
    }

    public void UpdateFenCode()
    {
        string fen = game.GetFen();
        fenCode.text = fen;
    }
   
    
    public void PlacePieces()
    {
        
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                chessPiece= game.GetPieceAt( new Position(boardPosition[i, j].GetComponent<TileScript>().getTilePosition()));

                foreach (Transform child in boardPosition[i, j].transform)
                {
                    GameObject.Destroy(child.gameObject);
                }

                if (chessPiece != null)
                {
                    boardPosition[i,j].GetComponent<TileScript>().setPiece( chessPiece.GetFenCharacter().ToString());


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



    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100.0f))
            {
                if (rayHit.collider.tag == "tile")
                {
                    SetTileToNormal();
                    chessPiece =game.GetPieceAt(new Position(rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition()));
                    if (isPieceSelected)
                    {
                        SetTileToNormal();
                        isPieceSelected = false;
                        if(game.GetPieceAt(new Position(selectedPiece.GetComponent<TileScript>().getTilePosition())) is Pawn)
                        {
                            Debug.Log("position "+ rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition()[1]);
                      
                             if(rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition()[1] == '8' && game.WhoseTurn == Player.White)
                            {
                                promotion = true;
                            }else if(rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition()[1] == '1' && game.WhoseTurn == Player.Black)
                            {
                                promotion = true;
                            }
                        }
                        Move move =null;

                        if (promotion)
                        {
                            if(game.WhoseTurn==Player.White)
                            move = new Move(selectedPiece.GetComponent<TileScript>().getTilePosition(), rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition(), game.WhoseTurn, 'R');
                            else if (game.WhoseTurn == Player.Black)
                                move = new Move(selectedPiece.GetComponent<TileScript>().getTilePosition(), rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition(), game.WhoseTurn, 'q');
                            promotion = false;
                        }
                        else
                            move = new Move(selectedPiece.GetComponent<TileScript>().getTilePosition(), rayHit.collider.gameObject.GetComponent<TileScript>().getTilePosition(), game.WhoseTurn);

                        Debug.Log(game.IsValidMove(move));
                        if (game.IsValidMove(move))
                            Debug.Log(game.MakeMove(move, true));
                        PlacePieces();
                        TextBox.text = "";
                        UpdateFenCode();
                        CheckForCondition(); 
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
    }

    public void CheckForCondition()
    {
        if (game.IsInCheck(game.WhoseTurn))
        {
            Debug.LogWarning("Check " + game.WhoseTurn);
            TextBox.text = "Check " + game.WhoseTurn;
        }
        if (game.IsCheckmated(game.WhoseTurn))
        {
            Debug.LogWarning("Check Mate " + game.WhoseTurn);
            TextBox.text = "Check Mate " + game.WhoseTurn;
        }
        if (game.IsDraw())
        {
            Debug.LogWarning("Draw " + game.WhoseTurn);
            TextBox.text = "Draw " + game.WhoseTurn;
        }

        if (game.IsStalemated(game.WhoseTurn))
        {
            Debug.LogWarning("Stalemated " + game.WhoseTurn);
            TextBox.text = "Stalemated " + game.WhoseTurn;
        }
        if (game.IsWinner(Player.White))
        {

            Debug.LogWarning("Winner " + "white");
            TextBox.text = "Winner " + "white";
        }
        if (game.IsWinner(Player.Black))
        {
            Debug.LogWarning("Winner " + "black");
            TextBox.text = "Winner " + "black";
        }
    }

    public void movePlayer(string str)
    {
        string currentPos =((char)((int)str[9]-32)).ToString()+ str[10].ToString();
        string nextPos = ((char)((int)str[11] - 32)).ToString() + str[12].ToString();
        Move move=null;
        if (str.Length > 13 )
        {
            if (str[13] == 'q')
            {
                move = new Move(currentPos, nextPos, game.WhoseTurn, 'q');
            }
            else if (str[13] == 'Q')
            {
                move = new Move(currentPos, nextPos, game.WhoseTurn, 'Q');
            }
            else
            {
                move = new Move(currentPos, nextPos, game.WhoseTurn);
            }
        }
        else
            move = new Move(currentPos, nextPos, game.WhoseTurn);
        if (game.IsValidMove(move))
        {
            Debug.Log(game.MakeMove(move, true));
        }
        PlacePieces();
        TextBox.text = "";
        UpdateFenCode();

        CheckForCondition();

    }

    private void SetTileToGray(string str)
    {
        GameObject gameObject = GameObject.Find(str);

        gameObject.transform.GetComponent<MeshRenderer>().material = gray;
    }

    public void UndoGame()
    {
        game.Undo();
        PlacePieces();
        UpdateFenCode();

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
                        boardPosition[i,j].transform.GetComponent<MeshRenderer>().material = green;
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
}
