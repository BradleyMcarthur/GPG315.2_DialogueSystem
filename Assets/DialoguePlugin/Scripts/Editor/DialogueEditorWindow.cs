using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DialogueEditorWindow : EditorWindow
{
    private DialogueGraph currentGraph;
    
    private Vector2 scrollPosition;
    private Vector2 panOffset = Vector2.zero;
    private Vector2 drag;
    
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

        if(currentGraph == null)
            return;
        
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
        
        DrawConnections();
        DrawNodes();
        ProcessEvents(Event.current);
        EditorGUILayout.EndScrollView();
    }

    private void DrawGrid() //creates background grid for window
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
    
    private void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
        currentGraph = (DialogueGraph)EditorGUILayout.ObjectField(currentGraph, typeof(DialogueGraph), false, GUILayout.Width(250));
        
        //Graph save and load functions
        if (GUILayout.Button("New Graph", EditorStyles.toolbarButton))
        {
            CreateNewGraph();
        }
        if (GUILayout.Button("Save Graph", EditorStyles.toolbarButton))
        {
            SaveGraph();
        }
        if (GUILayout.Button("Load Graph", EditorStyles.toolbarButton))
        {
            LoadGraph();
        }
        if (GUILayout.Button("Export JSON", EditorStyles.toolbarButton))
        {
            ExportJSON();
        }

        if (GUILayout.Button("Import JSON", EditorStyles.toolbarButton))
        {
            ImportJSON();
        }
        
        //this was more helpful in testing/development but left it for users as a quality of life. just sets back to Vector.zero
        if (GUILayout.Button("Reset View", EditorStyles.toolbarButton))
        {
            panOffset = Vector2.zero;
        }
        EditorGUILayout.EndHorizontal();
    }

    #region Create/Save/Load functions for managing graphs to save/load both internally and to export/import also
    void CreateNewGraph()
    {
        currentGraph = CreateInstance<DialogueGraph>();
        currentGraph.nodes = new List<DialogueNodeData>();
    }
    void SaveGraph()
    {
        if (currentGraph == null)
            return;

        string path = EditorUtility.SaveFilePanelInProject("Save Dialogue Graph", "NewDialogueGraph", "asset", "Save dialogue graph");

        if (string.IsNullOrEmpty(path))
            return;

        AssetDatabase.CreateAsset(currentGraph, path);
        AssetDatabase.SaveAssets();
    }
    void LoadGraph()
    {
        string path = EditorUtility.OpenFilePanel("Load Dialogue Graph", "Assets", "asset");

        if (string.IsNullOrEmpty(path))
            return;
        
        path = "Assets" + path.Substring(Application.dataPath.Length);

        currentGraph = AssetDatabase.LoadAssetAtPath<DialogueGraph>(path);
    }
    void ExportJSON()
    {
        if (currentGraph == null) return;

        string json = JsonUtility.ToJson(currentGraph, true);
        string path = EditorUtility.SaveFilePanel("Export Dialogue JSON", "", "DialogueGraph.json", "json");

        if (string.IsNullOrEmpty(path)) return;
        System.IO.File.WriteAllText(path, json);
    }
    void ImportJSON()
    {
        string path = EditorUtility.OpenFilePanel("Import Dialogue JSON", "", "json");
        if (string.IsNullOrEmpty(path)) return;
        
        string json = System.IO.File.ReadAllText(path);

        currentGraph = ScriptableObject.CreateInstance<DialogueGraph>();

        JsonUtility.FromJsonOverwrite(json, currentGraph);
    }

    #endregion
    
    private void ProcessEvents(Event e) //handles panning camera and click events like making new nodes
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDrag:
                if (e.button == 2) // middle mouse button
                {
                    OnDrag(e.delta);
                }
                break;

            case EventType.ContextClick:
                ShowContextMenu(e.mousePosition);
                break;
        }
    }

    private void DrawNodes()
    {
        BeginWindows();
        for (int i = 0; i < currentGraph.nodes.Count; i++)
        {
            DialogueNodeData node = currentGraph.nodes[i];

            // Apply pan offset when drawing
            Rect nodeRect = new Rect(node.position.x + panOffset.x, node.position.y + panOffset.y, 250, 250);
            
            Rect newRect = GUI.Window(i, nodeRect, DrawNodeWindow, node.characterID);

            // Removes pan offset when saving position
            node.position = newRect.position - panOffset;
        }
        EndWindows();
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(currentGraph);
        }
    }//Creates the actual nodes in the editor window and populates them with info using DrawNodeWindow function

    private void DrawNodeWindow(int index)
    {
        DialogueNodeData node = currentGraph.nodes[index];
        
        // Starts Scroll Area
        node.scrollPosition = EditorGUILayout.BeginScrollView(node.scrollPosition);
        
        node.isStartNode = EditorGUILayout.Toggle("Start Node", node.isStartNode);
        
        //Fill in labels and text fields for Ids, character names, dialogue text and button inputs
        GUILayout.Label("NodeID: " + node.nodeID);
        node.nodeID = EditorGUILayout.TextField(node.nodeID);

        GUILayout.Space(5);

        GUILayout.Label("Character:");
        node.characterID = EditorGUILayout.TextField(node.characterID);

        GUILayout.Space(5);

        //This feels a bit clunky right now and makes the box pretty big but it has a check where if you dont input one it defaults to english so you dont have to fill all out.
        GUILayout.Label("Dialogue (EN):");
        node.dialogue_EN = EditorGUILayout.TextArea(node.dialogue_EN, GUILayout.Height(20));
        GUILayout.Label("Dialogue (FR):");
        node.dialogue_FR = EditorGUILayout.TextArea(node.dialogue_FR, GUILayout.Height(20));
        GUILayout.Label("Dialogue (JP):");
        node.dialogue_JP = EditorGUILayout.TextArea(node.dialogue_JP, GUILayout.Height(20));
        
        GUILayout.Space(5);
        
        GUILayout.Label("Choices:");
        for (int i = 0; i < node.choices.Count; i++)
        {
            EditorGUILayout.BeginVertical("box");
            
            node.choices[i] = EditorGUILayout.TextField("Choice:", node.choices[i]);
            
            node.nextNodeIDs[i] = DrawNextNodeDropdown(node.nextNodeIDs[i]);
            
            if (GUILayout.Button("Remove Choice"))
            {
                node.choices.RemoveAt(i);
                node.nextNodeIDs.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("+ Add Choice"))
        {
            node.choices.Add("New Choice");
            node.nextNodeIDs.Add("");
        }
        
        if (GUILayout.Button("Delete Node"))
        {
            currentGraph.nodes.RemoveAt(index);
            return;
        }
        
        //Ends scroll area, found they got big pretty fast so added this later on
        EditorGUILayout.EndScrollView();
        
        GUI.DragWindow(); //enables dragging around
        EditorUtility.SetDirty(currentGraph);
    }
    
    private string DrawNextNodeDropdown(string currentID)
    {
        if (currentGraph == null)
            return currentID;

        // Creates a list of node IDs to use in dropdown
        string[] nodeIDs = new string[currentGraph.nodes.Count];

        for (int i = 0; i < currentGraph.nodes.Count; i++)
        {
            nodeIDs[i] = currentGraph.nodes[i].nodeID;
        }
        
        int selectedIndex = System.Array.IndexOf(nodeIDs, currentID);

        if (selectedIndex < 0) selectedIndex = 0;

        selectedIndex = EditorGUILayout.Popup("Next Node", selectedIndex, nodeIDs);

        return nodeIDs[selectedIndex];
    }
    
    private void DrawConnections() //had to look up Beziers in order to get the effect i wanted with the dynamic lines drawing to each node, looks similar unreal engine style
    {
        if (currentGraph == null)
            return;

        Handles.BeginGUI();

        for (int i = 0; i < currentGraph.nodes.Count; i++)
        {
            DialogueNodeData node = currentGraph.nodes[i];

            Vector2 startPos = node.position + panOffset + new Vector2(250, 90);

            for (int c = 0; c < node.nextNodeIDs.Count; c++)
            {
                string nextID = node.nextNodeIDs[c];

                DialogueNodeData target = currentGraph.nodes.Find(n => n.nodeID == nextID);

                if (target != null)
                {
                    Vector2 endPos = target.position + panOffset + new Vector2(0, 90);

                    Handles.DrawBezier(startPos, endPos, startPos + Vector2.right * 50, endPos + Vector2.left * 50, Color.white, null, 2f);
                }
            }
        }

        Handles.EndGUI();
    }
    
    private void OnDrag(Vector2 delta) //allows camera panning
    {
        drag = delta;

        panOffset += drag;

        panOffset.x = Mathf.Clamp(panOffset.x, -2000, 2000);

        panOffset.y = Mathf.Clamp(panOffset.y, -2000, 2000);

        GUI.changed = true;
    }
    
    private void ShowContextMenu(Vector2 position) //where to access adding new nodes function below
    {
        GenericMenu menu = new GenericMenu();
        menu.AddItem(new GUIContent("Create Node"), false, () => CreateNode(position - panOffset));
        menu.ShowAsContext();
    }
    
    private void CreateNode(Vector2 position)
    {
        if (currentGraph == null) return;

        DialogueNodeData newNode = new DialogueNodeData();

        newNode.nodeID = "New Node";
        newNode.characterID = "New Character";
        newNode.dialogue_EN = "";
        newNode.dialogue_FR = "";
        newNode.dialogue_JP = "";
        newNode.position = position;

        currentGraph.nodes.Add(newNode);

        EditorUtility.SetDirty(currentGraph);
    }
}
