using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{
    [SerializeField] public Color[] colors;

    [SerializeField] private string playerPrefabName;
    [SerializeField] private string ballPrefabName;

    [SerializeField] private Text timerText;
    [SerializeField] private Text pingText;
    [SerializeField] private Text player1NameText;
    [SerializeField] private Text player2NameText;
    [SerializeField] private Text player1ScoreText;
    [SerializeField] private Text player2ScoreText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private Text winnerText;

    [SerializeField] private AudioSource bounceSound;
    [SerializeField] private AudioSource scoreSound;

    private GameObject player1;
    private GameObject player2;
    private GameObject ball;

    private float timer = 60;
    private int player1Score = 0;
    private int player2Score = 0;
    private bool gameOver = false;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            player1 = PhotonNetwork.Instantiate(playerPrefabName, new Vector3(-10, 0, 0), Quaternion.identity);
            player1NameText.text = player1.GetComponent<PhotonView>().Owner.NickName;
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            player2 = PhotonNetwork.Instantiate(playerPrefabName, new Vector3(10, 0, 0), Quaternion.identity);
            player2NameText.text = player2.GetComponent<PhotonView>().Owner.NickName;
            PhotonView photonView = PhotonView.Get(this);
            photonView.RPC("SetPlayer2Name", RpcTarget.All, player2NameText.text);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                player2NameText.text = newPlayer.NickName;

                PhotonView photonView = PhotonView.Get(this);
                photonView.RPC("SetPlayer1Name", RpcTarget.All, player1NameText.text);

                print("Starting Game");
                StartGame();
            }     
        }
    }

    void StartGame()
    {
        ball = PhotonNetwork.Instantiate(ballPrefabName, Vector3.zero, Quaternion.identity);
    }

    [PunRPC]
    void UpdateScore(int player1Score, int player2Score)
    {
        this.player1Score = player1Score;
        this.player2Score = player2Score;
        player1ScoreText.text = player1Score.ToString();
        player2ScoreText.text = player2Score.ToString();

        if (player1 != null)
            player1.transform.position = new Vector3(-10, 0, 0);
        
        if (player2 != null)
            player2.transform.position = new Vector3(10, 0, 0);
    }

    [PunRPC]
    void SetPlayer1Name(string player1Name)
    {
        player1NameText.text = player1Name;
    }

    [PunRPC]
    void SetPlayer2Name(string player2Name)
    {
        player2NameText.text = player2Name;
    }

    [PunRPC]
    void PlayBounceSound()
    {
        bounceSound.Play();
    }

    [PunRPC]
    void PlayScoreSound()
    {
        scoreSound.Play();
    }

    [PunRPC]
    void UpdateTimerRPC(float time)
    {
        timer = time;
        timerText.text = timer.ToString("F1");
    }

    [PunRPC]
    public void GameOver()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Destroy(ball);

        if (player1 != null)
            PhotonNetwork.Destroy(player1);

        if (player2 != null)
            PhotonNetwork.Destroy(player2);

        gameOverPanel.SetActive(true);

        if (player1Score > player2Score)
            winnerText.text = "Player 1";
        else if (player2Score > player1Score)
            winnerText.text = "Player 2";
        else
            winnerText.text = "Draw";
    }

    public void Restart(bool isPlayer1)
    {
        if (isPlayer1)
            player1Score++;
        else
            player2Score++;

        PhotonView photonView = PhotonView.Get(this);
        photonView.RPC("UpdateScore", RpcTarget.All, player1Score, player2Score);
        photonView.RPC("PlayScoreSound", RpcTarget.All);

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(ball);
            StartGame();
        }
    }

    private void Update()
    {
        pingText.text = "Ping: " + PhotonNetwork.GetPing().ToString() + "ms";

        if (!photonView.IsMine) return; // Only update the timer for the local player

        if (PhotonNetwork.CurrentRoom != null)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                if (timer > 0f)
                {
                    timer -= Time.deltaTime;
                    photonView.RPC("UpdateTimerRPC", RpcTarget.All, timer);
                }
                else
                {
                    if (!gameOver)
                    {
                        gameOver = true;
                        PhotonView photonView = PhotonView.Get(this);
                        photonView.RPC("GameOver", RpcTarget.All);
                    }
                }
            }
        } 
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        LeaveRoom();
    }
}
