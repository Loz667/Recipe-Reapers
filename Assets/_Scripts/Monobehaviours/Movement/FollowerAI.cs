using Reapers.Control;
using UnityEngine;

namespace Reapers.Movement
{
    public class FollowerAI : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private int speed;

        private Animator anim;
        private float followDistance;

        private const string IS_WALKING_PARAM = "IsWalking";

        void Start()
        {
            anim = gameObject.GetComponent<Animator>();

            followTarget = FindFirstObjectByType<PlayerController>().transform;
        }

        void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, followTarget.position) > followDistance)
            {
                anim.SetBool(IS_WALKING_PARAM, true);
                float step = speed * Time.deltaTime;
                transform.position = Vector3.MoveTowards(transform.position, followTarget.position, step);
                transform.rotation = followTarget.rotation;
            }
            else
            {
                anim.SetBool(IS_WALKING_PARAM, false);
            }
        }

        public void SetFollowDistance(float distance)
        {
            followDistance = distance;
        }
    }

}