using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoleMiniGame
{
    public class Shield : MonoBehaviour
    {
        private SpriteRenderer sprite;
        private Collider2D _collider2D;
        private Animator animator;
        private void Awake()
        {
            _collider2D = gameObject.GetComponent<Collider2D>();
            animator = gameObject.GetComponent<Animator>();
            sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
            Hide();
        }
        public void Enable()
        {
            _collider2D.enabled = true;
            _collider2D.isTrigger = true;
            animator.SetBool("Enabled", true);
            animator.SetTrigger("Spawn");
        }
        public void Disable()
        {
            animator.SetBool("Enabled", false);
            animator.Play("Destroy");
        }
        public void Hide()
        {
            _collider2D.enabled = false;
            sprite.enabled = false;
        }
        public void Hit()
        {   
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName("Destroy"))
                animator.Play("Hit");
        }
    }
}
