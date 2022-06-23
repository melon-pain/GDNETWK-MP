using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    /// <summary>Sends a packet to a client via TCP.</summary>
    /// <param name="toClient">The client to send the packet the packet to.</param>
    /// <param name="packet">The packet to send to the client.</param>
    private static void SendTCPData(int toClient, Packet packet)
    {
        packet.WriteLength();
        Server.clients[toClient].tcp.SendData(packet);
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="packet">The packet to send.</param>
    private static void SendTCPDataToAll(Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(packet);
        }
    }

    /// <summary>Sends a packet to all clients via TCP.</summary>
    /// <param name="packet">The packet to send.</param>
    private static void SendTCPDataToAll(int exclude, Packet packet)
    {
        packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != exclude)
                Server.clients[i].tcp.SendData(packet);
        }
    }

    /// <summary>Sends a welcome message to the given client.</summary>
    /// <param name="toClient">The client to send the packet to.</param>
    /// <param name="message">The message to send.</param>
    public static void Welcome(int toClient, string message)
    {
        //Create a welcome packet
        using Packet packet = new Packet((int)ServerPackets.welcome);
        packet.Write(message);
        packet.Write(toClient);

        SendTCPData(toClient, packet);
    }

    /// <summary>Tells a client to spawn a player.</summary>
    /// <param name="toClient">The client that should spawn the player.</param>
    /// <param name="player">The player to spawn.</param>
    public static void SpawnPlayer(int toClient, Player player)
    {
        using Packet packet = new Packet((int)ServerPackets.spawnPlayer);
        packet.Write(player.ID);
        packet.Write(player.username);
        packet.Write(player.transform.position);
        packet.Write(player.transform.rotation);

        SendTCPData(toClient, packet);
    }

    public static void PlayerDisconnected(int playerID)
    {
        using Packet packet = new Packet((int)ServerPackets.playerDisconnected);
        packet.Write(playerID);

        SendTCPDataToAll(packet);
    }

    public static void PlayerTransform(Player player)
    {
        using Packet packet = new Packet((int)ServerPackets.playerTransform);
        packet.Write(player.ID);
        packet.Write(player.transform.position);
        packet.Write(player.transform.rotation);
        packet.Write(player.IsMoving);

        SendTCPDataToAll(packet);
    }
    
    public static void PlayerHealth(Player player)
    {
        using Packet packet = new Packet((int)ServerPackets.playerHealth);
        packet.Write(player.ID);
        packet.Write(player.Health);

        SendTCPDataToAll(packet);
    }

    public static void SpawnProjectile(Projectile projectile)
    {
        using Packet packet = new Packet((int)ServerPackets.spawnProjectile);
        packet.Write(projectile.ID);
        packet.Write((int)projectile.Source);
        packet.Write(projectile.transform.position);
        packet.Write(projectile.transform.rotation);

        SendTCPDataToAll(packet);
    }

    public static void ProjectileTransform(Projectile projectile)
    {
        using Packet packet = new Packet((int)ServerPackets.projectileTransform);
        packet.Write(projectile.ID);
        packet.Write(projectile.transform.position);

        SendTCPDataToAll(packet);
    }

    public static void ProjectileDestroyed(Projectile projectile)
    {
        using Packet packet = new Packet((int)ServerPackets.projectileDestroyed);
        packet.Write(projectile.ID);

        SendTCPDataToAll(packet);
    }

    public static void SpawnEnemy(Enemy enemy)
    {
        using Packet packet = new Packet((int)ServerPackets.spawnEnemy);
        packet.Write(enemy.ID);
        packet.Write(enemy.transform.position);
        packet.Write(enemy.transform.rotation);

        SendTCPDataToAll(packet);
    }

    public static void SpawnItem(Item item)
    {
        using Packet packet = new Packet((int)ServerPackets.spawnItem);
        packet.Write(item.ID);
        packet.Write(item.transform.position);

        SendTCPDataToAll(packet);
    }

    public static void DestroyItem(Item item)
    {
        using Packet packet = new Packet((int)ServerPackets.destroyItem);
        packet.Write(item.ID);
        packet.Write(item.PickedUp);

        SendTCPDataToAll(packet);
    }

    public static void SpawnEnemyToClient(int playerID, Enemy enemy)
    {
        using Packet packet = new Packet((int)ServerPackets.spawnEnemy);
        packet.Write(enemy.ID);
        packet.Write(enemy.transform.position);
        packet.Write(enemy.transform.rotation);

        SendTCPData(playerID, packet);
    }

    public static void EnemyTransform(Enemy enemy)
    {
        using Packet packet = new Packet((int)ServerPackets.enemyTransform);
        packet.Write(enemy.ID);
        packet.Write(enemy.transform.position);
        packet.Write(enemy.transform.rotation);

        SendTCPDataToAll(packet);
    }

    public static void EnemyHealth(Enemy enemy)
    {
        using Packet packet = new Packet((int)ServerPackets.enemyHealth);
        packet.Write(enemy.ID);
        packet.Write(enemy.Health);

        SendTCPDataToAll(packet);
    }

    public static void EnemyDestroyed(Enemy enemy)
    {
        using Packet packet = new Packet((int)ServerPackets.enemyDestroyed);
        packet.Write(enemy.ID);

        SendTCPDataToAll(packet);
    }

    public static void PodTransform(Player player)
    {
        using Packet packet = new Packet((int)ServerPackets.podTransform);
        packet.Write(player.ID);
        packet.Write(player.Pod.transform.rotation);

        SendTCPDataToAll(player.ID, packet);
    }

    public static void PlayerDeath(Player player)
    {
        using Packet packet = new Packet((int)ServerPackets.playerDeath);
        packet.Write(player.ID);

        SendTCPDataToAll(packet);
    }

    public static void PlayerRespawned(Player player)
    {
        using Packet packet = new Packet((int)ServerPackets.playerRespawned);
        packet.Write(player.ID);

        SendTCPDataToAll(packet);
    }
}
