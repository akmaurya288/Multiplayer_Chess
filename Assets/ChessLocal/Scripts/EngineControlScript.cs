using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class EngineControlScript : MonoBehaviour
{
    public GameObject chessboard;
    public Text logText;

    ProcessStartInfo startInfo = new ProcessStartInfo();
    Process process = new Process();

    private void Start()
    {
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardInput = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.RedirectStandardError = false;
        startInfo.CreateNoWindow = true;
#if UNITY_EDITOR
        startInfo.FileName = Application.streamingAssetsPath + "/stockfish_20011801_x64.exe";
#elif UNITY_STANDALONE
        startInfo.FileName = Application.streamingAssetsPath + "/stockfish_20011801_x64.exe";
#elif UNITY_ANDROID
        startInfo.FileName = "/data/data/com.Krishna.Limt.Chess/lib" + "/libstockfish-10-armv7.so";
#endif

    }

    public void RunProcess()
    {
        try
        {
            string output;
            process.StartInfo = startInfo;
            process.Start();

            //process.StandardInput.WriteLine("uci");
            //process.StandardInput.WriteLine("isready");

            process.StandardInput.WriteLine("position fen " + chessboard.GetComponent<board>().game.GetFen());
            process.StandardInput.WriteLine("go deep 200");
            //process.StandardInput.WriteLine("stop");
            //process.StandardInput.WriteLine("quit");

            do
            {
                output = process.StandardOutput.ReadLine();
                if (!output.Contains("Stockfish"))

                    if (output.Contains("bestmove"))
                        logText.text += output + "\n\n";
                    else
                        logText.text += output + "\n";

            } while (!output.Contains("bestmove"));


            if (!output.Contains("none"))
            {
                UnityEngine.Debug.Log(output);
                chessboard.GetComponent<board>().movePlayer(output);
            }
            process.Close();
        }catch(Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
    }
}





//public void RunProcess()
//{
//    ProcessStartInfo startInfo = new ProcessStartInfo();
//    startInfo.UseShellExecute = false;
//    startInfo.RedirectStandardInput = true;
//    startInfo.RedirectStandardOutput = true;
//    startInfo.RedirectStandardError = false;
//    startInfo.CreateNoWindow = true;
//    startInfo.FileName = Application.streamingAssetsPath + "/stockfish_20011801_x64.exe";

//    Process process = new Process();
//    process.StartInfo = startInfo;
//    process.Start();

//    string output;

//    process.StandardInput.WriteLine("uci");
//    process.StandardInput.WriteLine("isready");
//    process.StandardInput.WriteLine("position fen " + chessboard.GetComponent<board>().game.GetFen());
//    UnityEngine.Debug.Log(chessboard.GetComponent<board>().game.GetFen());
//    //process.StandardInput.WriteLine("position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
//    process.StandardInput.WriteLine("go wtime 122000 btime 120000 winc 2000 binc 2000");
//    process.StandardInput.WriteLine("stop");
//    process.StandardInput.WriteLine("quit");

//    do
//    {
//        output = process.StandardOutput.ReadLine();
//        UnityEngine.Debug.Log(output);
//    } while (!output.Contains("move"));

//    UnityEngine.Debug.Log(output);
//    chessboard.GetComponent<board>().movePlayer(output);
//    process.Close();
//}


//public void RunProcess()
//{
//    var p = new Process();
//    p.StartInfo.FileName = Application.streamingAssetsPath + "/stockfish_20011801_x64.exe";
//    p.StartInfo.UseShellExecute = false;
//    p.StartInfo.RedirectStandardInput = true;
//    p.StartInfo.RedirectStandardOutput = true;

//    p.Start();

//    string output;

//    p.StandardInput.WriteLine("position fen " + chessboard.GetComponent<board>().game.GetFen());
//    UnityEngine.Debug.Log(chessboard.GetComponent<board>().game.GetFen());
//    //process.StandardInput.WriteLine("position fen rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1");
//    p.StandardInput.WriteLine("go movetime 5000");

//    do
//    {
//        output = p.StandardOutput.ReadLine();
//        UnityEngine.Debug.Log(output);
//    } while (!output.Contains("move"));

//    UnityEngine.Debug.Log(output);
//    chessboard.GetComponent<board>().movePlayer(output);
//    p.Close();
//}