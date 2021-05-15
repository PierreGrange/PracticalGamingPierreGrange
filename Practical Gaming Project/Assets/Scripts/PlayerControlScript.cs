using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlScript : MonoBehaviour
{
    float speed = 30;
    ManagerScript myManager;
    CameraControlScript myCamera;
    Collider myCollider;
    float attackRange = 5f;
    private Vector3 previous_Position;
    Animator playerAnimator;

    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main.GetComponent<CameraControlScript>();
        myCollider = this.GetComponent<CapsuleCollider>();
        playerAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        previous_Position = transform.position;
        GetCameraForward();

        playerAnimator.SetBool("isIdling", true);
        playerAnimator.SetBool("isWalking", false);
        playerAnimator.SetBool("isHitting", false);
        playerAnimator.SetBool("isDying", false);

        if (ShouldMoveForward())
            MoveForward();
        if (ShouldMoveBackwards())
            MoveBackwards();
        if (ShouldStrafeLeft())
            StrafeLeft();
        if (ShouldStrafeRight())
            StrafeRight();
        if (ShouldAttack())
            if (CanAttack())
                Attack();
         
        CapsuleCollider ourCapsule = myCollider as CapsuleCollider;
        float radius = ourCapsule.radius, height = ourCapsule.height, d = (height - 2 * radius) / 2;
        Vector3 dir = ourCapsule.transform.up;
        Collider[] collidingWith = Physics.OverlapCapsule(transform.TransformPoint(ourCapsule.center + dir * d), transform.TransformPoint(ourCapsule.center - dir * d), radius);

        foreach (Collider c in collidingWith)
        {
            if (c.gameObject.tag == "Wall")
            {
                transform.position = previous_Position;
            }
        }
        myCamera.SetCameraPosition(transform);
    }

    internal void SetManager(ManagerScript managerScript)
    {
        myManager = managerScript;
    }

    internal void SetSpawnPosition(Vector3 position)
    {
        transform.position = new Vector3(position.x, 0.1f, position.z);
    }

    private void GetCameraForward()
    {
        Vector3 desiredForward = Camera.main.transform.forward;
        desiredForward.y = 0;
        transform.forward = desiredForward.normalized;
    }

    private void StrafeRight() {
        transform.position += (speed /2) * transform.right * Time.deltaTime;
        SetAnimatorOnWalk(); }
    private void StrafeLeft() {
        transform.position -= (speed / 2) * transform.right * Time.deltaTime;
        SetAnimatorOnWalk(); }
    private void MoveBackwards() {
        transform.position -= (speed / 2) * transform.forward * Time.deltaTime;
        SetAnimatorOnWalk(); }
    private void MoveForward() {
        transform.position += speed * transform.forward * Time.deltaTime;
        SetAnimatorOnWalk(); }
    private void Attack() {
        myManager.TargetKilled();
        playerAnimator.SetBool("isIdling", false);
        playerAnimator.SetBool("isWalking", false);
        playerAnimator.SetBool("isHitting", true);
        playerAnimator.SetBool("isDying", false); }

    private bool ShouldStrafeRight() {
        return Input.GetKey(KeyCode.RightArrow); }
    private bool ShouldStrafeLeft() {
        return Input.GetKey(KeyCode.LeftArrow); }
    private bool ShouldMoveBackwards() {
        return Input.GetKey(KeyCode.DownArrow); }
    private bool ShouldMoveForward() {
        return Input.GetKey(KeyCode.UpArrow); }
    private bool ShouldAttack() {
        return Input.GetKey(KeyCode.Mouse0); }
    
    private bool CanAttack()
    {
        Vector3 rayStart = new Vector3(0, 2f, 0);
        rayStart += transform.position;
        RaycastHit hit;
        if (Physics.Raycast(rayStart, transform.TransformDirection(Vector3.forward), out hit, attackRange))
        {
            if (hit.transform.gameObject.tag == "Target")
            {
                return true;
            }
            Debug.DrawRay(rayStart, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
        }
        else
        {
            Debug.DrawRay(rayStart, transform.TransformDirection(Vector3.forward) * 1000, Color.white);
        }
        return false;
    }

    public void KillPlayer()
    {
        playerAnimator.SetBool("isIdling", false);
        playerAnimator.SetBool("isWalking", false);
        playerAnimator.SetBool("isHitting", false);
        playerAnimator.SetBool("isDying", true);
        Destroy(this.gameObject);
    }

    public void SetAnimatorOnWalk()
    {
        playerAnimator.SetBool("isIdling", false);
        playerAnimator.SetBool("isWalking", true);
        playerAnimator.SetBool("isHitting", false);
        playerAnimator.SetBool("isDying", false);
    }
}
