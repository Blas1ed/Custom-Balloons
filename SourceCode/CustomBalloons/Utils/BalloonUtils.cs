using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;

namespace CustomBalloons.Utils
{
    [System.Serializable]
    public class CustomBalloon
    {
        public GameObject BalloonObject;
        public Transform BalloonKnot;
        public BoxCollider Collider;
    }

    public static class BalloonUtils
    {
        public static string CopyBalloonPath = "Player Objects/Local VRRig/Local Gorilla Player/Holdables/ChocolateDonutBalloonAnchor";
        public static List<CustomBalloon> BalloonAssets = new List<CustomBalloon>();
        public static List<GameObject> Assets = new List<GameObject>();
        public static List<GameObject> CopyBalloons = new List<GameObject>();
       
        public static void SetupAssetBundles(List<AssetBundle> bundles)
        {
            Assets = new List<GameObject>();
            foreach (AssetBundle bundle in bundles)
            {
                GameObject loadedAsset = bundle.LoadAllAssets<GameObject>()[0];
                Assets.Add(loadedAsset);
                Plugin.BalloonNames.Add(loadedAsset.name);
                Debug.Log(loadedAsset.name);
            }
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CustomBalloons.Resources.customballoons");
            Plugin.BalloonsBundle = AssetBundle.LoadFromStream(stream);
            Plugin.BalloonsPad = Plugin.BalloonsBundle.LoadAsset<GameObject>("BalloonsPad");

            foreach (GameObject obj in Assets)
            {
                GameObject obje = BalloonUtils.CopyBalloon(BalloonUtils.CopyBalloonPath);
                CopyBalloons.Add(obje);
                obje.transform.localPosition = Vector3.zero;
                BalloonUtils.FindInParent("CHOCOLATE DONUT BALLOON", obje.transform).name = Plugin.BalloonNames[Assets.IndexOf(obj)];
                CustomBalloon balloonObj = new CustomBalloon();
                balloonObj.BalloonObject = FindInParent(Plugin.BalloonNames[Assets.IndexOf(obj)], obje.transform).gameObject;
                balloonObj.BalloonKnot = FindInParent("Knot", obj.transform);
                balloonObj.Collider = FindInParent("Collider", obj.transform).GetComponent<BoxCollider>();
                BalloonAssets.Add(balloonObj);
                BalloonUtils.FindInParent(Plugin.BalloonNames[Assets.IndexOf(obj)], obje.transform).GetComponent<BalloonHoldable>().myIndex = 62;
                FindInParent(Plugin.BalloonNames[Assets.IndexOf(obj)], obje.transform).GetComponent<BalloonHoldable>().currentState = TransferrableObject.PositionState.OnRightShoulder;
                BalloonUtils.FindInParent(Plugin.BalloonNames[Assets.IndexOf(obj)], obje.transform).GetComponent<BalloonHoldable>().storedZone = BodyDockPositions.DropPositions.RightBack;
                GameObject.Find("Local Gorilla Player").GetComponent<VRRigReliableState>().activeTransferrableObjectIndex[2] = 62;
                FindInParent(Plugin.BalloonNames[Assets.IndexOf(obj)], obje.transform).GetComponent<MeshFilter>().mesh = null;

                BalloonUtils.FindInParent(Plugin.BalloonNames[Assets.IndexOf(obj)], obje.transform).GetComponent<Renderer>().material = obj.GetComponent<Renderer>().material;

                Plugin.ChildBalloonObj = FindInParent(Plugin.BalloonNames[Assets.IndexOf(obj)], obje.transform).gameObject;
                
                FindInParent("Cube (1)", Plugin.ChildBalloonObj.transform).GetComponent<BoxCollider>().size = BalloonAssets[Assets.IndexOf(obj)].Collider.size;
                BalloonAssets[Assets.IndexOf(obj)].Collider.gameObject.SetActive(false);
               GameObject Gameobje = GameObject.Instantiate(obj, Plugin.ChildBalloonObj.transform);
                Plugin.ChildBalloonObj.GetComponent<BalloonString>().startPositionXf = FindInParent("Knot", Gameobje.transform);
            }

            SetupPad();
        }

