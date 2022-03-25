using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class AdvancedMovements : MonoBehaviour
{


    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private SpriteRenderer playerSprite;


    [Header("Movement values through velocity")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float _maxspeed;
    [SerializeField] float _linearDrag;
    [SerializeField] float jForce = 5f;

    [Header("Movement components through force")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deacceleration;
    [SerializeField] private float velocityPower;
    [SerializeField] private float frictionAmount;


    [Header("Jumping through force")]
    [SerializeField] private float jumpForce;
    [SerializeField] [Range(0, 1)] public float jumpCutMultiplier;
    [SerializeField] private float jumpCoyoteTime;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float fallGravityMultiplier;
    [SerializeField] private int aaditionalJumps;
    [SerializeField] private float jumpBufferingRayCastLength;


    [Header("Checks for ground")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 GroundCheckBoxSize;
    [SerializeField] private float groundCheckCircleSize;



    [Header("Layer Details")]
    [SerializeField] private LayerMask groundLayer;

    private float horizontalDirection;
    public float LastOnGroundTime { get; private set; }
    public float LastJumpTime { get; private set; }

    public  bool isJumping { get; private set; }
    public  bool jumpKeyReleased { get; private set; }


    private float coyoteTime = 0.2f;
    public float coyoteTimeCounter = 0;
    public  int currentJumpCounter;
    private bool jumpQueued = false;

    private bool onGround = true;


    

    private bool facingRight;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        moveThroughForce();
        // move();
        applyFriction();
        checkIfGrounded();


    }

    void checkIfGrounded(){
        onGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckCircleSize, groundLayer); 

        if(onGround){  // 
            coyoteTimeCounter = jumpCoyoteTime;
        }else{
            coyoteTimeCounter -= Time.deltaTime;
        }
    }

    

    private void OnDrawGizmosSelected()
    {
        if(groundCheckPoint == null){
            return;
        }
        Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckCircleSize);    
    }

    private void applyFriction()
    {
        if (Mathf.Abs(horizontalDirection) < 0.4f)
        {
            playerRb.drag = _linearDrag; // Approach 1

            float amount = Mathf.Min(Mathf.Abs(playerRb.velocity.x), Mathf.Abs(frictionAmount));
            amount *= Mathf.Sign(playerRb.velocity.x);
            playerRb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
        }
        else
        {
            playerRb.drag = 0;
        }
    }

    

    private void Move()
    {
        playerRb.AddForce(new Vector2(horizontalDirection, 0f) * movementAcceleration);

        if (Mathf.Abs(playerRb.velocity.x) >= _maxspeed)
        {
            playerRb.velocity = new Vector2(Mathf.Sign(playerRb.velocity.x) * _maxspeed, playerRb.velocity.y);
            // Debug.Log(Mathf.Sign(playerRb.velocity.x) + "   **    " + Mathf.Sign(playerRb.velocity.x) * _maxspeed + "   velocity **  " + playerRb.velocity);
        }


        checkAndFlip();
    }


    private  void moveThroughForce(){
        float targetSpeed = horizontalDirection * moveSpeed;
        float speedDif = targetSpeed - playerRb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deacceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velocityPower) * Mathf.Sign(speedDif);
        float fc = (float)Math.Round(movement * 100f) / 100f;
        // Debug.Log("Still applying movement ++  " + movement  + " accel rate being  " + accelRate + "  targetSpeed being" + targetSpeed + " speedDifference beging" + speedDif);
        // Debug.Log("Velocity power statement  ++ " + Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velocityPower) + "  " + (int)movement + "  using mathf round " + Mathf.Round(movement) + "  " + fc);

        if(!Mathf.Approximately(fc, 0.0f)){    
            playerRb.AddForce(movement * Vector2.right);
        }
        
        checkAndFlip();
    }

    private void checkAndFlip()
    {
        if (Mathf.Sign(playerRb.velocity.x) < 0 && facingRight)
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            playerSprite.color = Color.yellow;
            facingRight = !facingRight;
        }
        else if (Mathf.Sign(playerRb.velocity.x) > 0 && !facingRight)
        {
            transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
            playerSprite.color = Color.white;
            facingRight = !facingRight;
        }
    }

    public void  getXDirection(InputAction.CallbackContext context){
        horizontalDirection = context.ReadValue<Vector2>().x;
        // Debug.Log(context + "  horizon value " + horizontalDirection);
    }

    public void TriggerJump(InputAction.CallbackContext context)
    {
        if (context.performed && (coyoteTimeCounter > 0f))// || currentJumpCounter > 0) )
        {
            // playerRb.velocity = new Vector2(playerRb.velocity.x, jForce);
            Jump();
        }else if(context.performed){
            checkIfNearGround();
        }

        if (context.canceled){
            jumpKeyReleased = true;
            if(currentJumpCounter == 0){
               coyoteTimeCounter = 0f; 
            }
            // coyoteTimeCounter = 0f;
            if(playerRb.velocity.y > 0){
               playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y * 0.5f); 
            }
        }
    }

    private void checkIfNearGround(){
        RaycastHit2D hit = Physics2D.Raycast(groundCheckPoint.position, Vector2.down, jumpBufferingRayCastLength, groundLayer);
        Debug.Log("Check if near ground got called");
        Debug.DrawRay(hit.point, hit.normal, Color.green);
        if(hit && playerRb.velocity.y < 0){
            Debug.Log("Inside it's iff condition");
            Debug.DrawRay(hit.point, hit.normal, Color.red);
            jumpQueued = true;
        }
    }

    private void Jump()
    {
        playerRb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        LastOnGroundTime = 0;
        LastJumpTime = 0;
        isJumping = true;
        jumpKeyReleased = false;
        currentJumpCounter--;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Ground"){
            currentJumpCounter = aaditionalJumps;
            if(jumpQueued){
                jumpQueued = false;
                Jump();
            }
        }
    }
}
