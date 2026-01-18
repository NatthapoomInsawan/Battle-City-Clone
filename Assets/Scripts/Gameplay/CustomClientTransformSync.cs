using Cysharp.Threading.Tasks;
using Mirage;
using UnityEngine;

namespace BattleCityClone.Gameplay
{
    [DisallowMultipleComponent]
    public class CustomClientTransformSync : NetworkBehaviour
    {
        [Header("Settings")]
        [SerializeField] private int syncRateInMilliSec = 30;
        [SerializeField] private float lerpRate = 5f;

        [SyncVar(hook = nameof(OnPositionChange))] private Vector3 syncPosition;
        [SyncVar(hook = nameof(OnRotationChange))] private Quaternion syncRotation;
        
        private float lerpTimer = 0f;

        private void Start()
        {
            syncPosition = transform.position;
            syncRotation =  transform.rotation;

            if (!Server)
                SendUpdates().Forget();
        }

        private void FixedUpdate()
        {
            lerpTimer += Time.fixedDeltaTime * lerpRate;
            if (!HasAuthority)
            {
                transform.position = Vector3.Lerp(transform.position, syncPosition, lerpTimer);
                transform.rotation = syncRotation;
            }
            else
            {
                syncPosition = transform.position;
                syncRotation = transform.rotation;
            }

        }

        private void OnPositionChange(Vector3 position)
        {
            syncPosition = position;
            lerpTimer = 0;
        }

        private void OnRotationChange(Quaternion rotation) => syncRotation = rotation;

        private async UniTaskVoid SendUpdates()
        {
            if (this == null)
                return;

            if (HasAuthority)
                SendPositionToServerRpc(transform.position, transform.rotation);

            await UniTask.Delay(syncRateInMilliSec);
            SendUpdates().Forget();
        }


        [ServerRpc]
        private void SendPositionToServerRpc(Vector3 position, Quaternion rotation)
        {
            transform.position = position;
            transform.rotation = rotation;
            syncPosition = position;
            syncRotation = rotation;
        }
    }
}
