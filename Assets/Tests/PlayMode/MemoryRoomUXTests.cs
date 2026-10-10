using System.Collections;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using FXR.Memory;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.LowLevel;

public sealed class MemoryRoomUXTests
{
    [UnityTest]
    public IEnumerator ReviewPreviewsResetAndAllParticipantScreens()
    {
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("MainScene");
        yield return null;
        var room=GameObject.Find("MemoryRoom").GetComponent<MemoryRoomController>();
        var camera=Camera.main;
        Assert.That(camera,Is.Not.Null);
        Assert.That(room.GetComponentInChildren<Canvas>().worldCamera,Is.SameAs(camera));
        Assert.That(EventSystem.current.currentInputModule,Is.TypeOf<InputSystemUIInputModule>());
        Assert.That(room.setupPanel.activeSelf,Is.True);
        CheckText(room); CheckButtonRay(room.actionButton,camera);
        Capture(camera,"office-overview",new Vector3(0,1.75f,-1.7f),new Vector3(0,1.7f,3));
        Capture(camera,"office-interior",new Vector3(-3.25f,1.8f,-.75f),new Vector3(-.1f,1.5f,4.1f));
        ClickButton(room.naturePreviewButton,camera);
        yield return null;
        Assert.That(room.natureProps.activeSelf,Is.True);
        Assert.That(room.Session.Phase,Is.EqualTo(SessionPhase.Idle));
        Capture(camera,"nature-overview",new Vector3(0,1.75f,-1.7f),new Vector3(0,1.7f,3));
        Capture(camera,"nature-interior",new Vector3(-3.25f,1.8f,-.75f),new Vector3(-.1f,1.5f,4.1f));
        room.StartOrContinue();
        yield return null;
        Assert.That(room.officeProps.activeSelf,Is.True,"Start must follow the selected order, even after a nature preview.");
        Assert.That(room.setupPanel.activeSelf,Is.False);
        Assert.That(room.actionButton.gameObject.activeSelf,Is.False);
        var pairs=room.Session.PairIds;
        int mismatch=Enumerable.Range(1,12).First(i=>pairs[i]!=pairs[0]);
        room.SelectCard(0); room.SelectCard(mismatch);
        room.RequestReset();
        float pausedAt=room.Session.RemainingSeconds;
        yield return new WaitForSeconds(.2f);
        Assert.That(room.Session.RemainingSeconds,Is.EqualTo(pausedAt).Within(.001f));
        Assert.That(room.Session.SelectionPending,Is.True);
        Assert.That(room.CanSelectCard(3),Is.False);
        Assert.That(room.HasInterruption,Is.True,"A paused trial must be marked for research review.");
        CheckText(room);
        Capture(camera,"reset-confirmation",new Vector3(0,1.8f,.2f),new Vector3(.2f,1.8f,2.4f));
        room.CancelReset();
        yield return new WaitForSeconds(1f);
        Assert.That(room.cards[0].front.activeSelf,Is.False);
        Assert.That(room.cards[mismatch].front.activeSelf,Is.False);
        Assert.That(room.Session.Mismatches,Is.EqualTo(1));
        Capture(camera,"running-task",new Vector3(0,1.8f,.2f),new Vector3(.2f,1.8f,2.4f));
        room.ResetRoom();
        Assert.That(room.HasInterruption,Is.False);
        room.StartOrContinue();
        string folder=Path.Combine(Application.persistentDataPath,"MemoryResults");
        var existing=Directory.Exists(folder)?Directory.GetFiles(folder):new string[0];
        for(int round=0;round<2;round++)
        {
            for(int pair=0;pair<6;pair++)
            {
                var cards=Enumerable.Range(0,12).Where(i=>room.Session.PairIds[i]==pair).ToArray();
                room.SelectCard(cards[0]);room.SelectCard(cards[1]);yield return new WaitForSeconds(.55f);
            }
            CheckText(room);
            var rating=room.ratingPanel.GetComponentsInChildren<Button>()[2]; CheckButtonRay(rating,camera);
            Capture(camera,round==0?"effort-rating":"nature-rating",new Vector3(0,1.8f,.2f),new Vector3(.2f,1.8f,2.4f));
            room.RateEffort(0);
            Assert.That(room.Session.Phase,Is.EqualTo(SessionPhase.Rating));
            rating.onClick.Invoke();
            yield return null;
            if(round==0)
            {
                Assert.That(room.Session.Phase,Is.EqualTo(SessionPhase.ReadyForNextRound));
                CheckText(room); Capture(camera,"between-rounds",new Vector3(0,1.8f,.2f),new Vector3(.2f,1.8f,2.4f));
                room.StartOrContinue();
                room.RequestReset();yield return null;room.CancelReset();
            }
        }
        Assert.That(room.Session.Phase,Is.EqualTo(SessionPhase.Completed));
        Assert.That(room.resultsPanel.activeSelf,Is.True);
        yield return new WaitForSeconds(1.2f);
        Assert.That(room.cards.All(card=>card.transform.position.y<1.5f),Is.True,"Matched cards must settle below the board rather than spin in front of the participant.");
        var exported=Directory.GetFiles(folder).Except(existing).Single();
        Assert.That(File.ReadAllLines(exported)[2].EndsWith(",1"),Is.True,"Record an interrupted second round in CSV.");
        CheckText(room); CheckButtonRay(room.actionButton,camera);
        Capture(camera,"final-results",new Vector3(0,1.8f,.2f),new Vector3(.2f,1.8f,2.4f));
        foreach(string file in Directory.GetFiles(folder).Except(existing))File.Delete(file);
        room.ResetRoom();
        room.ToggleOrder(); room.StartOrContinue();
        Assert.That(room.natureProps.activeSelf,Is.True,"Nature-first order must start with nature.");
        // A timeout must dismiss the pending card interaction and still require a rating.
        room.SelectCard(0);room.Session.Tick(179.9f);yield return new WaitForSeconds(.3f);
        Assert.That(room.Session.Phase,Is.EqualTo(SessionPhase.Rating));
        Assert.That(room.ratingPanel.activeSelf,Is.True);
        Assert.That(room.CanSelectCard(1),Is.False);
        CheckText(room);
    }

