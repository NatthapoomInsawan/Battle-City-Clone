using BattleCityClone.UI;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCityClone.Gameplay.Manager
{
    public class UIManager : MonoBehaviour
    {
        [Header("UI references")]
        [SerializeField] private List<BaseUI> uis = new();

        private void Start()
        {
            foreach (var ui in uis)
                ui.Init();
        }
    }
}
