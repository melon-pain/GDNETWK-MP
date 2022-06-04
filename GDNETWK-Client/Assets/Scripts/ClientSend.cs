using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientSend : MonoBehaviour
{
    /// <summary>Sends a packet to the server via TCP.</summary>
    /// <param name="packet">The packet to send to the sever.</param>
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();
        Client.Instance.tcp.SendData(packet);
    }

    public static void WelcomeReceived()
    {
        using Packet packet = new Packet((int)ClientPackets.welcomeReceived);
        packet.Write(Client.Instance.ID);
        packet.Write(UIManager.Instance.usernameField.text);

        SendTCPData(packet);
    }

    public static void PlayerMovement(bool[] inputs)
    {
        using Packet packet = new Packet((int)ClientPackets.playerMovement);

        packet.Write(inputs.Length);
        foreach (bool input in inputs)
        {
            packet.Write(input);
        }

        SendTCPData(packet);
    }

    public static void PlayerProjectile(bool input)
    {
        using Packet packet = new Packet((int)ClientPackets.playerProjectile);
        packet.Write(input);

        SendTCPData(packet);
    }

    public static void PodRotation(Quaternion rotation)
    {
        using Packet packet = new Packet((int)ClientPackets.playerPod);
        packet.Write(rotation);

        SendTCPData(packet);
    }
}
