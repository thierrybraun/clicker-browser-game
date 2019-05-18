using UnityEngine;

namespace UI
{
    [ExecuteInEditMode]
    public abstract class FlexibleUI : MonoBehaviour
    {
        public FlexibleUIData skinData;

        protected abstract void OnSkinUI();

        public virtual void Awake()
        {
            OnSkinUI();
        }

        public virtual void Update()
        {
            if (Application.isEditor)
            {
                OnSkinUI();
            }
        }
    }
}