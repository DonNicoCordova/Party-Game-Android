using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PoleMiniGame
{
    public class ShieldCrystal : MonoBehaviour, IPointerClickHandler
    {
        public Shield shield;
        private SpriteRenderer sprite;
        private Collider2D _collider2D;
        private Animator animator;
        private void Awake()
        {
            _collider2D = gameObject.GetComponent<Collider2D>();
            animator = gameObject.GetComponent<Animator>();
            sprite = gameObject.GetComponentInChildren<SpriteRenderer>();
            Disable();
        }
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            //Output to console the clicked GameObject's name and the following message. You can replace this with your own actions for when clicking the GameObject.
            shield.Disable();
            Die();
        }
        public bool IsEnabled()
        {
            return sprite.enabled;
        }
        public void Enable()
        {
            animator.Play("Spawn");
            _collider2D.enabled = true;
            sprite.enabled = true;
            shield.Enable();
        }
        public void Die()
        {
            animator.Play("Destroy");
        }
        public void Disable()
        {
            _collider2D.enabled = false;
            sprite.enabled = false;
        }

    }
}
