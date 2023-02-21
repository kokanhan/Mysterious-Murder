using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityStandardAssets.ImageEffects;

namespace ExamineSystem
{
    public class ExamineDisableManager : MonoBehaviour
    {
        [SerializeField] private ExamineInteractor interactorScript = null;
        [SerializeField] private FirstPersonController player = null;
        [SerializeField] private BlurOptimized blur = null;

        public static ExamineDisableManager instance;

        void Awake()
        {
            if (instance != null) { Destroy(gameObject); }
            else { instance = this; DontDestroyOnLoad(gameObject); }
        }

        public void DisablePlayer(bool disable)
        {
            if (disable)
            {
                player.enabled = false;
                interactorScript.enabled = false;

                blur.enabled = true;
                ExamineUIManager.instance.EnableCrosshair(false);

            }
            else
            {
                player.enabled = true;
                interactorScript.enabled = true;

                blur.enabled = false;
                ExamineUIManager.instance.EnableCrosshair(true);

            }
        }
    }
}
