using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Ball : MonoBehaviour
{
    [SerializeField] private float speed;
    private Vector2 direction;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Launch();
    }

    void Launch()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = Random.Range(0, 2) == 0 ? -1 : 1;
        rb.velocity = new Vector2(speed * x, speed * y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonView photonView = GameManager.instance.GetComponent<PhotonView>();
            photonView.RPC("PlayBounceSound", RpcTarget.All);
        }
    }
}
