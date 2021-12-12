using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThirdPersonController : MonoBehaviour, IPunObservable
{
    // For showing info on overlay canvas
    private Text myName;
    private Text myPosition;
    private Text otherPlayers;

    //input fields
    private ThirdPersonActionsAsset playerActionAsset;
    private InputAction move;
    private GameObject player;

    //movements
    private Rigidbody rb;
    [SerializeField]
    private float movementForce = 1f;
    [SerializeField]
    private float jumpForce = 5f;
    [SerializeField]
    private float maxSpeed = 5f;      //must be same as in ThirdPersonAnimation!!
    private Vector3 forceDirection = Vector3.zero;
    [SerializeField]
    private Camera playerCamera;
    Vector3 right;
    Vector3 forward;

    private Animator animator;
    PhotonView view;
    public Transform cameraTransform;
    bool isFollowing;

    //called before Start()
    private void Awake()
    {
        view = GetComponent<PhotonView>();
        rb = this.GetComponent<Rigidbody>();
        playerActionAsset = new ThirdPersonActionsAsset();
        animator = this.GetComponent<Animator>();
        myName = GameObject.FindWithTag("OwnID").GetComponent<Text>();
        myPosition = GameObject.FindWithTag("OwnPosition").GetComponent<Text>();
        otherPlayers = GameObject.FindWithTag("OtherPlayers").GetComponent<Text>();
    }

    private void Update()
    {
        if (view.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
        else
        {
            GameObject[] ch = GameObject.FindGameObjectsWithTag("CameraHolder");
            foreach (GameObject c in ch)
            {
                if (PhotonView.Get(c).IsMine)
                {
                    c.SetActive(true);
                }
                else
                {
                    c.SetActive(false);
                }
            }
        }
    }

    private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in players)
        {
            if (PhotonView.Get(p).IsMine)
            {
                this.cameraTransform = p.transform;
                break;
            }
        }
        player = GameObject.FindGameObjectWithTag("Player");
        myName.text = "My name: \n" + PhotonNetwork.LocalPlayer.NickName;
        OnStartFollowing();
        StartCoroutine(UpdateOtherPlayers());
        StartCoroutine(UpdatePosition());
    }

    public void OnStartFollowing()
    {
        cameraTransform = Camera.main.transform;
        isFollowing = true;
    }

    private IEnumerator UpdateOtherPlayers()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            StringBuilder playerInfo = new StringBuilder();
            playerInfo.Append("Players in room: " + PhotonNetwork.PlayerList.Length.ToString() + "\n");
            int i = 1;
            foreach (Player p in PhotonNetwork.PlayerListOthers)
            {
                i += 1;
                playerInfo.Append("Player " + i.ToString() + ": " + p.NickName + "\n");
            }
            otherPlayers.text = playerInfo.ToString();
        }
    }

    private IEnumerator UpdatePosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            myPosition.text = "X: " + player.transform.position.x.ToString("0") + "\nZ: " + player.transform.position.z.ToString("0");
        }
    }

    private void OnEnable()
    {
        if (view.IsMine)
        {
            playerActionAsset.Player.Jump.started += DoJump;
            playerActionAsset.Player.Attack.started += DoAttack;
            playerActionAsset.Player.Shoot.started += DoShoot;
            move = playerActionAsset.Player.Move;
            playerActionAsset.Player.Enable();
        }
    }

    private void OnDisable()
    {
        if (view.IsMine)
        {
            playerActionAsset.Player.Jump.started -= DoJump;
            playerActionAsset.Player.Attack.started -= DoAttack;
            playerActionAsset.Player.Shoot.started -= DoShoot;
            playerActionAsset.Player.Disable();
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine && isFollowing)
        {
            //Debug.Log(move.ReadValue<Vector2>());
            GetCameraForward(playerCamera);
            GetCameraRight(playerCamera);
            forceDirection += move.ReadValue<Vector2>().x * right * movementForce;
            forceDirection += move.ReadValue<Vector2>().y * forward * movementForce;
            rb.AddForce(forceDirection, ForceMode.Impulse);
            //reset after action
            forceDirection = Vector3.zero;

            //remove Unity's floatyness from the jumps --- negative values go downwards
            if (rb.velocity.y < 0f)
                rb.velocity -= Vector3.down * Physics.gravity.y * Time.fixedDeltaTime; //fixedDeltaTime because method is fixedUpdate method, not update

            //cap velocity --- horizontal speed
            Vector3 horizontalVelocity = rb.velocity;
            horizontalVelocity.y = 0; //velocity only horizontally
            if (horizontalVelocity.sqrMagnitude > maxSpeed * maxSpeed) //if exceeded maxSpeed 
                rb.velocity = horizontalVelocity.normalized * maxSpeed + Vector3.up * rb.velocity.y;

            LookAt();
        }
    }

    private void LookAt()
    {
        if (view.IsMine)
        {
            Vector3 direction = rb.velocity;
            direction.y = 0f; //to not look up or down

            if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
                this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
            else
                rb.angularVelocity = Vector3.zero;
        }
    }

    private void GetCameraForward(Camera playerCamera)
    {
        if (view.IsMine)
        {
            forward = playerCamera.transform.forward;
            forward.y = 0;
            forward = forward.normalized;
        }
    }

    private void GetCameraRight(Camera playerCamera)
    {
        if (view.IsMine)
        {
            right = playerCamera.transform.right;
            right.y = 0;
            right = right.normalized;
        }
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        if (view.IsMine)
        {
            if (IsGrounded())
                forceDirection += Vector3.up * jumpForce;
        }
    }

    private bool IsGrounded()
    {
        Ray ray = new Ray(this.transform.position + Vector3.up * 0.25f, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 0.3f))
            return true;
        else
            return false;
    }

    private void DoAttack(InputAction.CallbackContext obj)
    {
        if (view.IsMine)
        {
            animator.SetTrigger("attack");
        }
    }

    private void DoShoot(InputAction.CallbackContext obj)
    {
        if (view.IsMine)
        {
            animator.SetTrigger("shoot");
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
    }
}