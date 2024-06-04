using UnityEngine;
using Reapers.Core;
using Reapers.SceneManagement;

namespace Reapers.Control
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private int speed;
        [SerializeField] private Animator anim;
        [SerializeField] private LayerMask grassLayer;
        [SerializeField] private int stepsTakenInGrass;
        [SerializeField] private int minStepsToEncounter;
        [SerializeField] private int maxStepsToEncounter;

        private PlayerControls playerControls;
        private Rigidbody rb;
        private PartyManager partyManager;

        private Vector3 movement;
        private bool movingInGrass;
        private float stepTimer;
        private int stepsToEncounter;

        private const string IS_WALKING_PARAM = "IsWalking";
        private const int BATTLE_SCENE_INDEX = 1;
        private const float TIME_PER_STEP = 0.5f;

        private void Awake()
        {
            playerControls = new PlayerControls();
            CalcStepsToNextEncounter();
        }

        private void OnEnable()
        {
            playerControls.Enable();
        }

        private void OnDisable()
        {
            playerControls.Disable();
        }

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
            partyManager = FindFirstObjectByType<PartyManager>();
            //If there is a saved position, set player's position to this value
            if (partyManager.GetPosition() != Vector3.zero)
            {
                transform.position = partyManager.GetPosition();
            }
        }

        private void Update()
        {
            float x = playerControls.Player.Move.ReadValue<Vector2>().x;
            float z = playerControls.Player.Move.ReadValue<Vector2>().y;

            movement = new Vector3(x, 0, z).normalized;

            if (movement != Vector3.zero) { transform.rotation = Quaternion.LookRotation(movement); }

            anim.SetBool(IS_WALKING_PARAM, movement != Vector3.zero);
        }

        private void FixedUpdate()
        {
            rb.MovePosition(transform.position + movement * speed * Time.deltaTime);

            Collider[] colliders = Physics.OverlapSphere(transform.position, 1, grassLayer);
            movingInGrass = colliders.Length != 0 && movement != Vector3.zero;

            if (movingInGrass)
            {
                stepTimer += Time.deltaTime;
                if (stepTimer > TIME_PER_STEP)
                {
                    stepsTakenInGrass++;
                    stepTimer = 0;

                    //check to see if encounter has been reached
                    if (stepsTakenInGrass >= stepsToEncounter)
                    {
                        //Save current position
                        partyManager.SetPosition(transform.position);
                        //change the scene
                        SceneTransition.TransitionToScene(BATTLE_SCENE_INDEX);
                        enabled = false;
                    }
                }
            }
        }

        private void CalcStepsToNextEncounter()
        {
            stepsToEncounter = Random.Range(minStepsToEncounter, maxStepsToEncounter);
        }

        public void SetOverworldVisuals(Animator animator)
        {
            anim = animator;
        }
    }

}