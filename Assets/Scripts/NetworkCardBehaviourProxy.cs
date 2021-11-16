using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkCardBehaviourProxy : NetworkBehaviour
{
    [Command]
    public void AssignClientAuthority(NetworkIdentity ni, NetworkConnectionToClient sender = null)
    {
        ni.AssignClientAuthority(sender);
    }

    [Command]
    public void CmdMove(NetworkIdentity ni, Vector3 targetPosition, NetworkConnectionToClient sender = null)
    {
        ni.gameObject.transform.position = targetPosition;
    }

    [Command]
    public void CmdGetAuthority(NetworkIdentity ni, NetworkConnectionToClient sender = null)
    {
        ni.AssignClientAuthority(sender);
    }

    [Command]
    public void CmdReleaseAuthority(NetworkIdentity ni, NetworkConnectionToClient sender = null)
    {
        ni.RemoveClientAuthority();
    }
}
