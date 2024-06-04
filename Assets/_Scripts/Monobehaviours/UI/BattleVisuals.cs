using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Reapers.UI
{
    public class BattleVisuals : MonoBehaviour
    {
        [SerializeField] private Slider healthBar;
        [SerializeField] private TextMeshProUGUI levelText;

        private int currHealth;
        private int maxHealth;
        private int level;

        private Animator anim;

        private const string LEVEL_ABB = "LVL: ";

        private const string IS_ATTACKING_PARAM = "IsAttacking";
        private const string IS_HIT_PARAM = "IsHit";
        private const string IS_DEAD_PARAM = "IsDead";

        void Awake()
        {
            anim = gameObject.GetComponent<Animator>();
        }

        public void SetStartingValues(int _currHealth, int _maxHealth, int _level)
        {
            this.currHealth = _currHealth;
            this.maxHealth = _maxHealth;
            this.level = _level;
            levelText.text = LEVEL_ABB + level.ToString();
            UpdateHealthBar();
        }

        public void ChangeHealthValue(int _currHealth)
        {
            this.currHealth = _currHealth;
            if (currHealth <= 0)
            {
                PlayDeathAnim();
                Destroy(gameObject, 2f);
            }
            UpdateHealthBar();
        }

        public void UpdateHealthBar()
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currHealth;
        }

        public void PlayAttackAnim()
        {
            anim.SetTrigger(IS_ATTACKING_PARAM);
        }

        public void PlayHitAnim()
        {
            anim.SetTrigger(IS_HIT_PARAM);
        }

        public void PlayDeathAnim()
        {
            anim.SetTrigger(IS_DEAD_PARAM);
        }
    }

}