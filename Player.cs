using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
    private Transform cam;
    private World world;

    public bool IsGrounded;
    public bool isSprinting;

    public float walkSpeed = 3f;
    public float sprintSpeed = 6f;
    public float jumpForce = 5f;
    public float gravity = -12.8f;

    public float playerWidth = 0.15f; 


    private float horizontal;
    private float vertical;
    private float mouseHorizontal;
    private float mouseVertical;
    private Vector3 velocity;
    private float verticalMomentum = 0;
    private bool jumpRequest;

    private void Start() {
        
        cam = GameObject.Find("Main Camera").transform;
        world = GameObject.Find("World").GetComponent<World>();

    }
    
    private void FixedUpdate() {
        calculateVelocity();

        if (jumpRequest)
            jump();

        transform.Rotate(Vector3.up * mouseHorizontal);
        cam.Rotate(Vector3.right * -mouseVertical);
        transform.Translate(velocity, Space.World);
        
    }
    private void Update() {
        
        GetPlayerInput();

    }

    private void calculateVelocity() {

          // Affect vertical momentum with gravity.
        if (verticalMomentum > gravity)
            verticalMomentum += Time.fixedDeltaTime * gravity;

        // if we're sprinting, use the sprint multiplier.
        if (isSprinting)
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * sprintSpeed;
        else
            velocity = ((transform.forward * vertical) + (transform.right * horizontal)) * Time.fixedDeltaTime * walkSpeed;

        // Apply vertical momentum (falling/jumping).
        velocity += Vector3.up * verticalMomentum * Time.fixedDeltaTime;

        if ((velocity.z > 0 && front) || (velocity.z < 0 && back))
            velocity.z = 0;
        if ((velocity.x > 0 && right) || (velocity.x < 0 && left))
            velocity.x = 0;

        if (velocity.y < 0)
            velocity.y = checkDownVel(velocity.y);
        else if (velocity.y > 0)
            velocity.y = checkUpVel(velocity.y);

    }

    void jump(){
        verticalMomentum = jumpForce;
        IsGrounded = false;
        jumpRequest = false;
    }

    private void GetPlayerInput(){

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        mouseHorizontal = Input.GetAxis("Mouse X");
        mouseVertical = Input.GetAxis("Mouse Y");

        if (IsGrounded && Input.GetButtonDown("Jump"))
            jumpRequest = true;
        
    }

    private float checkDownVel(float downSpeed){

        if (world.CheckVoxelIsSoilid(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
            world.CheckVoxelIsSoilid(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z + playerWidth)) ||
            world.CheckVoxelIsSoilid(new Vector3(transform.position.x - playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) ||
            world.CheckVoxelIsSoilid(new Vector3(transform.position.x + playerWidth, transform.position.y + downSpeed, transform.position.z - playerWidth)) 
        ) {
            IsGrounded = true;
            return 0;
        }
        else{
            IsGrounded = false;
            return downSpeed;
        }
    }

     private float checkUpVel(float upSpeed){

        if (world.CheckVoxelIsSoilid(new Vector3(transform.position.x + playerWidth, transform.position.y + upSpeed + 2f, transform.position.z + playerWidth)) ||
            world.CheckVoxelIsSoilid(new Vector3(transform.position.x - playerWidth, transform.position.y + upSpeed + 2f, transform.position.z + playerWidth)) ||
            world.CheckVoxelIsSoilid(new Vector3(transform.position.x - playerWidth, transform.position.y + upSpeed + 2f, transform.position.z - playerWidth)) ||
            world.CheckVoxelIsSoilid(new Vector3(transform.position.x + playerWidth, transform.position.y + upSpeed + 2f, transform.position.z - playerWidth)) 
        ) {
            return 0;
        }
        else{
            return upSpeed;
        }
    }

    public bool front {
        get{
            if (
               world.CheckVoxelIsSoilid(new Vector3(transform.position.x, transform.position.y, transform.position.z + playerWidth)) || 
               world.CheckVoxelIsSoilid(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z + playerWidth))
            )
                return true;
            else
                return false;
        }
    }

    public bool back {
        get{
            if (
               world.CheckVoxelIsSoilid(new Vector3(transform.position.x, transform.position.y, transform.position.z - playerWidth)) || 
               world.CheckVoxelIsSoilid(new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z - playerWidth))
            )
                return true;
            else
                return false;
        }
    }

    public bool right {
        get{
            if (
               world.CheckVoxelIsSoilid(new Vector3(transform.position.x + playerWidth, transform.position.y, transform.position.z)) || 
               world.CheckVoxelIsSoilid(new Vector3(transform.position.x + playerWidth, transform.position.y + 1f, transform.position.z))
            )
                return true;
            else
                return false;
        }
    }

    public bool left {
        get{
            if (
               world.CheckVoxelIsSoilid(new Vector3(transform.position.x - playerWidth, transform.position.y, transform.position.z)) || 
               world.CheckVoxelIsSoilid(new Vector3(transform.position.x - playerWidth, transform.position.y + 1f, transform.position.z))
            )
                return true;
            else
                return false;
        }
    }


    
}
