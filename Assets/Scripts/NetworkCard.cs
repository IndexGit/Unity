using Mirror;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(SyncPosition))]
    public class NetworkCard : NetworkBehaviour
    {
        private SyncPosition syncPosition;
        // Start is called before the first frame update

        void Awake()
        {
            syncPosition = GetComponent<SyncPosition>();
            syncPosition.Init(() => true);
        }

        void OnMouseDown()
        {
            if (Input.GetMouseButton(0))
            {
                syncPosition.SetAuthority(true);
            }
        }

        [ClientCallback]
        private void OnMouseDrag()
        {
            syncPosition.Move();
        }

        [ClientCallback]
        private void OnMouseUp()
        {
            syncPosition.SetAuthority(false);
        }
    }
}
