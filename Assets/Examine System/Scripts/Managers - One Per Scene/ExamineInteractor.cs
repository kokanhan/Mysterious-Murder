using UnityEngine;

namespace ExamineSystem
{
    [RequireComponent(typeof(Camera))]
    public class ExamineInteractor : MonoBehaviour
    {
        [Header("Raycast Features")]
        [SerializeField] private float rayLength = 5;
        private ExaminableItem examinableItem;
        private Camera _camera;

        void Start()
        {
            _camera = GetComponent<Camera>();
        }

        void Update()
        {
            if (Physics.Raycast(_camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f)), transform.forward, out RaycastHit hit, rayLength))
            {
                var examineItem = hit.collider.GetComponent<ExaminableItem>();
                if (examineItem != null)
                {
                    examinableItem = examineItem;
                    examinableItem.ItemHighlight(true);
                    HighlightCrosshair(true);
                }
                else
                {
                    ClearExaminable();
                }
            }
            else
            {
                ClearExaminable();
            }

            if (examinableItem != null)
            {
                if (Input.GetKeyDown(ExamineInputManager.instance.interactKey))
                {
                    examinableItem.ExamineObject();
                }
            }
        }

        private void ClearExaminable()
        {
            if (examinableItem != null)
            {
                examinableItem.ItemHighlight(false);
                HighlightCrosshair(false);
                examinableItem = null;
            }
        }

        void HighlightCrosshair(bool on)
        {
            ExamineUIManager.instance.HighlightCrosshair(on);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(gameObject.transform.position, rayLength);
        }
    }
}
