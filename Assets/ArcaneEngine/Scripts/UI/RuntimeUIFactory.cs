using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneEngine
{
    /// <summary>
    /// Owns the single runtime UI Toolkit panel used by every Arcane Engine screen.
    /// Each caller receives a dedicated child UIDocument so DisallowMultipleComponent
    /// never causes AddComponent to return null on the bootstrap GameObject.
    /// </summary>
    public static class RuntimeUIFactory
    {
        private const string RuntimeThemeResource = "ArcaneRuntimeTheme";
        private static PanelSettings _sharedPanel;

        public static PanelSettings SharedPanel
        {
            get
            {
                EnsurePanel();
                return _sharedPanel;
            }
        }

        public static UIDocument CreateDocument(Transform owner, string documentName, int sortingOrder)
        {
            EnsurePanel();
            GameObject host = new GameObject(documentName);
            host.transform.SetParent(owner, false);
            host.SetActive(false);
            UIDocument document = host.AddComponent<UIDocument>();
            document.panelSettings = _sharedPanel;
            document.sortingOrder = sortingOrder;
            RuntimeUIDocumentInput input = host.AddComponent<RuntimeUIDocumentInput>();
            input.Initialize(document);
            host.SetActive(true);
            return document;
        }

        public static Button CreateButton(Action action)
        {
            return new ReliableButton(action);
        }

        private static void EnsurePanel()
        {
            if (_sharedPanel != null) return;
            ThemeStyleSheet theme = Resources.Load<ThemeStyleSheet>(RuntimeThemeResource);
            if (theme == null)
            {
                Debug.LogError("Arcane Engine runtime theme could not be loaded from Resources/" + RuntimeThemeResource + ".tss. Using a temporary fallback theme.");
                theme = ScriptableObject.CreateInstance<ThemeStyleSheet>();
                theme.hideFlags = HideFlags.HideAndDontSave;
            }

            _sharedPanel = ScriptableObject.CreateInstance<PanelSettings>();
            _sharedPanel.name = "Arcane Engine Runtime Panel";
            _sharedPanel.hideFlags = HideFlags.HideAndDontSave;
            _sharedPanel.themeStyleSheet = theme;
            _sharedPanel.scaleMode = PanelScaleMode.ScaleWithScreenSize;
            _sharedPanel.referenceResolution = new Vector2Int(1920, 1080);
            _sharedPanel.match = 0.5f;
        }
    }

    internal sealed class ReliableButton : Button
    {
        private readonly Action _action;
        private float _suppressToolkitClickUntil = -1f;

        public ReliableButton(Action action)
        {
            _action = action;
            clicked += HandleToolkitClick;
        }

        public void InvokeFromInputFallback()
        {
            if (!enabledInHierarchy || resolvedStyle.display == DisplayStyle.None) return;
            if (Time.unscaledTime <= _suppressToolkitClickUntil) return;
            _suppressToolkitClickUntil = Time.unscaledTime + 0.75f;
            _action?.Invoke();
        }

        private void HandleToolkitClick()
        {
            if (Time.unscaledTime <= _suppressToolkitClickUntil) return;
            _action?.Invoke();
        }
    }

    internal sealed class RuntimeUIDocumentInput : MonoBehaviour
    {
        private UIDocument _document;
        private int _controllerIndex;
        private float _nextControllerMove;
        private ReliableButton _controllerFocus;
        private ReliableButton _fallbackPressedButton;
        private Vector2 _fallbackMouseDownPosition;
        private bool _fallbackMouseDragged;
        private const float FallbackDragThreshold = 6f;

        public void Initialize(UIDocument document) { _document = document; }

        private void Update()
        {
            if (_document == null) return;
            VisualElement root = _document.rootVisualElement;
            if (root == null || root.resolvedStyle.display == DisplayStyle.None) return;
            HandleController(root);
        }

        private void HandleController(VisualElement root)
        {
            if (!IsTopVisibleDocument()) return;
            List<ReliableButton> buttons = root.Query<ReliableButton>().ToList()
                .Where(button => button != null && button.panel != null && button.enabledInHierarchy &&
                    button.resolvedStyle.display != DisplayStyle.None && button.resolvedStyle.visibility == Visibility.Visible).ToList();
            if (buttons.Count == 0) return;
            _controllerIndex = Mathf.Clamp(_controllerIndex, 0, buttons.Count - 1);
            Vector2 direction = ArcaneInput.GamepadNavigate;
            if (direction.sqrMagnitude > 0.1f && Time.unscaledTime >= _nextControllerMove)
            {
                int change = Mathf.Abs(direction.y) >= Mathf.Abs(direction.x)
                    ? (direction.y < 0f ? 1 : -1)
                    : (direction.x > 0f ? 1 : -1);
                _controllerIndex = (_controllerIndex + change + buttons.Count) % buttons.Count;
                _nextControllerMove = Time.unscaledTime + 0.18f;
                SetControllerFocus(buttons[_controllerIndex]);
            }
            else if (direction.sqrMagnitude <= 0.1f) _nextControllerMove = 0f;

            if (_controllerFocus == null || !buttons.Contains(_controllerFocus)) SetControllerFocus(buttons[_controllerIndex]);
            if (ArcaneInput.GamepadSubmitDown && _controllerFocus != null) _controllerFocus.InvokeFromInputFallback();
        }

        private void SetControllerFocus(ReliableButton button)
        {
            if (_controllerFocus != null)
            {
                _controllerFocus.style.borderLeftWidth = _controllerFocus.style.borderRightWidth = 0f;
                _controllerFocus.style.borderTopWidth = _controllerFocus.style.borderBottomWidth = 0f;
            }
            _controllerFocus = button;
            if (_controllerFocus == null) return;
            Color focus = ProfileManager.Current.accessibility.highContrastTelegraphs ? Color.yellow : new Color(0.1f, 0.85f, 1f);
            _controllerFocus.style.borderLeftWidth = _controllerFocus.style.borderRightWidth = 3f;
            _controllerFocus.style.borderTopWidth = _controllerFocus.style.borderBottomWidth = 3f;
            _controllerFocus.style.borderLeftColor = _controllerFocus.style.borderRightColor = focus;
            _controllerFocus.style.borderTopColor = _controllerFocus.style.borderBottomColor = focus;
            _controllerFocus.Focus();
        }

        private bool IsTopVisibleDocument()
        {
            UIDocument[] documents = FindObjectsByType<UIDocument>();
            for (int i = 0; i < documents.Length; i++)
            {
                UIDocument other = documents[i];
                if (other == null || other == _document || other.sortingOrder <= _document.sortingOrder) continue;
                VisualElement otherRoot = other.rootVisualElement;
                if (otherRoot != null && otherRoot.resolvedStyle.display != DisplayStyle.None) return false;
            }
            return true;
        }

        private ReliableButton FindReliableButtonAt(Vector2 screenPosition)
        {
            if (_document == null) return null;
            VisualElement root = _document.rootVisualElement;
            if (root == null || root.resolvedStyle.display == DisplayStyle.None) return null;
            float width = root.resolvedStyle.width;
            float height = root.resolvedStyle.height;
            if (float.IsNaN(width) || width <= 0f || float.IsNaN(height) || height <= 0f) return null;
            Vector2 panelPoint = new Vector2(screenPosition.x / Mathf.Max(1f, Screen.width) * width,
                (Screen.height - screenPosition.y) / Mathf.Max(1f, Screen.height) * height);
            List<ReliableButton> buttons = root.Query<ReliableButton>().ToList();
            for (int i = buttons.Count - 1; i >= 0; i--)
            {
                ReliableButton button = buttons[i];
                if (button == null || button.panel == null || !button.enabledInHierarchy ||
                    button.resolvedStyle.display == DisplayStyle.None || !button.worldBound.Contains(panelPoint)) continue;
                return button;
            }
            return null;
        }

        private void OnGUI()
        {
            Event evt = Event.current;
            if (evt == null || evt.button != 0) return;

            switch (evt.type)
            {
                case EventType.MouseDown:
                {
                    _fallbackPressedButton = FindReliableButtonAt(evt.mousePosition);
                    _fallbackMouseDownPosition = evt.mousePosition;
                    _fallbackMouseDragged = false;
                    break;
                }

                case EventType.MouseDrag:
                {
                    if (_fallbackPressedButton != null)
                    {
                        float distanceSq = (evt.mousePosition - _fallbackMouseDownPosition).sqrMagnitude;
                        if (distanceSq > FallbackDragThreshold * FallbackDragThreshold)
                            _fallbackMouseDragged = true;
                    }
                    break;
                }

                case EventType.MouseUp:
                {
                    if (_fallbackPressedButton != null && !_fallbackMouseDragged)
                    {
                        ReliableButton released = FindReliableButtonAt(evt.mousePosition);
                        if (released != null && ReferenceEquals(_fallbackPressedButton, released))
                        {
                            released.InvokeFromInputFallback();
                            evt.Use();
                        }
                    }
                    _fallbackPressedButton = null;
                    _fallbackMouseDragged = false;
                    break;
                }
            }
        }
    }
}
