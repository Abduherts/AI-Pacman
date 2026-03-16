#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

public class AutoSetup : MonoBehaviour
{
    [MenuItem("Tools/Load Pacman Scene")]
    public static void LoadScene()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Cannot load scene while in Play Mode. Please stop the game first.");
            return;
        }

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            string scenePath = "Assets/Scenes/Pacman.unity";
            EditorSceneManager.OpenScene(scenePath);
            Debug.Log("Loaded Pacman Scene");
        }
    }

    [MenuItem("Tools/Auto Setup")]
    public static void Setup()
    {
        if (Application.isPlaying)
        {
            Debug.LogError("Cannot run Auto Setup while in Play Mode.");
            return;
        }

        // Add Layers
        string[] layers = new string[] { "Pacman", "Ghost", "Pellet", "PowerPellet", "Wall", "Node" };
        SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        SerializedProperty layersProp = tagManager.FindProperty("layers");

        foreach (string layerName in layers)
        {
            bool found = false;
            for (int i = 8; i < layersProp.arraySize; i++)
            {
                SerializedProperty layerSP = layersProp.GetArrayElementAtIndex(i);
                if (layerSP.stringValue == layerName)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                for (int i = 8; i < layersProp.arraySize; i++)
                {
                    SerializedProperty layerSP = layersProp.GetArrayElementAtIndex(i);
                    if (string.IsNullOrEmpty(layerSP.stringValue))
                    {
                        layerSP.stringValue = layerName;
                        Debug.Log("Created Layer: " + layerName);
                        break;
                    }
                }
            }
        }
        
        tagManager.ApplyModifiedProperties();
        Debug.Log("Auto Setup Complete: Layers and Tags checked.");
        
        CreateEssentialObjects();
        AssignGhostRoles();
        ApplyPhysicsSettings();
    }

    private static void ApplyPhysicsSettings()
    {
        // 1. Create Frictionless Material
        string matPath = "Assets/Materials/Frictionless.physicsMaterial2D";
        PhysicsMaterial2D frictionless = AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(matPath);

        if (frictionless == null)
        {
            // Ensure Materials folder exists
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            {
                AssetDatabase.CreateFolder("Assets", "Materials");
            }

            frictionless = new PhysicsMaterial2D("Frictionless");
            frictionless.friction = 0f;
            frictionless.bounciness = 0f;
            AssetDatabase.CreateAsset(frictionless, matPath);
            Debug.Log("Created Frictionless PhysicsMaterial2D");
        }

        // 2. Assign to Pacman
        Pacman p = Object.FindFirstObjectByType<Pacman>();
        if (p != null)
        {
            Rigidbody2D rb = p.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.sharedMaterial = frictionless;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // Better for high speed
                rb.interpolation = RigidbodyInterpolation2D.Interpolate; // Smoother movement
                EditorUtility.SetDirty(rb);
            }

            Collider2D col = p.GetComponent<Collider2D>();
            if (col != null)
            {
                col.sharedMaterial = frictionless;
                EditorUtility.SetDirty(col);
            }
            Debug.Log("Applied Frictionless Material to Pacman");
        }
    }

    private static void AssignGhostRoles()
    {
        Ghost[] ghosts = Object.FindObjectsByType<Ghost>(FindObjectsSortMode.None);
        foreach (Ghost ghost in ghosts)
        {
            if (ghost.name.Contains("Blinky") || ghost.name.Contains("Red"))
                ghost.aiType = GhostType.Aggressive;
            else if (ghost.name.Contains("Pinky") || ghost.name.Contains("Pink"))
                ghost.aiType = GhostType.Intercept;
            else if (ghost.name.Contains("Inky") || ghost.name.Contains("Blue"))
                ghost.aiType = GhostType.Defensive;
            else if (ghost.name.Contains("Clyde") || ghost.name.Contains("Orange"))
                ghost.aiType = GhostType.Random;
            else
                ghost.aiType = GhostType.Aggressive; // Default
            
            EditorUtility.SetDirty(ghost);
        }
        Debug.Log($"Assigned roles to {ghosts.Length} ghosts.");
    }

    private static void CreateEssentialObjects()
    {
        GameManager gm = Object.FindFirstObjectByType<GameManager>();
        if (gm == null)
        {
            GameObject go = new GameObject("GameManager");
            gm = go.AddComponent<GameManager>();
            Debug.Log("Created GameManager");
        }

        if (gm.pellets == null)
        {
            GameObject pelletsObj = GameObject.Find("Pellets");
            if (pelletsObj == null) pelletsObj = GameObject.Find("Nodes"); // Fallback check
            
            if (pelletsObj != null)
            {
                gm.pellets = pelletsObj.transform;
                EditorUtility.SetDirty(gm);
                Debug.Log("Assigned Pellets to GameManager");
            }
            else
            {
                Debug.LogWarning("Could not find 'Pellets' object in scene. Please assign it to GameManager manually.");
            }
        }

        Pacman p = Object.FindFirstObjectByType<Pacman>();
        if (p != null && p.GetComponent<PacmanAI>() == null)
        {
            p.gameObject.AddComponent<PacmanAI>();
            Debug.Log("Added PacmanAI to Pacman");
        }
        
        if (p != null)
        {
             p.controlType = ControlType.HeuristicAI;
             EditorUtility.SetDirty(p);
             Debug.Log("Set Pacman ControlType to HeuristicAI");
        }

        /*
        if (p != null && p.GetComponent<PacmanRLAgent>() == null)
        {
            p.gameObject.AddComponent<PacmanRLAgent>();
            Debug.Log("Added PacmanRLAgent to Pacman");
        }
        */
        
        if (Object.FindFirstObjectByType<PerformanceMetrics>() == null)
        {
             GameObject go = new GameObject("PerformanceMetrics");
             go.AddComponent<PerformanceMetrics>();
             Debug.Log("Created PerformanceMetrics");
        }
    }
}
#endif
