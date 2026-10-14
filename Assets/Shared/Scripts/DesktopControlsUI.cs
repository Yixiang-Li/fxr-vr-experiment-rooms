using System;
using System.IO;
using FXR.Memory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace FXR.Shared
{
    // A compact desktop overlay complements the room's world-space experiment
    // panel. Screen-space camera rendering also works in deterministic captures.
    public sealed class DesktopControlsUI : MonoBehaviour
    {
        public Canvas Canvas { get; private set; }
        public Button TaskViewButton { get; private set; }
        public Button HelpButton { get; private set; }
        public Button ResumeButton { get; private set; }
        public Button ResultsFolderButton { get; private set; }
        public GameObject PausePanel { get; private set; }
        public bool HelpOpen => helpPanel != null && helpPanel.activeSelf;
        public bool BlocksNavigation => Canvas != null && Canvas.enabled && (HelpOpen || (PausePanel != null && PausePanel.activeSelf));
        DesktopExplorer explorer;
        MemoryRoomController room;
        GameObject helpPanel;
        TMP_Text heading, instructions, pauseText;
        TMP_FontAsset font;
        Canvas worldPanel;
        TMP_Text helpText;
        string moveKeys = "WASD", verticalKeys = "Q/E", keyboardLayout = "__uninitialized__";
        readonly Color ink = new Color(.045f, .095f, .11f);
        readonly Color accent = new Color(.22f, .48f, .41f);

        public void Initialize(DesktopExplorer owner, MemoryRoomController controller, Camera camera)
        {
            explorer = owner; room = controller; font = room.statusText.font;
            worldPanel = room.actionButton.GetComponentInParent<Canvas>();
            var worldRaycaster = worldPanel.GetComponent<GraphicRaycaster>();
            // A foreground practice object or wall must occlude world UI, rather
            // than allow a click to activate a button behind that geometry.
            worldRaycaster.blockingObjects = GraphicRaycaster.BlockingObjects.ThreeD;
            worldRaycaster.blockingMask = ~0;
            var root = new GameObject("Desktop controls", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.transform.SetParent(transform, false);
            Canvas = root.GetComponent<Canvas>();
            Canvas.renderMode = RenderMode.ScreenSpaceCamera; Canvas.worldCamera = camera;
            Canvas.planeDistance = .2f; Canvas.sortingOrder = 80;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1600, 1000); scaler.matchWidthOrHeight = 1;
            var dock = Panel("Control guide", root.transform, new Vector2(314, 94), new Vector2(580, 146), new Color(ink.r, ink.g, ink.b, .93f));
            var dockRect = dock.GetComponent<RectTransform>();
            dockRect.anchorMin = dockRect.anchorMax = Vector2.zero;
            dock.GetComponent<Image>().raycastTarget = false;
            heading = Label("Control mode", dock.transform, "KEYBOARD + MOUSE", new Vector2(0, 51), new Vector2(532, 28), 21);
            instructions = Label("Controls", dock.transform, "", new Vector2(0, 9), new Vector2(532, 58), 22);
            TaskViewButton = Button("F  TASK VIEW", dock.transform, new Vector2(-183, -46), new Vector2(176, 38), explorer.FocusTaskView);
            HelpButton = Button("H  HELP", dock.transform, new Vector2(0, -46), new Vector2(158, 38), ToggleHelp);
            ResultsFolderButton = Button("CSV FOLDER", dock.transform, new Vector2(183, -46), new Vector2(176, 38), OpenResultsFolder);
            helpPanel = Modal("Desktop help", root.transform);
            Label("Help title", helpPanel.transform, "MEMORY ROOM / QUICK GUIDE", new Vector2(0, 210), new Vector2(620, 54), 32);
            helpText = Label("Help text", helpPanel.transform,
                "1. Preview the settings; choose order and sound.\n2. Click START ROUND 1 or press Space.\n3. Click two cards to find six matching pairs.\n4. Rate mental effort 1-5 after each round.\n5. Finish both rounds to view and save results.\n\nExplore: WASD + right drag. F: task view.\nG: glide/fly. Q/E: down/up while flying.\nDrag the orange cube to practise.\nDuring rounds: fixed view. Esc: pause.",
                new Vector2(0, 5), new Vector2(620, 320), 24);
            Button("BACK TO ROOM", helpPanel.transform, new Vector2(0, -206), new Vector2(280, 58), CloseHelp);
            helpPanel.SetActive(false);
            PausePanel = Modal("Paused session", root.transform);
            Label("Pause title", PausePanel.transform, "SESSION PAUSED", new Vector2(0, 147), new Vector2(620, 60), 36);
            pauseText = Label("Pause explanation", PausePanel.transform, "", new Vector2(0, 0), new Vector2(620, 200), 28);
            ResumeButton = Button("RESUME / ESC", PausePanel.transform, new Vector2(-155, -169), new Vector2(280, 60), room.TogglePause);
            Button("RESTART SESSION", PausePanel.transform, new Vector2(155, -169), new Vector2(280, 60), room.RequestReset);
            PausePanel.SetActive(false);
        }

        void LateUpdate()
        {
            if (room == null || room.Session == null || Canvas == null) return;
            var keyboard = Keyboard.current;
            if (keyboard != null && keyboardLayout != keyboard.keyboardLayout)
            {
                keyboardLayout = keyboard.keyboardLayout;
                moveKeys = KeyName(keyboard.wKey) + KeyName(keyboard.aKey) + KeyName(keyboard.sKey) + KeyName(keyboard.dKey);
                verticalKeys = KeyName(keyboard.qKey) + "/" + KeyName(keyboard.eKey);
                helpText.text = "1. Preview the settings; choose order and sound.\n2. Click START ROUND 1 or press Space.\n3. Click two cards to find six matching pairs.\n4. Rate mental effort 1-5 after each round.\n5. Finish both rounds to view and save results.\n\n"
                    + "Explore: " + moveKeys + "/arrows + right drag. F: task view.\nG: glide/fly. " + verticalKeys + ": down/up while flying.\nDrag the orange cube to practise.\nDuring rounds: fixed view. Esc: pause.";
            }
            bool locked = explorer.TaskLocked;
            heading.text = locked ? "TASK VIEW / CAMERA FIXED" : (explorer.IsFlying ? "EXPLORE / FLY" : "EXPLORE / GLIDE");
            instructions.text = locked ? "Left click: select cards and buttons\nEsc: pause  /  1-5: rate mental effort"
                : moveKeys + "/arrows: move  /  Right drag: look\nG: glide/fly  /  " + verticalKeys + ": down/up in fly mode";
            TaskViewButton.interactable = !room.ResetPending && !room.IsPaused && !HelpOpen;
            HelpButton.interactable = !locked;
            ResultsFolderButton.gameObject.SetActive(room.Session.Phase == SessionPhase.Completed);
            ResultsFolderButton.interactable = room.SavedResultsFile != null;
            PausePanel.SetActive(room.IsPaused && room.Session.Phase == SessionPhase.Running && !room.ResetPending);
            if (PausePanel.activeSelf)
            {
                int seconds = Mathf.CeilToInt(room.Session.RemainingSeconds);
                pauseText.text = $"Time remaining: {seconds / 60:00}:{seconds % 60:00}\n\nThe timer and card interaction are paused.\nFor a clean comparison, restart after an interruption.";
            }
            // Do not cover another teammate's room when walking out of Memory.
            var p = room.transform.InverseTransformPoint(explorer.transform.position);
            Canvas.enabled = room.gameObject.activeInHierarchy && Mathf.Abs(p.x) < 4.7f && p.z > -4.5f && p.z < 7f;
            worldPanel.enabled = Canvas.enabled;
        }

        static string KeyName(UnityEngine.InputSystem.Controls.KeyControl key)
        {
            return string.IsNullOrEmpty(key.displayName) ? key.keyCode.ToString() : key.displayName.ToUpperInvariant();
        }

        public void ToggleHelp()
        {
            if (explorer.TaskLocked) return;
            helpPanel.SetActive(!HelpOpen);
        }
        public void CloseHelp() { if (helpPanel != null) helpPanel.SetActive(false); }

        void OpenResultsFolder()
        {
            if (room.SavedResultsFile == null) return;
            try { Application.OpenURL(new Uri(Path.GetDirectoryName(room.SavedResultsFile)).AbsoluteUri); }
            catch (Exception error) { Debug.LogWarning("Cannot open results folder: " + error.Message); }
        }

        GameObject Modal(string name, Transform parent)
        {
            var blocker = Panel(name + " blocker", parent, Vector2.zero, Vector2.zero, new Color(ink.r, ink.g, ink.b, .83f));
            var rect = blocker.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one; rect.offsetMin = rect.offsetMax = Vector2.zero;
            var content = Panel(name, blocker.transform, Vector2.zero, new Vector2(700, 560), ink);
            // Return the full blocker for visibility and navigation; content holds
            // the centered labels/buttons because its transform is also centered.
            content.GetComponent<Image>().raycastTarget = false;
            return blocker;
        }

        GameObject Panel(string name, Transform parent, Vector2 position, Vector2 size, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>(); rect.sizeDelta = size; rect.anchoredPosition = position;
            go.GetComponent<Image>().color = color;
            return go;
        }

        TMP_Text Label(string name, Transform parent, string text, Vector2 position, Vector2 size, float fontSize)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
            go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>(); rect.sizeDelta = size; rect.anchoredPosition = position;
            var label = go.GetComponent<TextMeshProUGUI>(); label.font = font;
            label.text = text; label.fontSize = fontSize; label.alignment = TextAlignmentOptions.MidlineLeft;
            label.color = new Color(.85f, .93f, .9f); label.raycastTarget = false; label.enableWordWrapping = true;
            return label;
        }

        Button Button(string text, Transform parent, Vector2 position, Vector2 size, UnityEngine.Events.UnityAction action)
        {
            var go = Panel(text, parent, position, size, accent);
            var button = go.AddComponent<Button>(); button.targetGraphic = go.GetComponent<Image>();
            var colors = button.colors; colors.highlightedColor = new Color(1.2f, 1.2f, 1.2f);
            colors.pressedColor = new Color(.7f, .85f, .8f); colors.disabledColor = new Color(.5f, .5f, .5f); button.colors = colors;
            var navigation = button.navigation; navigation.mode = Navigation.Mode.None; button.navigation = navigation;
            var label = Label("Label", go.transform, text, Vector2.zero, size - new Vector2(12, 4), 20);
            label.alignment = TextAlignmentOptions.Center;
            button.onClick.AddListener(action);
            return button;
        }
    }
}
