using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MyPlayer : MonoBehaviourPun,IPunObservable
{
    public PhotonView pv;

    public float moveSpeed = 10;
    public float jumpforce = 800;

    private Vector3 smoothMove;
    private GameObject sceneCamera;
    public GameObject playerCamera;

    public SpriteRenderer sr;
    private Rigidbody2D rb;
    private bool isGround;

    public GameObject bulletePrefab;
    public Transform bulleteSpawn;

    void Start()
    {
        PhotonNetwork.SendRate = 20;
        PhotonNetwork.SerializationRate = 15;

        if (photonView.IsMine)
        {
            rb = GetComponent<Rigidbody2D>();

            sceneCamera = GameObject.Find("Main Camera");
            sceneCamera.SetActive(false);
            playerCamera.SetActive(true);
        }
        
    }

    private void Update()
    {
        if (photonView.IsMine)
        {
            ProcessInputs();
        }
        else
        {
            smoothMovement();
        }
    }

    void ProcessInputs()
    {
        var move = new Vector3(Input.GetAxisRaw("Horizontal"), 0);
        transform.position += move * moveSpeed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            sr.flipX = false;
            //他人に伝える(関数,誰に情報を送るか)
            pv.RPC("OnDirectonChange_RIGHT", RpcTarget.Others);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            sr.flipX = true;
            pv.RPC("OnDirectonChange_LEFT", RpcTarget.Others);
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGround)
        {
            jump();
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            Shoot();
        }
    }

    public void Shoot()
    {
        GameObject bullete = PhotonNetwork.Instantiate(bulletePrefab.name,bulleteSpawn.position,Quaternion.identity);
        if (sr.flipX == true)
        {
            bullete.GetComponent<PhotonView>().RPC("changeDirection", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    void OnDirectonChange_RIGHT()
    {
        sr.flipX = false;
    }

    [PunRPC]
    void OnDirectonChange_LEFT()
    {
        sr.flipX = true;
    }

    void onCollisionEnter2D(Collider2D col)
    {
        if (photonView.IsMine) {
            if (col.gameObject.tag == "ground")
            {
                isGround = true;
            }
        }
    }


    void onCollisionExit2D(Collider2D col)
    {
        if (photonView.IsMine)
        {
            if (col.gameObject.tag == "ground")
            {
                isGround = false;
            }
        }

    }

    void jump()
    {
        rb.AddForce(Vector2.up*jumpforce);
    }

    void smoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime*10);
    }

    //位置を送る
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)//位置を送ってる
        {
            //みんなに場所を教えてる
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)//位置を受け取ってる
        {
            smoothMove =(Vector3) stream.ReceiveNext();
        }
    }
}
