using System;
using System.IO;
using FXR.Memory;
using FXR.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace FXR.Integration
{
    [Serializable]
    public sealed class RoomView
    {
        public Vector3 position, cameraPosition, center;
        public Quaternion rotation=Quaternion.identity, cameraRotation=Quaternion.identity;
        public float fieldOfView=60, height=2, radius=.25f;
        public Material skybox;
        public AmbientMode ambientMode;
        public Color ambientLight, ambientSky, ambientEquator, ambientGround, background, fogColor;
        public float ambientIntensity=1, reflectionIntensity=1, fogDensity;
        public bool fog;
        public CameraClearFlags clearFlags;
        public void Apply(Camera camera)
        {
            RenderSettings.skybox=skybox; RenderSettings.ambientMode=ambientMode;
            RenderSettings.ambientLight=ambientLight; RenderSettings.ambientSkyColor=ambientSky;
            RenderSettings.ambientEquatorColor=ambientEquator;RenderSettings.ambientGroundColor=ambientGround;
            RenderSettings.ambientIntensity=ambientIntensity; RenderSettings.reflectionIntensity=reflectionIntensity;
            RenderSettings.fog=fog;RenderSettings.fogColor=fogColor;RenderSettings.fogDensity=fogDensity;
            camera.clearFlags=clearFlags;camera.backgroundColor=background;
        }
    }

    [Serializable]
    public sealed class RoomSlot
    {
        public string title;
        public GameObject prefab, instance;
        public Vector3 offset;
        public RoomView view;
    }

    [DefaultExecutionOrder(-100)]
    public sealed class RoomIntegrationManager : MonoBehaviour
    {
        public RoomSlot[] rooms;
        public DesktopExplorer explorer;
        public PlayerController museumMovement;
        public Light sharedDaylight;
        public TMP_FontAsset font;
        public int CurrentRoom { get; private set; }
        public bool MenuOpen => modal != null && modal.activeSelf;
        public GameObject CurrentInstance => rooms[CurrentRoom].instance;
        public Button[] RoomButtons {get; private set;}
        public Button ConfirmButton {get; private set;}
        public Button CancelButton {get; private set;}
        public Button ResetButton {get; private set;}
        public Button StartStroopButton {get; private set;}
        Canvas canvas;
        Transform navigationBar;
        GameObject modal, detailsPanel;
        TMP_Text modalText, detailsText, title;
        int pending=-2;
        bool previousExplorer, previousMuseum, introducedMemoryPause;
        float previousScale=1, openedAt;
        bool previousAudioPause, pausedByMenu;
        CursorLockMode previousCursor;
        MemoryRoomController pausedMemory;
        GameObject previousSelection;

        void Awake()
        {
            // Imported views can sit behind the shared capsule. It must collide
            // with the room without intercepting the user's interaction rays.
            explorer.gameObject.layer=LayerMask.NameToLayer("Ignore Raycast");
            CurrentRoom=0;
            for(int i=1;i<rooms.Length;i++) if(rooms[i].instance!=null)rooms[i].instance.SetActive(false);
            museumMovement.enabled=false;
        }
        void Start() { BuildUI(); }

        void Update()
        {
            var keyboard=Keyboard.current;
            if(keyboard!=null && keyboard.tabKey.wasPressedThisFrame)
            { if(MenuOpen)CancelNavigation(); else OpenMenu(); }
            if(MenuOpen && keyboard!=null && keyboard.escapeKey.wasPressedThisFrame)CancelNavigation();
        }

        void LateUpdate()
        {
            if(canvas==null)return;
            title.text="FXR / "+rooms[CurrentRoom].title;
            for(int i=0;i<RoomButtons.Length;i++)RoomButtons[i].interactable=i!=CurrentRoom;
            detailsPanel.SetActive(CurrentRoom==2 && !MenuOpen);
            if(CurrentRoom==2)
            {
                var stroop=CurrentInstance.GetComponentInChildren<StroopExperiment>(true);
                var consent=CurrentInstance.GetComponentInChildren<ConsentPaperInteraction>(true);
                var guideRect=detailsPanel.GetComponent<RectTransform>();
                guideRect.anchorMin=guideRect.anchorMax=stroop.HasStarted?new Vector2(.5f,0):Vector2.zero;
                guideRect.anchoredPosition=stroop.HasStarted?new Vector2(0,90):new Vector2(580,100);
                guideRect.sizeDelta=stroop.HasStarted?new Vector2(1100,110):new Vector2(1100,150);
                detailsText.rectTransform.anchoredPosition=stroop.HasStarted?Vector2.zero:new Vector2(-100,0);
                detailsText.rectTransform.sizeDelta=stroop.HasStarted?new Vector2(1050,100):new Vector2(850,132);
                string state=stroop.Completed?$"Complete · B1: {stroop.Block1Accuracy:F0}% · B2: {stroop.Block2Accuracy:F0}%"
                    : stroop.InTransition?"Part 1 complete. Get ready for phone notifications in Part 2."
                    : stroop.IsRunning?$"Part {stroop.BlockNumber}/2 · {Mathf.CeilToInt(stroop.RemainingSeconds)}s · choose the INK colour."
                    : consent.HasConsent ? consent.PanelOpen ? "Consent accepted. Wait for the paper to close." : "Ready. Click START TASK or the desk Start button."
                    : consent.Answered ? "Consent declined. Use RESTART if you want to begin a new session."
                    : "Click the consent paper, accept, then click START on the desk.";
                detailsText.text="STROOP / "+state+"\n"+(stroop.HasStarted&&!stroop.Completed?"Click the desk colour buttons. Movement is locked during the task.":"WASD/arrows: move · right drag: look · click: interact")+"\nTAB: rooms and restart"+(stroop.Interrupted?" · Interrupted session: repeat before comparison.":" · lamp lever: light on/off");
                StartStroopButton.gameObject.SetActive(consent.HasConsent && !consent.PanelOpen && !stroop.HasStarted);
            }
        }

        public void OpenMenu()
        {
            PauseForMenu(); pending=-2;
            modalText.text="ROOM MENU\n\nChoose a room from the bar above, or restart this room.\nSwitching or restarting clears the current room's progress.\nPreviously saved CSV files remain on this computer.";
            ConfirmButton.gameObject.SetActive(false);
        }
        public void RequestSwitch(int index)
        {
            if(index<0 || index>=rooms.Length)return;
            PauseForMenu();pending=index;
            modalText.text=(index==CurrentRoom?"Restart ":"Switch to ")+rooms[index].title+"?\n\nCurrent room progress will be cleared.\nSaved CSV files will be kept.";
            ConfirmButton.gameObject.SetActive(true);
        }
        void PauseForMenu()
        {
            if(MenuOpen)return;
            previousExplorer=explorer.enabled;previousMuseum=museumMovement.enabled;
            previousCursor=Cursor.lockState;previousScale=Time.timeScale;previousAudioPause=AudioListener.pause;
            previousSelection=EventSystem.current!=null?EventSystem.current.currentSelectedGameObject:null;
            if(EventSystem.current!=null)EventSystem.current.SetSelectedGameObject(null);
            introducedMemoryPause=false;pausedMemory=CurrentInstance.GetComponent<MemoryRoomController>();
            if(pausedMemory!=null && pausedMemory.Session.Phase==SessionPhase.Running && !pausedMemory.IsPaused && !pausedMemory.ResetPending)
            {pausedMemory.TogglePause();introducedMemoryPause=true;}
            if(CurrentRoom==1)
            {
                var experiment=CurrentInstance.GetComponentInChildren<ExperimentManager>(true);
                var csv=CurrentInstance.GetComponentInChildren<QuizCSVExporter>(true);
                if(experiment.experimentStarted && csv!=null)csv.Interrupted=true;
            }
            if(CurrentRoom==2)CurrentInstance.GetComponentInChildren<StroopExperiment>(true).MarkInterrupted();
            openedAt=Time.realtimeSinceStartup;
            pausedByMenu=true;
            explorer.enabled=false;museumMovement.enabled=false;
            Time.timeScale=0;AudioListener.pause=true;Cursor.lockState=CursorLockMode.None;Cursor.visible=true;
            modal.SetActive(true);
            navigationBar.SetAsLastSibling();
        }
        public void CancelNavigation()
        {
            if(!MenuOpen)return;
            if(CurrentRoom==1)
            {
                var quiz=CurrentInstance.GetComponentInChildren<MemoryQuizManager>(true);
                if(quiz!=null && quiz.gameObject.activeInHierarchy)quiz.CompensatePause(Time.realtimeSinceStartup-openedAt);
            }
            modal.SetActive(false);Time.timeScale=previousScale;AudioListener.pause=previousAudioPause;
            pausedByMenu=false;
            explorer.enabled=previousExplorer;museumMovement.enabled=previousMuseum;
            Cursor.lockState=previousCursor;Cursor.visible=previousCursor!=CursorLockMode.Locked;
            if(introducedMemoryPause && pausedMemory!=null)pausedMemory.TogglePause();
            introducedMemoryPause=false;pausedMemory=null;pending=-2;
            if(EventSystem.current!=null && previousSelection!=null && previousSelection.activeInHierarchy)EventSystem.current.SetSelectedGameObject(previousSelection);
            previousSelection=null;
        }
        public void ConfirmNavigation()
        {
            if(!MenuOpen || pending<0)return;
            int next=pending;
            CancelNavigation();
            ActivateRoom(next);
        }

        void ActivateRoom(int index)
        {
            explorer.BindMemoryRoom(null);explorer.enabled=false;museumMovement.enabled=false;
            var old=rooms[CurrentRoom].instance;
            if(old!=null){old.SetActive(false);Destroy(old);rooms[CurrentRoom].instance=null;}
            CurrentRoom=index;
            var slot=rooms[index];
            if(slot.instance==null)
            {
                var staging=new GameObject("Inactive room staging");staging.SetActive(false);
                slot.instance=Instantiate(slot.prefab,staging.transform);
                slot.instance.name=slot.title;slot.instance.SetActive(false);
                slot.instance.transform.SetParent(null,false);slot.instance.transform.position=slot.offset;
                Destroy(staging);
            }
            var body=explorer.GetComponent<CharacterController>();body.enabled=false;
            body.height=slot.view.height;body.radius=slot.view.radius;body.center=slot.view.center;
            explorer.ConfigureView(slot.offset+slot.view.position,slot.view.rotation,slot.view.cameraPosition,slot.view.cameraRotation,slot.view.fieldOfView);
            slot.view.Apply(explorer.view);
            sharedDaylight.enabled=index==0;
            var bindings=slot.instance.GetComponent<IntegratedRoom>();
            if(bindings!=null)bindings.Bind(explorer.transform,explorer.view,museumMovement);
            museumMovement.ConfigureForRoom();
            slot.instance.SetActive(true);
            if(index==2)bindings.FocusStroopEntry(explorer);
            if(index==0)explorer.BindMemoryRoom(slot.instance.GetComponent<MemoryRoomController>());
            explorer.enabled=index!=1;museumMovement.enabled=index==1;
            Cursor.lockState=CursorLockMode.None;Cursor.visible=true;
        }

        void BuildUI()
        {
            var go=new GameObject("Shared room navigation",typeof(RectTransform),typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));
            go.transform.SetParent(transform,false);canvas=go.GetComponent<Canvas>();canvas.renderMode=RenderMode.ScreenSpaceCamera;canvas.worldCamera=explorer.view;canvas.planeDistance=.2f;canvas.sortingOrder=2000;
            var scaler=go.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1600,1000);scaler.matchWidthOrHeight=1;
            var bar=Panel("Room navigation",go.transform,new Vector2(0,-35),new Vector2(1150,60),new Color(.025f,.07f,.09f,.96f));
            navigationBar=bar.transform;
            var br=bar.GetComponent<RectTransform>();br.anchorMin=br.anchorMax=new Vector2(.5f,1);
            title=Label(bar.transform,"FXR",new Vector2(-410,0),new Vector2(290,48),20);
            RoomButtons=new Button[3];
            for(int i=0;i<3;i++){int choice=i;RoomButtons[i]=MakeButton(bar.transform,rooms[i].title,new Vector2(-150+i*175,0),new Vector2(165,42),()=>RequestSwitch(choice));}
            ResetButton=MakeButton(bar.transform,"RESTART",new Vector2(360,0),new Vector2(140,42),()=>RequestSwitch(CurrentRoom));
            MakeButton(bar.transform,"TAB / ROOMS",new Vector2(505,0),new Vector2(138,42),OpenMenu);
            modal=Panel("Confirm room navigation",go.transform,Vector2.zero,Vector2.zero,new Color(.025f,.07f,.09f,.85f));
            var mr=modal.GetComponent<RectTransform>();mr.anchorMin=Vector2.zero;mr.anchorMax=Vector2.one;mr.offsetMin=mr.offsetMax=Vector2.zero;
            Panel("Navigation dialog",modal.transform,Vector2.zero,new Vector2(850,450),new Color(.025f,.07f,.09f,1));
            modalText=Label(modal.transform,"",new Vector2(0,45),new Vector2(770,270),28);
            ConfirmButton=MakeButton(modal.transform,"CONFIRM",new Vector2(180,-155),new Vector2(300,64),ConfirmNavigation);
            CancelButton=MakeButton(modal.transform,"BACK / ESC",new Vector2(-180,-155),new Vector2(300,64),CancelNavigation);
            modal.SetActive(false);
            detailsPanel=Panel("Stroop guide",go.transform,new Vector2(580,100),new Vector2(1100,150),new Color(.025f,.07f,.09f,.95f));
            var dr=detailsPanel.GetComponent<RectTransform>();dr.anchorMin=dr.anchorMax=Vector2.zero;
            detailsPanel.GetComponent<Image>().raycastTarget=false;
            detailsText=Label(detailsPanel.transform,"",new Vector2(-100,0),new Vector2(850,132),23);
            StartStroopButton=MakeButton(detailsPanel.transform,"START TASK",new Vector2(460,0),new Vector2(150,66),()=>
            {
                if(CurrentRoom!=2 || MenuOpen)return;
                var start=Array.Find(CurrentInstance.GetComponentsInChildren<PhysicalButton>(true),b=>b.name=="StartButton");
                if(start!=null)start.Press();
            });
            StartStroopButton.gameObject.SetActive(false);
            detailsPanel.SetActive(false);
        }
        GameObject Panel(string name,Transform parent,Vector2 pos,Vector2 size,Color color)
        {
            var go=new GameObject(name,typeof(RectTransform),typeof(Image));go.transform.SetParent(parent,false);
            var rect=go.GetComponent<RectTransform>();rect.sizeDelta=size;rect.anchoredPosition=pos;go.GetComponent<Image>().color=color;return go;
        }
        TMP_Text Label(Transform parent,string value,Vector2 pos,Vector2 size,float fontSize)
        {
            var go=new GameObject("Label",typeof(RectTransform),typeof(TextMeshProUGUI));go.transform.SetParent(parent,false);
            var r=go.GetComponent<RectTransform>();r.sizeDelta=size;r.anchoredPosition=pos;
            var t=go.GetComponent<TextMeshProUGUI>();t.font=font;t.text=value;t.fontSize=fontSize;t.color=new Color(.9f,.96f,.94f);t.alignment=TextAlignmentOptions.MidlineLeft;t.raycastTarget=false;return t;
        }
        Button MakeButton(Transform parent,string value,Vector2 pos,Vector2 size,UnityEngine.Events.UnityAction action)
        {
            var go=Panel(value,parent,pos,size,new Color(.13f,.38f,.33f));var button=go.AddComponent<Button>();
            button.targetGraphic=go.GetComponent<Image>();var n=button.navigation;n.mode=Navigation.Mode.None;button.navigation=n;
            var text=Label(go.transform,value,Vector2.zero,size-new Vector2(10,4),20);text.alignment=TextAlignmentOptions.Center;
            button.onClick.AddListener(action);return button;
        }
        void OnDestroy(){ if(pausedByMenu){Time.timeScale=previousScale;AudioListener.pause=previousAudioPause;}Cursor.lockState=CursorLockMode.None;Cursor.visible=true; }
    }
}
