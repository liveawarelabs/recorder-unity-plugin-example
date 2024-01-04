using UnityEngine;
using UnityEngine.UIElements;

public class SimpleRuntimeUI : MonoBehaviour {
  private Button startButton;
  private Button clipButton;
  private Button stopButton;

  private void OnEnable() {
    var uiDocument = GetComponent<UIDocument>();
    startButton = uiDocument.rootVisualElement.Q("start") as Button;
    clipButton = uiDocument.rootVisualElement.Q("clip") as Button;
    stopButton = uiDocument.rootVisualElement.Q("stop") as Button;
    startButton.RegisterCallback<ClickEvent>(OnStartClicked);
    clipButton.RegisterCallback<ClickEvent>(OnClipClicked);
    stopButton.RegisterCallback<ClickEvent>(OnStopClicked);
    var eventInput = uiDocument.rootVisualElement.Q("event");
    eventInput.RegisterCallback<ChangeEvent<string>>(OnEventChanged);
  }

  private void OnDisable() {
    stopButton.UnregisterCallback<ClickEvent>(OnStopClicked);
    clipButton.UnregisterCallback<ClickEvent>(OnClipClicked);
    startButton.UnregisterCallback<ClickEvent>(OnStartClicked);
  }

  private void OnStopClicked(ClickEvent evt) {
    var element = (VisualElement)evt.target;
    Debug.Log($"{element.name} was clicked!");
  }

  private void OnClipClicked(ClickEvent evt) {
    var element = (VisualElement)evt.target;
    Debug.Log($"{element.name} was clicked!");
  }

  private void OnStartClicked(ClickEvent evt) {
    var element = (VisualElement)evt.target;
    Debug.Log($"{element.name} was clicked!");
  }

  public static void OnEventChanged(ChangeEvent<string> evt) {
    Debug.Log($"{evt.newValue} -> {evt.target}");
  }
}
