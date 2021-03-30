using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceShield : MonoBehaviour
{
    private Collider col;
    private SpriteRenderer shieldRenderer;
    // Start is called before the first frame update
    void Start()
    {
        shieldRenderer = gameObject.GetComponent<SpriteRenderer>();
        col = gameObject.GetComponent<Collider>();
    }

    public void Down()
    {
        if (col != null)
        {
            col.enabled = false;
        }
        shieldRenderer.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
    }
}
