
using UnityEngine;

public class ExhibitInteraction : MonoBehaviour
{
    public enum TrialCondition
    {
        Photo,
        Observe
    }

    public static ExhibitInteraction activeExhibit;

    [Header("Experiment Settings")]
    public TrialCondition condition = TrialCondition.Photo;
    public float observationDuration = 20f;

    [Header("Interaction Settings")]
    public Transform player;
    public float interactionDistance = 1.5f;

    [Header("Player Controller")]
    public PlayerController playerController;

    [Header("Experiment Manager")]
    public ExperimentManager experimentManager;

    [Header("Camera Settings")]
    public Camera playerCamera;
    public Transform photoTarget;
    [Range(1f, 30f)]
    public float framingTolerance = 12f;

    private bool isObserving = false;
    private bool hasFinished = false;
    private bool photoTaken = false;
    private bool cameraRaised = false;

    private float flashTimer = 0f;
    private float flashDuration = 0.15f;
    private float remainingTime = 0f;
    private float completionMessageTimer = 0f;

    private string completionMessage = "";

    float GetHorizontalDistance()
    {
        return Vector2.Distance(
            new Vector2(player.position.x, player.position.z),
            new Vector2(transform.position.x, transform.position.z)
        );
    }

    void Update()
    {
        if (player == null) return;

        if (activeExhibit != null && activeExhibit != this)
            return;

        float distance = GetHorizontalDistance();

        if (!isObserving &&
            !hasFinished &&
            activeExhibit == null &&
            experimentManager != null &&
            experimentManager.CanInteract(this) &&
            distance <= interactionDistance &&
            Input.GetKeyDown(KeyCode.E))
        {
            StartObservation();
        }

        if (isObserving)
        {
            if (condition == TrialCondition.Photo && !photoTaken)
            {
                if (Input.GetKeyDown(KeyCode.P))
                {
                    cameraRaised = !cameraRaised;

                    Debug.Log(
                        cameraRaised
                            ? "Camera raised."
                            : "Camera lowered."
                    );
                }

                if (cameraRaised &&
                    Input.GetMouseButtonDown(0))
                {
                    TryTakePhoto();
                }
            }

            remainingTime = Mathf.Max(
                0f,
                remainingTime - Time.deltaTime
            );

            if (remainingTime <= 0f)
            {
                if (condition == TrialCondition.Observe ||
                    photoTaken)
                {
                    FinishObservation();
                }
            }
        }

        if (completionMessageTimer > 0f)
            completionMessageTimer -= Time.deltaTime;

        if (flashTimer > 0f)
            flashTimer -= Time.deltaTime;
    }

    void StartObservation()
    {
        if (activeExhibit != null) return;

        if (playerController == null)
        {
            Debug.LogWarning(
                "PlayerController is not assigned!"
            );
            return;
        }

        activeExhibit = this;
        isObserving = true;
        photoTaken = false;
        cameraRaised = false;

        remainingTime = experimentManager != null
            ? experimentManager.GetObservationDuration(observationDuration)
            : observationDuration;

        playerController.BeginObservation();

        if (experimentManager != null)
        {
            experimentManager.OnExhibitObservationStarted();
        }

        Debug.Log("Trial started: " + condition);
    }

    bool IsExhibitFramed()
    {
        if (playerCamera == null || photoTarget == null)
            return false;

        Vector3 direction =
            photoTarget.position -
            playerCamera.transform.position;

        float angle = Vector3.Angle(
            playerCamera.transform.forward,
            direction
        );

        return angle <= framingTolerance;
    }

    void TryTakePhoto()
    {
        if (!isObserving ||
            condition != TrialCondition.Photo ||
            photoTaken ||
            !cameraRaised)
        {
            return;
        }

        if (!IsExhibitFramed())
        {
            Debug.Log(
                "Photo failed: Exhibit is not centered."
            );
            return;
        }

        photoTaken = true;
        cameraRaised = false;
        flashTimer = flashDuration;

        Debug.Log("Photo captured successfully!");

        if (remainingTime <= 0f)
        {
            FinishObservation();
        }
    }

    void FinishObservation()
    {
        remainingTime = 0f;
        isObserving = false;
        hasFinished = true;
        cameraRaised = false;

        if (playerController != null)
            playerController.EndObservation();

        activeExhibit = null;

        completionMessageTimer = 3f;
        completionMessage = "Observation completed!";

        if (experimentManager != null)
        {
            experimentManager.OnExhibitCompleted(this);
        }

        Debug.Log(
            "Trial finished. Condition: " + condition +
            ", Photo taken: " + photoTaken
        );
    }

    void OnDisable()
    {
        if (activeExhibit == this)
        {
            if (playerController != null)
                playerController.EndObservation();

            isObserving = false;
            cameraRaised = false;
            activeExhibit = null;
        }
    }

