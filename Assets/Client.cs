using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{

    private Socket _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
    private byte[] _recieveBuffer = new byte[8142];
    public static float XReceived = 0;
    public static float YReceived = 0;
    public static String URLReceived = "";
    public static int ImageNumber = -1;
    public static bool isSocketReady = false;
    public static bool isURLReceived = false;
    public static bool isImageReceived = false;
    public string ip = "192.168.1.12";
    public void SetupServer()
    {
        try
        {
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _clientSocket.Connect(new IPEndPoint(IPAddress.Parse(ip) , 1234));
            _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
            isSocketReady = true;
        }
        catch (SocketException ex)
        {
            Debug.Log(ex.Message);
        }

        

    }

    public void CloseSocket()
    {
        if (!isSocketReady)
            return;
        try
        {
            _clientSocket.Close();
            _clientSocket = null;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

    }

    private void ReceiveCallback(IAsyncResult AR)
    {
        //Check how much bytes are recieved and call EndRecieve to finalize handshake
        int recieved = _clientSocket.EndReceive(AR);

        if (recieved <= 0)
            return;

        //Copy the recieved data into new buffer , to avoid null bytes
        byte[] recData = new byte[recieved];
        Buffer.BlockCopy(_recieveBuffer, 0, recData, 0, recieved);

        //Process data here the way you want , all your bytes will be stored in recData
        string Receiver = Encoding.ASCII.GetString(recData);
        string[] tokens = Receiver.Split(Constant.TOKEN_END.ToCharArray());

        string keyWord = tokens[0].Substring(0, 5);

        switch (keyWord)
        {
            case Constant.TOKEN_BEGIN_POSITION + ":":
                extractCoordination(tokens[0]);
                break;
            case Constant.TOKEN_BEGIN_URL + ":":
                extractURL(tokens[0]);
                break;
            case Constant.TOKEN_BEGIN_GET + ":":
                extractImageNumber(tokens[0]);
                break;
        }

        //Start receiving again
        _clientSocket.BeginReceive(_recieveBuffer, 0, _recieveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    }

    private void extractImageNumber(string v)
    {
        ImageNumber = int.Parse( v.Substring(5, v.Length - 5));
        isImageReceived = true;
    }

    private void extractURL(string v)
    {
        URLReceived = v.Substring(5, v.Length - 5);
        isURLReceived = true;
        MoveCameraController.isLoaded = true;
    }

    private void extractCoordination(String buffer)
    {   
        String data = buffer.Substring(5, buffer.Length - 5);
        String[] res = data.Split(Constant.TOKEN_SPLIT.ToCharArray());
        XReceived = float.Parse(res[0]);
        YReceived = float.Parse(res[1]);
        MoveCameraController.isLoaded = true;
    }

    public static Vector2 getReceivedCoord()
    {
        return new Vector2(XReceived, YReceived);
    }

    public void SendData(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        SocketAsyncEventArgs socketAsyncData = new SocketAsyncEventArgs();
        socketAsyncData.SetBuffer(data, 0, data.Length);
        _clientSocket.SendAsync(socketAsyncData);
    }
    
}
