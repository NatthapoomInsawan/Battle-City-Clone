using BattleCityClone.UI;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCityClone.Gameplay.Manager
{
    public class UIManager : MonoBehaviour, IInitializable
    {
        [Header("UI references")]
        [SerializeField] private List<BaseUI> uis = new();

        public void Init()
        {
            foreach (var ui in uis)
                ui.Init();
        }
    }
}
