using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
namespace PoleMiniGame
{
    public class Pole : GenericDestroyableSingletonClass<Pole>
    {
        [Header("Configuración")]
        public List<PolePieceInfo> polePiecesChoices;
        public GameObject piecePrefab;
        public int amountOfPieces;
        
        private float offset = 0f;
        private List<PolePiece> polePieces = new List<PolePiece>();
        private PolePiece _trophyPiece;
        private void Start()
        {
        }
        public void InitializeWithRandomPieces(int numberOfPieces) {
            PolePiece botPiece = SpawnPolePieceOfType(PolePieceType.Bot);
            polePieces.Add(botPiece);

            PolePiece fillPiece = SpawnPolePieceOfType(PolePieceType.NoPlate);
            polePieces.Add(fillPiece);
            while (polePieces.Count < numberOfPieces+2)
            {
                float chance = Random.value;
                if (chance <= .5f)
                {
                    PolePiece rightPiece = SpawnPolePieceOfType(PolePieceType.RightPlate);
                    polePieces.Add(rightPiece);
                } else if (chance <= 1f)
                {
                    PolePiece leftPiece = SpawnPolePieceOfType(PolePieceType.LeftPlate);
                    polePieces.Add(leftPiece);
                }
            }

            PolePiece topPiece = SpawnPolePieceOfType(PolePieceType.Top);
            polePieces.Add(topPiece);
            PolePiece trophyPiece = SpawnPolePieceOfType(PolePieceType.Trophy);
            polePieces.Add(trophyPiece);
            _trophyPiece = trophyPiece;
        }
        IEnumerator LerpPosition(Vector2 targetPosition, float duration)
        {
            float time = 0;
            Vector2 startPosition = transform.position;
            bool fork = false;
            float originalX = transform.position.x;
            while (time < duration)
            {
                Vector2 lerpPosition = Vector2.Lerp(startPosition, targetPosition, time / duration);
                if (fork)
                {
                    lerpPosition = new Vector2(0.08f, lerpPosition.y);
                    transform.position = lerpPosition;
                    fork = false;
                } else
                {
                    lerpPosition = new Vector2(-0.08f, lerpPosition.y);
                    transform.position = lerpPosition;
                    fork = true;
                }
                time += Time.deltaTime;
                yield return null;
            }
            transform.position = targetPosition;
        }
        public void MoveDown()
        {
            PolePiece polePiece = polePieces[0];
            Vector2 targetPosition = transform.position - new Vector3(0, polePiece.SpriteRenderer.bounds.size.y, 0);

            StartCoroutine(LerpPosition(targetPosition,0.3f));
        }
        public void HighlightTrophy()
        {
            _trophyPiece.AnimateHighlight();
        }
        private PolePiece SpawnPolePieceOfType(PolePieceType type)
        {
            PolePieceInfo info = GetPoleOfType(type);
            Vector3 offsetPosition = transform.position + new Vector3(0, offset, 0);
            GameObject piece = Instantiate<GameObject>(piecePrefab, offsetPosition, transform.rotation, transform);
            PolePiece pieceController = piece.gameObject.GetComponent<PolePiece>();
            pieceController.poleInfo = info;
            
            pieceController.RefreshSprite();
            if (type == PolePieceType.Top)
            {

                offset += pieceController.SpriteRenderer.bounds.size.y/2;
            }
            else
            {
                offset += pieceController.SpriteRenderer.bounds.size.y;
            }
            return pieceController;
        }
        public PolePieceInfo GetPoleOfType(PolePieceType type)
        {
            return polePiecesChoices.First<PolePieceInfo>(pieceInfo => pieceInfo.poleType == type);
        }
    }
    [System.Serializable]
    public class PolePieceInfo    
    {
        public PolePieceType poleType;
        public Sprite sprite;
    }
    public enum PolePieceType
    {
        LeftPlate, RightPlate, NoPlate, Top, Bot, Trophy
    }
}
