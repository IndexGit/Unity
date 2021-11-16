using System.Collections;
using Mirror;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(NetworkIdentity))]
    public class SyncPosition : NetworkBehaviour
    {
        public delegate bool CheckCanMoveMethod();

        private CheckCanMoveMethod _checkCheckCanMoveMethod;

        private NetworkIdentity networkIdentity;
        private NetworkCardBehaviourProxy _proxy;

        private float NetworkCardMoveFreq = 0.10f;
        private float NetworkCardMoveMinDelta = 0.1f;

        private NetworkCardBehaviourProxy proxy
        {
            get
            {
                if (_proxy == null)
                {
                    _proxy = NetworkClient.connection.identity.GetComponent<NetworkCardBehaviourProxy>();
                }

                return _proxy;
            }
        }
        private Vector3 networkPosition;

        public Vector3 screenPoint;
        public Vector3 offset;

        public void Init(CheckCanMoveMethod checkCheckCanMoveMethod, float networkCardMoveFreq = 0.10f, float networkCardMoveMinDelta = 0.1f)
        {
            _checkCheckCanMoveMethod = checkCheckCanMoveMethod;

            networkIdentity = GetComponent<NetworkIdentity>();
            networkPosition = transform.position;

            NetworkCardMoveFreq = networkCardMoveFreq;
            NetworkCardMoveMinDelta = networkCardMoveMinDelta;
        }

        // Send position to the server and run the RPC for everyone, including the server. 
        [Command]
        protected void CmdSyncPos(Vector3 position, Quaternion rotation)
        {
            gameObject.transform.position = position;
            gameObject.transform.rotation = rotation;

            //RpcSyncPos(position, rotation);
        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
        }

        public override void OnStopAuthority()
        {
            base.OnStopAuthority();
        }

        // For each player, transfer the position from the server to the client, and set it as long as it's not the local player. 
        [ClientRpc]
        void RpcSyncPos(Vector3 position, Quaternion rotation)
        {
            if (!networkIdentity.hasAuthority)
            {
                transform.position = position;
                //StartCoroutine(NetworkSmoothMoveCard(position));
                transform.rotation = rotation;
            }
        }
        /// <summary>
        /// Двигаем карту плавно по данным из сети
        /// </summary>
        /// <param name="endPosition"></param>
        /// <returns></returns>
        private IEnumerator NetworkSmoothMoveCard(Vector3 endPosition)
        {
            var start = transform.position;

            float elapsedTime = 0;

            while (elapsedTime < NetworkCardMoveFreq)
            {
                transform.position = Vector3.Lerp(start, endPosition, (elapsedTime / NetworkCardMoveFreq));
                elapsedTime += Time.deltaTime;
                yield return null;
            }

        }

        [ClientCallback]
        public void Move(Vector3? deltaPrm = null)
        {
            if (!hasAuthority)
                return;

            if (!_checkCheckCanMoveMethod())
                return;

            Vector3 delta;

            if (deltaPrm == null)
            {
                Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
                Vector3 cursorPosition = UnityEngine.Camera.main.ScreenToWorldPoint(cursorPoint) + offset;

                cursorPosition = new Vector3(cursorPosition.x, cursorPosition.y, transform.position.z);

                delta = networkPosition - cursorPosition;
            }
            else
            {
                delta = deltaPrm.Value;
            }

            if (delta.magnitude > NetworkCardMoveMinDelta)
            {
                // move localy on client
                transform.position -= delta;
                networkPosition = transform.position;

                // send to server
                CmdSyncPos(transform.position, transform.rotation);
            }
        }

        /// <summary>
        /// Fix cursor date relative postition
        /// </summary>
        private void SaveMouseClickedPosition()
        {
            screenPoint = UnityEngine.Camera.main.WorldToScreenPoint(gameObject.transform.position);
            offset = gameObject.transform.position -
                     UnityEngine.Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                         Input.mousePosition.y, screenPoint.z));
        }

        private void OnMouseDown()
        {
            if (Input.GetMouseButton(0))
            {
                SaveMouseClickedPosition();
            }
        }

        public bool SetAuthority(bool on)
        {
            if (on && _checkCheckCanMoveMethod())
            {
                proxy.CmdGetAuthority(networkIdentity);
                return networkIdentity.hasAuthority;
            }
            else
            {
                proxy.CmdReleaseAuthority(networkIdentity);
                return true;
            }
        }
    }
}
