# Unity Save System

This is a **Save System** designed specifically for saving and loading **Scriptable Objects** in Unity. It supports serialization, data encryption, and event broadcasting, making it ideal for persistent game data.

## Features

- **Save and Load Scriptable Objects**: Handles saving and loading of Scriptable Objects asynchronously.
- **Custom Serialization**: Supports default serialization or custom serialization logic via the `IPersistentCustomSerializable` interface.
- **Data Encryption**: Provides encryption for stored data.
- **Event Broadcasting**: Allows registration for save/load events through the `SaveLoadBroadcaster`.

## How to Use

### 1. Saving a Scriptable Object

To save a Scriptable Object, you can use the `Save()` extension method provided in `ScriptableObjectExtensions`.

```csharp
await myScriptableObject.Save();
```

### 2. Loading a Scriptable Object

Similarly, you can load a Scriptable Object by calling the `Load()` method.

```csharp
var report = await myScriptableObject.Load();
if (!report.Success) {
    Debug.LogError("Failed to load the object: " + report.FailureReason);
}
```

### 3. Load or Create a Scriptable Object

If the data is not found during loading, you can use `LoadOrCreate()` to reset the object to its default state and save it immediately.

```csharp
var report = await myScriptableObject.LoadOrCreate();
if (!report.Success) {
    Debug.Log("Object was not saved before, reset to default and saved.");
}
```

### 4. Checking if an Object is Saved

To check if an object has already been saved:

```csharp
bool isSaved = await myScriptableObject.IsSaved();
```

### 5. Resetting an Object to Default

You can reset a Scriptable Object to its default state using:

```csharp
myScriptableObject.ResetToDefault();
```

## Advanced Usage

### Custom Serialization

If your Scriptable Object implements `IPersistentCustomSerializable`, you can define custom save/load logic. This is useful for complex objects or specific serialization needs.

### Event Listeners

You can register listeners to be notified when an object is saved or loaded:

```csharp
myScriptableObject.RegisterOnSaveListener(() => Debug.Log("Saved!"));
myScriptableObject.RegisterOnLoadListener(() => Debug.Log("Loaded!"));
```

## Security & Encryption

The system supports optional encryption for saved data using DES encryption. This ensures that your saved data is secure. Encryption can be enabled or disabled via the `SaveSystemSettings`.

## Conclusion

This save system is designed to make saving and loading **Scriptable Objects** in Unity simple, reliable, and secure. Its flexibility and built-in features ensure that it can be adapted to various game requirements.

---

Let me know if you'd like further changes!


## License

MIT License
