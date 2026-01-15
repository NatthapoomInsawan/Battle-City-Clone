using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    [Header("Network")]
    [SerializeField] private GameplayNetworkManager gameplayNetworkManager;
    private void Start()
    {
        Init().Forget();
    }

    private async UniTaskVoid Init()
    {
        try
        {
            await gameplayNetworkManager.Init();
        }
        catch (System.Exception e)
        {
            Debug.LogError(e);
            return;
        }

        Debug.Log("GameplayManager initialized.");
    }

}
