using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float speed;
    [SerializeField] private float minY;
    [SerializeField] private float maxY;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PhotonView view;

    private Color playerColor;
    private float middleY;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
        playerColor = GameManager.instance.colors[PlayerPrefs.GetInt("color", 0)];

        if (photonView.IsMine)
        {
            // Generate a random color for the player
            photonView.RPC("UpdatePlayerColor", RpcTarget.AllBuffered, playerColor.r, playerColor.g, playerColor.b);
        }

        Camera mainCamera = Camera.main;
        Vector3 screenMiddle = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        middleY = mainCamera.ScreenToWorldPoint(screenMiddle).y;
    }

    [PunRPC]
    void UpdatePlayerColor(float r, float g, float b)
    {
        playerColor = new Color(r, g, b);
        GetComponent<SpriteRenderer>().color = playerColor;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Sending player's color to other players
            stream.SendNext(ColorToString(playerColor));
        }
        else
        {
            // Receiving other players' color
            playerColor = StringToColor((string)stream.ReceiveNext());
            spriteRenderer.color = playerColor;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            if (Input.GetMouseButton(0))
            {
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Move the paddle based on mouse position relative to the middle of the screen
                if (mousePosition.y >= middleY && transform.position.y <= maxY)
                {
                    transform.Translate(Vector3.up * speed * Time.deltaTime);
                }
                else if (mousePosition.y < middleY && transform.position.y >= minY)
                {
                    transform.Translate(Vector3.down * speed * Time.deltaTime);
                }
            }
        }
    }

    public static string ColorToString(Color color)
    {
        // Convert the color components (R, G, B, and A) to strings
        string r = Mathf.RoundToInt(color.r * 255).ToString();
        string g = Mathf.RoundToInt(color.g * 255).ToString();
        string b = Mathf.RoundToInt(color.b * 255).ToString();
        string a = Mathf.RoundToInt(color.a * 255).ToString();

        // Combine the color components into a single string separated by commas
        return string.Format("{0},{1},{2},{3}", r, g, b, a);
    }

    public static Color StringToColor(string colorString)
    {
        // Split the string into individual components (R, G, B, and A) based on commas
        string[] components = colorString.Split(',');

        // Parse the components and convert them back to float values
        float r = int.Parse(components[0]) / 255f;
        float g = int.Parse(components[1]) / 255f;
        float b = int.Parse(components[2]) / 255f;
        float a = int.Parse(components[3]) / 255f;

        // Create and return the Color object
        return new Color(r, g, b, a);
    }
}
