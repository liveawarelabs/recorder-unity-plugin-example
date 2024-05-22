# RecorderPlugin

Namespace: LiveAwareLabs

This is the Live Aware Labs plug-in for the LiveAware Desktop Recorder.

```csharp
public sealed class RecorderPlugin : System.IAsyncDisposable
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [RecorderPlugin](./liveawarelabs.recorderplugin.md)<br>
Implements [IAsyncDisposable](https://docs.microsoft.com/en-us/dotnet/api/system.iasyncdisposable)

**Remarks:**

You must invoke the [RecorderPlugin.DisposeAsync()](./liveawarelabs.recorderplugin.md#disposeasync) method or your application will not shut down properly.

## Properties

### **IsBuffering**

Specifies whether or not the Live Aware Desktop Recorder is buffering. The value of this property is invalid if the
 recorder is not running.

```csharp
public bool IsBuffering { get; private set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsRecording**

Specifies whether or not the Live Aware Desktop Recorder is currently recording or streaming. The value of this
 property is invalid if the recorder is not running.

```csharp
public bool IsRecording { get; private set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **IsRunning**

Specifies whether or not the Live Aware Desktop Recorder is currently running. If this property is false, all other
 property values are invalid and all methods have no effect. You must create a new [RecorderPlugin](./liveawarelabs.recorderplugin.md)
 instance to restart the recorder.

```csharp
public bool IsRunning { get; private set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

## Methods

### **CreateAsync()**

Create a new [RecorderPlugin](./liveawarelabs.recorderplugin.md). This ensures the Live Aware Desktop Recorder is running. This might take a
 while if it needs to launch the app.

```csharp
public static Task<RecorderPlugin> CreateAsync()
```

#### Returns

[Task&lt;RecorderPlugin&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
A new [NamedPipe](./liveawarelabs.namedpipe.md) object.

#### Exceptions

[TimeoutException](https://docs.microsoft.com/en-us/dotnet/api/system.timeoutexception)<br>
Thrown if the recorder does not accept a connection within ten seconds.

### **StartStreamingAsync(String, String, Boolean, Boolean, Boolean, Boolean)**

Send a message to the Live Aware Desktop Recorder to start live streaming.

```csharp
public Task<bool> StartStreamingAsync(string teamName, string eventName, bool live, bool useMicrophone, bool useCamera, bool fullScreen)
```

#### Parameters

`teamName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The name of the team the recorder will use if not empty, otherwise it will use the currently
 selected team. You must specify a team name if you want to create a new event.

`eventName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The name of the event the recorder will use if not empty, otherwise it will use the currently
 selected event.

`live` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether or not to stream live versus record.

`useMicrophone` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether or not to stream the default system microphone audio.

`useCamera` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether or not to use the available camera.

`fullScreen` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether or not to record the entire screen instead of only the window.

#### Returns

[Task&lt;Boolean&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
True if request sent successfully, false otherwise.

#### Exceptions

[IOException](https://docs.microsoft.com/en-us/dotnet/api/system.io.ioexception)<br>
Thrown if communication with the recorder is interrupted.

### **StopStreamingAsync(Boolean, String)**

Send a message to the Live Aware Desktop Recorder to stop live streaming.

```csharp
public Task<bool> StopStreamingAsync(bool upload, string recordingName)
```

#### Parameters

`upload` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether or not to upload the recording. This parameter is ignored if live.

`recordingName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The name to assign to the recording. The recorder will generate and assign a name if this
 parameter is absent or empty.

#### Returns

[Task&lt;Boolean&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
True if request sent successfully, false otherwise.

#### Exceptions

[IOException](https://docs.microsoft.com/en-us/dotnet/api/system.io.ioexception)<br>
Thrown if communication with the recorder is interrupted.

### **ChangeBufferingAsync(Boolean)**

Send a message to the Live Aware Desktop Recorder to change buffering.

```csharp
public Task ChangeBufferingAsync(bool wantsBuffering)
```

#### Parameters

`wantsBuffering` [Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>
Whether or not to enable buffering.

#### Returns

[Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)<br>
True if request sent successfully, false otherwise.

#### Exceptions

[IOException](https://docs.microsoft.com/en-us/dotnet/api/system.io.ioexception)<br>
Thrown if communication with the recorder is interrupted.

### **CreateSliceAsync()**

Send a message to the Live Aware Desktop Recorder to create a slice.

```csharp
public Task<bool> CreateSliceAsync()
```

#### Returns

[Task&lt;Boolean&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)<br>
True if request sent successfully, false otherwise.

#### Exceptions

[IOException](https://docs.microsoft.com/en-us/dotnet/api/system.io.ioexception)<br>
Thrown if communication with the recorder is interrupted.

### **DisposeAsync()**

Dispose of this [RecorderPlugin](./liveawarelabs.recorderplugin.md) instance asynchronously.

```csharp
public ValueTask DisposeAsync()
```

#### Returns

[ValueTask](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.valuetask)<br>
A task that represents the asynchronous dispose operation.

## Events

### **IsBufferingChanged**

Fires when the Live Aware Desktop Recorder starts or stops buffering. Check the [RecorderPlugin.IsBuffering](./liveawarelabs.recorderplugin.md#isbuffering) property to
 determine whether or not the recorder is buffering. This event handler will run on a background thread.

```csharp
public event EventHandler IsBufferingChanged;
```

### **IsRecordingChanged**

Fires when the Live Aware Desktop Recorder starts or stops recording or streaming. This event also fires if the
 recorder enters or exits background mode. Check the [RecorderPlugin.IsRecording](./liveawarelabs.recorderplugin.md#isrecording) property to determine whether or not the
 recorder is currently recording or streaming. This event handler will run on a background thread.

```csharp
public event EventHandler IsRecordingChanged;
```

### **IsRunningChanged**

Fires when the Live Aware Desktop Recorder closes. Check the [RecorderPlugin.IsRunning](./liveawarelabs.recorderplugin.md#isrunning) property to determine whether or
 not the recorder is currently running. This event handler will run on a background thread.

```csharp
public event EventHandler IsRunningChanged;
```

### **SliceCreated**

Fires when the Live Aware Desktop Recorder creates a slice. This event handler will run on a background thread.

```csharp
public event EventHandler SliceCreated;
```

### **TeamSelected**

Fires when the Live Aware Desktop Recorder selects a team. This event handler will run on a background thread.

```csharp
public event StringFieldChanged TeamSelected;
```

### **EventSelected**

Fires when the Live Aware Desktop Recorder selects an event. This event handler will run on a background thread.

```csharp
public event StringFieldChanged EventSelected;
```