    void OnGUI()
    {
        if (player == null) return;

        // Hide all exhibit interaction UI after the experiment ends.
        if (experimentManager != null &&
            !experimentManager.experimentStarted)
        {
            return;
        }

        if (activeExhibit != null && activeExhibit != this)
            return;

        if (flashTimer > 0f)
        {
            float alpha = flashTimer / flashDuration;

            Color oldColor = GUI.color;
            GUI.color = new Color(
                1f, 1f, 1f, alpha * 0.8f
            );

            GUI.DrawTexture(
                new Rect(
                    0, 0,
                    Screen.width,
                    Screen.height
                ),
                Texture2D.whiteTexture
            );

            GUI.color = oldColor;
        }

        float distance = GetHorizontalDistance();
        string message = "";

        if (isObserving)
        {
            int seconds = Mathf.CeilToInt(remainingTime);

            if (condition == TrialCondition.Photo)
            {
                if (photoTaken)
                {
                    message =
                        "Photo captured! Keep observing: " +
                        seconds + "s";
                }
                else if (remainingTime <= 0f)
                {
                    message = cameraRaised
                        ? "Time's up! Aim and left-click to capture."
                        : "Time's up! Press P to raise your camera.";
                }
                else if (cameraRaised)
                {
                    message = IsExhibitFramed()
                        ? "Exhibit centered! Left-click to take photo. " +
                          seconds + "s"
                        : "Move mouse to center the exhibit. " +
                          seconds + "s";
                }
                else
                {
                    message =
                        "Press P to raise camera: " +
                        seconds + "s";
                }
            }
            else
            {
                message =
                    "Observe the exhibit: " +
                    seconds + "s";
            }
        }
        else if (hasFinished &&
         completionMessageTimer > 0f)
        {
            bool nearNextExhibit = false;

            if (experimentManager != null &&
                experimentManager.exhibits != null)
            {
                foreach (ExhibitInteraction exhibit in experimentManager.exhibits)
                {
                    if (exhibit == null || exhibit == this)
                        continue;

                    if (experimentManager.CanInteract(exhibit) &&
                        Vector2.Distance(
                            new Vector2(player.position.x, player.position.z),
                            new Vector2(exhibit.transform.position.x, exhibit.transform.position.z)
                        ) <= exhibit.interactionDistance)
                    {
                        nearNextExhibit = true;
                        break;
                    }
                }
            }

            if (!nearNextExhibit)
                message = completionMessage;
        }
        else if (!hasFinished &&
                 experimentManager != null &&
                 experimentManager.CanInteract(this) &&
                 distance <= interactionDistance)
        {
            message = condition == TrialCondition.Photo
                ? "Press E: Observe and take one photo"
                : "Press E: Observe only";
        }

        if (cameraRaised && isObserving && !photoTaken)
        {
            DrawViewfinder();
        }

        if (string.IsNullOrEmpty(message))
            return;

        float boxWidth = Mathf.Min(
            540f,
            Screen.width * 0.9f
        );

        float boxHeight = 55f;

        float boxX =
            (Screen.width - boxWidth) / 2f;

        float boxY = 25f;

        Rect boxRect = new Rect(
            boxX, boxY,
            boxWidth, boxHeight
        );

        Color previousColor = GUI.color;

        GUI.color = new Color(
            0.10f, 0.14f, 0.20f, 0.9f
        );

        GUI.DrawTexture(
            boxRect,
            Texture2D.whiteTexture
        );

        GUI.color = previousColor;

        GUIStyle style =
            new GUIStyle(GUI.skin.label);

        style.fontSize = Mathf.Clamp(
            Mathf.RoundToInt(
                Screen.width * 0.035f
            ),
            12,
            22
        );

        style.fontStyle = FontStyle.Bold;
        style.alignment = TextAnchor.MiddleCenter;
        style.wordWrap = true;
        style.normal.textColor = Color.white;

        GUI.Label(
            new Rect(
                boxX + 10f,
                boxY + 5f,
                boxWidth - 20f,
                boxHeight - 10f
            ),
            message,
            style
        );
    }

    void DrawViewfinder()
    {
        bool centered = IsExhibitFramed();

        Color frameColor = centered
            ? new Color(0.2f, 1f, 0.4f, 1f)
            : Color.white;

        float width = Screen.width * 0.5f;
        float height = Screen.height * 0.55f;

        float x = (Screen.width - width) / 2f;
        float y = (Screen.height - height) / 2f;

        float thickness = 3f;

        Color previousColor = GUI.color;
        GUI.color = frameColor;

        GUI.DrawTexture(
            new Rect(x, y, width, thickness),
            Texture2D.whiteTexture
        );

        GUI.DrawTexture(
            new Rect(
                x,
                y + height - thickness,
                width,
                thickness
            ),
            Texture2D.whiteTexture
        );

        GUI.DrawTexture(
            new Rect(x, y, thickness, height),
            Texture2D.whiteTexture
        );

        GUI.DrawTexture(
            new Rect(
                x + width - thickness,
                y,
                thickness,
                height
            ),
            Texture2D.whiteTexture
        );

        float centerX = Screen.width / 2f;
        float centerY = Screen.height / 2f;

        GUI.DrawTexture(
            new Rect(
                centerX - 10f,
                centerY - 1f,
                20f,
                2f
            ),
            Texture2D.whiteTexture
        );

        GUI.DrawTexture(
            new Rect(
                centerX - 1f,
                centerY - 10f,
                2f,
                20f
            ),
            Texture2D.whiteTexture
        );

        GUI.color = previousColor;
    }
}
