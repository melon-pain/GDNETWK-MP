using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerReceive
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int _clientIdCheck = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"{Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");
        if (fromClient != _clientIdCheck)
        {
            Debug.Log($"Player \"{username}\" (ID: {fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
        }
        Server.clients[fromClient].SendIntoGame(username);

        packet.Dispose();
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }

        Server.clients[fromClient].player.MovementInput(inputs);

        packet.Dispose();
    }

    public static void PlayerProjectile(int fromClient, Packet packet)
    {
        bool input = packet.ReadBool();

        Server.clients[fromClient].player.ProjectileAbility(input);

        packet.Dispose();
    }

    public static void PlayerPod(int fromClient, Packet packet)
    {
        Vector3 mousePosition = packet.ReadVector3();

        float angle = (Mathf.Atan2(mousePosition.y, mousePosition.x) * Mathf.Rad2Deg) - 90.0f;
        Vector3 direction = new Vector3(0.0f, -angle, 0.0f);

        Server.clients[fromClient].player.RotatePod(Quaternion.Euler(direction));

        packet.Dispose();
    }
}
