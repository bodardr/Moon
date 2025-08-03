using System;
using System.Collections.Generic;
using System.Linq;
using Loadout.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static Loadout.LoadoutLoader;
using Object = UnityEngine.Object;

public static class LoadoutSettingsProvider
{
    private static Dictionary<Type, Func<object, string, Action<object>, VisualElement>> uiToolkitFields = new()
    {
        {
            typeof(bool),
            (obj, label, callback) =>
            {
                var field = new Toggle(label);
                field.SetValueWithoutNotify((bool)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(int),
            (obj, label, callback) =>
            {
                var field = new IntegerField(label);
                field.SetValueWithoutNotify((int)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(float),
            (obj, label, callback) =>
            {
                var field = new FloatField(label);
                field.SetValueWithoutNotify((float)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(double),
            (obj, label, callback) =>
            {
                var field = new DoubleField(label);
                field.SetValueWithoutNotify((double)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(string),
            (obj, label, callback) =>
            {
                var field = new TextField(label);
                field.SetValueWithoutNotify((string)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(Enum),
            (obj, label, callback) =>
            {
                var field = new EnumField(label, (Enum)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(Vector2),
            (obj, label, callback) =>
            {
                var field = new Vector2Field(label);
                field.SetValueWithoutNotify((Vector2)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(Vector3),
            (obj, label, callback) =>
            {
                var field = new Vector3Field(label);
                field.SetValueWithoutNotify((Vector3)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(Vector4),
            (obj, label, callback) =>
            {
                var field = new Vector4Field(label);
                field.SetValueWithoutNotify((Vector4)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(Color),
            (obj, label, callback) =>
            {
                var field = new ColorField(label);
                field.SetValueWithoutNotify((Color)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(AnimationCurve),
            (obj, label, callback) =>
            {
                var field = new CurveField(label);
                field.SetValueWithoutNotify((AnimationCurve)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(Object),
            (obj, label, callback) =>
            {
                var field = new ObjectField(label)
                {
                    objectType = obj?.GetType() ?? typeof(Object)
                };
                field.SetValueWithoutNotify((Object)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(Rect),
            (obj, label, callback) =>
            {
                var field = new RectField(label);
                field.SetValueWithoutNotify((Rect)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        },
        {
            typeof(Bounds),
            (obj, label, callback) =>
            {
                var field = new BoundsField(label);
                field.SetValueWithoutNotify((Bounds)obj);
                field.RegisterValueChangedCallback(evt => callback(evt.newValue));
                return field;
            }
        }
    };

    private static readonly List<(LoadoutAttribute, SerializedObject)> serializedObjects = new();
    private static readonly List<ILoadoutElement> keys = new();

    [SettingsProvider]
    public static SettingsProvider CreateTypesSettingsProvider()
    {
        var provider = new SettingsProvider("Project/Loadout/Types", SettingsScope.Project)
        {
            label = "Loadout Types",
            activateHandler = DrawTypesSettingsProvider,
            deactivateHandler = SaveTypes
        };

        return provider;
    }

    private static void DrawTypesSettingsProvider(string searchContext, VisualElement rootElement)
    {
        var title = new Label("Loadout Types")
        {
            style = { fontSize = 24 }
        };
        rootElement.Add(title);

        var types = GetAllTypes();

        var loadoutSave = GetLoadoutSave();
        keys.Clear();
        foreach (var type in types)
        {
            var drawType = type;

            if (type.IsEnum)
                drawType = typeof(Enum);
            else if (typeof(Object).IsAssignableFrom(type))
                drawType = typeof(Object);

            var controlInvoker = uiToolkitFields[drawType];
            var loadoutAttribute = (LoadoutAttribute)type.GetCustomAttributes(typeof(LoadoutAttribute), true)[0];

            ILoadoutElement loadoutElement;
            switch (loadoutAttribute.storageType)
            {
                case LoadoutAttribute.StorageType.File:
                    loadoutElement = new LoadoutElementFile(type);
                    break;
                default:
                case LoadoutAttribute.StorageType.EditorPrefs:
                    loadoutElement = new LoadoutKeyPrefs(type);
                    break;
            }
            loadoutElement.Load(loadoutSave);
            keys.Add(loadoutElement);

            var container = new VisualElement
            {
                style =
                {
                    alignContent = new(Align.Center),
                    flexDirection = new(FlexDirection.Row),
                    marginBottom = 4,
                    marginTop = 4,
                    marginLeft = 4,
                    marginRight = 4
                },
            };

            var toggle = new Toggle
            {
                style =
                {
                    marginRight = 12,
                    marginBottom = 0
                },
                value = loadoutElement.Active
            };

            toggle.RegisterValueChangedCallback(val => loadoutElement.Active = val.newValue);
            container.Add(toggle);

            var control = controlInvoker(loadoutElement.Value, $"<b>{type.DeclaringType}</b>",
                val => loadoutElement.Value = val);
            container.Add(control);

            rootElement.Add(container);
        }
    }

    private static void SaveTypes()
    {
        var save = GetLoadoutSave();
        foreach (var key in keys)
            key.Save(save);

        save.Save();
    }

    [SettingsProvider]
    public static SettingsProvider CreateKeysSettingsProvider()
    {
        var provider = new SettingsProvider("Project/Loadout/Keys", SettingsScope.Project)
        {
            label = "Loadout Keys",
            activateHandler = DrawKeysSettingsProvider,
            deactivateHandler = SaveKeys
        };

        return provider;
    }

    private static void DrawKeysSettingsProvider(string searchContext, VisualElement rootElement)
    {
        var title = new Label("Loadout Keys")
        {
            style = { fontSize = 24 }
        };
        rootElement.Add(title);

        var fields = GetAllFields();
        var loadoutSave = GetLoadoutSave();
        keys.Clear();

        foreach (var field in fields)
        {
            var type = field.FieldType;

            if (type.IsEnum)
                type = typeof(Enum);
            else if (typeof(Object).IsAssignableFrom(type))
                type = typeof(Object);

            var controlInvoker = uiToolkitFields[type];
            var loadoutAttribute = (LoadoutAttribute)field.GetCustomAttributes(typeof(LoadoutAttribute), true)[0];

            ILoadoutElement loadoutElement;
            switch (loadoutAttribute.storageType)
            {
                case LoadoutAttribute.StorageType.File:
                    loadoutElement = new LoadoutElementFile(field);
                    break;
                default:
                case LoadoutAttribute.StorageType.EditorPrefs:
                    loadoutElement = new LoadoutKeyPrefs(field);
                    break;
            }
            loadoutElement.Load(loadoutSave);
            keys.Add(loadoutElement);

            var container = new VisualElement
            {
                style =
                {
                    alignContent = new(Align.Center),
                    flexDirection = new(FlexDirection.Row),
                    marginBottom = 4,
                    marginTop = 4,
                    marginLeft = 4,
                    marginRight = 4
                },
            };

            var toggle = new Toggle
            {
                style =
                {
                    marginRight = 12,
                    marginBottom = 0
                },
                value = loadoutElement.Active
            };

            toggle.RegisterValueChangedCallback(val => loadoutElement.Active = val.newValue);
            container.Add(toggle);

            var control = controlInvoker(loadoutElement.Value, $"<b>{field.DeclaringType}</b> {field.Name}",
                val => loadoutElement.Value = val);
            container.Add(control);

            rootElement.Add(container);
        }
    }

    private static void SaveKeys()
    {
        var save = GetLoadoutSave();
        foreach (var key in keys)
            key.Save(save);

        save.Save();
    }
}
