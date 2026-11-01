using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FXR.Integration;
using FXR.Memory;
using FXR.Shared;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public sealed class ThreeRoomTests
{
    RoomIntegrationManager manager;
    Mouse mouse;
    InputSettings.BackgroundBehavior oldBackground;
#if UNITY_EDITOR
    InputSettings.EditorInputBehaviorInPlayMode oldRouting;
#endif
    [SetUp]public void RoutePointer()
    {
        oldBackground=InputSystem.settings.backgroundBehavior;
        InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
#if UNITY_EDITOR
        oldRouting=InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
        mouse=InputSystem.AddDevice<Mouse>();
    }
    IEnumerator Click(Vector2 position)
    {
        InputSystem.QueueStateEvent(mouse,new MouseState{position=position});yield return null;yield return null;
        InputSystem.QueueStateEvent(mouse,new MouseState{position=position}.WithButton(MouseButton.Left));yield return null;
        InputSystem.QueueStateEvent(mouse,new MouseState{position=position});yield return null;
    }
    [UnitySetUp]
    public IEnumerator Load()
    {
        Time.timeScale=1;AudioListener.pause=false;
        yield return SceneManager.LoadSceneAsync("MainScene");yield return null;yield return null;
        manager=Object.FindFirstObjectByType<RoomIntegrationManager>();
        Assert.That(manager,Is.Not.Null);
    }
    [TearDown]public void Cleanup()
    {
        Time.timeScale=1;AudioListener.pause=false;
        InputSystem.RemoveDevice(mouse);InputSystem.settings.backgroundBehavior=oldBackground;
#if UNITY_EDITOR
        InputSystem.settings.editorInputBehaviorInPlayMode=oldRouting;
#endif
    }
    IEnumerator Switch(int room)
    {
        manager.RoomButtons[room].onClick.Invoke();
        Assert.That(manager.MenuOpen,Is.True);
        manager.ConfirmButton.onClick.Invoke();
        yield return null;yield return null;
        CheckSharedRig();
    }
    void CheckSharedRig()
    {
        Assert.That(SceneManager.sceneCount,Is.EqualTo(1));
        Assert.That(Object.FindObjectsByType<EventSystem>(FindObjectsSortMode.None).Length,Is.EqualTo(1));
        Assert.That(Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None).Length,Is.EqualTo(1));
        Assert.That(Object.FindObjectsByType<Camera>(FindObjectsSortMode.None).Count(c=>c.targetTexture==null),Is.EqualTo(1));
        Assert.That(manager.rooms.Count(s=>s.instance!=null && s.instance.activeInHierarchy),Is.EqualTo(1));
        Assert.That(Camera.main,Is.EqualTo(manager.explorer.view));
    }

    [UnityTest]
    public IEnumerator SwitchingCancelPauseAndRestartKeepOneSceneAndOnePlayer()
    {
        CheckSharedRig();
        var memory=manager.CurrentInstance.GetComponent<MemoryRoomController>();
        memory.StartOrContinue();yield return null;
        manager.OpenMenu();
        Canvas.ForceUpdateCanvases();
        var buttonRect=(RectTransform)manager.RoomButtons[1].transform;
        var pointer=new PointerEventData(EventSystem.current)
        {
            position=RectTransformUtility.WorldToScreenPoint(Camera.main,buttonRect.TransformPoint(buttonRect.rect.center))
        };
        var hits=new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointer,hits);
        Assert.That(hits.Count,Is.GreaterThan(0));
        Assert.That(hits[0].gameObject,Is.EqualTo(manager.RoomButtons[1].gameObject),"The modal must not cover the room navigation buttons.");
        manager.RoomButtons[1].onClick.Invoke();
        float time=memory.Session.RemainingSeconds;
        yield return new WaitForSecondsRealtime(.08f);
        Assert.That(memory.Session.RemainingSeconds,Is.EqualTo(time));
        Assert.That(memory.IsPaused,Is.True);
        manager.CancelButton.onClick.Invoke();yield return null;
        Assert.That(manager.CurrentRoom,Is.EqualTo(0));Assert.That(memory.IsPaused,Is.False);
        Assert.That(memory.HasInterruption,Is.True);
        yield return Switch(1);
        var root=manager.CurrentInstance;
        Assert.That(root.GetComponentsInChildren<CharacterController>(true).Length,Is.Zero);
        Assert.That(root.GetComponentsInChildren<ExhibitInteraction>(true).All(e=>e.playerCamera==Camera.main),Is.True);
        Assert.That(root.GetComponentsInChildren<ExperimentManager>(true).All(e=>!e.testMode),Is.True);
        Capture("integration-museum-welcome");
        yield return Switch(2);
        Assert.That(manager.CurrentInstance.GetComponentsInChildren<Camera>(true).Single().targetTexture,Is.Not.Null);
        Canvas.ForceUpdateCanvases();
        var paper=manager.CurrentInstance.GetComponentInChildren<ConsentPaperInteraction>(true);
        pointer.position=Camera.main.WorldToScreenPoint(paper.GetComponent<Collider>().bounds.center);
        var paperRay=Camera.main.ScreenPointToRay(pointer.position);
        Assert.That(Physics.Raycast(paperRay,out var paperHit),Is.True);
        Assert.That(paperHit.collider.gameObject,Is.EqualTo(paper.gameObject),"Paper must be the first physical hit; screen point: "+pointer.position+" actual hit: "+paperHit.collider.name);
        hits.Clear();EventSystem.current.RaycastAll(pointer,hits);
        Assert.That(hits,Is.Empty,"The monitor RenderTexture canvas must not block paper/desk clicks.");
        Capture("integration-stroop-entry");
        yield return Switch(0);
        memory=manager.CurrentInstance.GetComponent<MemoryRoomController>();
        Assert.That(memory.Session.Phase,Is.EqualTo(SessionPhase.Idle));
        memory.StartOrContinue();yield return null;
        manager.ResetButton.onClick.Invoke();manager.ConfirmButton.onClick.Invoke();yield return null;yield return null;
        Assert.That(manager.CurrentInstance.GetComponent<MemoryRoomController>().Session.Phase,Is.EqualTo(SessionPhase.Idle));
        CheckSharedRig();Capture("integration-memory");
    }

    [UnityTest]
    public IEnumerator MuseumObservationsDistractionQuizAndCsvSurviveIntegratedReset()
    {
        yield return Switch(1);
        var root=manager.CurrentInstance;
        root.GetComponentInChildren<WelcomeScreen>(true).StartExperiment();yield return null;
        var experiment=root.GetComponentInChildren<ExperimentManager>(true);
        Assert.That(experiment.experimentStarted,Is.True);
        Assert.That(experiment.exhibits.Count(e=>e.condition==ExhibitInteraction.TrialCondition.Photo),Is.EqualTo(6));
        foreach(var exhibit in experiment.exhibits)
        {
            exhibit.observationDuration=.015f;exhibit.StartObservation();
            if(exhibit.condition==ExhibitInteraction.TrialCondition.Photo)
            {
                manager.explorer.view.transform.position=exhibit.photoTarget.position-Vector3.forward*2;
                manager.explorer.view.transform.LookAt(exhibit.photoTarget);
                exhibit.ToggleCamera();exhibit.TryTakePhoto();
            }
            yield return new WaitForSeconds(.08f);
        }
        Assert.That(experiment.observationPhaseFinished,Is.True);
        var task=root.GetComponentInChildren<DistractorTask>(true);task.taskDuration=.015f;
        root.GetComponentInChildren<QuizStationInteraction>(true).SendMessage("StartInteraction");
        yield return new WaitForSeconds(.1f);
        var quiz=root.GetComponentInChildren<MemoryQuizManager>(true);
        Assert.That(quiz.gameObject.activeInHierarchy,Is.True);
        for(int i=0;i<12;i++)
        {
            Assert.That(quiz.questionText.text,Is.Not.Empty);
            if(quiz.imageOptions.activeSelf)quiz.imageButtonA.onClick.Invoke();else quiz.textButtons[0].onClick.Invoke();
            Assert.That(quiz.nextButton.interactable,Is.True);quiz.nextButton.onClick.Invoke();yield return null;
        }
        Assert.That(quiz.QuizFinished,Is.True);
        var csv=root.GetComponentInChildren<QuizCSVExporter>(true);string path=csv.SavedPath;
        Assert.That(File.Exists(path),Is.True);Assert.That(File.ReadAllLines(path).Length,Is.EqualTo(13));
        Assert.That(path,Does.StartWith(Application.persistentDataPath));
        Capture("integration-museum-results");
        manager.ResetButton.onClick.Invoke();manager.ConfirmButton.onClick.Invoke();yield return null;yield return null;
        Assert.That(File.Exists(path),Is.True);
        Assert.That(manager.CurrentInstance.GetComponentInChildren<ExperimentManager>(true).experimentStarted,Is.False);
        File.Delete(path);
    }

    [UnityTest]
    public IEnumerator StroopConsentButtonsResultsAndWholeRoomResetWork()
    {
        yield return Switch(2);
        var root=manager.CurrentInstance;
        var stroop=root.GetComponentInChildren<StroopExperiment>(true);
        var consent=root.GetComponentInChildren<ConsentPaperInteraction>(true);
        var start=root.GetComponentsInChildren<PhysicalButton>(true).Single(b=>b.name=="StartButton");
        start.Press();Assert.That(stroop.IsRunning,Is.False);
        yield return Click(Camera.main.WorldToScreenPoint(consent.GetComponent<Collider>().bounds.center));
        Assert.That(consent.PanelOpen,Is.True,"A real pointer click must open the consent paper.");
        typeof(ConsentPaperInteraction).GetField("closeDelay",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(consent,.02f);
        consent.AcceptConsent();Assert.That(consent.HasConsent,Is.True);
        yield return new WaitForSeconds(.04f);
        Assert.That(consent.PanelOpen,Is.False);
        var camera=Camera.main;var oldPosition=camera.transform.position;var oldRotation=camera.transform.rotation;
        var startPoint=start.GetComponent<Collider>().bounds.center;
        camera.transform.position=startPoint+new Vector3(0,.3f,-.75f);camera.transform.LookAt(startPoint);
        yield return Click(camera.WorldToScreenPoint(startPoint));
        Assert.That(stroop.IsRunning,Is.True,"A real pointer click must activate the physical Start button.");
        camera.transform.SetPositionAndRotation(oldPosition,oldRotation);
        yield return null;yield return null;
        Capture("integration-stroop-trial");
        var word=root.GetComponentsInChildren<TMP_Text>(true).Single(t=>t.name=="StroopWord");
        for(int i=0;i<stroop.TotalTrials;i++)
        {
            var c=word.color;
            if(c.r>c.g && c.r>c.b)stroop.SubmitRed();else if(c.g>c.b)stroop.SubmitGreen();else stroop.SubmitBlue();
            yield return null;
        }
        Assert.That(stroop.Completed,Is.True);Assert.That(stroop.CorrectAnswers,Is.EqualTo(stroop.TotalTrials));
        Capture("integration-stroop-results");
        manager.ResetButton.onClick.Invoke();manager.ConfirmButton.onClick.Invoke();yield return null;yield return null;
        root=manager.CurrentInstance;
        Assert.That(root.GetComponentInChildren<ConsentPaperInteraction>(true).HasConsent,Is.False);
        Assert.That(root.GetComponentInChildren<StroopExperiment>(true).Completed,Is.False);
        consent=root.GetComponentInChildren<ConsentPaperInteraction>(true);
        typeof(ConsentPaperInteraction).GetField("closeDelay",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(consent,.02f);
        consent.AcceptConsent();yield return new WaitForSeconds(.05f);yield return null;
        Assert.That(manager.StartStroopButton.gameObject.activeInHierarchy,Is.True);
        var startRect=(RectTransform)manager.StartStroopButton.transform;
        yield return Click(RectTransformUtility.WorldToScreenPoint(Camera.main,startRect.TransformPoint(startRect.rect.center)));
        Assert.That(root.GetComponentInChildren<StroopExperiment>(true).IsRunning,Is.True,"Shared START TASK must work with the normal UI pointer.");
        CheckSharedRig();
    }
    void Capture(string name)
    {
        Directory.CreateDirectory("Builds/Review");var camera=Camera.main;
        var target=RenderTexture.GetTemporary(1280,800,24);var previous=RenderTexture.active;var old=camera.targetTexture;
        camera.targetTexture=target;Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;
        var image=new Texture2D(1280,800,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,800),0,0);image.Apply();
        File.WriteAllBytes("Builds/Review/"+name+".png",image.EncodeToPNG());
        camera.targetTexture=old;RenderTexture.active=previous;Object.Destroy(image);RenderTexture.ReleaseTemporary(target);
    }
}
