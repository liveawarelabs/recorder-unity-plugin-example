using System.Threading.Tasks;
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
    statusLabel.text = recorderPlugin == null ? "Idle" : recorderPlugin.IsRecording ? "Recording" : "Connected";
  }

  private async void OnInitializeClicked(ClickEvent clickEvent) {
    recorderPlugin = await RecorderPlugin.CreateAsync();
    before.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
    after.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.Initial);
    Debug.Log("Initialized");
  }

  private async void OnStartClicked(ClickEvent clickEvent) {
    if (await recorderPlugin.StartStreamingAsync(eventName: eventInput.value, useCamera: useCameraToggle.value, useMicrophone: useMicrophoneToggle.value)) {
      string message = "Started streaming ";
      if (string.IsNullOrEmpty(eventInput.text)) {
        message += "default event";
      } else {
        message += $"event \"{eventInput.text}\"";
      }
      if (useCameraToggle.value) {
        message += " with camera";
      }
      if (useMicrophoneToggle.value) {
        message += " with microphone";
      }
      Debug.Log(message);
    } else {
      Debug.LogWarning("cannot start streaming");
    }
  }

  private async void OnClipClicked(ClickEvent clickEvent) {
    await recorderPlugin.CreateClipAsync();
    Debug.Log("Created clip");
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
      Debug.LogWarning("cannot stop streaming");
    }
  }

  private async void OnResetClicked(ClickEvent clickEvent) {
    after.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
    before.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.Initial);
    await recorderPlugin.DisposeAsync();
    recorderPlugin = null;
  }
}
