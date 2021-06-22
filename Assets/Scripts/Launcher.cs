using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{

    public GameObject connectedScreen;
    public GameObject disconnectedScreen;

    public void OnClick_ConnectedBtn()
    {
        //photonに接続
        PhotonNetwork.ConnectUsingSettings();
    }

    //クライアントがマスターサーバーに接続されたときに呼び出される 
    public override void OnConnectedToMaster()
    {
        //マスターサーバーと同時にロビーに入るようにしてあげる
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        disconnectedScreen.SetActive(true);
    }

    public override void OnJoinedLobby()
    {
        if(disconnectedScreen.activeSelf)//disconnectedScreenが有効ならTrue
            disconnectedScreen.SetActive(false);

        connectedScreen.SetActive(true);
    }

}
