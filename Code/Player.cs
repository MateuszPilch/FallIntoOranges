using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Animator anim;
    Rigidbody rb;
    GameManager gm;
    Vector2 pressDownPosition, pressLastPosition;

    public GameObject endTargetLocation;
    public ParticleSystem particlesObject;
    public float playerMoveSpeed, playerJumpPower;
    private float defaultCameraFOV = 75f, cameraFOVOffest = 0;

    void Start()
    {
        anim = GetComponent<Animator>();
        gm = FindObjectOfType<GameManager>();
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        if(gm.currentPhase == 0)
        {
            Camera.main.fieldOfView = defaultCameraFOV;
            if (!particlesObject.isStopped)
            {
                particlesObject.Stop();
            }
        }
        else if (gm.currentPhase == 2)
        {

            if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Returning"))
            {
                if (Input.GetMouseButtonDown(0) == true)
                {
                    pressDownPosition = Input.mousePosition;
                }

                pressLastPosition = Input.mousePosition;
            }
            else
            {
                pressLastPosition = Input.mousePosition;
                pressDownPosition = pressLastPosition;
            }

            if (endTargetLocation != null)
            {
                transform.position = Vector3.MoveTowards(transform.position, endTargetLocation.transform.position, Time.deltaTime * playerMoveSpeed);
            }

            if (transform.position == endTargetLocation.transform.position)
            {
                gm.FailPhase();
            }

            Movement();
        }
        else if(gm.currentPhase == 3 || gm.currentPhase == 4)
        { 
            if (rb.velocity.magnitude > 4f && rb.velocity.y < 0f)
            {
                cameraFOVOffest += 10 * Time.deltaTime;

                if(!particlesObject.isPlaying)
                {
                    particlesObject.Play();
                }
               
            }
            else
            {
                if (Camera.main.fieldOfView != defaultCameraFOV)
                {
                    cameraFOVOffest = 0;
                    Camera.main.fieldOfView = defaultCameraFOV;
                }

                if (!particlesObject.isStopped)
                {        
                    particlesObject.Stop();
                }
                
            }

            Camera.main.fieldOfView = defaultCameraFOV + cameraFOVOffest;
        }
    }

    float SwipePath()
    {
        float dragValue;

        if (pressDownPosition.y / Screen.height < pressLastPosition.y / Screen.height)
        {
            dragValue = Mathf.Clamp(Vector2.Distance(new Vector2(0, pressDownPosition.y / Screen.height), new Vector2(0, pressLastPosition.y / Screen.height)) * 2.25f, 0f, 1f);
        }
        else
        {
            dragValue = Mathf.Clamp(-Vector2.Distance(new Vector2(0, pressDownPosition.y / Screen.height), new Vector2(0, pressLastPosition.y / Screen.height)) * 2.25f, -1f, 0f);
        }
        return dragValue;
    }

    void Movement()
    {
        if (!gm.pauseMenuObject.activeInHierarchy)
        {
            if (Input.GetMouseButton(0))
            {
                anim.SetBool("animReturn", false);

                if (!anim.GetCurrentAnimatorStateInfo(0).IsTag("Returning") || (anim.GetCurrentAnimatorStateInfo(0).IsTag("Returning") && anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f))
                {
                    if (SwipePath() > 0f)
                    {
                        anim.Play("Looking", 0, SwipePath());
                    }
                    else
                    {
                        anim.Play("StepBack", 0, -SwipePath());
                        playerJumpPower = Mathf.Abs(SwipePath());
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) == true && !anim.GetCurrentAnimatorStateInfo(0).IsTag("Returning"))
            {
                anim.SetBool("animReturn", true);
                anim.SetFloat("jumpPower", 1 + playerJumpPower);

                if (SwipePath() > 0f)
                {
                    anim.Play("LookingReturn", 0, 1 - SwipePath());
                }
                else
                {
                    anim.Play("StepBackReturn", 0, 1 + SwipePath());
                }

            }
        }
    }
    
    public void Jump()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Returning") && playerJumpPower > 0.2f)
        {
            gm.currentPhase = 3;
            
            rb.isKinematic = false;
            rb.AddForce(Camera.main.transform.forward * 300f * playerJumpPower); 
            rb.AddForce(Camera.main.transform.up * 250f);
            rb.AddTorque(Camera.main.transform.right * 10);
        }

    }
    private void OnCollisionEnter(Collision col)
    {
        if(col.gameObject.tag == "Floor" && gm.currentPhase == 3)
        {
            gm.FailPhase();
        }
    }
}