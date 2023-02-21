using System.Collections;
using UnityEngine;

namespace ExamineSystem
{
    public class ExaminableItem : MonoBehaviour
    {
        #region Parent / Child Fields
        [Tooltip("Select this option if the object that has this script attached has no mesh renderer and it's an empty parent which holds children")]
        [SerializeField] private bool isEmptyParent = false;

        [Tooltip("Select this option if the object you're examining has multiple children - Add the child objects to the array")]
        [SerializeField] private bool _hasChildren = false;
        [SerializeField] private GameObject[] childObjects = null;
        #endregion

        #region Offset Fields
        [SerializeField] private Vector3 initialRotationOffset = new Vector3(0, 0, 0);
        [Tooltip("Horizontal and Vertical offsets control how far vertically and horizentally the object is positioned when examined. Keep this a low value (-0.X) is left and (+0.X) is right")]
        [Range(-1, 1)][SerializeField] private float horizontalOffset = 0;
        [Range(-1, 1)][SerializeField] private float verticalOffset = 0;
        #endregion

        #region Zoom Fields
        [SerializeField] private float initialZoom = 1f;
        [SerializeField] private Vector2 zoomRange = new Vector2(0.5f, 2f);
        [SerializeField] private float zoomSensitivity = 0.1f;
        #endregion

        #region Rotation Fields
        [SerializeField] private float rotationSpeed = 5.0f;
        [SerializeField] private bool invertRotation = false;
        #endregion

        #region Highlight Fields
        [SerializeField] private bool showEmissionHighlight = false;
        [SerializeField] private bool showNameHighlight = false;
        #endregion

        #region Inspect Point Fields
        [SerializeField] private bool _hasInspectPoints = false;
        [SerializeField] private GameObject[] inspectPoints = null;
        private LayerMask inspectPointLayer;
        private float viewDistance = 25;
        #endregion

        #region Sound Fields
        [SerializeField] private Sound pickupSound = null;
        [SerializeField] private Sound dropSound = null;
        #endregion

        #region Text Customisation Fields
        [SerializeField] private UIType _UIType = UIType.None;
        [SerializeField] private enum UIType { None, BasicLowerUI, RightSideUI }

        [SerializeField] private string itemName = null;

        [SerializeField] private int textSize = 32;
        [SerializeField] private Font fontType = null;
        [SerializeField] private FontStyle fontStyle = FontStyle.Normal;
        [SerializeField] private Color fontColor = Color.white;

        [Space(5)] [TextArea] [SerializeField] private string itemDescription = null;

        [SerializeField] private int textSizeDesc = 30;
        [SerializeField] private Font fontTypeDesc = null;
        [SerializeField] private FontStyle fontStyleDesc = FontStyle.Normal;
        [SerializeField] private Color fontColorDesc = Color.white;
        #endregion

        #region Initialisation Fields
        private Material objectMaterial;
        Vector3 originalPosition;
        Quaternion originalRotation;
        private Vector3 startPos;
        private bool canRotate;
        private float currentZoom = 1;
        private Camera mainCamera;
        private Transform examinePoint = null;
        private ExamineInteractor raycastManager;
        private ExamineUIManager examineUIManager;
        #endregion

        #region String Reference Fields
        private const string emissive = "_EMISSION";
        private const string mouseX = "Mouse X";
        private const string mouseY = "Mouse Y";
        private const string examineLayer = "ExamineLayer";
        private const string defaultLayer = "Default";
        private const string inspectLayer = "InspectPointLayer";
        #endregion

        #region Public Properties
        public bool hasChildren
        {
            get { return _hasChildren; }
            set { _hasChildren = value; }
        }

        public bool hasInspectPoints
        {
            get { return _hasInspectPoints; }
            set { _hasInspectPoints = value; }
        }
        #endregion

        void Start()
        {
            inspectPointLayer = 1 << LayerMask.NameToLayer(inspectLayer); //This finds the mask we want and adds it to the variable "inspectPointLayer"

            initialZoom = Mathf.Clamp(initialZoom, zoomRange.x, zoomRange.y);
            originalPosition = transform.position; originalRotation = transform.rotation;
            startPos = gameObject.transform.localEulerAngles;

            DisableEmissionOnChildrenHighlight(true);

            if (!isEmptyParent)
            {
                objectMaterial = GetComponent<Renderer>().material;
                DisableMatEmissive(true);
            }

            mainCamera = Camera.main;
            raycastManager = mainCamera.GetComponent<ExamineInteractor>();
            examinePoint = GameObject.FindWithTag("ExaminePoint").GetComponent<Transform>();
            CheckSoundDebug();
        }

