using UnityEngine;
using UnityEditor;

public class DialogueEditorWindow : EditorWindow
{
    private Vector2 scrollPosition;

    private DialogueGraph currentGraph;
    
    [MenuItem("Window/Dialogue/Dialogue Editor")]
    public static void OpenWindow()
    {
        DialogueEditorWindow window = GetWindow<DialogueEditorWindow>();
        window.titleContent = new GUIContent("Dialogue Editor");
    }

    private void OnGUI()
    {
        DrawGrid();
        DrawToolbar();

        scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

        GUILayout.Space(10);

        if (currentGraph == null)
        {
            GUILayout.Label("Select a Dialogue Graph", EditorStyles.boldLabel);
        }
        else
        {
            GUILayout.Label("Editing: " + currentGraph.name, EditorStyles.boldLabel);
        }
        
        DrawNodes();
        ProcessEvents(Event.current);
        EditorGUILayout.EndScrollView();
    }


    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        currentGraph = (DialogueGraph)EditorGUILayout.ObjectField(currentGraph, typeof(DialogueGraph), false, GUILayout.Width(250));
        EditorGUILayout.EndHorizontal();
    }

    private void DrawNodes()
    {
        BeginWindows();

        for (int i = 0;
             i < currentGraph.nodes.Count;
             i++)
        {
            DialogueNodeData node = currentGraph.nodes[i];

            Rect nodeRect = new Rect(node.position.x, node.position.y, 250, 150
                );

            nodeRect = GUI.Window(i, nodeRect, DrawNodeWindow, node.characterID);

            // saves new position
            node.position = nodeRect.position;
        }

        EndWindows();
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(currentGraph);
        }
    }

    private void DrawNodeWindow(int index)
    {
        DialogueNodeData node = currentGraph.nodes[index];
        
        GUILayout.Label("NodeID: " + node.nodeID);

        GUILayout.Space(5);

        GUILayout.Label("Character: " + node.characterID);

        GUILayout.Space(5);

        GUILayout.Label(node.dialogueText, EditorStyles.wordWrappedLabel);

        GUILayout.FlexibleSpace();
        
        GUI.DragWindow(); //enables dragging
    }
    
    private void DrawGrid()
    {
        int gridSpacing = 20;
        int gridOpacity = 15;

        Color gridColor = new Color(0f, 0f, 0f, gridOpacity / 255f);

        Handles.BeginGUI();

        Handles.color = gridColor;

        for (int x = 0; x < position.width; x += gridSpacing)
        {
            Handles.DrawLine(new Vector3(x, 0, 0), new Vector3(x, position.height, 0));
        }

        for (int y = 0; y < position.height; y += gridSpacing)
        {
            Handles.DrawLine(new Vector3(0, y, 0), new Vector3(position.width, y, 0));
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }
    
    private void ProcessEvents(Event e)
    {
        if (e.type == EventType.ContextClick)
        {
            Vector2 mousePosition = e.mousePosition;
            ShowContextMenu(mousePosition);
        }
    }
    
    private void ShowContextMenu(Vector2 position)
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Node"), false, () => CreateNode(position));
        menu.ShowAsContext();
    }
    
    private void CreateNode(Vector2 position)
    {
        if (currentGraph == null) return;

        DialogueNodeData newNode = new DialogueNodeData();

        newNode.nodeID = System.Guid.NewGuid().ToString();
        newNode.characterID = "New Character";
        newNode.dialogueText = "New Dialogue";
        newNode.position = position;

        currentGraph.nodes.Add(newNode);

        EditorUtility.SetDirty(currentGraph);
    }
}
