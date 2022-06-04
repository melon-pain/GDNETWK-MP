using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class ClientReceive : MonoBehaviour
{
    public static void Welcome(Packet packet)
    {
        string message = packet.ReadString();
        int id = packet.ReadInt();

        Debug.Log($"Message from server: {message}");
        Client.Instance.ID = id;
        ClientSend.WelcomeReceived();
    }

    public static void SpawnPlayer(Packet packet)
    {
        int id = packet.ReadInt();
        string username = packet.ReadString();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.Instance.SpawnPlayer(id, username, position, rotation);
    }

    public static void PlayerTransform(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        bool isMoving = packet.ReadBool();

        if (GameManager.players.TryGetValue(id, out Player player))
        {
            player.UpdateTransform(position, rotation);
            player.Animator.SetBool("IsMoving", isMoving);
        }
    }

    public static void PlayerDisconnected(Packet packet)
    {
        int id = packet.ReadInt();

        Destroy(GameManager.players[id].gameObject);
        GameManager.players.Remove(id);
    }

    public static void PlayerHealth(Packet packet)
    {
        int id = packet.ReadInt();
        float health = packet.ReadFloat();

        if (GameManager.players.TryGetValue(id, out Player player))
        {
            player.Health = health;
        }
    }

    public static void SpawnProjectile(Packet packet)
    {
        int id = packet.ReadInt();
        int source = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.Instance.SpawnProjectile(id, source, position, rotation);
    }

    public static void ProjectileTransform(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();

        if (GameManager.projectiles.TryGetValue(id, out Projectile projectile))
        {
            projectile.UpdatePosition(position);
        }
    }

    public static void ProjectileDestroyed(Packet packet)
    {
        int id = packet.ReadInt();

        if (GameManager.projectiles.TryGetValue(id, out Projectile projectile))
        {
            projectile.Explode();
            GameManager.projectiles.Remove(id);
            Destroy(projectile.gameObject);
        }
    }

    public static void SpawnEnemy(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        GameManager.Instance.SpawnEnemy(id, position, rotation);
    }

    public static void EnemyTransform(Packet packet)
    {
        int id = packet.ReadInt();
        Vector3 position = packet.ReadVector3();
        Quaternion rotation = packet.ReadQuaternion();

        if (GameManager.enemies.TryGetValue(id, out Enemy enemy))
        {
            enemy.UpdateTransform(position, rotation);
        }
    }

    public static void EnemyHealth(Packet packet)
    {
        int id = packet.ReadInt();
        float health = packet.ReadFloat();

        if (GameManager.enemies.TryGetValue(id, out Enemy enemy))
        {
            enemy.Health = health;
        }
    }

    public static void EnemyDestroyed(Packet packet)
    {
        int id = packet.ReadInt();

        if (GameManager.enemies.TryGetValue(id, out Enemy enemy))
        {
            GameManager.enemies.Remove(id);
            enemy.gameObject.SetActive(false);
            Destroy(enemy.gameObject, 3.0f);
        }
    }

    public static void PodTransform(Packet packet)
    {
        int id = packet.ReadInt();
        Quaternion rotation = packet.ReadQuaternion();

        if (GameManager.players.TryGetValue(id, out Player player))
        {
            player.UpdatePodRotation(rotation);
        }
    }
}
