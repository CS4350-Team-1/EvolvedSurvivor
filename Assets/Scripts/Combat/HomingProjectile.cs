using UnityEngine;

namespace TeamOne.EvolvedSurvivor
{
    public class HomingProjectile : Projectile
    {
        private GameObject target;
        private bool isTracking = true;
        private float angularVelocity = 180f;

        [SerializeField] private LoopingSfxHandler sfxHandler;

        public void SetTarget(GameObject target)
        {
            this.target = target;
            isTracking = true;
        }

        protected override void Update()
        {
            base.Update();

            if (!isTracking)
            {
                return;
            }

            Vector3 direction = (target.transform.position - transform.position).normalized;
            if (Vector3.Dot(direction, motion) >= -0.1f)
            {
                float rotateAmount = Vector3.Cross(direction, motion).z;
                this.transform.Rotate(0f, 0f, -rotateAmount * angularVelocity * Time.deltaTime);
                this.motion = transform.up;
            } 
            else
            {
                isTracking = false;
                sfxHandler.StopSfx();
            }
        }

        void OnEnable()
        {
            sfxHandler.PlaySfx();
        }

        void OnDisable()
        {
            sfxHandler.StopSfx();
        }
    }
}
