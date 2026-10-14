using System.Collections;
using System.IO;
using System.Linq;
using FXR.Memory;
using FXR.Shared;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.TestTools;

public sealed class DesktopMemoryTests
{
    Keyboard keyboard;
    Mouse mouse;
    InputSettings.BackgroundBehavior oldBackground;
#if UNITY_EDITOR
    InputSettings.EditorInputBehaviorInPlayMode oldRouting;
#endif

    [SetUp]
    public void RouteInput()
    {
        oldBackground = InputSystem.settings.backgroundBehavior;
#if UNITY_EDITOR
        oldRouting = InputSystem.settings.editorInputBehaviorInPlayMode;
#endif
        InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
#if UNITY_EDITOR
        InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
        keyboard = InputSystem.AddDevice<Keyboard>(); mouse = InputSystem.AddDevice<Mouse>();
    }

    [TearDown]
    public void RestoreInput()
    {
        InputSystem.RemoveDevice(keyboard); InputSystem.RemoveDevice(mouse);
        InputSystem.settings.backgroundBehavior = oldBackground;
#if UNITY_EDITOR
        InputSystem.settings.editorInputBehaviorInPlayMode = oldRouting;
#endif
    }

    IEnumerator Press(Key key)
    {
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(key));
        yield return null;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState());
        yield return null;
    }

    IEnumerator Point(Vector2 position, bool pressed)
    {
        var state = new MouseState { position = position };
        if (pressed) state = state.WithButton(MouseButton.Left);
        InputSystem.QueueStateEvent(mouse, state);
        yield return null; yield return new WaitForFixedUpdate();
    }

    [UnityTest]
    public IEnumerator DesktopDragNavigationPauseAndRatingsWorkThroughInput()
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainScene");
        yield return null;
        var room = Object.FindFirstObjectByType<MemoryRoomController>();
        var explorer = Object.FindFirstObjectByType<DesktopExplorer>();
        var camera = Camera.main;
        Assert.That(room.DesktopInput, Is.True);
        Assert.That(explorer.Controls, Is.Not.Null);
        Assert.That(room.environmentLight, Is.Not.Null);
        float taskIntensity = room.roomLight.intensity;
        Color officeAccent = room.environmentLight.color;
        room.PreviewNature();
        Assert.That(room.environmentLight.color, Is.Not.EqualTo(officeAccent));
        Assert.That(room.roomLight.intensity, Is.EqualTo(taskIntensity));
        room.PreviewOffice();
        var cube = room.GetComponentInChildren<MemoryPracticeObject>();
        Vector3 home = cube.GetComponent<Rigidbody>().position;
        var screen = (Vector2)camera.WorldToScreenPoint(cube.transform.position);
        yield return Point(screen, false);
        yield return Point(screen, true);
        Assert.That(cube.IsDragging, Is.True, "Actual mouse ray should grab the practice cube.");
        yield return Point((Vector2)camera.WorldToScreenPoint(cube.transform.position + Vector3.right * .18f), true);
        yield return new WaitForSeconds(.08f);
        Assert.That(cube.GetComponent<Rigidbody>().position.x, Is.GreaterThan(home.x + .08f));
        yield return Point(screen, false);
        Assert.That(cube.IsDragging, Is.False);
        Assert.That(cube.GetComponent<Rigidbody>().isKinematic, Is.False);
        room.ResetRoom();
        Assert.That(Vector3.Distance(cube.GetComponent<Rigidbody>().position, home), Is.LessThan(.04f));
        yield return Press(Key.G);
        Assert.That(explorer.IsFlying, Is.True);
        float height = explorer.transform.position.y;
        float finish = Time.time + .15f;
        while (Time.time < finish)
        {
            keyboard.MakeCurrent();
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.E));
            yield return null;
        }
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null;
        Assert.That(explorer.transform.position.y, Is.GreaterThan(height + .1f));
        yield return Press(Key.F);
        Assert.That(explorer.IsFlying, Is.False);
        float forward = explorer.transform.position.z;
        finish = Time.time + .15f;
        while (Time.time < finish)
        {
            keyboard.MakeCurrent();
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.UpArrow));
            yield return null;
        }
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null;
        Assert.That(explorer.transform.position.z, Is.GreaterThan(forward + .1f), "Arrow-key navigation is independent of letter layout.");
        yield return Press(Key.F);
        yield return Press(Key.Space);
        Assert.That(room.Session.Phase, Is.EqualTo(SessionPhase.Running));
        Vector3 taskPosition = explorer.transform.position;
        InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W, Key.G));
        yield return new WaitForSeconds(.15f);
        InputSystem.QueueStateEvent(keyboard, new KeyboardState()); yield return null;
        Assert.That(explorer.transform.position, Is.EqualTo(taskPosition));
        Assert.That(explorer.IsFlying, Is.False, "Flight is locked during the measured task.");
        Assert.That(cube.CanDrag, Is.False);
        int mismatch = Enumerable.Range(1, 12).First(i => room.Session.PairIds[i] != room.Session.PairIds[0]);
        yield return Point(camera.WorldToScreenPoint(room.cards[0].transform.position), true);
        yield return Point(camera.WorldToScreenPoint(room.cards[0].transform.position), false);
        Assert.That(room.Session.Flips, Is.EqualTo(1), "Actual mouse ray should select a card.");
        room.SelectCard(mismatch);
        yield return Press(Key.Escape);
        float remaining = room.Session.RemainingSeconds;
        yield return new WaitForSeconds(.25f);
        Assert.That(room.Session.RemainingSeconds, Is.EqualTo(remaining).Within(.001f));
        Assert.That(room.Session.SelectionPending, Is.True);
        Assert.That(room.CanSelectCard(5), Is.False);
        Assert.That(explorer.Controls.PausePanel.activeSelf, Is.True);
        Capture(camera, "desktop-paused", 1280, 800);
        CheckText(explorer.Controls);
        room.SetWindowFocus(false);
        yield return Press(Key.Escape);
        Assert.That(room.IsPaused, Is.True, "Resume cannot count time while the window is unfocused.");
        room.SetWindowFocus(true);
        Assert.That(room.IsPaused, Is.False);
        yield return new WaitForSeconds(1f);
        Assert.That(room.Session.SelectionPending, Is.False);
        Assert.That(room.HasInterruption, Is.True);
        room.Session.Tick(room.Session.RemainingSeconds - .01f);
        yield return new WaitForSeconds(.1f);
        Assert.That(room.Session.Phase, Is.EqualTo(SessionPhase.Rating));
        yield return Press(Key.Digit3);
        Assert.That(room.Session.Results[0].mentalEffort, Is.EqualTo(3));
        Assert.That(room.Session.Phase, Is.EqualTo(SessionPhase.ReadyForNextRound));
        yield return Press(Key.Space);
        Assert.That(room.Session.Condition, Is.EqualTo(EnvironmentCondition.Nature));
        Capture(camera, "desktop-nature-task", 1280, 800);
        room.ResetRoom();
        Assert.That(room.HasInterruption, Is.False);
        Assert.That(room.IsPaused, Is.False);
        Assert.That(explorer.IsFlying, Is.False);
    }

    [UnityTest]
    public IEnumerator DesktopHelpAndResultsFitAndNewSessionNeedsConfirmation()
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainScene");
        yield return null;
        var room = Object.FindFirstObjectByType<MemoryRoomController>();
        var controls = Object.FindFirstObjectByType<DesktopExplorer>().Controls;
        Capture(Camera.main, "desktop-office-task", 1280, 800);
        CheckText(controls);
        yield return Press(Key.H);
        Assert.That(controls.HelpOpen, Is.True);
        CheckText(controls); Capture(Camera.main, "desktop-help", 1280, 720);
        yield return Press(Key.Escape);
        Assert.That(controls.HelpOpen, Is.False);
        string folder = Path.Combine(Application.persistentDataPath, "MemoryResults");
        var existing = Directory.Exists(folder) ? Directory.GetFiles(folder) : new string[0];
        room.StartOrContinue();
        for (int round = 0; round < 2; round++)
        {
            room.Session.Tick(179.99f); yield return new WaitForSeconds(.1f);
            room.RateEffort(3);
            if (round == 0) room.StartOrContinue();
        }
        yield return null;
        Assert.That(room.Session.Phase, Is.EqualTo(SessionPhase.Completed));
        Assert.That(controls.ResultsFolderButton.gameObject.activeSelf, Is.True);
        Assert.That(controls.ResultsFolderButton.interactable, Is.True);
        var exported = Directory.GetFiles(folder).Except(existing).Single();
        var csv = File.ReadAllText(exported);
        Assert.That(csv, Does.Contain("input_mode,app_version"));
        Assert.That(csv, Does.Contain(",keyboard_mouse,0.4.0,"));
        Capture(Camera.main, "desktop-results", 1920, 1080);
        CheckText(controls);
        room.actionButton.onClick.Invoke();
        Assert.That(room.ResetPending, Is.True);
        Assert.That(room.Session.Results.Count, Is.EqualTo(2));
        room.CancelReset();
        Assert.That(room.Session.Phase, Is.EqualTo(SessionPhase.Completed));
        room.ResetRoom();
        Assert.That(File.Exists(exported), Is.True, "Reset must retain completed CSV files.");
        File.Delete(exported);
    }

    static void CheckText(DesktopControlsUI controls)
    {
        Canvas.ForceUpdateCanvases();
        foreach (var text in controls.GetComponentsInChildren<TextMeshProUGUI>(false))
        { text.ForceMeshUpdate(); Assert.That(text.isTextOverflowing, Is.False, text.name + " / " + text.text); }
    }

    static void Capture(Camera camera, string name, int width, int height)
    {
        Directory.CreateDirectory("Builds/Review");
        var rt = RenderTexture.GetTemporary(width, height, 24);
        var previousTarget = camera.targetTexture; var previous = RenderTexture.active;
        camera.targetTexture = rt; Canvas.ForceUpdateCanvases();
#if UNITY_EDITOR
        bool asyncCompilation = UnityEditor.ShaderUtil.allowAsyncCompilation;
        UnityEditor.ShaderUtil.allowAsyncCompilation = false;
        try { camera.Render(); }
        finally { UnityEditor.ShaderUtil.allowAsyncCompilation = asyncCompilation; }
#else
        camera.Render();
#endif
        RenderTexture.active = rt;
        var texture = new Texture2D(width, height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, width, height), 0, 0); texture.Apply();
        File.WriteAllBytes("Builds/Review/" + name + ".png", texture.EncodeToPNG());
        camera.targetTexture = previousTarget; RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(rt); Object.Destroy(texture);
    }
}
