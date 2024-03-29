using System;
using LiveAwareLabs;
using UnityEngine;
using UnityEngine.UIElements;

public class SimpleRuntimeUI : MonoBehaviour {
  private VisualElement before;
  private VisualElement after;
  private Button initializeButton;
  private Label statusLabel;
  private Button changeModeButton;
  private Button startButton;
  private Button sliceButton;
  private Button stopButton;
  private TextField teamInput;
  private TextField eventInput;
  private Toggle useCameraToggle;
  private Toggle useMicrophoneToggle;
  private Toggle goLiveToggle;
  private Button resetButton;
  private RecorderPlugin recorderPlugin;
  private DateTime? streamingStartTime;

  private void OnEnable() {
    var uiDocument = GetComponent<UIDocument>();
    before = uiDocument.rootVisualElement.Q("before");
    after = uiDocument.rootVisualElement.Q("after");
    after.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
    initializeButton = (Button)uiDocument.rootVisualElement.Q("initialize");
    initializeButton.RegisterCallback<ClickEvent>(OnInitializeClicked);
    statusLabel = (Label)uiDocument.rootVisualElement.Q("status");
    changeModeButton = (Button)uiDocument.rootVisualElement.Q("changeMode");
    startButton = (Button)uiDocument.rootVisualElement.Q("start");
    sliceButton = (Button)uiDocument.rootVisualElement.Q("slice");
    stopButton = (Button)uiDocument.rootVisualElement.Q("stop");
    changeModeButton.RegisterCallback<ClickEvent>(OnChangeModeClicked);
    startButton.RegisterCallback<ClickEvent>(OnStartClicked);
    sliceButton.RegisterCallback<ClickEvent>(OnSliceClicked);
    stopButton.RegisterCallback<ClickEvent>(OnStopClicked);
    teamInput = (TextField)uiDocument.rootVisualElement.Q("team-name");
    eventInput = (TextField)uiDocument.rootVisualElement.Q("event-name");
    useCameraToggle = (Toggle)uiDocument.rootVisualElement.Q("use-camera");
    useMicrophoneToggle = (Toggle)uiDocument.rootVisualElement.Q("use-microphone");
    goLiveToggle = (Toggle)uiDocument.rootVisualElement.Q("go-live");
    resetButton = (Button)uiDocument.rootVisualElement.Q("reset");
    resetButton.RegisterCallback<ClickEvent>(OnResetClicked);
  }

  private void OnDisable() {
    resetButton.UnregisterCallback<ClickEvent>(OnResetClicked);
    stopButton.UnregisterCallback<ClickEvent>(OnStopClicked);
    sliceButton.UnregisterCallback<ClickEvent>(OnSliceClicked);
    startButton.UnregisterCallback<ClickEvent>(OnStartClicked);
    initializeButton.UnregisterCallback<ClickEvent>(OnInitializeClicked);
  }

  void OnGUI() {
    string state = recorderPlugin == null ? "Idle" : recorderPlugin.IsRunning ?
      recorderPlugin.IsRecording ? "Recording" : "Connected" : "Disconnected";
    string mode = recorderPlugin != null && recorderPlugin.IsBuffering ? ", buffering" : "";
    statusLabel.text = $"{state}{mode}";
    if (goLiveToggle.value) {
      startButton.text = "GO LIVE";
      stopButton.text = "End Stream";
    } else {
      startButton.text = "Start Recording";
      stopButton.text = "Stop Recording";
    }
    if (recorderPlugin != null) {
      resetButton.text = recorderPlugin.IsRunning ? "Disconnect from Live Aware" : "Reset";
      if (recorderPlugin.IsBuffering) {
        changeModeButton.text = "Stop buffering";
      } else {
        changeModeButton.text = "Start buffering";
      }
    }
  }

  private void OnApplicationQuit() {
    Debug.Log("quitting");
    _ = recorderPlugin?.DisposeAsync();
  }

  private async void OnInitializeClicked(ClickEvent clickEvent) {
    try {
      recorderPlugin = await RecorderPlugin.CreateAsync();
      recorderPlugin.SliceCreated += RecorderPlugin_SliceCreated;
      recorderPlugin.IsBufferingChanged += RecorderPlugin_IsBufferingChanged;
      recorderPlugin.IsRecordingChanged += RecorderPlugin_IsRecordingChanged;
      recorderPlugin.IsRunningChanged += RecorderPlugin_IsRunningChanged;
      before.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
      after.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.Initial);
      Debug.Log("Initialized");
    } catch (TimeoutException) {
      Debug.LogError("Cannot connect with the Live Aware application");
    }
  }

  private void RecorderPlugin_IsBufferingChanged(object sender, EventArgs e) {
    string mode = recorderPlugin.IsBuffering ? "en" : "dis";
    Debug.Log($"Buffering {mode}abled");
  }

  private void RecorderPlugin_SliceCreated(object sender, EventArgs e) {
    Debug.Log("Slice created");
  }

  private void RecorderPlugin_IsRecordingChanged(object sender, EventArgs e) {
    Debug.Log(recorderPlugin.IsRecording ? "Recording" : "Not recording");
  }

  private void RecorderPlugin_IsRunningChanged(object sender, EventArgs e) {
    Debug.Log(recorderPlugin.IsRunning ? "Running" : "Not running");
  }

  private void OnChangeModeClicked(ClickEvent evt) {
    recorderPlugin.ChangeBufferingAsync(!recorderPlugin.IsBuffering);
    string mode = recorderPlugin.IsBuffering ? "dis" : "en";
    Debug.Log($"Requested to {mode}able buffering");
  }

  private async void OnStartClicked(ClickEvent clickEvent) {
    if (goLiveToggle.value) {
      streamingStartTime = DateTime.Now;
    }
    if (await recorderPlugin.StartStreamingAsync(teamName: teamInput.value, eventName: eventInput.value, live: goLiveToggle.value, useCamera: useCameraToggle.value, useMicrophone: useMicrophoneToggle.value)) {
      string message = "Requested to start streaming";
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

  private async void OnSliceClicked(ClickEvent clickEvent) {
    if (await recorderPlugin.CreateSliceAsync()) {
      Debug.Log("Requested slice");
    } else {
      Debug.LogWarning("Cannot request slice");
    }
  }

  private async void OnStopClicked(ClickEvent clickEvent) {
    if (streamingStartTime != null && DateTime.Now - streamingStartTime < TimeSpan.FromSeconds(15)) {
      // Live streaming must last at least fifteen seconds or the stream will fail.  This is a known issue.
      return;
    }
    streamingStartTime = null;
    if (await recorderPlugin.StopStreamingAsync()) {
      string message = "Requested to stop streaming ";
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
