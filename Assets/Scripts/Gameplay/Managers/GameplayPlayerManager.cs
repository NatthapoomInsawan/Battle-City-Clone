using BattleCityClone.Gameplay.Player;
using Mirage;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCityClone.Gameplay.Manager
{
    public struct PlayerInfo
    {
        public string Name;
    }

    public class GameplayPlayerManager : MonoBehaviour, IInitializable
    {
        [Header("References")]
        [SerializeField] private NetworkClient Client;
        [SerializeField] private NetworkServer Server;
        [SerializeField] private ServerObjectManager ServerObjectManager;

        [Header("References")]
        [SerializeField] private BoxCollider2D spawnArea;

        [Header("Prefabs")]
        [SerializeField] private PlayerStateController playerPrefab;

        [Header("Settings")]
        [SerializeField] private int maxSpawnAttempt = 15;
        [SerializeField] private int spawnDistance = 3;

        private List<INetworkPlayer> players = new();

        public void Init()
        {
            Client.Authenticated.AddListener(OnClientAuthenticated);
            Server.Started.AddListener(OnServerStarted);
        }

        private void OnServerStarted() 
        {
            Server.MessageHandler.RegisterHandler<PlayerInfo>(OnCreateCharacter);
        }

        private void OnClientAuthenticated(INetworkPlayer player)
        {
            player.Send(GameplayManager.Instance.GameplayNetworkManager.LocalPlayerInfo);
        }

        private void OnCreateCharacter(INetworkPlayer player, PlayerInfo info)
        {
            PlayerStateController playerObject = Instantiate(playerPrefab, GetRandomPosition(), Quaternion.identity);
            string tag = player.IsHost ? "Host" : "Client";
            playerObject.name = $"[{tag}] {info.Name}";
            playerObject.PlayerInfo = info;
            players.Add(player);

            ServerObjectManager.AddCharacter(player, playerObject.Identity);
        }

        private Vector3 GetRandomPosition()
        {
            Vector3 resultPosition = Vector3.zero;

            Bounds bounds = spawnArea.bounds;
            int attempts = 0;
            do
            {
                resultPosition = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );
                attempts++;

                bool isTooClose = false;
                foreach(var player in players)
                {
                    if (Vector3.Distance(player.Identity.transform.position, resultPosition) < spawnDistance)
                    {
                        isTooClose = true;
                        break;
                    }
                }
                    

                if (Vector3.Distance(resultPosition, spawnArea.ClosestPoint(resultPosition)) < 0.01f && !isTooClose)
                    return resultPosition;

            } while (attempts < maxSpawnAttempt);

            Debug.LogWarning("Max attempts reached, returning center of bounds.");

            return bounds.center;
        }

    }
}
