using RPG.Control;
using UnityEngine;

namespace RPG.Movement
{
    public class FollowerAI : MonoBehaviour
    {
        [SerializeField] private Transform followTarget;
        [SerializeField] private int speed;

        private float followDist;
        private Animator anim;

        private const string IS_WALKING_PARAM = "IsWalking";

        private void Start()
        {
            anim = gameObject.GetComponent<Animator>();

            followTarget = FindFirstObjectByType<PlayerController>().transform;
        }

        private void FixedUpdate()
        {
            if (Vector3.Distance(transform.position, followTarget.position) > followDist)
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

        public void SetFollowDistance(float followDistance)
        {
            followDist = followDistance;
        }
    }
}
