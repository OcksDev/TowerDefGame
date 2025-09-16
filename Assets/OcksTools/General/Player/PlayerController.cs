using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private Rigidbody2D rigid;
    public float move_speed = 2;
    public float decay = 0.8f;
    public float grapo_decay = 0.8f;
    public float grapo_decay_out = 0.8f;
    public float grap_tower_radius = 5;
    private Vector3 move = new Vector3(0, 0, 0);
    public GameObject Grapp;
    public GameObject Visual;
    public static PlayerController Instance;

    public bool IsRealNerd = false;
    public bool IsHost = false;
    public ulong ID = 6969;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        GameHandler.Players.Add(this);
    }
    private void OnDestroy()
    {
        GameHandler.Players.Remove(this);
    }
    private void Start()
    {
        rigid= GetComponent<Rigidbody2D>();
        if(GameHandler.Instance.CurrentMultiplayerState == GameHandler.NetworkState.Singleplayer)
        {
            Destroy(GetComponent<ClientNetworkTransform>());
            Destroy(GetComponent<NetworkObject>());
            IsRealNerd = true;
            IsHost = true;
            ID = 0;
        }
        else
        {
            IsRealNerd = NetworkObject.IsLocalPlayer;
            IsHost = NetworkObject.IsOwnedByServer;
            ID = NetworkObject.OwnerClientId;
        }

        if (IsRealNerd)
        {
            //Destroy(Visual.GetComponent<FunnyLerpOnPlayer>());
        }
    }
    Vector3 GrappleDir;

    void FixedUpdate()
    {
        if(!IsRealNerd) { return; }
        move *= decay;
        Vector3 dir = new Vector3(0, 0, 0);
        if (InputManager.IsKey("move_forward")) dir += Vector3.up;
        if (InputManager.IsKey("move_back")) dir += Vector3.down;
        if (InputManager.IsKey("move_right")) dir += Vector3.right;
        if (InputManager.IsKey("move_left")) dir += Vector3.left;
        if(dir.magnitude > 0.5f)
        {
            dir.Normalize();
            move += dir;
        }
        Vector3 bgalls = move * Time.deltaTime * move_speed * 20;
        if (ShouldMoveByGrap)
        {
            GrappleDir *= grapo_decay;
        }
        else
        {
            GrappleDir *= grapo_decay_out;
        }
        bgalls += GrappleDir;
        if(ShouldMoveByGrap && nerdl != null)
        {
            var d = (nerdl.transform.position - transform.position);
            var w = d.normalized;
            w += move * Time.deltaTime * move_speed * 20 * Mathf.Min(d.magnitude / 6, 1);
            w += (d.normalized /2f)*Mathf.Min(d.magnitude,5);
            GrappleDir += w/8;
        }

        rigid.velocity += new Vector2(bgalls.x, bgalls.y);
        if (CameraLol.Instance != null)
        {
            CameraLol.Instance.targetpos = transform.position;
        }

    }
    private void Update()
    {
        if (!IsRealNerd) { return; }
        if (InputManager.IsKeyDown("aim"))
        {
            Grapple();
        }
    }


    public void Grapple()
    {
        var pp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        pp.z = 0;

        Tower smegma = null;
        float dist = grap_tower_radius*grap_tower_radius;
        foreach (var a in GameHandler.Instance.AllActiveTowers)
        {
            var zinkle = a.transform.position;
            zinkle.z = 0;
            if ((zinkle-pp).sqrMagnitude <= dist)
            {
                smegma = a;
                dist = (zinkle - pp).sqrMagnitude;
            }
        }
        Quaternion rot = Quaternion.identity;
        if (smegma != null)
        {
            rot = RandomFunctions.PointAtPoint2D(transform.position, smegma.transform.position, 180);
            ShouldMoveByGrap = false;
            nerdl = Instantiate(Grapp,transform.position, rot).GetComponent<Grapp>();
            nerdl.controller = this;
            StartCoroutine(WaitForNoGrap());
            if (GameHandler.Instance.CurrentMultiplayerState == GameHandler.NetworkState.Multiplayer)
            {
                Console.Log("Grapple Init");
                List<string> Data = new List<string>()
                {
                    "Start",
                    ID.ToString(),
                    rot.eulerAngles.ToString(),
                    transform.position.ToString(),
                };
                ServerGamer.Instance.MessageServerRpc(ServerGamer.Instance.ClientID, "Grapple", Converter.ListToString(Data,"<->"));
            }
        }
        /*else
        {
            var ding = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            ding.z = transform.position.z;
            nerdl = Instantiate(Grapp, transform.position, RandomFunctions.PointAtPoint2D(transform.position, ding, 180)).GetComponent<Grapp>();
            nerdl.spd *= Mathf.Clamp(RandomFunctions.Dist(transform.position, ding) / 10,0.5f,1);
        }*/
    }
    public void FakeGrapple(Vector3 pos,Quaternion rot)
    {
        ShouldMoveByGrap = false;
        nerdl = Instantiate(Grapp, pos, rot).GetComponent<Grapp>();
        nerdl.controller = this;
    }
    private Grapp nerdl;
    public bool ShouldMoveByGrap = false;
    public IEnumerator WaitForNoGrap()
    {
        yield return new WaitUntil(() => { return !InputManager.IsKey("aim"); });

        // stop grap
        Destroy(nerdl.gameObject);
        ShouldMoveByGrap = false;
        if (GameHandler.Instance.CurrentMultiplayerState == GameHandler.NetworkState.Multiplayer)
        {
            List<string> Data = new List<string>()
            {
                "Stop",
                ID.ToString(),
            };
            ServerGamer.Instance.MessageServerRpc(ServerGamer.Instance.ClientID, "Grapple", Converter.ListToString(Data, "<->"));
        }
    }
    public void EndFakeGrapple()
    {
        Destroy(nerdl.gameObject);
        ShouldMoveByGrap = false;
    }
}
