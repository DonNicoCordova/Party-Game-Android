using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceController : MonoBehaviour
{
    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        StartCoroutine(RandomAppearence());
    }

    
    IEnumerator RandomAppearence()
    {
        yield return new WaitForSeconds(7);
        anim.SetTrigger("JiggleTR");
        anim.SetTrigger("JiggleTL");
        anim.SetTrigger("JiggleBR");
        anim.SetTrigger("JiggleBL");
    }
}
