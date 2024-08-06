using Il2CppRUMBLE.MoveSystem;
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
        private bool PKeyReleased = true;
        private bool EqualKeyPressed = false;
        private bool EqualKeyReleased = true;

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
            if ((Input.GetKeyDown(KeyCode.Equals)) && (EqualKeyReleased))
            {
                EqualKeyPressed = true;
                EqualKeyReleased = false;
            }
            if ((Input.GetKeyUp(KeyCode.Equals)) && (!EqualKeyReleased))
            {
                EqualKeyReleased = true;
            }
        }

        public override void OnFixedUpdate()
        {
            if (PKeyPressed)
            {
                PKeyPressed = false;
                parentGO = GameObject.Find("StoreHere");
                SaveText();
                SaveCustomMap();
            }
            if (EqualKeyPressed)
            {
                EqualKeyPressed = false;
                LoadCustomMap();
            }
        }
        private void LoadCustomMap()
        {
            try
            {
                string[] fileText = File.ReadAllLines(@"UserData\MapCreator\CustomMap.txt");
                string sentString = "1";
                foreach (string text in fileText)
                {
                    sentString += "|" + text;
                }
                string recievedString = sentString;
                string[] processedString = recievedString.Split('|');
                LoadCustomMap(processedString);
            }
            catch
            {
                MelonLogger.Error(@"Failed to Read Custom Map: UserData\MapCreator\CustomMap.txt");
            }
        }
        
        private void LoadCustomMap(string[] recievedText)
        {
            GameObject mapParent = parentGO;
            MeshCollider meshCollider;
            GroundCollider groundCollider;
            Shader urp = Shader.Find("Universal Render Pipeline/Lit");
            int childCount = int.Parse(recievedText[1]);
            for (int i = 0; i < childCount; i++)
            {
                string primitiveName = recievedText[13 * i + 2];
                float r = float.Parse(recievedText[13 * i + 3], CultureInfo.InvariantCulture);
                float g = float.Parse(recievedText[13 * i + 4], CultureInfo.InvariantCulture);
                float b = float.Parse(recievedText[13 * i + 5], CultureInfo.InvariantCulture);
                float posX = float.Parse(recievedText[13 * i + 6], CultureInfo.InvariantCulture);
                float posY = float.Parse(recievedText[13 * i + 7], CultureInfo.InvariantCulture);
                float posZ = float.Parse(recievedText[13 * i + 8], CultureInfo.InvariantCulture);
                float rotX = float.Parse(recievedText[13 * i + 9], CultureInfo.InvariantCulture);
                float rotY = float.Parse(recievedText[13 * i + 10], CultureInfo.InvariantCulture);
                float rotZ = float.Parse(recievedText[13 * i + 11], CultureInfo.InvariantCulture);
                float scaleX = float.Parse(recievedText[13 * i + 12], CultureInfo.InvariantCulture);
                float scaleY = float.Parse(recievedText[13 * i + 13], CultureInfo.InvariantCulture);
                float scaleZ = float.Parse(recievedText[13 * i + 14], CultureInfo.InvariantCulture);
                PrimitiveType type = PrimitiveType.Cube;
                if (primitiveName.Contains("Capsule"))
                {
                    type = PrimitiveType.Capsule;
                }
                else if (primitiveName.Contains("Cylinder"))
                {
                    type = PrimitiveType.Cylinder;
                }
                else if (primitiveName.Contains("Plane"))
                {
                    type = PrimitiveType.Plane;
                }
                else if (primitiveName.Contains("Quad"))
                {
                    type = PrimitiveType.Quad;
                }
                else if (primitiveName.Contains("Sphere"))
                {
                    type = PrimitiveType.Sphere;
                }
                GameObject shape = GameObject.CreatePrimitive(type);
                shape.name = primitiveName;
                if (type == PrimitiveType.Cube)
                {
                    Component.Destroy(shape.GetComponent<BoxCollider>());
                }
                else if ((type == PrimitiveType.Capsule) || (type == PrimitiveType.Cylinder))
                {
                    Component.Destroy(shape.GetComponent<CapsuleCollider>());
                }
                else if (type == PrimitiveType.Sphere)
                {
                    Component.Destroy(shape.GetComponent<SphereCollider>());
                }
                if (primitiveName.ToLower().Contains("wall"))
                {
                    shape.AddComponent<BoxCollider>();
                }
                if (primitiveName.ToLower().Contains("invisible"))
                {
                    Component.Destroy(shape.GetComponent<MeshRenderer>());
                }
                else
                {
                    shape.GetComponent<Renderer>().material.shader = urp;
                    shape.GetComponent<Renderer>().material.color = new Color(r, g, b);
                }
                if (primitiveName.ToLower().Contains("environment"))
                {
                    shape.layer = 10;
                }
                else
                {
                    shape.layer = 9;
                }
                shape.transform.parent = mapParent.transform;
                shape.transform.position = new Vector3(posX, posY, posZ);
                shape.transform.rotation = Quaternion.Euler(rotX, rotY, rotZ);
                shape.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                if ((type == PrimitiveType.Plane) || (type == PrimitiveType.Quad))
                {
                    meshCollider = shape.GetComponent<MeshCollider>();
                }
                else
                {
                    meshCollider = shape.AddComponent<MeshCollider>();
                }
                groundCollider = shape.AddComponent<GroundCollider>();
                groundCollider.isMainGroundCollider = true;
                groundCollider.collider = meshCollider;
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
                saveText.Add($"{primitive.name}");
                float r, g, b;
                if (!primitive.name.ToLower().Contains("invisible"))
                {
                    r = primitive.GetComponent<Renderer>().material.color.r;
                    g = primitive.GetComponent<Renderer>().material.color.g;
                    b = primitive.GetComponent<Renderer>().material.color.b;
                }
                else
                {
                    r = 0;
                    g = 0;
                    b = 0;
                }
                saveText.Add($"{r.ToString(CultureInfo.InvariantCulture)}|{g.ToString(CultureInfo.InvariantCulture)}|{b.ToString(CultureInfo.InvariantCulture)}");
                saveText.Add($"{primitive.transform.position.x.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.position.y.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.position.z.ToString(CultureInfo.InvariantCulture)}");
                saveText.Add($"{primitive.transform.rotation.eulerAngles.x.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.rotation.eulerAngles.y.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.rotation.eulerAngles.z.ToString(CultureInfo.InvariantCulture)}");
                saveText.Add($"{primitive.transform.localScale.x.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.localScale.y.ToString(CultureInfo.InvariantCulture)}|{primitive.transform.localScale.z.ToString(CultureInfo.InvariantCulture)}");
            }
            File.WriteAllLines(@"UserData\MapCreator\CustomMap.txt", saveText);
            MelonLogger.Msg("CustomMap File Saved");
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
                    continue;
                }
                saveText.Add($"shape = GameObject.CreatePrimitive({primitiveType});");
                saveText.Add($"shape.name = \"{primitive.name}\";");
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
                if (primitive.name.ToLower().Contains("wall"))
                {
                    saveText.Add("shape.AddComponent<BoxCollider>();");
                }
                if (primitive.name.ToLower().Contains("invisible"))
                {
                    saveText.Add("Component.Destroy(shape.GetComponent<MeshRenderer>());");
                }
                else
                {
                    saveText.Add($"shape.GetComponent<Renderer>().material.shader = urp;");
                    float r, g, b;
                    r = primitive.GetComponent<Renderer>().material.color.r;
                    g = primitive.GetComponent<Renderer>().material.color.g;
                    b = primitive.GetComponent<Renderer>().material.color.b;
                    saveText.Add($"shape.GetComponent<Renderer>().material.color = new Color({r.ToString(CultureInfo.InvariantCulture)}f, {g.ToString(CultureInfo.InvariantCulture)}f, {b.ToString(CultureInfo.InvariantCulture)}f); ");
                }
                if (primitive.name.ToLower().Contains("environment"))
                {
                    saveText.Add("shape.layer = 10;");
                }
                else
                {
                    saveText.Add("shape.layer = 9;");
                }
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
            MelonLogger.Msg("Map File Saved");
        }
    }
}
