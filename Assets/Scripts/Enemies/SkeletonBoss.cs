using System.Collections;
using UnityEngine;

namespace SimplePlatformer.Enemy
{
    public class SkeletonBoss : GroundedEnemy
    {
        public GameObject healthBarGO;

        public enum BossState
        {
            NONE,
            START,
            FIRST,
            SECOND
        }

        private BossState currentBossState;

        protected override void Start()
        {
            base.Start();
            //Starting boss phase
            SetPhase(BossState.NONE);
        }

        private void SetPhase(BossState s)
        {
            currentBossState = s;
        }

        private void DisplayHealthBar()
        {
            LeanTween.alphaCanvas(healthBarGO.GetComponent<CanvasGroup>(), 1.0f, 1f);
        }

        protected override void Update()
        {
            base.Update();
            if (currentBossState.Equals(BossState.NONE) && sawPlayer)
            {
                StartBossBattle();
            }
        }

        protected void StartBossBattle()
        {
            currentBossState = BossState.START;
            DisplayHealthBar();
        }

    }
}