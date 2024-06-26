using System;
using LiveAwareLabs;
using UnityEngine;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Diagnostics;

public class SimpleRuntimeUI : MonoBehaviour {
    private AudioSource audioSource;
    private VisualElement before_visual, after_visual, checkboxes_visual, properties_visual;
    private Button initializeButton;
    private Button exitButton, mainExitButton, resetButton, crashButton;
	private Label statusLabel, userLabel, loggerLabel;
	private Button startStreamingButton, stopStreamingButton, startRecordingButton, stopRecordingButton;
	private Button sliceButton, eventButton;
    private TextField sliceText, eventText, uploadNameText, uploadEventText, recordNameText, recordsLocationText;
    private IntegerField sliceLenField;
    private Toggle uploadToggle;

    private bool want_exit, main_menu, menu_changed = true, buffering;
	private string status = "none", user = "none";
    uint qsize = 5;  // number of messages to keep
    Queue myLogQueue = new Queue();
    Dictionary<string, Toggle> checkboxes_ui = new Dictionary<string, Toggle>();
    Dictionary<string, DropdownField> properties_ui = new Dictionary<string, DropdownField>();
    Dictionary<string, List<string>> properties_values = new Dictionary<string, List<string>>();
    Dictionary<string, string> properties_active = new Dictionary<string, string>();

    private void OnEnable() 
	{
        audioSource = GetComponent<AudioSource>();
        var uiDocument = GetComponent<UIDocument>();
		before_visual = uiDocument.rootVisualElement.Q("before");
		after_visual = uiDocument.rootVisualElement.Q("after");
        checkboxes_visual = uiDocument.rootVisualElement.Q("checkboxes");
        properties_visual = uiDocument.rootVisualElement.Q("properties");

        initializeButton = uiDocument.rootVisualElement.Q<Button>("initialize");
        exitButton = uiDocument.rootVisualElement.Q<Button>("exit");
        statusLabel = uiDocument.rootVisualElement.Q<Label>("status");
        userLabel = uiDocument.rootVisualElement.Q<Label>("user");
        loggerLabel = uiDocument.rootVisualElement.Q<Label>("logger");
		sliceButton = uiDocument.rootVisualElement.Q<Button>("create-slice");
        startStreamingButton = uiDocument.rootVisualElement.Q<Button>("start-streaming");
        stopStreamingButton = uiDocument.rootVisualElement.Q<Button>("stop-streaming");
        startRecordingButton = uiDocument.rootVisualElement.Q<Button>("start-recording");
        stopRecordingButton = uiDocument.rootVisualElement.Q<Button>("stop-recording");
        eventButton = uiDocument.rootVisualElement.Q<Button>("create-event");
        sliceText = uiDocument.rootVisualElement.Q<TextField>("slice-name");
        eventText = uiDocument.rootVisualElement.Q<TextField>("event-name");
        recordNameText = uiDocument.rootVisualElement.Q<TextField>("record-name");
        recordsLocationText = uiDocument.rootVisualElement.Q<TextField>("records-location");
        uploadNameText = uiDocument.rootVisualElement.Q<TextField>("upload-name");
        uploadEventText = uiDocument.rootVisualElement.Q<TextField>("upload-event");
        sliceLenField = uiDocument.rootVisualElement.Q<IntegerField>("slice-len");
        mainExitButton = uiDocument.rootVisualElement.Q<Button>("exit_main");
        crashButton = uiDocument.rootVisualElement.Q<Button>("crash");
        resetButton = uiDocument.rootVisualElement.Q<Button>("reset");
        uploadToggle = uiDocument.rootVisualElement.Q<Toggle>("upload-record");

        string[] checkboxes_list = new string[] {"Buffering", "Full Screen", "Camera", "Microphone", "Audio", "Cursor", "Ultra Low Latency" };
        foreach (string checkbox_name in checkboxes_list)
        {
            var checkbox_field = new Toggle(checkbox_name);
            checkboxes_visual.Add(checkbox_field);
            checkboxes_ui.Add(checkbox_name, checkbox_field);
            if (checkbox_name == "Buffering")
            {
                checkbox_field.RegisterCallback<ChangeEvent<bool>>(OnBufferingChanged);
            }
            else if (checkbox_name == "Audio")
            {
                checkbox_field.RegisterCallback<ChangeEvent<bool>>(OnAudioChanged);
            }
        }
        string[] properties_list = LiveAwareSDK.get_available_selection_properties();
        foreach (string property_name in properties_list)
        {
            var property_field = new DropdownField(property_name);
            properties_visual.Add(property_field);
            properties_ui.Add(property_name, property_field);
			properties_values.Add(property_name, new List<string>());
			properties_active.Add(property_name, "");
            property_field.RegisterCallback<ChangeEvent<string>>(OnPropertyChanged);
        }

        LiveAwareSDK.OnLogMessage += OnLogReceived;
        LiveAwareSDK.OnStateChanged += OnStateChanged;
        LiveAwareSDK.OnSelectionPropertyReady += OnPropertyChanged;
        LiveAwareSDK.OnVideoUploadReady += OnVideoUploadReady;
        Application.logMessageReceived += HandleLog;

        initializeButton.RegisterCallback<ClickEvent>(OnInitializeClicked);
        exitButton.RegisterCallback<ClickEvent>(OnExitClicked);
        mainExitButton.RegisterCallback<ClickEvent>(OnExitClicked);
        crashButton.RegisterCallback<ClickEvent>(OnCrashClicked);
        resetButton.RegisterCallback<ClickEvent>(OnResetClicked);
        sliceButton.RegisterCallback<ClickEvent>(OnCreateSliceClicked);
        eventButton.RegisterCallback<ClickEvent>(OnCreateEventClicked);
        startStreamingButton.RegisterCallback<ClickEvent>(OnStartStreamingClicked);
        stopStreamingButton.RegisterCallback<ClickEvent>(OnStopStreamingClicked);
        startRecordingButton.RegisterCallback<ClickEvent>(OnStartRecordingClicked);
        stopRecordingButton.RegisterCallback<ClickEvent>(OnStopRecordingClicked);

        Debug.Log(LiveAwareSDK.get_sdk_version_string());
        LiveAwareSDK.refresh_state();       
    }