    static void ClickButton(Button button,Camera camera)
    {
        // Headless PlayMode has no focused Game view. Route this synthetic device to
        // player input, then restore both settings even if an assertion fails.
        var background=InputSystem.settings.backgroundBehavior;
#if UNITY_EDITOR
        var editorRouting=InputSystem.settings.editorInputBehaviorInPlayMode;
        InputSystem.settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
#endif
        InputSystem.settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;
        var reviewMouse=InputSystem.AddDevice<Mouse>();
        try
        {
            var module=(InputSystemUIInputModule)EventSystem.current.currentInputModule;
            var target=(Vector2)camera.WorldToScreenPoint(button.transform.position);
            InputSystem.QueueStateEvent(reviewMouse,new MouseState{position=target});
            InputSystem.Update();module.Process();
            Assert.That(module.point.action.enabled,Is.True);
            Assert.That(module.leftClick.action.enabled,Is.True);
            Assert.That(module.GetLastRaycastResult(reviewMouse.deviceId).gameObject,Is.SameAs(button.gameObject));
            InputSystem.QueueStateEvent(reviewMouse,new MouseState{position=target}.WithButton(MouseButton.Left));
            InputSystem.Update();module.Process();
            InputSystem.QueueStateEvent(reviewMouse,new MouseState{position=target});
            InputSystem.Update();module.Process();
        }
        finally
        {
            InputSystem.RemoveDevice(reviewMouse);
            InputSystem.settings.backgroundBehavior=background;
#if UNITY_EDITOR
            InputSystem.settings.editorInputBehaviorInPlayMode=editorRouting;
#endif
        }
    }

    static void CheckText(MemoryRoomController room)
    {
        Canvas.ForceUpdateCanvases();
        foreach(var text in room.GetComponentsInChildren<TextMeshProUGUI>(false))
        {
            text.ForceMeshUpdate();
            Assert.That(text.isTextOverflowing,Is.False,"Text overflows its readable area: "+text.name+" / "+text.text);
        }
    }

    static void CheckButtonRay(Button button,Camera camera)
    {
        Canvas.ForceUpdateCanvases();
        var data=new PointerEventData(EventSystem.current){position=camera.WorldToScreenPoint(button.transform.position)};
        var hits=new List<RaycastResult>();EventSystem.current.RaycastAll(data,hits);
        Assert.That(hits.Count,Is.GreaterThan(0));
        Assert.That(hits[0].gameObject,Is.SameAs(button.gameObject),"Button must receive the first desktop UI ray hit: "+button.name);
    }

    static void Capture(Camera camera,string name,Vector3 position,Vector3 target)
    {
        Directory.CreateDirectory("Builds/Review");
        var oldPosition=camera.transform.position;var oldRotation=camera.transform.rotation;float oldFov=camera.fieldOfView;
        var rt=RenderTexture.GetTemporary(1920,1200,24,RenderTextureFormat.ARGB32);
        camera.transform.position=position;camera.transform.LookAt(target);camera.fieldOfView=64;camera.targetTexture=rt;
#if UNITY_EDITOR
        // Imported shader variants can still be compiling on the first capture.
        // Wait synchronously for this render; do not save a white placeholder frame.
        bool asyncCompilation=UnityEditor.ShaderUtil.allowAsyncCompilation;
        UnityEditor.ShaderUtil.allowAsyncCompilation=false;
        try{camera.Render();}
        finally{UnityEditor.ShaderUtil.allowAsyncCompilation=asyncCompilation;}
#else
        camera.Render();
#endif
        var previous=RenderTexture.active;RenderTexture.active=rt;
        var texture=new Texture2D(1920,1200,TextureFormat.RGB24,false);texture.ReadPixels(new Rect(0,0,1920,1200),0,0);texture.Apply();
        File.WriteAllBytes("Builds/Review/"+name+".png",texture.EncodeToPNG());
        RenderTexture.active=previous;camera.targetTexture=null;RenderTexture.ReleaseTemporary(rt);Object.Destroy(texture);
        camera.transform.position=oldPosition;camera.transform.rotation=oldRotation;camera.fieldOfView=oldFov;
    }
}
