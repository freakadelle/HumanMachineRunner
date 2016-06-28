using UnityEngine;
using System.Collections;

public class AvatarTest : MonoBehaviour
{

    private Vector3 _moveDirection = Vector3.zero;
    private float Gravity = 20;
    private float JumpHeight = 10;
    private float _xMovement;
    private float _zMovement;
    public int Speed = 10;

    private CharacterController _controller;

    private float _rotAngle;

    // Use this for initialization
    void Start()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (_controller.isGrounded)
        {
            float rot = Input.GetAxis("Horizontal");
            _rotAngle = transform.localRotation.eulerAngles.y + rot;
            transform.localRotation = Quaternion.Euler(0, _rotAngle, 0);

           
            float v = Time.deltaTime * Speed;
            _xMovement = Mathf.Sin(_rotAngle * Mathf.Deg2Rad) * v;
            _zMovement = Mathf.Cos(_rotAngle * Mathf.Deg2Rad) * v;

            if (Input.GetKeyDown(KeyCode.Y))
            {
                Strafe(-10);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                Strafe(10);
            }

            if (Input.GetButton("Jump"))
            {
                _moveDirection.y = JumpHeight;
            }
        }

        _moveDirection.x = _xMovement;
        _moveDirection.z = _zMovement;
        _moveDirection.y -= Gravity * Time.deltaTime;
        _controller.Move(_moveDirection * Time.deltaTime);
    }

    private void Strafe(float strafeValue)
    {
        var localPos = transform.InverseTransformPoint(transform.position);
        localPos.x = localPos.x + strafeValue;
        var worldPos = transform.TransformPoint(localPos);
        _xMovement = worldPos.x;
        _zMovement = worldPos.z;
    }
 }

