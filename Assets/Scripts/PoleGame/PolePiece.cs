using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoleMiniGame
{
    public class PolePiece : MonoBehaviour
    {
        public PolePieceInfo poleInfo;
        public SpriteRenderer SpriteRenderer { get; private set; }
        public Collider2D leftPlateCollider;
        public Collider2D rightPlateCollider;

        private Animator _highlightAnimator;
        private void Awake()
        {
            _highlightAnimator = gameObject.GetComponentInChildren<Animator>();
            SpriteRenderer = gameObject.GetComponent<SpriteRenderer>();  
        }
        public void AnimateHighlight()
        {
            _highlightAnimator.SetTrigger("Moving");
        }
        public void RefreshSprite()
        {
            if (poleInfo.poleType == PolePieceType.LeftPlate)
            {
                leftPlateCollider.isTrigger = true;
                rightPlateCollider.isTrigger = false;
            } else if (poleInfo.poleType == PolePieceType.RightPlate)
            {
                leftPlateCollider.isTrigger = false;
                rightPlateCollider.isTrigger = true;
            }
            SpriteRenderer.sprite = poleInfo.sprite;
        }
    }

}
