using System;
using LiveAwareLabs;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleRuntimeUI : MonoBehaviour {
  private VisualElement before;
  private VisualElement after;
  private Button initializeButton;
  private Label statusLabel;
  private Button startButton;
  private Button clipButton;
  private Button stopButton;
  private TextField teamInput;
  private TextField eventInput;
  private Toggle useCameraToggle;
  private Toggle useMicrophoneToggle;
  private Button resetButton;
  private RecorderPlugin recorderPlugin;

  private void OnEnable() {
    var uiDocument = GetComponent<UIDocument>();
    before = uiDocument.rootVisualElement.Q("before");
    after = uiDocument.rootVisualElement.Q("after");
    after.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
    initializeButton = (Button)uiDocument.rootVisualElement.Q("initialize");
    initializeButton.RegisterCallback<ClickEvent>(OnInitializeClicked);
    statusLabel = (Label)uiDocument.rootVisualElement.Q("status");
    startButton = (Button)uiDocument.rootVisualElement.Q("start");
    clipButton = (Button)uiDocument.rootVisualElement.Q("clip");
    stopButton = (Button)uiDocument.rootVisualElement.Q("stop");
    startButton.RegisterCallback<ClickEvent>(OnStartClicked);
    clipButton.RegisterCallback<ClickEvent>(OnClipClicked);
    stopButton.RegisterCallback<ClickEvent>(OnStopClicked);
    teamInput = (TextField)uiDocument.rootVisualElement.Q("team-name");
    eventInput = (TextField)uiDocument.rootVisualElement.Q("event-name");
    useCameraToggle = (Toggle)uiDocument.rootVisualElement.Q("use-camera");
    useMicrophoneToggle = (Toggle)uiDocument.rootVisualElement.Q("use-microphone");
    resetButton = (Button)uiDocument.rootVisualElement.Q("reset");
    resetButton.RegisterCallback<ClickEvent>(OnResetClicked);
  }

  private void OnDisable() {
    resetButton.UnregisterCallback<ClickEvent>(OnResetClicked);
    stopButton.UnregisterCallback<ClickEvent>(OnStopClicked);
    clipButton.UnregisterCallback<ClickEvent>(OnClipClicked);
    startButton.UnregisterCallback<ClickEvent>(OnStartClicked);
    initializeButton.UnregisterCallback<ClickEvent>(OnInitializeClicked);
  }

  void OnGUI() {
    statusLabel.text = recorderPlugin == null ? "Idle" : recorderPlugin.IsRunning ?
      recorderPlugin.IsRecording ? "Recording" : "Connected" : "Disconnected";
  }

  private async void OnInitializeClicked(ClickEvent clickEvent) {
    try {
      recorderPlugin = await RecorderPlugin.CreateAsync();
      recorderPlugin.ClipCreated += RecorderPlugin_ClipCreated;
      recorderPlugin.IsRecordingChanged += RecorderPlugin_IsRecordingChanged;
      recorderPlugin.IsRunningChanged += RecorderPlugin_IsRunningChanged;
      before.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
      after.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.Initial);
      Debug.Log("Initialized");
    } catch (TimeoutException) {
      Debug.LogError("Cannot connect with the Live Aware application");
    }
  }

  private void RecorderPlugin_ClipCreated(object sender, System.EventArgs e) {
    Debug.Log("Clip created");
  }

  private void RecorderPlugin_IsRecordingChanged(object sender, System.EventArgs e) {
    Debug.Log(recorderPlugin.IsRecording ? "Recording" : "Not recording");
  }

  private void RecorderPlugin_IsRunningChanged(object sender, System.EventArgs e) {
    Debug.Log(recorderPlugin.IsRunning ? "Running" : "Not running");
  }

  private async void OnStartClicked(ClickEvent clickEvent) {
    if (await recorderPlugin.StartStreamingAsync(teamName: teamInput.value, eventName: eventInput.value, useCamera: useCameraToggle.value, useMicrophone: useMicrophoneToggle.value)) {
      string message = "Started streaming";
      if (string.IsNullOrEmpty(teamInput.text)) {
        message += " default team";
      } else {
        message += $" team \"{teamInput.text}\"";
      }
      if (string.IsNullOrEmpty(eventInput.text)) {
        message += " default event";
      } else {
        message += $" event \"{eventInput.text}\"";
      }
      if (useCameraToggle.value) {
        message += " with camera";
      }
      if (useMicrophoneToggle.value) {
        message += " with microphone";
      }
      Debug.Log(message);
    } else {
      Debug.LogWarning("Cannot start streaming");
    }
  }

  private async void OnClipClicked(ClickEvent clickEvent) {
    if (await recorderPlugin.CreateClipAsync()) {
      Debug.Log("Requested clip");
    } else {
      Debug.LogWarning("Cannot request clip");
    }
  }

  private async void OnStopClicked(ClickEvent clickEvent) {
    if (await recorderPlugin.StopStreamingAsync()) {
      string message = "Stopped streaming ";
      if (string.IsNullOrEmpty(eventInput.text)) {
        message += "default event";
      } else {
        message += $"event \"{eventInput.text}\"";
      }
      Debug.Log(message);
    } else {
      Debug.LogWarning("Cannot stop streaming");
    }
  }

  private async void OnResetClicked(ClickEvent clickEvent) {
    after.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
    before.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.Initial);
    await recorderPlugin.DisposeAsync();
    recorderPlugin = null;
  }
}
