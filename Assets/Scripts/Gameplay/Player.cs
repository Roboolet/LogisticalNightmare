using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] CharacterController controller;
    [SerializeField] Camera viewCam;
    [SerializeField] float speed, sensitivity, jumpForce, gravity, coyoteTime;
    float yvel;
    float coyote;
    Vector2 inputBuffer;
    Vector2 lookBuffer;

    Vector3 straightForward => new Vector3(Mathf.Sin(viewCam.transform.eulerAngles.y * Mathf.Deg2Rad), 0, Mathf.Cos(viewCam.transform.eulerAngles.y * Mathf.Deg2Rad));

    [Header("Pickup")]
    [SerializeField] Transform holdPoint;
    [SerializeField] LayerMask layerMask;
    [SerializeField] float pickupRange, dropForce, throwForce;
    Pickup heldPickup;

    bool gameInFocus;

    private void Awake()
    {
        OnApplicationFocus(true);
    }

    // Update is called once per frame
    void Update()
    {     
        inputBuffer = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;
        lookBuffer = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * sensitivity;

        //
        //
        // dont process the rest when game is not in focus
        if (!gameInFocus) return;

        //looking
        float eulx = viewCam.transform.eulerAngles.x;
        float euly = viewCam.transform.eulerAngles.y;
        viewCam.transform.eulerAngles = new Vector3(ClampVertLook(eulx - lookBuffer.y), euly + lookBuffer.x, 0);

        //jump and gravity        
        if (controller.isGrounded)
        {
            yvel = -0.01f;
            coyote = coyoteTime;
        }
        else
        {
            yvel -= gravity * Time.deltaTime;
            coyote -= Time.deltaTime;
        }

        if (Input.GetButton("Jump") && coyote > 0)
        {
            yvel = jumpForce;
            coyote = 0;
        }

        // pickup/drop
        if (heldPickup == null)
        {
            Ray r = new Ray(viewCam.transform.position, viewCam.transform.forward);
            if (Physics.Raycast(r, out RaycastHit hit, pickupRange, layerMask))
            {
                if (Input.GetButton("Fire1"))
                {
                    if (hit.transform.gameObject.layer == 6)
                    {
                        Pickup p = hit.transform.GetComponent<Pickup>();
                        if (p.pickupDelay <= 0)
                        {
                            heldPickup = p;
                            heldPickup.OnPickup();
                            heldPickup.transform.SetParent(holdPoint);
                        }
                    }
                    else
                    {
                        Button b = hit.transform.GetComponent<Button>();
                        if (b.pressable)
                        {
                            b.Press();
                        }
                    }
                }
            }
        }
        else
        {
            // drop
            if (Input.GetButtonUp("Fire1"))
            {
                ThrowHeldItem(0);
            }

            // throw
            else if (Input.GetButtonDown("Fire2"))
            {
                ThrowHeldItem(throwForce);
            }
        }
    }    

    private void FixedUpdate()
    {
        //moving
        controller.Move(new Vector3(0, yvel, 0) + straightForward * inputBuffer.y + viewCam.transform.right * inputBuffer.x);

        // move pickup to hand
        if (heldPickup != null)
        {
            heldPickup.rb.AddForce((holdPoint.transform.position - heldPickup.transform.position)*heldPickup.pickupSpeedMultiplier, ForceMode.VelocityChange);
            heldPickup.rb.velocity *= 0.9f;

            heldPickup.transform.rotation = Quaternion.Lerp(heldPickup.transform.rotation, Quaternion.LookRotation(straightForward, Vector3.up), 0.1f);
        }
    }
    void ThrowHeldItem(float forwardForce)
    {
        heldPickup.transform.SetParent(null);
        heldPickup.rb.velocity = (viewCam.transform.right * lookBuffer.x + viewCam.transform.up * lookBuffer.y) * dropForce * heldPickup.pickupSpeedMultiplier
            + controller.velocity + viewCam.transform.forward * forwardForce * heldPickup.pickupSpeedMultiplier;
        heldPickup.pickupDelay = 0.32f;
        heldPickup.OnDrop();

        heldPickup = null;
    }

    float ClampVertLook(float v)
    {
        if (v < 270 && v > 180) v = 270;
        else if (v > 90 && v <= 180) v = 90;

        return v;
    }

    private void OnApplicationFocus(bool focus)
    {
        gameInFocus = focus;

        //lock mouse cursor
        if (focus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
