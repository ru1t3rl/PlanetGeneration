using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Ru1t3rl.Player.Movement
{
    public class ThirdPersonMovement : MonoBehaviour
    {
        [SerializeField] CharacterController controller;

        [Header("Animation")]
        [SerializeField] Animator animator;
        [SerializeField] GameObject visual;
        [SerializeField] PlayerState startState = PlayerState.Flying;
        [SerializeField] bool rotateFlatWhenFlying = true;

        [Header("Physics")]
        [SerializeField] float acceleration = 1f;
        [SerializeField] float maxSpeedBoostMultiplier = 1.5f;
        [SerializeField] float maxSpeed = 6f;
        [SerializeField] float minSpeed = .2f;
        [Tooltip("X: Horizontal Axis | Y: Vertical Axis")]
        [SerializeField] Vector2 rotationSpeed = new Vector2(500f, 500f);
        [SerializeField] float mass = 1f;
        [SerializeField] float gravity = 9.81f;
        [SerializeField] float drag = 1f;

        [Header("Events")]
        public UnityEvent OnStartFlying;
        public UnityEvent OnFlying, OnStopFlying;

        Vector2 input = Vector2.zero;
        Vector3 direction, pVelocity, velocity;
        Quaternion targetRotation;
        PlayerState state;

        private void Awake()
        {
            state = startState;
        }


        void Update()
        {
            animator.SetBool("InAir", state == PlayerState.Flying);
            switch (state)
            {
                case PlayerState.Flying:
                    InAir();
                    break;
                case PlayerState.Grounded:
                    OnGround();
                    break;
            }
        }

        void OnGround()
        {

        }

        void InAir()
        {
            if (rotateFlatWhenFlying)
                visual.transform.localRotation = Quaternion.RotateTowards(visual.transform.localRotation, Quaternion.Euler(velocity.sqrMagnitude >= minSpeed ? -90 : 0, 0, 0), rotationSpeed.y * Time.deltaTime);

            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input.Normalize();

            if (input.sqrMagnitude > 0 || velocity.sqrMagnitude >= minSpeed)
            {
                velocity += input.y * Camera.main.transform.forward * (Input.GetKeyDown(KeyCode.LeftShift) ? acceleration * maxSpeedBoostMultiplier : acceleration);
                velocity += input.x * Camera.main.transform.right * (Input.GetKeyDown(KeyCode.LeftShift) ? acceleration * maxSpeedBoostMultiplier : acceleration);

                Truncate(ref velocity, Input.GetKeyDown(KeyCode.LeftShift) ? maxSpeed * maxSpeedBoostMultiplier : maxSpeed);

                targetRotation = Quaternion.LookRotation(-Camera.main.transform.forward, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed.x * Time.deltaTime);

                velocity /= drag;

                if (controller)
                {
                    controller.Move(velocity * Time.deltaTime);
                }
                else
                {
                    transform.position += velocity * Time.deltaTime;
                }

                if (pVelocity.magnitude <= minSpeed)
                    OnStartFlying?.Invoke();

                OnFlying?.Invoke();
            }
            else
            {
                if (pVelocity.magnitude > minSpeed)
                    OnStopFlying?.Invoke();

                velocity = Vector3.zero;
            }

            animator.SetFloat("Speed", velocity.sqrMagnitude);

            pVelocity = velocity;
        }

        /// <summary>
        ///  Check if a vector's length is longer as x and if so set it's length to x
        /// </summary>
        /// <param name="vec">The vector which it's magnitude should be checked</param>
        /// <param name="maxLength">Max length of the vector</param>
        void Truncate(ref Vector3 vec, float maxLength)
        {
            if (vec.sqrMagnitude > maxLength * maxLength)
            {
                vec = vec.normalized * maxLength;
            }
        }
    }

    public enum PlayerState
    {
        Flying,
        Grounded
    }
}