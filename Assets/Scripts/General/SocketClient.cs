using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class SocketClient : MonoBehaviour
{
    Thread receiveThread; 
    UdpClient client; 
    int port;
    public static string dataString;

    void Start()
    {
        port = 5065;
        ExecuteCommand(); // Execute python server script
        InitUDP();
    }

    private void InitUDP()
    {
        Debug.Log("UDP Initialized");

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true; 
        receiveThread.Start();
    }

    // Execute cmd command for running python code
    public static void ExecuteCommand()
    {
        Debug.Log("Execute");
        if (SystemInfo.operatingSystem.Contains("Windows")) {
            //Cammand from Windows system
            // Calling python script
            string command = "cd Assets/Scripts/PythonScript & python OpenCV_FaceTracking.py";

            var processInfo = new System.Diagnostics.ProcessStartInfo()
            {
                // Calling python script
                FileName = "cmd.exe",
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                Arguments = "/S /C " + command

                // Runing executable python application
                //FileName = Environment.CurrentDirectory + "/PythonApplication/dist/OpenCV_FaceTracking/OpenCV_FaceTracking.exe",
                //CreateNoWindow = true,
                //UseShellExecute = true,
                //WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            System.Diagnostics.Process.Start(processInfo);
        }
        else
        {
            //Shell from OS/Linux system
            string command = "cd Assets/Scripts/PythonScript && python OpenCV_FaceTracking.py";
            var processInfo = new System.Diagnostics.ProcessStartInfo()
            {
                FileName = "/bin/bash",
                CreateNoWindow = true,
                UseShellExecute = true,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                Arguments = " -c \"" + command + " \""
            };
            System.Diagnostics.Process.Start(processInfo);
        }
        
    }

    private void ReceiveData()
    {
        client = new UdpClient(port); 
        while (true) 
        {
            try
            {

                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port); 
                byte[] data = client.Receive(ref anyIP);
                dataString = Encoding.UTF8.GetString(data);
                //print(dataString);
                Thread.Sleep(1);

            }
            catch (Exception e)
            {
                print(e.ToString());
            }
        }
    }

    void StopServer()
    {
        receiveThread.Abort();
        Debug.Log("Disconnected!");
    }

    public void OnDisable()
    {
        StopServer();
    }
}
