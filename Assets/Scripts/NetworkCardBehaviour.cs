using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class NetworkCardBehaviour : NetworkBehaviour
{
    public bool isMouseOver = false;
    public bool isSelected = false;
    public bool IsCardBlocked = false;
    private BoxCollider2D mapCanvasBoxCollider2D = null;
    public Vector3 screenPoint;
    public Vector3 offset;

    private void Awake()
    {
        mapCanvasBoxCollider2D = GameObject.FindGameObjectWithTag("MapCanvas")?.GetComponent<BoxCollider2D>();
    }

    private IEnumerator OnMouseDrag()
    {
        if (IsCardBlocked) yield return null;

        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 cursorPosition = UnityEngine.Camera.main.ScreenToWorldPoint(cursorPoint) + offset;

        cursorPosition = new Vector3(cursorPosition.x, cursorPosition.y, transform.position.z);
        var delta = transform.position - cursorPosition;

        if (mapCanvasBoxCollider2D == null || mapCanvasBoxCollider2D.bounds.Contains(new Vector2(cursorPosition.x, cursorPosition.y)))
        {
            if (delta != Vector3.zero)
            {
                //MouseDragCard?.Invoke(this, delta, cursorPosition, false);
                //NetworkMoveToPosition(cursorPosition);
                NetworkClient.connection.identity.GetComponent<NetworkCardBehaviourProxy>()
                    .CmdMove(this.GetComponent<NetworkIdentity>(), cursorPosition);

                //transform.position = cursorPosition;
            }
        }

        yield return null;
    }

    private void OnMouseOver()
    {
        screenPoint = UnityEngine.Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position -
        UnityEngine.Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
            Input.mousePosition.y, screenPoint.z));
    }
}
