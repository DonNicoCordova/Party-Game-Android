using UnityEngine;
public class CharacterMoveController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 10f;

    CharacterController _characterController;
    public bool IsGrounded;
    public Joystick joystick = null;
    public PlayerStats player = null;
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("CapturePoint") && player.MovesLeft() != 0)
        {
            Debug.Log($"COLLISION DETECTED {other}");
            LocationController controller = other.GetComponentInParent<LocationController>();
            if (!controller.CheckIfPlayerOnTop(player))
            {
                controller.AddPlayer(player);
                player.CaptureZone(controller);
                if (player.isPlayer)
                {
                    GameManager.instance.SetThrowText();
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CapturePoint"))
        {
            LocationController controller = other.GetComponentInParent<LocationController>();
            controller.RemovePlayer(player);
        }
    }
    void Awake() => _characterController = GetComponent<CharacterController>();
    void FixedUpdate()
    {
        if (joystick != null)
        {
            float horizontal = joystick.Horizontal;
            float vertical = joystick.Vertical;
            Vector3 direction = new Vector3(horizontal, 0, vertical);
            Vector3 movement = transform.TransformDirection(direction) * _moveSpeed;
            IsGrounded = _characterController.SimpleMove(movement);
        }
    }
}