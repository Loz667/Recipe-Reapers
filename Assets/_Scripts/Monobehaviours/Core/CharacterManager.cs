using System.Collections.Generic;
using UnityEngine;
using TMPro;
using RPG.Control;
using RPG.Movement;

namespace RPG.Core
{
    public class CharacterManager : MonoBehaviour
    {
        [SerializeField] private GameObject joinPopup;
        [SerializeField] private TextMeshProUGUI joinPopupText;

        private PlayerControls playerControls;
        private List<GameObject> overworldCharacters = new List<GameObject>();

        private bool inFrontOfJoinableMember;
        private GameObject joinableMember;

        private const string NPC_JOINABLE_TAG = "NPC_Joinable";
        private const string NPC_JOINED_MSG = " joined the party!";

        private void Awake()
        {
            playerControls = new PlayerControls();
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
            playerControls.Player.Interact.performed += _ => Interact();
            SpawnOverworldMembers();
        }

        private void Interact()
        {
            if (inFrontOfJoinableMember == true && joinableMember != null)
            {
                MemberJoin(joinableMember.GetComponent<JoinableCharacter>().memberToJoin);
                inFrontOfJoinableMember = false;
                joinableMember = null;
            }
        }

        private void MemberJoin(PartyMemberInfo partyMember)
        {
            //add new party member
            GameObject.FindFirstObjectByType<PartyManager>().AddMemberToPartyByName(partyMember.MemberName);
            //disable the joinable character
            joinableMember.GetComponent<JoinableCharacter>().CheckIfJoined();
            //member joined pop up
            joinPopupText.text = partyMember.MemberName + NPC_JOINED_MSG;
            joinPopup.SetActive(true);
            //add an overworld member
            SpawnOverworldMembers();
        }

        private void SpawnOverworldMembers()
        {
            for (int i = 0; i < overworldCharacters.Count; i++)
            {
                Destroy(overworldCharacters[i]);
            }
            overworldCharacters.Clear();

            List<PartyMember> currentParty = GameObject.FindFirstObjectByType<PartyManager>().GetCurrentParty();

            //First member will be player
            for (int i = 0; i < currentParty.Count; i++)
            {
                if (i == 0)
                {
                    //Get the player
                    GameObject player = gameObject;
                    //Spawn the member visual
                    GameObject playerVisual = Instantiate(currentParty[i].MemberOverworldVisualPrefab,
                        player.transform.position, Quaternion.identity);
                    playerVisual.transform.SetParent(player.transform);

                    //Assign player controller
                    player.GetComponent<PlayerController>().SetOverWorldVisuals(playerVisual.GetComponent<Animator>());
                    playerVisual.GetComponent<FollowerAI>().enabled = false;

                    //Add to list
                    overworldCharacters.Add(playerVisual);
                }
                else // add member as a follower
                {
                    Vector3 pos = transform.position;
                    Vector3 positionToSpawn = new Vector3(pos.x -= i, 0, pos.z -= i);

                    GameObject tempFollower = Instantiate(currentParty[i].MemberOverworldVisualPrefab, positionToSpawn, Quaternion.identity);

                    //Set AI follow settings
                    tempFollower.GetComponent<FollowerAI>().SetFollowDistance(i);

                    overworldCharacters.Add(tempFollower);
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(NPC_JOINABLE_TAG))
            {
                inFrontOfJoinableMember = true;
                joinableMember = other.gameObject;
                joinableMember.GetComponent<JoinableCharacter>().ShowInteractPrompt(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(NPC_JOINABLE_TAG))
            {
                inFrontOfJoinableMember = false;
                joinableMember.GetComponent<JoinableCharacter>().ShowInteractPrompt(false);
                joinableMember = null;
            }
        }
    }
}