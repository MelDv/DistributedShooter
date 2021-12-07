using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;
    private float maxSpeed = 5f; //must be same as in ThirdPersonController!!
    private PhotonView view;

    void Start()
    {
        view = GetComponent<PhotonView>();
        animator = this.GetComponent<Animator>();
        rb = this.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            animator.SetFloat("speed", rb.velocity.magnitude / maxSpeed);
        }
    }
}
