using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoleMiniGame
{
    public class PolePlayer : MonoBehaviour
    {
        public Animator impactAnimator;
        public SpriteRenderer colorRenderer;
        
        private Animator animator;
        private BoxCollider2D _collider;
        private bool shieldBlocking = false;
        private bool plateOnRange = false;
        private Shield shieldController;
        private void Start()
        {
            animator = gameObject.GetComponent<Animator>();
            _collider = gameObject.GetComponent<BoxCollider2D>();
        }
        public void SetColor(Material newMaterial)
        {
            colorRenderer.color = newMaterial.color;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Shield"))
            {
                shieldController = collision.gameObject.GetComponent<Shield>();
                shieldBlocking = true;
            } else if (collision.CompareTag("Plate"))
            {
                plateOnRange = true;
            }
        }
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Shield"))
            {
                shieldController = null;
                shieldBlocking = false;
            }
            else if (other.CompareTag("Plate"))
            {
                plateOnRange = false;
            }
        }
        public void AnimateImpact()
        {
            if (plateOnRange && !shieldBlocking)
            {
                impactAnimator.Play("Hit");
                Pole.Instance.MoveDown();
                PoleGameManager.Instance.photonView.RPC("ReducePoints", Photon.Pun.RpcTarget.All, GameManager.Instance.GetMainPlayer().playerStats.id);
                PoleGameManager.Instance.photonView.RPC("UpdateTime", Photon.Pun.RpcTarget.Others, GameManager.Instance.GetMainPlayer().playerStats.id, PoleGameManager.Instance.playerStats.timeSpent);

                if (PoleGameManager.Instance.playerStats.piecesLeft == 0)
                {
                    Pole.Instance.HighlightTrophy();
                    PoleGameManager.Instance.enabledToPlay = false;
                }
            } else if (shieldBlocking)
            {
                shieldController.Hit();
            }
        }
        public void Attack()
        {
            if (plateOnRange && !shieldBlocking)
            {
                animator.SetBool("Hit", true);
            }
            else
            {
                animator.SetBool("Hit", false);
            }

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("FailedRecovery") || animator.GetCurrentAnimatorStateInfo(0).IsName("Impact"))
                return;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("IddleAttack"))
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                animator.SetTrigger("FastAttack");
            }
        }
        private void TestAttack()
        {
            if (plateOnRange && !shieldBlocking)
            {
                animator.SetBool("Hit", true);
            }
            else
            {
                animator.SetBool("Hit", false);
            }
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("FailedRecovery") || animator.GetCurrentAnimatorStateInfo(0).IsName("Impact"))
                return;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("IddleAttack"))
            {
                animator.SetTrigger("Attack");
            } else
            {
                animator.SetTrigger("FastAttack");
            }
        }
        private void TestMiss()
        {
            animator.SetBool("Hit", false); 
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("FailedRecovery") || animator.GetCurrentAnimatorStateInfo(0).IsName("Impact"))
                return;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("IddleAttack"))
            {
                animator.SetTrigger("Attack");
            }
            else
            {
                animator.SetTrigger("FastAttack");
            }
        }
    }
}
