using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    [SerializeField] private InputField nickNameInputField;
    [SerializeField] private InputField roomNameInputField;
    [SerializeField] private Button[] colorButtons;

    private void Start()
    {
        if (PlayerPrefs.HasKey("nickname"))
        {
            nickNameInputField.text = PlayerPrefs.GetString("nickname");
        }

        SelectColor(PlayerPrefs.GetInt("color", 0));
    }

    public void SelectColor(int x)
    {
        for (int i= 0; i < colorButtons.Length; i++)
        {
            if (i == x)
            {
                colorButtons[i].transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                colorButtons[i].transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        PlayerPrefs.SetInt("color", x);
    }

    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
            PhotonNetwork.CreateRoom(roomNameInputField.text);
    }

    public void JoinRoom()
    {
        if (!string.IsNullOrEmpty(roomNameInputField.text))
            PhotonNetwork.JoinRoom(roomNameInputField.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.NickName = nickNameInputField.text;
        PlayerPrefs.SetString("nickname", nickNameInputField.text);
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        print("Error: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        print("Error: " + message);
    }
}
