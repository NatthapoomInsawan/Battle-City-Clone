using UnityEngine;

namespace BattleCityClone.UI
{
    public class BaseUI : MonoBehaviour, IUIbehavior, IInitializable
    {
        public virtual void Init() { }
        public virtual void Close() => gameObject.SetActive(false);
        public virtual void Open() => gameObject.SetActive(true);
    }
}
