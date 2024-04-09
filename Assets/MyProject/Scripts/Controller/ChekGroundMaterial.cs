using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChekGroundMaterial : MonoBehaviour
{
    RaycastHit hit;
    public Material currentMaterial;

    void Start()
    {
        
    }

    void Update()
    {
        Ray ray = new Ray(transform.position + Vector3.up, Vector3.down);
        if (Physics.Raycast(ray, out hit, 2, Physics.AllLayers, QueryTriggerInteraction.Collide))
        {
            Renderer renderer = hit.collider.GetComponentInChildren<Renderer>();
            currentMaterial = renderer ? renderer.sharedMaterial : null;
        }
        else
        {
            currentMaterial = null;
        }

    }
}
