using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundFusedControlScript : MonoBehaviour
{
    public float horizontalInput;
    public Vector2 horizontalVector;

    public float maxVelocityCompensation;

    public float stoppingVelocityCompensation;

    public float maxVelocityLeft;
    public float maxVelocityRight;

    public bool moveLeftChecked = false;
    public bool moveRightChecked = false;
    
    public float speed;
    public float initialSpeed;

    public Rigidbody2D groundFusedRigidbody;

    public Animator groundFusedAnimator;


    void Awake()
    {
        groundFusedAnimator = GetComponent<Animator>();
    }

    void Start()
    {
        
    }

    void FixedUpdate()
    {

        groundFusedRigidbody.gravityScale = 0;

        groundFusedRigidbody.AddForce(Vector2.one * horizontalVector * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);

        if(groundFusedRigidbody.velocity.x <= maxVelocityLeft)
        {
            groundFusedRigidbody.AddForce(Vector2.right * maxVelocityCompensation);
        } else if(groundFusedRigidbody.velocity.x >= maxVelocityRight)
        {
            groundFusedRigidbody.AddForce(Vector2.left * maxVelocityCompensation);
        } else if(groundFusedRigidbody.velocity.x == 0)
        {
            Debug.Log("no velocity");
        }

        if(horizontalInput == 0 && groundFusedRigidbody.velocity.x > 0)
        {
            groundFusedRigidbody.AddForce(Vector2.left * groundFusedRigidbody.velocity.x * stoppingVelocityCompensation);
        } else if(horizontalInput == 0 && groundFusedRigidbody.velocity.x < 0)
        {
            groundFusedRigidbody.AddForce(Vector2.right * groundFusedRigidbody.velocity.x * -1 * stoppingVelocityCompensation);
        }

        if(moveRightChecked == true)
        {
            groundFusedRigidbody.AddForce(Vector2.right * initialSpeed);
            moveRightChecked = false;
        } else if(moveLeftChecked == true)
        {
            groundFusedRigidbody.AddForce(Vector2.left*initialSpeed);
            moveLeftChecked = false;
        }
    }

    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        horizontalVector = new Vector2(horizontalInput, 0);

        if(Input.GetKeyDown(KeyCode.Space))
        {
            //Ground spikes code goes here.
            groundFusedAnimator.SetTrigger("spikeEntry");
        }
    }
}
