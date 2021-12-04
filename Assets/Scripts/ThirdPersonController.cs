using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ThirdPersonController : MonoBehaviour
{
    // For showing info on overlay canvas
    private Text myID;
    private Text myPosition;
    private int id;
    private Text otherPlayers;

    //input fields
    private ThirdPersonActionsAsset playerActionAsset;
    private InputAction move;
    Vector2 currentMovementInput;
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

    private Animator animator;
    PhotonView view;

    //called before Start()
    private void Awake()
    {
        view = GetComponent<PhotonView>();
        rb = this.GetComponent<Rigidbody>();
        playerActionAsset = new ThirdPersonActionsAsset();
        animator = this.GetComponent<Animator>();
        myID = GameObject.FindWithTag("OwnID").GetComponent<Text>();
        myPosition = GameObject.FindWithTag("OwnPosition").GetComponent<Text>();
        otherPlayers = GameObject.FindWithTag("OtherPlayers").GetComponent<Text>();
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        id = player.GetInstanceID();
        myID.text = "My id: \n" + id;
        StartCoroutine(UpdatePosition());
        StartCoroutine(UpdateOtherPlayers());
    }

    private IEnumerator UpdateOtherPlayers()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            foreach (Player p in PhotonNetwork.PlayerListOthers)
            {
                otherPlayers.text = "Player " + p.NickName;
            }
        }
    }

    private IEnumerator UpdatePosition()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            myPosition.text = "X: " + player.transform.position.x.ToString("0.00") + ", Y: " + player.transform.position.y.ToString("0.00");
        }
    }

    private void OnEnable()
    {
        playerActionAsset.Player.Jump.started += DoJump;
        playerActionAsset.Player.Attack.started += DoAttack;
        playerActionAsset.Player.Shoot.started += DoShoot;
        move = playerActionAsset.Player.Move;
        playerActionAsset.Player.Enable();
    }

    private void OnDisable()
    {
        playerActionAsset.Player.Jump.started -= DoJump;
        playerActionAsset.Player.Attack.started -= DoAttack;
        playerActionAsset.Player.Shoot.started -= DoShoot;
        playerActionAsset.Player.Disable();
    }

    private void FixedUpdate()
    {
        // If-clause ensures that you can only control your own player.
        if (view.IsMine)
        {
            //Debug.Log(move.ReadValue<Vector2>());
            forceDirection += move.ReadValue<Vector2>().x * GetCameraRight(playerCamera) * movementForce;
            forceDirection += move.ReadValue<Vector2>().y * GetCameraForward(playerCamera) * movementForce;
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
        Vector3 direction = rb.velocity;
        direction.y = 0f; //to not look up or down

        if (move.ReadValue<Vector2>().sqrMagnitude > 0.1f && direction.sqrMagnitude > 0.1f)
            this.rb.rotation = Quaternion.LookRotation(direction, Vector3.up);
        else
            rb.angularVelocity = Vector3.zero;
    }

    private Vector3 GetCameraForward(Camera playerCamera)
    {
        Vector3 forward = playerCamera.transform.forward;
        forward.y = 0;
        return forward.normalized;
    }

    private Vector3 GetCameraRight(Camera playerCamera)
    {
        Vector3 right = playerCamera.transform.right;
        right.y = 0;                            //xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
        return right.normalized;
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        if (IsGrounded())
            forceDirection += Vector3.up * jumpForce;
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
        animator.SetTrigger("attack");
    }

    private void DoShoot(InputAction.CallbackContext obj)
    {
        animator.SetTrigger("shoot");
    }
}