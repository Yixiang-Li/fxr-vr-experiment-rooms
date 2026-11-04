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
    Keyboard keyboard;
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
        mouse=InputSystem.AddDevice<Mouse>();keyboard=InputSystem.AddDevice<Keyboard>();
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
        InputSystem.RemoveDevice(mouse);InputSystem.RemoveDevice(keyboard);InputSystem.settings.backgroundBehavior=oldBackground;
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
        var paperViewport=Camera.main.WorldToViewportPoint(paper.GetComponent<Collider>().bounds.center);
        Assert.That(paperViewport.x,Is.InRange(.05f,.95f));
        Assert.That(paperViewport.y,Is.InRange(.22f,.9f),"The consent paper must be visible above the guide on entry.");
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

    static void SetFixture(object target,string field,object value)
    {
        target.GetType().GetField(field,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).SetValue(target,value);
    }
    static T ReadField<T>(object target,string field) => (T)target.GetType().GetField(field,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(target);
    IEnumerator KeyPress(Key key)
    {
        InputSystem.QueueStateEvent(keyboard,new KeyboardState(key));yield return null;
        InputSystem.QueueStateEvent(keyboard,new KeyboardState());yield return null;
    }
    IEnumerator ClickButton(UnityEngine.UI.Button button)
    {
        Canvas.ForceUpdateCanvases();var rect=(RectTransform)button.transform;
        yield return Click(RectTransformUtility.WorldToScreenPoint(Camera.main,rect.TransformPoint(rect.rect.center)));
    }

    [UnityTest]
    public IEnumerator MuseumEntryStaysGroundedAndVisibleAfterMemoryTask()
    {
        manager.CurrentInstance.GetComponent<MemoryRoomController>().StartOrContinue();yield return null;
        yield return Switch(1);
        var root=manager.CurrentInstance;
        root.GetComponentInChildren<WelcomeScreen>(true).StartExperiment();
        var floor=root.GetComponentsInChildren<Collider>().Single(c=>c.name=="Floor");
        Debug.Log($"Museum entry: player {manager.explorer.transform.position}, camera {Camera.main.transform.position}, floor {floor.bounds}, enabled {floor.enabled}");
        yield return new WaitForSeconds(2f);
        var camera=Camera.main;var planes=GeometryUtility.CalculateFrustumPlanes(camera);
        int visible=root.GetComponentsInChildren<Renderer>().Count(r=>r.enabled && (camera.cullingMask & (1<<r.gameObject.layer))!=0 && GeometryUtility.TestPlanesAABB(planes,r.bounds));
        Assert.That(manager.explorer.transform.position.y,Is.GreaterThan(.5f),$"Museum player fell below the floor: {manager.explorer.transform.position}; camera {camera.transform.position}; floor {floor.bounds}");
        Assert.That(visible,Is.GreaterThan(2),$"Museum must remain in view after entry; player {manager.explorer.transform.position}; camera {camera.transform.position}");
        Capture("integration-museum-entered");
    }

    [UnityTest]
    public IEnumerator MuseumNewInputAndNavigationCannotCaptureBackgroundPhotos()
    {
        yield return Switch(1);
        var root=manager.CurrentInstance;
        root.GetComponentInChildren<WelcomeScreen>(true).StartExperiment();yield return null;
        var experiment=root.GetComponentInChildren<ExperimentManager>(true);
        var exhibit=experiment.exhibits[0];exhibit.condition=ExhibitInteraction.TrialCondition.Photo;
        var point=new GameObject("Museum input fixture point");
        point.transform.position=exhibit.transform.position+Vector3.back*.8f;
        point.transform.position=new Vector3(point.transform.position.x,manager.explorer.transform.position.y,point.transform.position.z);
        manager.museumMovement.TeleportTo(point.transform);Object.Destroy(point);
        yield return KeyPress(Key.E);
        Assert.That(ExhibitInteraction.activeExhibit,Is.EqualTo(exhibit),"The new E input must start observation.");
        yield return KeyPress(Key.P);
        Assert.That(ReadField<bool>(exhibit,"cameraRaised"),Is.True);
        yield return KeyPress(Key.Escape);
        Assert.That(Cursor.lockState,Is.EqualTo(CursorLockMode.None));
        yield return ClickButton(manager.RoomButtons[2]);
        Assert.That(manager.MenuOpen,Is.True,"Clicking the top bar must not recapture the cursor.");
        Assert.That(Cursor.lockState,Is.EqualTo(CursorLockMode.None));
        Assert.That(ReadField<bool>(exhibit,"photoTaken"),Is.False,"A navigation click must not take a photo.");
        var camera=Camera.main;camera.transform.position=exhibit.photoTarget.position-Vector3.forward*2;camera.transform.LookAt(exhibit.photoTarget);
        yield return Click(new Vector2(5,5));
        Assert.That(ReadField<bool>(exhibit,"photoTaken"),Is.False,"A menu click must not change the paused observation.");
        manager.CancelButton.onClick.Invoke();yield return null;
        Assert.That(manager.CurrentRoom,Is.EqualTo(1));
        manager.museumMovement.enabled=false;
        camera.transform.position=exhibit.photoTarget.position-Vector3.forward*2;camera.transform.LookAt(exhibit.photoTarget);
        yield return Click(new Vector2(Screen.width*.5f,Screen.height*.5f));
        Assert.That(ReadField<bool>(exhibit,"photoTaken"),Is.True,"A valid world click must still capture after cancelling the menu.");
        CheckSharedRig();
    }

    [UnityTest]
    public IEnumerator StroopTwoTimedBlocksNotificationsPauseResultsAndResetWork()
    {
        yield return Switch(2);
        var root=manager.CurrentInstance;
        var stroop=root.GetComponentInChildren<StroopExperiment>(true);
        var consent=root.GetComponentInChildren<ConsentPaperInteraction>(true);
        SetFixture(stroop,"blockDuration",1.1f);SetFixture(stroop,"transitionDuration",.15f);
        SetFixture(stroop,"notificationInterval",.1f);SetFixture(stroop,"notificationFlashDuration",.05f);
        SetFixture(consent,"closeDelay",.02f);
        var start=root.GetComponentsInChildren<PhysicalButton>(true).Single(b=>b.name=="StartButton");
        start.Press();Assert.That(stroop.HasStarted,Is.False);
        yield return Click(Camera.main.WorldToScreenPoint(consent.GetComponent<Collider>().bounds.center));
        Assert.That(consent.PanelOpen,Is.True,"A real pointer click must lift/open the consent paper.");
        consent.AcceptConsent();yield return new WaitForSeconds(.05f);
        Assert.That(consent.HasConsent,Is.True);Assert.That(consent.PanelOpen,Is.False);
        var camera=Camera.main;var localPosition=camera.transform.localPosition;var localRotation=camera.transform.localRotation;
        var startPoint=start.GetComponent<Collider>().bounds.center;
        camera.transform.position=startPoint+new Vector3(0,.3f,-.75f);camera.transform.LookAt(startPoint);
        yield return Click(camera.WorldToScreenPoint(startPoint));
        Assert.That(stroop.IsRunning,Is.True,"Physical Start must activate Part 1.");
        Assert.That(stroop.TotalResponses,Is.Zero,"Starting must not also answer a trial after the camera moves.");
        Assert.That(Vector3.Distance(camera.transform.localPosition,localPosition),Is.LessThan(.001f));
        foreach(var button in root.GetComponentsInChildren<PhysicalButton>())
            if(button!=start)
            {
                var screen=camera.WorldToViewportPoint(button.GetComponent<Collider>().bounds.center);
                Assert.That(screen.z,Is.GreaterThan(0));
                Assert.That(screen.x,Is.InRange(.03f,.97f));
                Assert.That(screen.y,Is.InRange(.03f,.97f),"All response buttons must be visible at the locked task view.");
            }
        Assert.That(manager.explorer.enabled,Is.False,"The shared player must be locked for both timed blocks.");
        var word=root.GetComponentsInChildren<TMP_Text>(true).Single(t=>t.name=="StroopWord");
        var ink=word.color;
        string inkName=ink.r>ink.g && ink.r>ink.b?"RED":ink.g>ink.b?"GREEN":"BLUE";
        Assert.That(word.text,Is.Not.EqualTo(inkName),"The latest source specifies incongruent trials.");
        if(inkName=="RED")stroop.SubmitRed();else if(inkName=="GREEN")stroop.SubmitGreen();else stroop.SubmitBlue();
        Assert.That(stroop.Block1Responses,Is.EqualTo(1));
        Assert.That(stroop.NotificationCount,Is.Zero);
        Capture("integration-stroop-trial");
        manager.OpenMenu();float remaining=stroop.RemainingSeconds;
        yield return new WaitForSecondsRealtime(.08f);
        Assert.That(stroop.RemainingSeconds,Is.EqualTo(remaining));
        Assert.That(stroop.Interrupted,Is.True);
        manager.CancelButton.onClick.Invoke();yield return null;
        Assert.That(manager.explorer.enabled,Is.False);
        float deadline=Time.realtimeSinceStartup+4;
        while(!stroop.InTransition && Time.realtimeSinceStartup<deadline)yield return null;
        Assert.That(stroop.InTransition,Is.True);
        yield return null;Assert.That(manager.StartStroopButton.gameObject.activeSelf,Is.False);
        while(stroop.BlockNumber!=2 && Time.realtimeSinceStartup<deadline)yield return null;
        Assert.That(stroop.BlockNumber,Is.EqualTo(2));
        ink=word.color;
        if(ink.r>ink.g && ink.r>ink.b)stroop.SubmitRed();else if(ink.g>ink.b)stroop.SubmitGreen();else stroop.SubmitBlue();
        Assert.That(stroop.Block2Responses,Is.EqualTo(1));
        while(!stroop.Completed && Time.realtimeSinceStartup<deadline)yield return null;
        Assert.That(stroop.Completed,Is.True);Assert.That(stroop.CorrectAnswers,Is.EqualTo(2));
        Assert.That(stroop.NotificationCount,Is.GreaterThan(0));
        Assert.That(stroop.Block1Accuracy,Is.EqualTo(100));Assert.That(stroop.Block2Accuracy,Is.EqualTo(100));
        Assert.That(manager.explorer.enabled,Is.True);
        Assert.That(root.GetComponentsInChildren<TMP_Text>(true).Any(t=>t.text.Contains("EXPERIMENT COMPLETE")),Is.True);
        yield return null;yield return null;
        Capture("integration-stroop-results");
        manager.ResetButton.onClick.Invoke();manager.ConfirmButton.onClick.Invoke();yield return null;yield return null;
        root=manager.CurrentInstance;stroop=root.GetComponentInChildren<StroopExperiment>(true);consent=root.GetComponentInChildren<ConsentPaperInteraction>(true);
        Assert.That(consent.HasConsent,Is.False);Assert.That(stroop.HasStarted,Is.False);
        Assert.That(stroop.TotalResponses,Is.Zero);Assert.That(stroop.NotificationCount,Is.Zero);
        SetFixture(consent,"closeDelay",.02f);consent.AcceptConsent();yield return new WaitForSeconds(.05f);yield return null;
        Assert.That(manager.StartStroopButton.gameObject.activeInHierarchy,Is.True);
        yield return ClickButton(manager.StartStroopButton);
        Assert.That(stroop.IsRunning,Is.True,"Shared START TASK must preserve the latest positioning/audio sequence.");
        root.GetComponentInChildren<ResetButton>(true).Press();yield return null;
        Assert.That(stroop.HasStarted,Is.False);Assert.That(consent.HasConsent,Is.True);
        Assert.That(manager.explorer.enabled,Is.True);
        CheckSharedRig();
    }
    void Capture(string name)
    {
        Directory.CreateDirectory("Builds/Review");var camera=Camera.main;
        Canvas.ForceUpdateCanvases();
        foreach(var display in Object.FindObjectsByType<Camera>(FindObjectsSortMode.None))
            if(display.targetTexture!=null)display.Render();
        var target=RenderTexture.GetTemporary(1280,800,24);var previous=RenderTexture.active;var old=camera.targetTexture;
        camera.targetTexture=target;Canvas.ForceUpdateCanvases();camera.Render();RenderTexture.active=target;
        var image=new Texture2D(1280,800,TextureFormat.RGB24,false);image.ReadPixels(new Rect(0,0,1280,800),0,0);image.Apply();
        File.WriteAllBytes("Builds/Review/"+name+".png",image.EncodeToPNG());
        camera.targetTexture=old;RenderTexture.active=previous;Object.Destroy(image);RenderTexture.ReleaseTemporary(target);
    }
}
