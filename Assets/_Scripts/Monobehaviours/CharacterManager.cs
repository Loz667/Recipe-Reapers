using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    [SerializeField] private GameObject joinPopup;
    [SerializeField] private TextMeshProUGUI joinPopupMsg;

    private PlayerControls playerControls;
    private bool inFrontofJoinableCharacter;
    private GameObject joinableCharacter;
    private List<GameObject> overworldCharacters = new List<GameObject>();

    private const string PARTY_JOIN_MSG = " joined the party!";
    private const string JOINABLE_TAG = "NPC_Joinable";

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

    void Start()
    {
        playerControls.Player.Interact.performed += _ => Interact();
        SpawnOverworldCharacters();
    }

    void Update()
    {

    }

    private void Interact()
    {
        if (inFrontofJoinableCharacter && joinableCharacter != null)
        {
            CharacterJoin(joinableCharacter.GetComponent<JoinableCharacter>().memberToJoin);
            inFrontofJoinableCharacter = false;
            joinableCharacter = null;
        }
    }

    private void CharacterJoin(PartyMemberInfo partyMember)
    {
        FindObjectOfType<PartyManager>().AddMemberToPartyByName(partyMember.MemberName);
        joinableCharacter.GetComponent<JoinableCharacter>().CheckIfJoined();
        joinPopup.SetActive(true);
        joinPopupMsg.text = partyMember.MemberName + PARTY_JOIN_MSG;
        SpawnOverworldCharacters();
    }

    private void SpawnOverworldCharacters()
    {
        for (int i = 0; i < overworldCharacters.Count; i++)
        {
            Destroy(overworldCharacters[i]);
        }
        overworldCharacters.Clear();

        List<PartyMember> currentParty = FindFirstObjectByType<PartyManager>().GetCurrentParty();

        for (int i = 0; i < currentParty.Count; i++)
        {
            if (i == 0)
            {
                GameObject player = gameObject;
                GameObject playerVisual = Instantiate(currentParty[i].MemberOverworldVisualPrefab,
                    player.transform.position, Quaternion.identity);
                playerVisual.transform.SetParent(player.transform);

                player.GetComponent<PlayerController>().SetOverworldVisuals(playerVisual.GetComponent<Animator>());
                playerVisual.GetComponent<FollowerAI>().enabled = false;

                overworldCharacters.Add(playerVisual);
            }
            else
            {
                Vector3 positionToSpawn = transform.position;
                positionToSpawn.x -= 1;
                positionToSpawn.z -= 1;

                GameObject tempFollower = Instantiate(currentParty[i].MemberOverworldVisualPrefab,
                            positionToSpawn, Quaternion.identity);
                tempFollower.GetComponent<FollowerAI>().SetFollowDistance(i);

                overworldCharacters.Add(tempFollower);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(JOINABLE_TAG))
        {
            inFrontofJoinableCharacter = true;
            joinableCharacter = other.gameObject;
            joinableCharacter.GetComponent<JoinableCharacter>().ShowInteractPrompt(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(JOINABLE_TAG))
        {
            inFrontofJoinableCharacter = false;
            joinableCharacter.GetComponent<JoinableCharacter>().ShowInteractPrompt(false);
            joinableCharacter = null;
        }
    }
}
