using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Server
{
    public static int MaxPlayers { get; private set; }
    public static int Port { get; private set; }

    public static Dictionary<int, Client> clients = new Dictionary<int, Client>();
    public delegate void PacketHandler(int fromClient, Packet packet);
    public static Dictionary<int, PacketHandler> packetHandlers;

    private static TcpListener tcpListener;

    public static void Start(int inMaxPlayers, int inPort)
    {
        MaxPlayers = inMaxPlayers;
        Port = inPort;

        Debug.Log("Starting server...");
        InitializeServerData();

        tcpListener = new TcpListener(IPAddress.Any, Port);
        tcpListener.Start();
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);

        Debug.Log($"Server started on port {Port}.");
    }

    /// <summary>Initializes all necessary server data.</summary>
    private static void InitializeServerData()
    {
        for (int i = 1; i <= MaxPlayers; i++)
        {
            clients.Add(i, new Client(i));
        }

        packetHandlers = new Dictionary<int, PacketHandler>()
        {
            { (int)ClientPackets.welcomeReceived, ServerReceive.WelcomeReceived },
            { (int)ClientPackets.playerMovement, ServerReceive.PlayerMovement },
            { (int)ClientPackets.playerProjectile, ServerReceive.PlayerProjectile },
            { (int)ClientPackets.playerPod, ServerReceive.PlayerPod },
        };

        Debug.Log("Initialized packets.");
    }

    /// <summary>Handles new TCP connections.</summary>
    private static void TCPConnectCallback(IAsyncResult result)
    {
        TcpClient client = tcpListener.EndAcceptTcpClient(result);
        tcpListener.BeginAcceptTcpClient(TCPConnectCallback, null);
        Debug.Log($"Incoming connection from {client.Client.RemoteEndPoint}...");

        for (int i = 1; i <= MaxPlayers; i++)
        {
            if (clients[i].tcp.socket == null)
            {
                clients[i].tcp.Connect(client);
                clients[i].isConnected = true;
                return;
            }
        }

        Debug.LogError($"{client.Client.RemoteEndPoint} failed to connect: Server full!");
    }

    public static void Stop()
    {
        tcpListener.Stop();

        Debug.Log("Server closed.");
    }

    public static bool AreAnyClientsConnected()
    {
        foreach(Client client in clients.Values)
        {
            if (client.isConnected)
                return true;
        }

        return false;
    }
}