        void SetExamineLayer(string currentLayer)
        {
            gameObject.layer = LayerMask.NameToLayer(currentLayer);
        }

        void PlayPickupSound()
        {
            if (pickupSound != null)
            {
                ExamineAudioManager.instance.Play(pickupSound);
            }
        }

        void PlayDropSound()
        {
            if (dropSound != null)
            {
                ExamineAudioManager.instance.Play(dropSound);
            }
        }

        void Update()
        {
            if (canRotate)
            {
                ExamineInput();
                ExamineZooming();
            }
        }

        void ExamineInput()
        {
            float h = invertRotation ? rotationSpeed * Input.GetAxis(mouseX) : -rotationSpeed * Input.GetAxis(mouseX);
            float v = invertRotation ? rotationSpeed * Input.GetAxis(mouseY) : -rotationSpeed * Input.GetAxis(mouseY);

            if (Input.GetKey(ExamineInputManager.instance.rotateKey))
            {
                gameObject.transform.Rotate(v, h, 0);
            }
            else if (Input.GetKeyDown(ExamineInputManager.instance.dropKey))
            {
                DropObject();
            }

            if (hasInspectPoints)
            {
                FindInspectPoints();
            }
        }

        void ExamineZooming()
        {
            bool zoomAdjusted = false;
            float scrollDelta = Input.mouseScrollDelta.y;
            if (scrollDelta > 0)
            {
                currentZoom += zoomSensitivity;
                zoomAdjusted = true;
            }
            else if (scrollDelta < 0)
            {
                currentZoom -= zoomSensitivity;
                zoomAdjusted = true;
            }

            if (zoomAdjusted)
            {
                currentZoom = Mathf.Clamp(currentZoom, zoomRange.x, zoomRange.y);
                ItemZoom(currentZoom);
            }
        }

        public void ExamineObject()
        {
            ExamineUIManager.instance._examinableItem = this;
            examineUIManager = ExamineUIManager.instance;
            EnableInspectPoints();
            ExamineDisableManager.instance.DisablePlayer(true);
            examineUIManager.SetHighlightName(null, false);
            SetExamineLayer(examineLayer);
            PlayPickupSound();

            currentZoom = initialZoom; ItemZoom(initialZoom);
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
            transform.Rotate(initialRotationOffset);

            DisableEmissionOnChildrenLayer(examineLayer);
            DisableMatEmissive(true);
            canRotate = true;

            switch (_UIType)
            {
                case UIType.None:
                    examineUIManager.ShowCloseButton(true);
                    break;
                case UIType.BasicLowerUI:
                    examineUIManager.SetBasicUIText(itemName, itemDescription, true);
                    TextCustomisation();
                    break;
                case UIType.RightSideUI:
                    examineUIManager.SetRightSideUIText(itemName, itemDescription, true);
                    TextCustomisation();
                    break;
            }
        }

        public void DropObject()
        {
            ExamineDisableManager.instance.DisablePlayer(false);

            transform.position = originalPosition;
            transform.rotation = originalRotation;
            SetExamineLayer(defaultLayer);
            PlayDropSound();

            DisableEmissionOnChildrenLayer(defaultLayer);
            DisableInspectPoints();
            canRotate = false;

            switch (_UIType)
            {
                case UIType.None:
                    examineUIManager.ShowCloseButton(false);
                    break;
                case UIType.BasicLowerUI:
                    examineUIManager.SetBasicUIText(null, null, false);
                    break;
                case UIType.RightSideUI:
                    examineUIManager.SetRightSideUIText(null, null, false);
                    break;
            }
        }

        //value = The distance from the camera to position the object
        //MoveSelf = Whether to move the actual object. If set to false the object may not move, but only the represented point
        private void ItemZoom(float value, bool moveSelf = true)
        {
            examinePoint.transform.localPosition = new Vector3(horizontalOffset, verticalOffset, value);

            if (moveSelf)
            {
                transform.position = examinePoint.transform.position;
            }
        }

