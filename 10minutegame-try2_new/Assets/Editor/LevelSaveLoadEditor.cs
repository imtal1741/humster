using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelSaveLoad))]
public class LevelSaveLoadEditor : Editor
{

    private Texture2D _number;
    private Texture2D _obstacle;
    private Texture2D _other;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        LevelSaveLoad levelSaveLoad = (LevelSaveLoad)target;
        GUIStyle redBackgroundStyle = new GUIStyle(GUI.skin.button);
        redBackgroundStyle.normal.background = MakeBackgroundTexture(10, 10, new Color(0.55f, 0, 0));

        if (GUILayout.Button("Вставить объекты"))
        {
            levelSaveLoad.InsertObjects();
        }
        if (GUILayout.Button("Починить"))
        {
            levelSaveLoad.Repair();
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Сохранить"))
        {
            levelSaveLoad.SaveField();
        }

        if (GUILayout.Button("Load"))
        {
            levelSaveLoad.LoadField();
        }
        GUILayout.EndHorizontal();


        if (GUILayout.Button("Очистить"))
        {
            levelSaveLoad.Clear();
        }


        EditorGUILayout.Space(30f);
        EditorGUILayout.LabelField("Создание объекта");
        EditorGUILayout.Space();

        if (_number == null)
            _number = AssetPreview.GetAssetPreview(Resources.Load("number"));
        if (_obstacle == null)
            _obstacle = AssetPreview.GetAssetPreview(Resources.Load("obstacle"));
        if (_other == null)
            _other = AssetPreview.GetAssetPreview(Resources.Load("other"));


        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("↑", GUILayout.Width(40), GUILayout.Height(40)))
        {
            levelSaveLoad.MoveObject(new Vector2(0, 1));
        }
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("↻", GUILayout.Width(40), GUILayout.Height(40)))
        {
            levelSaveLoad.RotateObject(1);
        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("←", GUILayout.Width(40), GUILayout.Height(40)))
        {
            levelSaveLoad.MoveObject(new Vector2(-1, 0));
        }
        if (GUILayout.Button("↓", GUILayout.Width(40), GUILayout.Height(40)))
        {
            levelSaveLoad.MoveObject(new Vector2(0, -1));
        }
        if (GUILayout.Button("→", GUILayout.Width(40), GUILayout.Height(40)))
        {
            levelSaveLoad.MoveObject(new Vector2(1, 0));
        }

        GUILayout.FlexibleSpace();
        if (GUILayout.Button("↺", GUILayout.Width(40), GUILayout.Height(40)))
        {
            levelSaveLoad.RotateObject(-1);
        }
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();


        GUILayout.Space(10f);
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Удалить", redBackgroundStyle, GUILayout.Width(80), GUILayout.Height(40)))
        {
            levelSaveLoad.DeleteObject();
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();

        GUILayout.Space(20f);

        if (GUILayout.Button(_number, GUILayout.Width(60), GUILayout.Height(60)))
        {

        }

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        for (int i = 0; i < levelSaveLoad.m_numberPrefab.Length; i++)
        {
            if (GUILayout.Button(levelSaveLoad.m_numberPrefab[i].name, GUILayout.Width(60)))
            {
                levelSaveLoad.CreateObjectNumber(i);
            }

            if ((i + 1) % 5 == 0)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();


        GUILayout.Space(20f);


        GUILayout.BeginHorizontal();
        if (GUILayout.Button(_obstacle, GUILayout.Width(60), GUILayout.Height(60)))
        {

        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        for (int i = 0; i < levelSaveLoad.m_obstaclePrefab.Length; i++)
        {
            if (GUILayout.Button(levelSaveLoad.m_obstaclePrefab[i].name, GUILayout.Width(70)))
            {
                levelSaveLoad.CreateObjectObstacle(i);
            }

            if ((i + 1) % 5 == 0)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();


        GUILayout.Space(20f);


        GUILayout.BeginHorizontal();
        if (GUILayout.Button(_other, GUILayout.Width(60), GUILayout.Height(60)))
        {

        }
        GUILayout.EndHorizontal();

        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        for (int i = 0; i < levelSaveLoad.m_otherPrefab.Length; i++)
        {
            if (GUILayout.Button(levelSaveLoad.m_otherPrefab[i].name, GUILayout.Width(120)))
            {
                levelSaveLoad.CreateObjectOther(i);
            }

            if ((i + 1) % 5 == 0)
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();


        serializedObject.ApplyModifiedProperties();
    }


    private Texture2D MakeBackgroundTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        Texture2D backgroundTexture = new Texture2D(width, height);

        backgroundTexture.SetPixels(pixels);
        backgroundTexture.Apply();

        return backgroundTexture;
    }

}
