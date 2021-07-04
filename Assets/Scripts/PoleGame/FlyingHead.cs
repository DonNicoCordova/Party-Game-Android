using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PoleMiniGame
{
    public class FlyingHead : MonoBehaviour, IPointerClickHandler
    {
        public SpriteRenderer head;
        public SpriteRenderer leftWing;
        public SpriteRenderer rightWing;

        private SpriteRenderer[] sprites;
        private Collider2D _collider2D;
        private Animator animator;
        private bool fork;
        private void Awake()
        {
            _collider2D = gameObject.GetComponent<Collider2D>();
            animator = gameObject.GetComponent<Animator>();
            sprites = gameObject.GetComponentsInChildren<SpriteRenderer>();
            Disable();
            StartCoroutine(Setup());
        }
        public void OnPointerClick(PointerEventData pointerEventData)
        {
            Die();
        }
        public bool IsEnabled()
        {
            return _collider2D.enabled;
        }
        public void Enable()
        {
            animator.enabled = false;
            animator.enabled = true;
            bool enterType = Convert.ToBoolean(UnityEngine.Random.Range(0,2));
            if (enterType)
            {
                animator.SetTrigger("Enter1");
            } else
            {
                animator.SetTrigger("Enter2");
            }
            _collider2D.enabled = true;
            
            head.enabled = true;
            leftWing.enabled = true;
            rightWing.enabled = true;
        }
        public void Die()
        {
            animator.SetTrigger("Die");
            float chance = UnityEngine.Random.value;
            bool enterType = Convert.ToBoolean(UnityEngine.Random.Range(0, 2));
            if (enterType)
            {
                PoleGameManager.Instance.photonView.RPC("BlockRightPlate", RpcTarget.All, PoleGameManager.Instance.GetMostPoints().playerId);
            } else
            {

                PoleGameManager.Instance.photonView.RPC("BlockLeftPlate", RpcTarget.All, PoleGameManager.Instance.GetMostPoints().playerId);
            }
        }
        public void Disable()
        {
            _collider2D.enabled = false;
            head.enabled = false;
            leftWing.enabled = false;
            rightWing.enabled = false;
        }
        private IEnumerator Setup()
        {

            yield return new WaitForSeconds(3f);
            fork = !fork;
            animator.SetBool("Fork", fork);

        }
    }
}
