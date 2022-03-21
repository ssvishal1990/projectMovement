using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class AdvancedMovements : MonoBehaviour
{


    [Header("Components")]
    [SerializeField] private Rigidbody2D playerRb;
    [SerializeField] private SpriteRenderer playerSprite;


    [Header("Movement values")]
    [SerializeField] private float movementAcceleration;
    [SerializeField] private float _maxspeed;
    [SerializeField] float _linearDrag;
    [SerializeField] float jumpForce = 5f;


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
        Move();
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
            playerRb.velocity = new Vector2(playerRb.velocity.x, jumpForce);
        }

        // if (context.canceled && playerRb.velocity.y > 0f)
        // {
        //     playerRb.velocity = new Vector2(playerRb.velocity.x, playerRb.velocity.y * 0.5f);
        // }
    }
}
