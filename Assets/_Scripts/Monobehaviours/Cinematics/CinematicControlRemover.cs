using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicControlRemover : MonoBehaviour
{
    private GameObject player;

    private void Start()
    {
        GetComponent<PlayableDirector>().played += DisableControl;
        GetComponent<PlayableDirector>().stopped += EnableControl;

        player = GameObject.FindWithTag("Player");
    }

    void DisableControl(PlayableDirector director)
    {
        player.GetComponent<PlayerController>().enabled = false;
    }

    void EnableControl(PlayableDirector director)
    {
        player.GetComponent<PlayerController>().enabled = true;
    }
}
