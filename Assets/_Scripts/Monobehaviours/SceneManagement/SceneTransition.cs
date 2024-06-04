using UnityEngine;
using UnityEngine.SceneManagement;

namespace Reapers.SceneManagement
{
    public class SceneTransition : MonoBehaviour
    {
        static public SceneTransition Instance;

        [SerializeField] Transform topObj;
        [SerializeField] Transform bottomObj;

        Vector2 endTop, endBottom;

        [SerializeField] float currentTime = -0.5f;
        float endTime = 1f;

        [SerializeField] bool loadScene;
        [SerializeField] int sceneToLoad;

        private void Awake()
        {
            topObj.gameObject.SetActive(true);
            bottomObj.gameObject.SetActive(true);

            Instance = this;

            float offset = Screen.height * 0.75f;
            endTop = new Vector2(0, offset);
            endBottom = new Vector2(0, -offset);
        }

        private void FixedUpdate()
        {
            currentTime += Time.deltaTime;

            if (currentTime < 0) { return; }
            else if (currentTime > endTime)
            {
                if (loadScene)
                {
                    SceneManager.LoadScene(sceneToLoad);
                }
                return;
            }

            float t = Mathf.Pow(currentTime / endTime, 4f);

            if (loadScene)
            {
                topObj.localPosition = Vector2.Lerp(endTop, Vector2.zero, t);
                bottomObj.localPosition = Vector2.Lerp(endBottom, Vector2.zero, t);
            }
            else
            {
                topObj.localPosition = Vector2.Lerp(Vector2.zero, endTop, t);
                bottomObj.localPosition = Vector2.Lerp(Vector2.zero, endBottom, t);
            }
        }

        static public void TransitionToScene(int sceneToLoad)
        {
            Instance.currentTime = 0;
            Instance.sceneToLoad = sceneToLoad;
            Instance.loadScene = true;
        }
    }

}