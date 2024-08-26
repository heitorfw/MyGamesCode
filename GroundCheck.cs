using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider2D))]
public class GroundCheck : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The layers which are considered \"Ground\".")]
    public LayerMask groundLayers = new LayerMask();
    [Tooltip("The collider to check with. (Defaults to the collider on this game object.)")]
    public Collider2D groundCheckCollider = null;

    [Header("Effect Settings")]
    [Tooltip("The effect to create when landing")]
    public GameObject landingEffect;

    [HideInInspector]
    public bool groundedLastCheck = false;

   
    private void Start()
    {
        
        GetCollider();
    }

   
    public void GetCollider()
    {
        if (groundCheckCollider == null)
        {
            groundCheckCollider = gameObject.GetComponent<Collider2D>();
        }
    }

    
    public bool CheckGrounded()
    {
      
        if (groundCheckCollider == null)
        {
            GetCollider();
        }

        
        Collider2D[] overlaps = new Collider2D[5];
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.layerMask = groundLayers;
        groundCheckCollider.OverlapCollider(contactFilter, overlaps);

        
        foreach (Collider2D overlapCollider in overlaps)
        {
            if (overlapCollider != null)
            {
               
                int match = contactFilter.layerMask.value & (int)Mathf.Pow(2, overlapCollider.gameObject.layer);
                if (match > 0)
                {
                    if (landingEffect && !groundedLastCheck)
                    {
                        Instantiate(landingEffect, transform.position, Quaternion.identity, null);
                    }
                    groundedLastCheck = true;
                    return true;
                }
            }
        }
        groundedLastCheck = false;
        return false;
    }
}
