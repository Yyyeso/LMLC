using Photon.Pun;
using System.Linq;
using UnityEngine;

public class PizzaPlayerController : MonoBehaviour
{
    [SerializeField] private float speed = 0.6f;
    PizzaGameData data;
    PhotonView pv;
    public int ID => PhotonNetwork.CurrentRoom.Players.FirstOrDefault(p => p.Value == PhotonNetwork.LocalPlayer).Key;
    
    public Vector3 Pos => transform.position;
    public void ResetPos() => transform.position = Vector3.zero;


    GameObject parent = null;
    public Transform Parent
    {
        get
        {
            if (parent == null)
            {
                parent = GameObject.Find("[Player]");
                if (parent == null)
                {
                    parent = new GameObject("[Player]");
                }
            }
            return parent.transform;
        }
    }
    void Awake()
    {
        transform.SetParent(Parent);
        data = PizzaGameData.Instance;
        if (data.IsMulti)
        {
            pv = GetComponent<PhotonView>();

            if (!pv.IsMine) return;
        }
        data.Player = GetComponent<IPizzaPlayerController>();
    }

    void Update()
    {
        if (data.IsMulti)
        {
            if (!pv.IsMine) return;
        }
        Move();
    }

    void Move()
    {
        if (data.Player.IsFreeze) return;
        transform.position = data.Player.Move(speed);
        if (IsOutside) data.Player.GameOver();
    }

    bool IsOutside => (!data.IsOutside && Vector2.Distance(Vector2.zero, transform.position) >= 3.05f);
}