        public static void SetupPad()
        {
           GameObject PadObj = GameObject.Instantiate(Plugin.BalloonsPad, GameObject.Find("Player Objects/Local VRRig/Local Gorilla Player/rig/body/shoulder.L/upper_arm.L/forearm.L/hand.L").transform);

            Plugin.ActiveBalloonsPad = PadObj;
            List<GameObject> Buttons = new List<GameObject>()
            {
                FindInParent("Left", PadObj.transform).gameObject,
                FindInParent("Right", PadObj.transform).gameObject,
                FindInParent("Equip", PadObj.transform).gameObject,
            };

            foreach (GameObject Button in Buttons)
            {
                Button Btn = Button.AddComponent<Button>();
                if (Button.name != "Equip")
                {
                    Btn.Up = Button.name == "Right"? true : false;
                }
                else
                {
                    Btn.ButtonFunction = "Equip";
                }

            }


            Plugin.DisplayText = FindInParent("Name", PadObj.transform).GetComponent<TextMeshPro>();
            Debug.Log("DisplaySuccess");
            Plugin.ObjectParent = FindInParent("ObjectParent", PadObj.transform);
            Debug.Log("ObjectSuccess");
            foreach (GameObject Asset in Assets)
            {
                GameObject insobj = GameObject.Instantiate(Asset, Plugin.ObjectParent);
                Plugin.ObjectsInParent.Add(insobj);
                insobj.SetActive(false);
            }
        }

        public static void SetCustomBalloon(bool Active, int BalloonInt)
        {
            BalloonAssets[BalloonInt].BalloonObject.SetActive(Active);

            if (Active)
            {
                BalloonAssets[BalloonInt].BalloonObject.GetComponent<BalloonHoldable>().currentState = TransferrableObject.PositionState.OnRightShoulder;
                BalloonAssets[BalloonInt].BalloonObject.GetComponent<BalloonHoldable>().storedZone = BodyDockPositions.DropPositions.RightBack;
            }

        }

        public static GameObject CopyBalloon(string path)
        {
         GameObject Balloon = GameObject.Find(path);
         return GameObject.Instantiate(Balloon, Balloon.transform.parent, true);
        }


        public static Transform FindInParent(string ChildName, Transform Parent)
        {
            foreach (Transform child in Parent)
            {
                if (child.name == ChildName)
                {
                    return child;
                }
            }

            return null;
        }

        public static void SetupBalloons()
        {
            List<AssetBundle> bundles = new List<AssetBundle>();
            string folder = Path.GetDirectoryName(typeof(Plugin).Assembly.Location);
            IEnumerable<string> Files = Directory.GetFiles($"{folder}\\Balloons");

            foreach (string file in Files)
            {
                Debug.Log("File Found In Balloons: " + file);
                string FilePath = $"{file}";
                Debug.Log(FilePath);

                bundles.Add(AssetBundle.LoadFromFile(FilePath));
            }

            SetupAssetBundles(bundles);
        }

        public static IEnumerable<string> GetFileNames(string path, SearchOption searchOption, bool returnShortPath = false)
        {
            IList<string> filePaths = new List<string>();

            
                IEnumerable<string> directoryFiles = Directory.GetFiles(path, "", searchOption);

                if (returnShortPath)
                {
                    foreach (string directoryFile in directoryFiles)
                    {
                        string filePath = directoryFile.Replace(path, "");
                        if (filePath.Length > 0 && filePath.StartsWith(@"\"))
                        {
                            filePath = filePath.Substring(1, filePath.Length - 1);
                        }

                        if (!string.IsNullOrWhiteSpace(filePath) && !filePaths.Contains(filePath))
                        {
                            filePaths.Add(filePath);
                        }
                    }
                }
                else
                {
                    filePaths = filePaths.Union(directoryFiles).ToList();
                }
            

            return filePaths.Distinct();
        }
    }
}
