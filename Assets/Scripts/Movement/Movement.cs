using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] float x_dir_moveSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] CapsuleCollider2D groundCheckCapsuleCollider;
    [SerializeField] private float verticalSlopeCheckDistance;
    [SerializeField] LayerMask groundLayer;
    
    
    private float downAngle;
    private float horizontal;

    // private float speed;


    Vector2 groundCheckCapsuleSize;
    Vector2 slopeNormalPerpendicular;
    Vector2 newVelocity;



    Rigidbody2D playerBody;
    SpriteRenderer playerSpirte;
    ProjectMovement playerInputSystem;

    bool facingRight;
    void Start()
    {
        playerBody = GetComponent<Rigidbody2D>();
        playerSpirte  = GetComponent<SpriteRenderer>();
        playerInputSystem = new ProjectMovement();
        playerSpirte.color = Color.white;
        groundCheckCapsuleSize = groundCheckCapsuleCollider.size;
    }

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        move();
        slopeCheck();
        // updateRotation();
    }

    private void updateRotation(){
        Quaternion rotation = transform.rotation;
        Vector3 eulerAngle = rotation.eulerAngles;
        eulerAngle.z = downAngle;
        rotation.eulerAngles = eulerAngle;
        transform.rotation = rotation;
    }


    public void  moveThroughContext(InputAction.CallbackContext context){
        horizontal = context.ReadValue<Vector2>().x;
    }


    private void move()
    {
        playerBody.velocity = new Vector2(horizontal * x_dir_moveSpeed, playerBody.velocity.y);
        checkForFlip(horizontal * x_dir_moveSpeed);
    }

    private void checkForFlip(float x_speed)
    {
        if (x_speed < 0 && facingRight)
        {
            FlipPlayer();
            playerSpirte.color = Color.yellow;
        }
        else if (x_speed > 0 && !facingRight)
        {
            FlipPlayer();
            playerSpirte.color = Color.white;
        }
    }

    private void FlipPlayer()
    {
        transform.localScale = new Vector3(-1 * transform.localScale.x, transform.localScale.y, transform.localScale.z);
        facingRight = !facingRight;
    }


    private void slopeCheck(){
        Vector2 checkPosition = transform.position - new Vector3(0.0f, groundCheckCapsuleSize.y/2,0.0f);
        // Debug.Log("CCheck position value = " + checkPosition);
        slopeCheckVertical(checkPosition);
    }

    private void SlopeCheckHorizontal(Vector2 checkPosition){
    
    }

    private void slopeCheckVertical(Vector2 checkPosition){
        // 
        RaycastHit2D hit = Physics2D.Raycast(checkPosition, Vector2.down, verticalSlopeCheckDistance, groundLayer);
        

        if(hit){
            
            slopeNormalPerpendicular = Vector2.Perpendicular(hit.normal);
            downAngle = Vector2.Angle(hit.normal, Vector2.up);


            Debug.Log("Draw  " + hit.point + " " +hit.normal  + " " + downAngle + " slopeNormalPerpendicular " + slopeNormalPerpendicular);
            Debug.DrawRay(hit.point, hit.normal, Color.black);
            Debug.DrawRay(hit.point, slopeNormalPerpendicular, Color.red);

            Vector2 slopeNormalPerpendicularClockwise = Vector2.Perpendicular(Vector2.Perpendicular(Vector2.Perpendicular(hit.normal)));
            Debug.DrawRay(hit.point, slopeNormalPerpendicularClockwise, Color.magenta);
            
        }else{
            Debug.Log("Before ground hit   " + hit.point);
        }
    }   


    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed && playerBody.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, jumpForce);
        }

        if (context.canceled && playerBody.velocity.y > 0f)
        {
            playerBody.velocity = new Vector2(playerBody.velocity.x, playerBody.velocity.y * 0.5f);
        }
    }


}
