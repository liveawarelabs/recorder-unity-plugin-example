# LiveAwareBehavior

Namespace: LiveAwareLabs

This is the Live Aware Labs Unity MonoBehavior derivative to control the plug-in for the LiveAware Desktop Recorder.

```csharp
public class LiveAwareBehavior : UnityEngine.MonoBehaviour
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → Object → Component → Behaviour → MonoBehaviour → [LiveAwareBehavior](./liveawarelabs.liveawarebehavior.md)

## Fields

### **teamName**

Get or set the team name to provide to the plug-in. Leave empty to use the currently selected team.

```csharp
public string teamName;
```

### **eventName**

Get or set the event name to provide to the plug-in. Leave empty to use the currently selected event.

```csharp
public string eventName;
```

### **useCamera**

Specifies whether or not to use the camera during streaming.

```csharp
public bool useCamera;
```

### **useMicrophone**

Specifies whether or not to use the microphone during streaming.

```csharp
public bool useMicrophone;
```

## Properties

### **destroyCancellationToken**

```csharp
public CancellationToken destroyCancellationToken { get; }
```

#### Property Value

[CancellationToken](https://docs.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken)<br>

### **useGUILayout**

```csharp
public bool useGUILayout { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **runInEditMode**

```csharp
public bool runInEditMode { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **enabled**

```csharp
public bool enabled { get; set; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **isActiveAndEnabled**

```csharp
public bool isActiveAndEnabled { get; }
```

#### Property Value

[Boolean](https://docs.microsoft.com/en-us/dotnet/api/system.boolean)<br>

### **transform**

```csharp
public Transform transform { get; }
```

#### Property Value

Transform<br>

### **gameObject**

```csharp
public GameObject gameObject { get; }
```

#### Property Value

GameObject<br>

### **tag**

```csharp
public string tag { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **rigidbody**

#### Caution

Property rigidbody has been deprecated. Use GetComponent<Rigidbody>() instead. (UnityUpgradable)

---

```csharp
public Component rigidbody { get; }
```

#### Property Value

Component<br>

### **rigidbody2D**

#### Caution

Property rigidbody2D has been deprecated. Use GetComponent<Rigidbody2D>() instead. (UnityUpgradable)

---

```csharp
public Component rigidbody2D { get; }
```

#### Property Value

Component<br>

### **camera**

#### Caution

Property camera has been deprecated. Use GetComponent<Camera>() instead. (UnityUpgradable)

---

```csharp
public Component camera { get; }
```

#### Property Value

Component<br>

### **light**

#### Caution

Property light has been deprecated. Use GetComponent<Light>() instead. (UnityUpgradable)

---

```csharp
public Component light { get; }
```

#### Property Value

Component<br>

### **animation**

#### Caution

Property animation has been deprecated. Use GetComponent<Animation>() instead. (UnityUpgradable)

---

```csharp
public Component animation { get; }
```

#### Property Value

Component<br>

### **constantForce**

#### Caution

Property constantForce has been deprecated. Use GetComponent<ConstantForce>() instead. (UnityUpgradable)

---

```csharp
public Component constantForce { get; }
```

#### Property Value

Component<br>

### **renderer**

#### Caution

Property renderer has been deprecated. Use GetComponent<Renderer>() instead. (UnityUpgradable)

---

```csharp
public Component renderer { get; }
```

#### Property Value

Component<br>

### **audio**

#### Caution

Property audio has been deprecated. Use GetComponent<AudioSource>() instead. (UnityUpgradable)

---

```csharp
public Component audio { get; }
```

#### Property Value

Component<br>

### **networkView**

#### Caution

Property networkView has been deprecated. Use GetComponent<NetworkView>() instead. (UnityUpgradable)

---

```csharp
public Component networkView { get; }
```

#### Property Value

Component<br>

### **collider**

#### Caution

Property collider has been deprecated. Use GetComponent<Collider>() instead. (UnityUpgradable)

---

```csharp
public Component collider { get; }
```

#### Property Value

Component<br>

### **collider2D**

#### Caution

Property collider2D has been deprecated. Use GetComponent<Collider2D>() instead. (UnityUpgradable)

---

```csharp
public Component collider2D { get; }
```

#### Property Value

Component<br>

### **hingeJoint**

#### Caution

Property hingeJoint has been deprecated. Use GetComponent<HingeJoint>() instead. (UnityUpgradable)

---

```csharp
public Component hingeJoint { get; }
```

#### Property Value

Component<br>

### **particleSystem**

#### Caution

Property particleSystem has been deprecated. Use GetComponent<ParticleSystem>() instead. (UnityUpgradable)

---

```csharp
public Component particleSystem { get; }
```

#### Property Value

Component<br>

### **name**

```csharp
public string name { get; set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### **hideFlags**

```csharp
public HideFlags hideFlags { get; set; }
```

#### Property Value

HideFlags<br>

## Constructors

### **LiveAwareBehavior()**

```csharp
public LiveAwareBehavior()
```
