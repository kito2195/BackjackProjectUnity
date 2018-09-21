using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public GameObject chatContainer;
    public GameObject messagePrefab;

    private bool socketReady;
    private TcpClient socket;
    private NetworkStream stream;
    private StreamWriter streamWriter;
    private StreamReader streamReader;

    public void ConnectToServer()
    {
        //if already connected, ignore this function
        if (socketReady) { return; }

        // default host / port values
        string host = "127.0.0.1";
        int port = 6321;

        // Override default host / port values, if there is something in those boxes
        string hostInput;
        int portInput;

        hostInput = GameObject.Find("HostInput").GetComponent<InputField>().text;
        if (hostInput != "")
        {
            host = hostInput;
        }

        int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out portInput);
        if (portInput != 0)
        {
            port = portInput;
        }

        // Create the socket
        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            streamWriter = new StreamWriter(stream);
            streamReader = new StreamReader(stream);
            socketReady = true;
        }
        catch (Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }

    private void Update()
    {
        if (socketReady)
        {           
            if (stream.DataAvailable)
            {
                string data = streamReader.ReadLine();
                if (data != null)
                {
                    OnIncomingData(data);
                }
            }
        }
    }

    private void OnIncomingData(string data)
    {
        GameObject newMessageObject = Instantiate(messagePrefab, chatContainer.transform) as GameObject;
        newMessageObject.GetComponentInChildren<Text>().text = data;
    }

    private void Send(string data)
    {
        if (!socketReady){ return; }

        streamWriter.WriteLine(data);
        streamWriter.Flush();
    }

    public void OnSendButton()
    {
        string message = GameObject.Find("SendInput").GetComponent<InputField>().text;
        Send(message);  
    }
}
