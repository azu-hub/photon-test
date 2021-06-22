using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float speed = 10;
    public float destoryTime = 2f;
    public bool shootLeft = false;

    private void Start()
    {
        StartCoroutine(destoryBullete());
    }

    IEnumerator destoryBullete()
    {
        yield return new WaitForSeconds(destoryTime);
        this.GetComponent<PhotonView>().RPC("destroy", RpcTarget.AllBuffered);
    }

    private void Update()
    {
        if(!shootLeft)
        transform.Translate(Vector3.right * Time.deltaTime * speed);
        else
            transform.Translate(Vector3.left * Time.deltaTime * speed);
    }

    [PunRPC]
    public void destroy()
    {
        Destroy(this.gameObject);
    }

    [PunRPC]
    public void changeDirection()
    {
        shootLeft = true;
    }
}
