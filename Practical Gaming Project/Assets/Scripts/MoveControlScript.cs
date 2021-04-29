using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveControlScript : MonoBehaviour
{
    float speed = 30;
    CameraControlScript myCamera;
    Collider myCollider;
    float attackRange = 2.5f;
    private Vector3 previous_Position;

    // Start is called before the first frame update
    void Start()
    {
        myCamera = Camera.main.GetComponent<CameraControlScript>();
        myCollider = this.GetComponent<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        previous_Position = transform.position;
        GetCameraForward();

        if (ShouldMoveForward())
            MoveForward();
        if (ShouldMoveBackwards())
            MoveBackwards();
        if (ShouldStrafeLeft())
            StrafeLeft();
        if (ShouldStrafeRight())
            StrafeRight();
        if (ShouldAttack() & CanAttack())
            Attack();

        /*
        if (trigger.Equals(triggers.attackSphere))
        {
            print(trigger);
        }
        if (trigger.Equals(triggers.hitBox))
        {
            transform.position = previous_Position;
            print(trigger);
        }*/
        

         
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
            print(c.gameObject.tag);
        }

        myCamera.SetCameraPosition(transform);

        //Debug.DrawLine(ourCapsule.center + dir * d, ourCapsule.center - dir * d);

    }

    private void GetCameraForward()
    {
        Vector3 desiredForward = Camera.main.transform.forward;
        desiredForward.y = 0;
        transform.forward = desiredForward.normalized;
    }

    private void StrafeRight() {
        transform.position += (speed /2) * transform.right * Time.deltaTime; }
    private void StrafeLeft() {
        transform.position -= (speed / 2) * transform.right * Time.deltaTime; }
    private void MoveBackwards() {
        transform.position -= (speed / 2) * transform.forward * Time.deltaTime; }
    private void MoveForward() {        
        transform.position += speed * transform.forward * Time.deltaTime; }

    private void Attack()
    {
        // NOTHING YET
    }

    private bool ShouldStrafeRight() {
        return Input.GetKey(KeyCode.RightArrow); }
    private bool ShouldStrafeLeft() {
        return Input.GetKey(KeyCode.LeftArrow); }
    private bool ShouldMoveBackwards() {
        return Input.GetKey(KeyCode.DownArrow); }
    private bool ShouldMoveForward() {
        return Input.GetKey(KeyCode.UpArrow); }
    private bool ShouldAttack() {
        return Input.GetKey(KeyCode.Mouse1); }
    
    private bool CanAttack()
    {
        float distance = 0;//this.transform

        if (distance <= attackRange)
            return true;
        else
            return false;
    }

    // WORK IN PROGRESS
    private enum triggers { attackSphere, hitBox };
    private triggers trigger = 0;
    public void OnTriggerEnter(Collider other)
    {
        if(other is BoxCollider)
        {
            trigger++;
            Debug.Log(trigger);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other is BoxCollider)
        {
            trigger--;
            Debug.Log(trigger);
        }
        
    }
}
