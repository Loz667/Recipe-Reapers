using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using RPG.Core;
using RPG.Movement;
using RPG.Combat;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        [HideInInspector] public Animator anim;

        private PartyManager partyManager;

        private const string BATTLE_SCENE = "BattleScene";

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }        

        private void Start()
        {
            partyManager = FindFirstObjectByType<PartyManager>();
            //If there is a saved position, set player's position to this value
            if (partyManager.GetPosition() != Vector3.zero)
            {
                transform.position = partyManager.GetPosition();
            }
        }

        private void Update()
        {
            Step();
            if (CombatInteract()) return;
            if (MovementInteract()) return;
        }

        private bool MovementInteract()
        {
            RaycastHit hit;
            bool HasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (HasHit)
            {
                if (Input.GetMouseButton(0))
                {
                    GetComponent<Mover>().StartMoveAction(hit.point);
                }
                return true;
            }
            return false;
        }

        private bool CombatInteract()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;

                if (Input.GetMouseButtonDown(0))
                {
                    GetComponent<Combatant>().Attack(target);
                }
                return true;
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        private void Step()
        {
            GetComponent<Mover>().Move();
        }

        public void SetOverWorldVisuals(Animator animator)
        {
            anim = animator;
        }
    }
}