	private void OnDisable() {
        LiveAwareSDK.OnLogMessage -= OnLogReceived;
        LiveAwareSDK.OnStateChanged -= OnStateChanged;
        LiveAwareSDK.OnSelectionPropertyReady -= OnPropertyChanged;
        LiveAwareSDK.OnVideoUploadReady -= OnVideoUploadReady;
        Application.logMessageReceived -= HandleLog;

        initializeButton.UnregisterCallback<ClickEvent>(OnInitializeClicked);
        exitButton.UnregisterCallback<ClickEvent>(OnExitClicked);
        mainExitButton.UnregisterCallback<ClickEvent>(OnExitClicked);
        crashButton.UnregisterCallback<ClickEvent>(OnCrashClicked);
        resetButton.UnregisterCallback<ClickEvent>(OnResetClicked);
        sliceButton.UnregisterCallback<ClickEvent>(OnCreateSliceClicked);
        eventButton.UnregisterCallback<ClickEvent>(OnCreateEventClicked);
        startStreamingButton.UnregisterCallback<ClickEvent>(OnStartStreamingClicked);
        stopStreamingButton.UnregisterCallback<ClickEvent>(OnStopStreamingClicked);
        startRecordingButton.UnregisterCallback<ClickEvent>(OnStartRecordingClicked);
        stopRecordingButton.UnregisterCallback<ClickEvent>(OnStopRecordingClicked);
    }

