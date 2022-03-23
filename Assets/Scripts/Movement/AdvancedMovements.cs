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


    [Header("Checks for ground")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 GroundCheckBoxSize;



    [Header("Layer Details")]
    [SerializeField] private LayerMask groundLayer;

    private float horizontalDirection;

    

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
    }

    private void applyFriction()
    {
        if (Mathf.Abs(horizontalDirection) < 0.4f)
        {
            playerRb.drag = _linearDrag;
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


    private async void moveThroughForce(){
        float targetSpeed = horizontalDirection * moveSpeed;
        float speedDif = targetSpeed - playerRb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deacceleration;
        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velocityPower) * Mathf.Sign(speedDif);
        Debug.Log("Still applying movement ++  " + movement  + " accel rate being  " + accelRate + "  targetSpeed being" + targetSpeed + " speedDifference beging" + speedDif);
        Debug.Log("Velocity power statement  ++ " + Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velocityPower));

        if(!Mathf.Approximately(movement, 0.0f)){    
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

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && playerRb.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            playerRb.velocity = new Vector2(playerRb.velocity.x, jForce);
        }

        // if (context.canceled && playerRb.velocity.y > 0f)
        // {
        //     playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y * 0.5f);
        // }
    }
}