        public void ItemHighlight(bool isHighlighted)
        {
            if (showNameHighlight)
            {
                if (isHighlighted)
                {
                    ExamineUIManager.instance.SetHighlightName(itemName, isHighlighted);
                }
                else
                {
                    ExamineUIManager.instance.SetHighlightName(null, false);
                }
            }

            if (showEmissionHighlight)
            {
                if (isHighlighted)
                {
                    DisableMatEmissive(false);
                    DisableEmissionOnChildrenHighlight(false);
                }
                else
                {
                    DisableMatEmissive(true);
                    DisableEmissionOnChildrenHighlight(true);
                }
            }
        }

        private void TextCustomisation()
        {
            switch (_UIType)
            {
                case UIType.BasicLowerUI:
                    examineUIManager.SetBasicUITextSettings(textSize, fontType, fontStyle, fontColor, textSizeDesc, fontTypeDesc, fontStyleDesc, fontColorDesc);
                    break;
                case UIType.RightSideUI:
                    examineUIManager.SetRightUITextSettings(textSize, fontType, fontStyle, fontColor, textSizeDesc, fontTypeDesc, fontStyleDesc, fontColorDesc);
                    break;
            }         
        }

        void FindInspectPoints()
        {
            RaycastHit hit;
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out hit, viewDistance, inspectPointLayer))
            {
                if (hit.transform.CompareTag("InspectPoint"))
                {
                    InspectPointUI(hit.transform.gameObject, mainCamera, true); //Enable inspect point UI
                    if (Input.GetKeyDown(ExamineInputManager.instance.interactKey))
                    {
                        hit.transform.gameObject.GetComponent<ExamineInspectPoint>().InspectPointInteract();
                    }
                }
                else
                {
                    InspectPointUI(null, null, false); //Disable inspect point UI
                }
            }
            else
            {
                InspectPointUI(null, null, false); //Disable inspect point UI
            }
        }

        void InspectPointUI(GameObject item, Camera camera, bool detected) // Enable/disable inspect point UI
        { 
            if (detected)
            {
                Vector3 setPosition = camera.WorldToScreenPoint(item.transform.position);
                examineUIManager.SetInspectPointParent(true, setPosition);

                string inspectText = item.GetComponent<ExamineInspectPoint>().InspectInformation();
                examineUIManager.SetInspectPointText(inspectText);
            }
            else
            {
                examineUIManager.SetInspectPointParent(false, Vector3.zero); //Disable inspect UI - Doesn't need Vector3 ?
            }
        }

        void EnableInspectPoints()
        {
            StartCoroutine(WaitBeforeEnable(0.05f));
        }

        IEnumerator WaitBeforeEnable(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            if (inspectPoints.Length >= 1)
            {
                hasInspectPoints = true;

                foreach (GameObject pointToEnable in inspectPoints)
                {
                    pointToEnable.SetActive(true);
                }
            }
        }

        void DisableInspectPoints()
        {
            if(hasInspectPoints)
            {
                foreach (GameObject pointToEnable in inspectPoints)
                {
                    pointToEnable.SetActive(false);
                }
            }
        }

        void DisableMatEmissive(bool disable)
        {
            if (!isEmptyParent && disable)
            {
                objectMaterial.DisableKeyword(emissive);
            }
            else if (!isEmptyParent && !disable)
            {
                objectMaterial.EnableKeyword(emissive);
            }
        }

        void DisableEmissionOnChildrenHighlight(bool enable)
        {
            if (hasChildren)
            {
                foreach (GameObject gameobjectToLayer in childObjects)
                {
                    Material thisMat = gameobjectToLayer.GetComponent<Renderer>().material;
                    if(!enable)
                    {
                        thisMat.EnableKeyword(emissive);
                    }
                    else
                    {
                        thisMat.DisableKeyword(emissive);
                    }
                }
            }
        }

        void DisableEmissionOnChildrenLayer(string layerToSet)
        {
            if (hasChildren)
            {
                foreach (GameObject gameobjectToLayer in childObjects)
                {
                    gameobjectToLayer.layer = LayerMask.NameToLayer(layerToSet);
                    Material thisMat = gameobjectToLayer.GetComponent<Renderer>().material;
                    thisMat.DisableKeyword(emissive);
                }
            }
        }

        private void OnDestroy()
        {
            Destroy(objectMaterial);
        }

        private void CheckSoundDebug()
        {
            if (pickupSound == null)
            {
                print("Did you forget to add a sound Scriptable to item" + " " + gameObject);
            }

            else if (dropSound == null)
            {
                print("Did you forget to add a sound Scriptable to item" + " " + gameObject);
            }
        }
    }
}