	void OnGUI() {
		if (want_exit)
		{
#if UNITY_STANDALONE

            Application.Quit(0);
#endif
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
		statusLabel.text = status;
		userLabel.text = user;
		loggerLabel.text = string.Join("\n", myLogQueue.ToArray());

        string[] properties_list = LiveAwareSDK.get_available_selection_properties();
        if (menu_changed)
		{
			if (main_menu)
			{
                before_visual.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
                after_visual.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.Initial);
                foreach (string property_name in properties_list)
                {
                    LiveAwareSDK.refresh_selection_property(property_name);
                }
            }
			else
			{
                before_visual.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.Initial);
                after_visual.style.display = new StyleEnum<DisplayStyle>(StyleKeyword.None);
            }
			menu_changed = false;
		}
        checkboxes_ui["Buffering"].value = buffering;
        foreach (string property_name in properties_list)
        {
            properties_ui[property_name].choices = properties_values[property_name];
            properties_ui[property_name].SetValueWithoutNotify(properties_active[property_name]);
        }
    }

	private void OnApplicationQuit() {
        Debug.Log("quitting...");
		LiveAwareSDK.disconnect();
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        myLogQueue.Enqueue("[" + type + "] : " + logString);
        while (myLogQueue.Count > qsize)
            myLogQueue.Dequeue();
    }

    private void OnLogReceived(string level, string message)
	{
		message = "[SDK] " + message;
		switch (level)
		{
			case "error":
				Debug.LogError(message);
				HandleLog(message, "", LogType.Error);
                break;
			case "warning":
                Debug.LogWarning(message);
                HandleLog(message, "", LogType.Warning);
                break;
			case "info":
                Debug.Log(message);
                HandleLog(message, "", LogType.Log);
				break;
            default:
                Debug.Log(message);
                break;
		}
	}

	private void OnStateChanged(string state_type, string new_state)
	{
        switch (state_type)
		{
			case "status":
				status = new_state;
				if ((new string[] { "not initialized", "initializing", "initialized", "unauthorized"}).Contains(status))
				{
					if (main_menu)
					{
						main_menu = false;
						menu_changed = true;
					}
				}
				else
				{
                    if (!main_menu)
                    {
                        main_menu = true;
                        menu_changed = true;
                    }
                }
				break;
			case "buffering":
				buffering = (new_state == "enabled");
				break;
			case "user":
				user = new_state;
				break;
		}
    }

	private void OnPropertyChanged(string property_type, string[] available_Values, string active_value)
	{
        properties_values[property_type] = new List<string>(available_Values);
		properties_active[property_type] = active_value;
	}

    private void OnVideoUploadReady(string video_type, string project_id, string event_id, string video_id, string video_name, string video_url)
    {
        string msg = $"{video_type}:{video_name} upload completed:\nproject - {project_id}; event - {event_id}; video - {video_id};\nurl - {video_url}";
        Debug.Log(msg);
        HandleLog(msg, "", LogType.Log);
    }


    private async void OnInitializeClicked(ClickEvent clickEvent) {
        await Task.Factory.StartNew(() => LiveAwareLabs.LiveAwareSDK.connect());
	}

    private void OnExitClicked(ClickEvent clickEvent)
    {
		want_exit = true;
    }

    private void OnResetClicked(ClickEvent clickEvent)
    {
		LiveAwareSDK.disconnect();
    }
    private void OnCrashClicked(ClickEvent evt) 
	{
		Debug.Log("Crash Game on purpose");
        Utils.ForceCrash(ForcedCrashCategory.Abort);
    }

    private async void OnCreateSliceClicked(ClickEvent clickEvent)
    {
        await Task.Factory.StartNew(() => LiveAwareLabs.LiveAwareSDK.create_slice(sliceText.value, sliceLenField.value));
    }

    private async void OnCreateEventClicked(ClickEvent clickEvent)
    {
        await Task.Factory.StartNew(() => LiveAwareLabs.LiveAwareSDK.set_selection_property("event", eventText.value));
    }

    private async void OnStartStreamingClicked(ClickEvent clickEvent)
    {
        await Task.Factory.StartNew(() => LiveAwareLabs.LiveAwareSDK.start_streaming(
            recordNameText.value,
            checkboxes_ui["Full Screen"].value,
            checkboxes_ui["Camera"].value,
            checkboxes_ui["Microphone"].value,
            checkboxes_ui["Audio"].value,
            checkboxes_ui["Cursor"].value,
            checkboxes_ui["Ultra Low Latency"].value));
    }

    private async void OnStopStreamingClicked(ClickEvent clickEvent)
    {
        await Task.Factory.StartNew(() => LiveAwareLabs.LiveAwareSDK.stop_streaming());
    }

    private async void OnStartRecordingClicked(ClickEvent clickEvent)
    {
        await Task.Factory.StartNew(() => LiveAwareLabs.LiveAwareSDK.start_recording(
            recordNameText.value,
            recordsLocationText.value,
            checkboxes_ui["Full Screen"].value,
            checkboxes_ui["Camera"].value,
            checkboxes_ui["Microphone"].value,
            checkboxes_ui["Audio"].value,
            checkboxes_ui["Cursor"].value));
    }

    private async void OnStopRecordingClicked(ClickEvent clickEvent)
    {
        await Task.Factory.StartNew(() => LiveAwareLabs.LiveAwareSDK.stop_recording(
            uploadToggle.value,
            uploadNameText.value,
            uploadEventText.value));
    }

    private async void OnBufferingChanged(ChangeEvent<bool> evt)
    {
        bool enabled = evt.newValue;
        if (enabled != buffering)
        {
            Debug.Log($"Change buffering enabled to {enabled}");
            await Task.Factory.StartNew(() => LiveAwareLabs.LiveAwareSDK.set_buffering_enabled(enabled));
        }
    }

    private async void OnPropertyChanged(ChangeEvent<string> evt)
    {
        string property_type = ((DropdownField)evt.target).label;
        string value = evt.newValue;
        Debug.Log($"Set property {property_type} to {value}");
        await Task.Factory.StartNew(() => LiveAwareLabs.LiveAwareSDK.set_selection_property(property_type, value));
    }

    private void OnAudioChanged(ChangeEvent<bool> evt)
    {
        if (evt.newValue)
        {
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }
    }
}
