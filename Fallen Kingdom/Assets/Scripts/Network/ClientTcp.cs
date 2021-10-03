using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using KingdomData;

public class ClientTcp
{
    public TcpClient socket;
    public NetworkStream nStream;
    public byte[] bufferBytes;
    public bool isDisconect = true;

    public int timeOut = 0;

    public ClientTcp()
    {
        try
        {
            socket = new TcpClient(AddressFamily.InterNetwork);
            IPAddress ip = IPAddress.Parse("127.0.0.1");

            socket.ReceiveBufferSize = 4096;
            socket.SendBufferSize = 4096;
            bufferBytes = new byte[8192];

            socket.BeginConnect(ip, 5555, new AsyncCallback(ConnectCallback), socket);
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void ConnectCallback(IAsyncResult result)
    {
        socket.EndConnect(result);
        nStream = socket.GetStream();
        isDisconect = false;
        nStream.BeginRead(bufferBytes, 0, bufferBytes.Length, DataCallback, null);
    }
    public void DataCallback(IAsyncResult result)
    {
        int lng = nStream.EndRead(result);

        if (lng <= 0)
            return;

        Buffer.BlockCopy(bufferBytes, 0, bufferBytes, 0, lng);
        ByteBuffer data = new ByteBuffer();
        data.WriteBytes(bufferBytes);
        nStream.BeginRead(bufferBytes, 0, bufferBytes.Length, DataCallback, null);
        int input = data.ReadInteger();
        if (BundleManager.packets.ContainsKey(input))
            BundleManager.packets[input](data);
        else
            Debug.LogError("Input: " + input + " is null in Packets");
    }
    public void SendDataToServer(ByteBuffer data)
    {
        try
        {
             if (nStream != null)
                nStream.Write(data.ToArray(), 0, data.Count());
        }
        catch (Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }
    public void SendToServer(ByteBuffer data, PacketsConnection.PacketServer packet, int input)
    {
        ByteBuffer send = new ByteBuffer();
        send.WriteInteger((int)packet);
        send.WriteInteger(input);
        send.WriteBytes(data.ToArray());
        SendDataToServer(send);
    }
    public void Veryfication()
    {
        ByteBuffer data = new ByteBuffer();
        data.WriteInteger((int)PacketsConnection.PacketServer.Veryfication);

        try
        {
            if (nStream != null && socket.Connected)
                nStream.Write(data.ToArray(), 0, data.Count());
        }
        catch (Exception ex)
        {
            Debug.Log("ERROR : " + ex.ToString());
        }

    }

    public void Disconect()
    {
        isDisconect = true;

        Debug.Log("Active Scene: " + ClientManager.activeScene);
        ClientManager.SetScene("Menu");

         if(socket!= null)
            SendToServer(new ByteBuffer(), PacketsConnection.PacketServer.Disconect, 0);

        if (socket != null)
            socket.Close();

        if (nStream != null)
        {
            nStream.Close();
            nStream.Dispose();
        }

        timeOut = 0;
        Debug.Log("Client disconect");
    }

}
