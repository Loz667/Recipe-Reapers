using RPG.Combat;
using RPG.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour
    {
        [SerializeField] private float speed = 6f;

        private PlayerControls playerControls;
        private NavMeshAgent agent;
        
        private Vector3 movement;
        private float inputSqrMagnitude;

        private void Awake()
        {
            playerControls = new PlayerControls();
            agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void Update()
        {
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 _destination)
        {
            GetComponent<Combatant>().Cancel();
            MoveTo(_destination);
        }

        public void MoveTo(Vector3 _destination)
        {
            agent.destination = _destination;
            agent.isStopped = false;
        }

        public void Stop()
        {
            agent.isStopped = true;
        }

        public void Move()
        {
            float x = playerControls.Player.Move.ReadValue<Vector2>().x;
            float z = playerControls.Player.Move.ReadValue<Vector2>().y;

            movement = new Vector3(x, 0, z);

            inputSqrMagnitude = movement.sqrMagnitude;
            if (inputSqrMagnitude >= .01f)
            {
                Vector3 newPos = transform.position + movement * Time.deltaTime * speed;
                NavMeshHit hit;
                bool IsValid = NavMesh.SamplePosition(newPos, out hit, .3f, NavMesh.AllAreas);
                if (IsValid)
                {
                    if ((transform.position - hit.position).magnitude >= .02f)
                    {
                        transform.position = hit.position;
                        transform.rotation = Quaternion.LookRotation(movement);
                        agent.destination = transform.position;
                    }
                }
            }
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = agent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<PlayerController>().anim.SetFloat("forwardSpeed", speed);
        }
    }
}
