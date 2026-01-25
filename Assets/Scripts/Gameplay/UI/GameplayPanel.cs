using BattleCityClone.Gameplay.Manager;
using BattleCityClone.Gameplay;
using UnityEngine;
using BattleCityClone.Gameplay.Player;

namespace BattleCityClone.UI
{
    public class GameplayPanel : BaseUI
    {
        [Header("Prefabs")]
        [SerializeField] private PlayerStatusUI playerStatusPrefab;

        [Header("References")]
        [SerializeField] private RectTransform playerStatusContainer;

        public override void Init()
        {
            GameplayManager.Instance.GameplayStateManager.OnStateChanged += UIStateCheck;
        }

        private void UIStateCheck(GameplayState currentGameState)
        {
            if (currentGameState is GameplayStartedState)
                Open();
            else
                Close();
        }

        public override void Open()
        {
            InitPlayerStatus();
            base.Open();
        }


        private void InitPlayerStatus()
        {
            foreach (Transform child in playerStatusContainer)
                Destroy(child.gameObject);

            foreach (var player in FindObjectsByType<PlayerStateController>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            {
                PlayerStatusUI playerStatusUI = Instantiate(playerStatusPrefab, playerStatusContainer);
                playerStatusUI.Init(player);
            }
        }

    }
}
