using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json; // If using JSON.NET; otherwise use Unity's JsonUtility with a small wrapper.

public class Generate3DModel : MonoBehaviour
{
    public string jsonFilename = "layout_for_unity.json";
    public Material wallMaterial;
    public float wallHeight = 2.5f;

    void Start()
    {
        string path = Path.Combine(Application.dataPath, "../Output_Results", jsonFilename);
        if (!File.Exists(path)) { Debug.LogError("JSON not found: " + path); return; }
        var text = File.ReadAllText(path);
        var layout = JsonConvert.DeserializeObject<LayoutRoot>(text);

        foreach (var r in layout.rooms)
        {
            // create floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.transform.position = new Vector3(r.x_m + r.w_m/2f, 0, -(r.y_m + r.h_m/2f)); // convert to Unity coords
            floor.transform.localScale = new Vector3(r.w_m, 0.05f, r.h_m);
            floor.name = "Floor_" + r.id;

            // create walls (simple outer rectangle)
            CreateWall(new Vector3(r.x_m, 0, -r.y_m), new Vector3(r.x_m + r.w_m, 0, -r.y_m)); // top
            CreateWall(new Vector3(r.x_m, 0, -(r.y_m + r.h_m)), new Vector3(r.x_m + r.w_m, 0, -(r.y_m + r.h_m))); // bottom
            CreateWall(new Vector3(r.x_m, 0, -r.y_m), new Vector3(r.x_m, 0, -(r.y_m + r.h_m))); // left
            CreateWall(new Vector3(r.x_m + r.w_m, 0, -r.y_m), new Vector3(r.x_m + r.w_m, 0, -(r.y_m + r.h_m))); // right
        }
    }

    void CreateWall(Vector3 a, Vector3 b)
    {
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 center = (a + b)/2f + new Vector3(0, wallHeight/2f, 0);
        float length = Vector3.Distance(a, b);
        wall.transform.position = center;
        if (Mathf.Approximately(a.x, b.x))
            wall.transform.localScale = new Vector3(0.05f, wallHeight, length);
        else
            wall.transform.localScale = new Vector3(length, wallHeight, 0.05f);
        if (wallMaterial != null) wall.GetComponent<Renderer>().material = wallMaterial;
        wall.name = "Wall";
    }

    [System.Serializable]
    public class Room { public int id; public float x_m; public float y_m; public float w_m; public float h_m; }
    [System.Serializable]
    public class LayoutRoot { public List<Room> rooms; }
}
