using UnityEngine;
using UnityEngine.UI;

namespace ExamineSystem
{
    public class ExamineUIManager : MonoBehaviour
    {
        [Header("No UI Close Button")]
        [SerializeField] private GameObject noUICloseButton = null;

        [Header("Basic Example UI References")]
        [SerializeField] private Text basicItemNameUI = null;
        [SerializeField] private Text basicItemDescUI = null;
        [SerializeField] private GameObject basicExamineUI = null;

        [Header("Right Side Example UI References")]
        [SerializeField] private Text rightItemNameUI = null;
        [SerializeField] private Text rightItemDescUI = null;
        [SerializeField] private GameObject rightExamineUI = null;

        [Header("Interaction Name UI")]
        [SerializeField] private Text interactionItemNameUI = null;
        [SerializeField] private GameObject interactionNameMainUI = null;

        [Header("Interest Point UI's")]
        [SerializeField] private Text interestPointText = null;
        [SerializeField] private GameObject interestPointParentUI = null;

        [Header("Crosshair")]
        [SerializeField] private Image uiCrosshair = null;

        [HideInInspector] public ExaminableItem _examinableItem;

        [Header("Help Panel Visibility")]
        [SerializeField] private bool showHelp = false;
        [SerializeField] private GameObject examineHelpUI = null;

        public static ExamineUIManager instance;

        private void Awake()
        {
            if (instance == null) { instance = this; }
            ShowHelpPrompt();
        }

        public void SetInspectPointParent(bool on, Vector3 position)
        {
            interestPointParentUI.SetActive(on);
            interestPointParentUI.transform.position = position;
        }

        public void SetInspectPointText(string inspectText)
        {
            interestPointText.text = inspectText;
        }

        public void SetBasicUIText(string itemName, string itemDescription, bool on)
        {
            basicItemNameUI.text = itemName;
            basicItemDescUI.text = itemDescription;
            basicExamineUI.SetActive(on);
        }

        public void SetBasicUITextSettings(int textSize, Font fontType, FontStyle fontStyle, Color fontColor, int textSizeDesc, Font fontTypeDesc, FontStyle fontStyleDesc, Color fontColorDesc)
        {
            basicItemNameUI.fontSize = textSize;
            basicItemNameUI.font = fontType;
            basicItemNameUI.fontStyle = fontStyle;
            basicItemNameUI.color = fontColor;

            basicItemDescUI.fontSize = textSizeDesc;
            basicItemDescUI.font = fontTypeDesc;
            basicItemDescUI.fontStyle = fontStyleDesc;
            basicItemDescUI.color = fontColorDesc;
        }

        public void SetRightSideUIText(string itemName, string itemDescription, bool on)
        {
            rightItemNameUI.text = itemName;
            rightItemDescUI.text = itemDescription;
            rightExamineUI.SetActive(on);
        }

        public void SetRightUITextSettings(int textSize, Font fontType, FontStyle fontStyle, Color fontColor, int textSizeDesc, Font fontTypeDesc, FontStyle fontStyleDesc, Color fontColorDesc)
        {
            rightItemNameUI.fontSize = textSize;
            rightItemNameUI.font = fontType;
            rightItemNameUI.fontStyle = fontStyle;
            rightItemNameUI.color = fontColor;

            rightItemDescUI.fontSize = textSizeDesc;
            rightItemDescUI.font = fontTypeDesc;
            rightItemDescUI.fontStyle = fontStyleDesc;
            rightItemDescUI.color = fontColorDesc;
        }

        public void SetHighlightName(string itemName, bool on)
        {
            interactionItemNameUI.text = itemName;
            interactionNameMainUI.SetActive(on);
        }

        public void ShowCloseButton(bool on)
        {
            noUICloseButton.SetActive(on);
        }

        public void ShowInteractionName(bool on)
        {
            interactionNameMainUI.SetActive(on);
        }

        public void CloseButton()
        {
            _examinableItem.DropObject();
        }

        public void ShowHelpPrompt()
        {
            if (showHelp)
            {
                examineHelpUI.SetActive(true);
            }
            else
            {
                examineHelpUI.SetActive(false);
            }
        }

        public void EnableCrosshair(bool on)
        {
            uiCrosshair.enabled = on;
            if (on)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void HighlightCrosshair(bool on)
        {
            if (on)
            {
                uiCrosshair.color = Color.red;
            }
            else
            {
                uiCrosshair.color = Color.white;
            }
        }
    }
}
