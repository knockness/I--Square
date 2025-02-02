using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    public Vector2 horizontalVector;

    public float horizontalInput;
    public float verticalInput;
    
    public float horizontalStorage;
    public float verticalStorage;

    public Vector2 magnitude;

    public float maxVelocityCompensation;

    public float stoppingVelocityCompensation;

    public float maxVelocityLeft;
    public float maxVelocityRight;

    public bool moveLeftChecked = false;
    public bool moveRightChecked = false;
    
    public float speed;
    public float initialSpeed;

    public float jumpForce;
    public bool jumpCheck = false;
    public int currentJumpBufferFrames;
    public int maxJumpBufferFrames;
    public bool grounded;
    public bool jumpControlCheck;
    public float jumpControlForce;

    public float fallForce;

    public bool dashState;
    public bool canDash;
    public float dashSpeed;
    public Vector2 dashDirection;
    public float dashDistance;

    public GameObject dashPoint;
    public GameObject newDashPoint;

    public Collider2D dashCollider;
    public Collider2D groundCollider;

    public BoxCollider2D groundLimitColliderL;
    public BoxCollider2D groundLimitColliderR;
    public float groundLimitOffset;

    public Rigidbody2D playerRigidbody;
    public GameObject player;
    public GameObject groundFused;
    public float groundFusedOffset;
    
    GroundFusedControlScript groundFusedControlScript;

    void Awake()
    {
        groundCollider.enabled = true;
        dashCollider.enabled = false;

        groundFused.SetActive(false);
        player.SetActive(true);

        groundFusedControlScript = groundFused.GetComponent<GroundFusedControlScript>();
        groundFusedControlScript.enabled = false;
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        if(!dashState)
        {
            groundCollider.enabled = true;
            dashCollider.enabled = false;

            playerRigidbody.gravityScale = 1;

            playerRigidbody.AddForce(Vector2.one * horizontalVector * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);

            if(playerRigidbody.velocity.x <= maxVelocityLeft)
            {
                playerRigidbody.AddForce(Vector2.right * maxVelocityCompensation);
            } else if(playerRigidbody.velocity.x >= maxVelocityRight)
            {
                playerRigidbody.AddForce(Vector2.left * maxVelocityCompensation);
            } else if(playerRigidbody.velocity.x == 0)
            {
                Debug.Log("no velocity");
            }

            if(horizontalInput == 0 && playerRigidbody.velocity.x > 0)
            {
                playerRigidbody.AddForce(Vector2.left * playerRigidbody.velocity.x * stoppingVelocityCompensation);
            } else if(horizontalInput == 0 && playerRigidbody.velocity.x < 0)
            {
                playerRigidbody.AddForce(Vector2.right * playerRigidbody.velocity.x * -1 * stoppingVelocityCompensation);
            }

            if(moveRightChecked == true)
            {
                playerRigidbody.AddForce(Vector2.right * initialSpeed);
                moveRightChecked = false;
            } else if(moveLeftChecked == true)
            {
                playerRigidbody.AddForce(Vector2.left*initialSpeed);
                moveLeftChecked = false;
            }

            if(jumpCheck == true)
            {
                playerRigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumpCheck = false;
            } else if(jumpControlCheck)
            {
                playerRigidbody.AddForce(Vector2.down * playerRigidbody.velocity.y * jumpControlForce);
                jumpControlCheck = false;
            }

            if(playerRigidbody.velocity.y < 0 && !grounded)
            {
                playerRigidbody.AddForce(Vector2.down * fallForce);
            } 
        } else if(dashState)
        {
            groundCollider.enabled = false;
            dashCollider.enabled = true;

            playerRigidbody.gravityScale = 0;
            playerRigidbody.MovePosition(playerRigidbody.position+dashDirection*dashSpeed*Time.fixedDeltaTime);
        }
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        horizontalVector = new Vector2(horizontalInput, 0);
        verticalInput = Input.GetAxisRaw("Vertical");

        magnitude = new Vector2(horizontalInput, verticalInput).normalized;

        if(verticalInput > 0f)
        {
            verticalStorage = 1; //The player faces forwards.
            horizontalStorage = 0;
        } else if (verticalInput < 0f) 
        {
            verticalStorage = -1; //The player faces backwards.
            horizontalStorage = 0;
        } else if (horizontalInput > 0f)
        {
            horizontalStorage = 1; //The player faces right.
            verticalStorage = 0;
        } else if (horizontalInput < 0f)
        {
            horizontalStorage = -1; //The player faces left.
            verticalStorage = 0;
        }

        if(Input.GetKeyDown(KeyCode.Z) && grounded)
        {
            jumpCheck = true;
        } else if(Input.GetKeyDown(KeyCode.Z) && !grounded)
        {
            currentJumpBufferFrames = maxJumpBufferFrames;
            StartCoroutine(JumpBufferCoroutine());
        } else if(Input.GetKeyUp(KeyCode.Z) && !grounded && playerRigidbody.velocity.y > 0)
        {
            jumpControlCheck = true;
        }

        if(horizontalInput == 1)
        {
            moveRightChecked = true;
        } else if(horizontalInput == -1)
        {
            moveLeftChecked = true;
        }

        if(grounded)
        {
            canDash = true;
        }

        if(Input.GetKeyDown(KeyCode.X) && canDash)
        {
            canDash = false;
            playerRigidbody.velocity = Vector2.zero;

            if(horizontalInput == 0 && verticalInput == 0)
            {
                dashState = true;
                newDashPoint = Instantiate(dashPoint, player.transform.position + new Vector3(horizontalStorage * dashDistance, verticalStorage * dashDistance), player.transform.rotation);
                newDashPoint.tag = "dashPoint";
                dashDirection = new Vector2(newDashPoint.transform.position.x - player.transform.position.x, newDashPoint.transform.position.y - player.transform.position.y).normalized;
            } else
            {
                dashState = true;
                Vector2 dashMagnitude = new Vector2(horizontalInput, verticalInput).normalized;
                newDashPoint = Instantiate(dashPoint, player.transform.position + new Vector3(dashMagnitude.x * dashDistance, dashMagnitude.y * dashDistance), player.transform.rotation);
                newDashPoint.tag = "dashPoint";
                dashDirection = new Vector2(newDashPoint.transform.position.x - player.transform.position.x, newDashPoint.transform.position.y - player.transform.position.y).normalized;
            }
        }
    }

    IEnumerator JumpBufferCoroutine()
    {
        if(currentJumpBufferFrames > 0)
        {
            yield return new WaitForFixedUpdate();
            currentJumpBufferFrames -= 1;
            StartCoroutine(JumpBufferCoroutine());
            yield break;
        } else if(currentJumpBufferFrames <= 0)
        {
            currentJumpBufferFrames = 0;
            yield break;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        foreach(GameObject dashPoint in GameObject.FindGameObjectsWithTag("dashPoint")) Destroy(dashPoint);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(dashState)
        {
            if(other.CompareTag("dashPoint"))
            {
                dashState = false;
                Destroy(other.gameObject);
                Debug.Log("Dashed");

                playerRigidbody.AddForce(dashDirection, ForceMode2D.Impulse);
            }else if(other.CompareTag("ground"))
            {
                foreach(GameObject dashPoint in GameObject.FindGameObjectsWithTag("dashPoint")) Destroy(dashPoint);
                dashState = false;

                player.SetActive(false);
                groundFused.SetActive(true);

                groundFused.transform.position = new Vector2(player.transform.position.x, other.transform.position.y + groundFusedOffset);
                groundFusedControlScript.enabled = true;
                Debug.Log("Ground Fuse");

                groundLimitColliderL = other.AddComponent<BoxCollider2D>();
                groundLimitColliderR = other.AddComponent<BoxCollider2D>();

                groundLimitOffset = groundLimitColliderL.size.x * 0.5f;

                groundLimitColliderL.size = new Vector2(0.01f, groundLimitColliderL.size.y);
                groundLimitColliderR.size = new Vector2(0.01f, groundLimitColliderR.size.y);

                groundLimitColliderL.offset = new Vector2(groundLimitColliderL.offset.x - groundLimitOffset, groundLimitColliderL.offset.y + groundLimitColliderL.size.y);
                groundLimitColliderR.offset = new Vector2(groundLimitColliderR.offset.x + groundLimitOffset, groundLimitColliderR.offset.y + groundLimitColliderR.size.y);
            } else
            {
                dashState = false;
                foreach(GameObject dashPoint in GameObject.FindGameObjectsWithTag("dashPoint")) Destroy(dashPoint);
                Debug.Log("Dash interupted");
            }
        } else if(!dashState)
        {
            if(other.CompareTag("ground") || other.CompareTag("block"))
            {
                grounded = true;
                if(Input.GetKey(KeyCode.Z) && currentJumpBufferFrames > 0)
                {
                    jumpCheck = true;
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
    }

    void OnTriggerExit2D(Collider2D other)
    {
            if(other.CompareTag("ground") || other.CompareTag("block"))
            {
                grounded = false;
            }
    }
}