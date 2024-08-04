using Il2CppTMPro;
using MelonLoader;
using RumbleModdingAPI;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace CustomMapCreatorTool
{
    public class main : MelonMod
    {
        private GameObject parentGO, primitiveStorage;
        private bool PKeyPressed = false;
        private bool PKeyReleased = false;

        public override void OnLateInitializeMelon()
        {
            Calls.onMapInitialized += Init;
            if (!File.Exists(@"UserData\MapCreator\CustomMap.txt"))
            {
                File.Create(@"UserData\MapCreator\CustomMap.txt");
            }
            if (!File.Exists(@"UserData\MapCreator\Map.txt"))
            {
                File.Create(@"UserData\MapCreator\Map.txt");
            }
        }

        private void Init()
        {
            SpawnPrimitives();
        }

        public override void OnUpdate()
        {
            if ((Input.GetKeyDown(KeyCode.P)) && (PKeyReleased))
            {
                PKeyPressed = true;
                PKeyReleased = false;
            }
            if ((Input.GetKeyUp(KeyCode.P)) && (!PKeyReleased))
            {
                PKeyReleased = true;
            }
        }

        public override void OnFixedUpdate()
        {
            if (PKeyPressed)
            {
                SaveText();
                SaveCustomMap();
                PKeyPressed = false;
            }
        }

        private void SpawnPrimitives()
        {
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
            capsule.layer = 9;
            capsule.transform.parent = primitiveStorage.transform;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
            cube.layer = 9;
            cube.transform.parent = primitiveStorage.transform;
            GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylinder.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
            cylinder.layer = 9;
            cylinder.transform.parent = primitiveStorage.transform;
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            plane.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
            plane.layer = 9;
            plane.transform.parent = primitiveStorage.transform;
            GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            quad.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
            quad.layer = 9;
            quad.transform.parent = primitiveStorage.transform;
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.GetComponent<Renderer>().material.shader = Shader.Find("Universal Render Pipeline/Lit");
            sphere.layer = 9;
            sphere.transform.parent = primitiveStorage.transform;
            primitiveStorage.transform.position = new Vector3(0, -10, 0);
            primitiveStorage.SetActive(false);
        }

        public override void OnSceneWasLoaded(int buildIndex, string sceneName)
        {
            parentGO = new GameObject();
            parentGO.name = "StoreHere";
            primitiveStorage = new GameObject();
            primitiveStorage.name = "PrimitiveStorage";
        }

        public void SaveCustomMap()
        {
            List<string> saveText = new List<string>();
            saveText.Add($"{parentGO.transform.childCount}");
            for (int i = 0; i < parentGO.transform.childCount; i++)
            {
                GameObject primitive = parentGO.transform.GetChild(i).gameObject;
                string primitiveType;
                if (primitive.name.Contains("Capsule"))
                {
                    primitiveType = "PrimitiveType.Capsule";
                }
                else if (primitive.name.Contains("Cube"))
                {
                    primitiveType = "PrimitiveType.Cube";
                }
                else if (primitive.name.Contains("Cylinder"))
                {
                    primitiveType = "PrimitiveType.Cylinder";
                }
                else if (primitive.name.Contains("Plane"))
                {
                    primitiveType = "PrimitiveType.Plane";
                }
                else if (primitive.name.Contains("Quad"))
                {
                    primitiveType = "PrimitiveType.Quad";
                }
                else if (primitive.name.Contains("Sphere"))
                {
                    primitiveType = "PrimitiveType.Sphere";
                }
                else
                {
                    primitiveType = primitive.name;
                }
                saveText.Add($"{primitiveType}");
                float r, g, b;
                r = primitive.GetComponent<Renderer>().material.color.r;
                g = primitive.GetComponent<Renderer>().material.color.g;
                b = primitive.GetComponent<Renderer>().material.color.b;
                saveText.Add($"{r.ToString(CultureInfo.InvariantCulture)}|{g.ToString(CultureInfo.InvariantCulture)}|{b.ToString(CultureInfo.InvariantCulture)}");
                saveText.Add($"{primitive.transform.position.x.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.position.y.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.position.z.ToString(CultureInfo.InvariantCulture)}");
                saveText.Add($"{primitive.transform.rotation.eulerAngles.x.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.rotation.eulerAngles.y.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.rotation.eulerAngles.z.ToString(CultureInfo.InvariantCulture)}");
                saveText.Add($"{primitive.transform.localScale.x.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.localScale.y.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.localScale.z.ToString(CultureInfo.InvariantCulture)}");
            }
            File.WriteAllLines(@"UserData\MapCreator\CustomMap.txt", saveText);
        }

        public void SaveText()
        {
            List<string> saveText = new List<string>();
            saveText.Add("private void LoadMap()");
            saveText.Add("{");
            saveText.Add("GameObject mapParent = new GameObject();");
            saveText.Add("mapParent.name = \"mapParent\";");
            saveText.Add("mapParent.transform.parent = mapsParent.transform;");
            saveText.Add("MeshCollider meshCollider;");
            saveText.Add("GroundCollider groundCollider;");
            saveText.Add($"Shader urp = Shader.Find(\"Universal Render Pipeline/Lit\");");
            saveText.Add("GameObject shape;");
            for (int i = 0; i < parentGO.transform.childCount; i++)
            {
                GameObject primitive = parentGO.transform.GetChild(i).gameObject;
                string primitiveType;
                if (primitive.name.Contains("Capsule"))
                {
                    primitiveType = "PrimitiveType.Capsule";
                }
                else if (primitive.name.Contains("Cube"))
                {
                    primitiveType = "PrimitiveType.Cube";
                }
                else if (primitive.name.Contains("Cylinder"))
                {
                    primitiveType = "PrimitiveType.Cylinder";
                }
                else if (primitive.name.Contains("Plane"))
                {
                    primitiveType = "PrimitiveType.Plane";
                }
                else if (primitive.name.Contains("Quad"))
                {
                    primitiveType = "PrimitiveType.Quad";
                }
                else if (primitive.name.Contains("Sphere"))
                {
                    primitiveType = "PrimitiveType.Sphere";
                }
                else
                {
                    primitiveType = primitive.name;
                }
                saveText.Add($"shape = GameObject.CreatePrimitive({primitiveType});");
                if (primitiveType == "PrimitiveType.Cube")
                {
                    saveText.Add("Component.Destroy(shape.GetComponent<BoxCollider>());");
                }
                else if ((primitiveType == "PrimitiveType.Capsule") || (primitiveType == "PrimitiveType.Cylinder"))
                {
                    saveText.Add("Component.Destroy(shape.GetComponent<CapsuleCollider>());");
                }
                else if (primitiveType == "PrimitiveType.Sphere")
                {
                    saveText.Add("Component.Destroy(shape.GetComponent<SphereCollider>());");
                }
                saveText.Add($"shape.GetComponent<Renderer>().material.shader = urp;");
                float r, g, b;
                r = primitive.GetComponent<Renderer>().material.color.r;
                g = primitive.GetComponent<Renderer>().material.color.g;
                b = primitive.GetComponent<Renderer>().material.color.b;
                saveText.Add($"shape.GetComponent<Renderer>().material.color = new Color({r.ToString(CultureInfo.InvariantCulture)}f, {g.ToString(CultureInfo.InvariantCulture)}f, {b.ToString(CultureInfo.InvariantCulture)}f); ");
                saveText.Add("shape.layer = 9;");
                saveText.Add("shape.transform.parent = mapParent.transform;");
                saveText.Add($"shape.transform.position = new Vector3({primitive.transform.position.x.ToString(CultureInfo.InvariantCulture)}f, {primitive.transform.position.y.ToString(CultureInfo.InvariantCulture)}f, {primitive.transform.position.z.ToString(CultureInfo.InvariantCulture)}f);");
                saveText.Add($"shape.transform.rotation = Quaternion.Euler({primitive.transform.rotation.eulerAngles.x.ToString(CultureInfo.InvariantCulture)}f, {primitive.transform.rotation.eulerAngles.y.ToString(CultureInfo.InvariantCulture)}f, {primitive.transform.rotation.eulerAngles.z.ToString(CultureInfo.InvariantCulture)}f);");
                saveText.Add($"shape.transform.localScale = new Vector3({primitive.transform.localScale.x.ToString(CultureInfo.InvariantCulture)}f, {primitive.transform.localScale.y.ToString(CultureInfo.InvariantCulture)}f, {primitive.transform.localScale.z.ToString(CultureInfo.InvariantCulture)}f);");
                if ((primitiveType == "PrimitiveType.Plane") || (primitiveType == "PrimitiveType.Quad"))
                {
                    saveText.Add("meshCollider = shape.GetComponent<MeshCollider>();");
                }
                else
                {
                    saveText.Add("meshCollider = shape.AddComponent<MeshCollider>();");
                }
                saveText.Add("groundCollider = shape.AddComponent<GroundCollider>();");
                saveText.Add("groundCollider.isMainGroundCollider = true;");
                saveText.Add("groundCollider.collider = meshCollider;");
            }
            saveText.Add("mapParent.SetActive(false);");
            saveText.Add("}");
            File.WriteAllLines(@"UserData\MapCreator\Map.txt", saveText);
        }
    }
}
