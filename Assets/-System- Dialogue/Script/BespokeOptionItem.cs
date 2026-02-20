using UnityEngine;
using UnityEngine.EventSystems;
using Yarn.Unity.Attributes;
using TMPro;

#nullable enable

namespace Yarn.Unity
{
    [System.Serializable]
    public struct InternalAppearance
    {
        [SerializeField] public Sprite sprite;
        [SerializeField] public Color colour;

        public InternalAppearance(Color defaultColor)
        {
            sprite = null!;
            colour = defaultColor;
        }
    }

    public sealed class BespokeOptionItem : UnityEngine.UI.Selectable, ISubmitHandler, IPointerClickHandler, IPointerEnterHandler
    {
        [MustNotBeNull, SerializeField] TextMeshProUGUI? text;
        [SerializeField] UnityEngine.UI.Image? selectionImage;

        [Group("Appearance"), SerializeField] 
        public InternalAppearance normal = new InternalAppearance(Color.white);
        
        [Group("Appearance"), SerializeField] 
        public InternalAppearance selected = new InternalAppearance(Color.yellow);
        
        [Group("Appearance"), SerializeField] 
        public InternalAppearance disabled = new InternalAppearance(Color.gray);

        [Group("Appearance"), SerializeField] bool disabledStrikeThrough = true;

        public YarnTaskCompletionSource<DialogueOption?>? OnOptionSelected;
        public System.Threading.CancellationToken completionToken;

        private bool hasSubmittedOptionSelection = false;

        private DialogueOption? _option;
        public DialogueOption Option
        {
            get
            {
                if (_option == null)
                {
                    throw new System.NullReferenceException("Option has not been set on the option item");
                }
                return _option;
            }

            set
            {
                _option = value;
                hasSubmittedOptionSelection = false;

                string line = value.Line.TextWithoutCharacterName.Text;
                if (disabledStrikeThrough && !value.IsAvailable)
                {
                    line = $"<s>{value.Line.TextWithoutCharacterName.Text}</s>";
                }

                if (text == null)
                {
                    Debug.LogWarning($"The {nameof(text)} is null, is it not connected in the inspector?", this);
                    return;
                }

                text.text = line;
                interactable = value.IsAvailable;

                // Ensure visibility immediately
                ApplyStyle(normal);
            }
        }

        private void ApplyStyle(InternalAppearance style)
        {
            Color newColour = style.colour;
            
            // CRITICAL FIX: Ensure alpha is 1 if it was accidentally set to 0 in Inspector
            if (newColour.a == 0) newColour.a = 1;

            Sprite newSprite = style.sprite;
            if (!Option.IsAvailable)
            {
                newColour = disabled.colour;
                if (newColour.a == 0) newColour.a = 1;
                newSprite = disabled.sprite;
            }

            if (text == null) return;

            text.color = newColour;

            if (selectionImage != null)
            {
                selectionImage.color = newColour;
                if (newSprite != null)
                {
                    selectionImage.sprite = newSprite;
                    selectionImage.gameObject.SetActive(true);
                }
                else
                {
                    selectionImage.gameObject.SetActive(false);
                }
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            ApplyStyle(selected);
        }

        public override void OnDeselect(BaseEventData eventData)
        {
            base.OnDeselect(eventData);
            ApplyStyle(normal);
        }

        new public bool IsHighlighted
        {
            get
            {
                return EventSystem.current.currentSelectedGameObject == this.gameObject;
            }
        }

        public void OnSubmit(BaseEventData eventData)
        {
            InvokeOptionSelected();
        }

        public void InvokeOptionSelected()
        {
            if (!IsInteractable()) return;

            if (hasSubmittedOptionSelection == false && !completionToken.IsCancellationRequested)
            {
                hasSubmittedOptionSelection = true;
                OnOptionSelected?.TrySetResult(this.Option);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            InvokeOptionSelected();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.Select();
        }
    }
}
