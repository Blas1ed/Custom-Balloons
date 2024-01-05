using BepInEx;
using CustomBalloons.Utils;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using Utilla;
using ComputerPlusPlus;

namespace CustomBalloons
{
    [BepInDependency("org.legoandmars.gorillatag.utilla", "1.5.0")]
    [BepInDependency("com.kylethescientist.gorillatag.computerplusplus", "1.0.1")]
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]
    public class Plugin : BaseUnityPlugin
    {
        bool inRoom;
        public static Plugin Instance;
        public static GameObject ChildBalloonObj;
        public static AssetBundle BalloonsBundle;
        public static List<string> BalloonNames = new List<string>();
        public static List<GameObject> ObjectsInParent = new List<GameObject>();
        public GameObject CurrentSelectedObject = new GameObject();
        public static TextMeshPro DisplayText = new TextMeshPro();
        public static int CurrentBalloonTab = 0;
        public static Transform ObjectParent;
        public static GameObject BalloonsPad = new GameObject();
        public static GameObject ActiveBalloonsPad = new GameObject();
        public bool Enabled = false;
        public static Material GreenMat;
        public static Material RedMat;
        void Start()
        {
            Instance = this;
            Utilla.Events.GameInitialized += OnGameInitialized;
            string folder = Path.GetDirectoryName(typeof(Plugin).Assembly.Location);
            Debug.Log(folder);
            if (!Directory.Exists(folder + "\\Balloons"))
            {
                Directory.CreateDirectory(folder + "\\Balloons");
            }
        }

        void OnEnable()
        {
            ActiveBalloonsPad.SetActive(true);
        }

        void OnDisable()
        {
            ActiveBalloonsPad.SetActive(false);
        }

        void OnGameInitialized(object sender, EventArgs e)
        {
            BalloonUtils.SetupBalloons();

            GreenMat = BalloonsBundle.LoadAsset<Material>("TestMat 2");
            RedMat = BalloonsBundle.LoadAsset<Material>("redclicked");

            var entry = Plugin.Instance.Config.Bind("EnabledMods", PluginInfo.Name, true);
            Enabled = entry.Value;
            if (!Enabled)
            {
                ActiveBalloonsPad.SetActive(false);
            }
        }
        
/*        public IEnumerator StartupBalloon()
        {
            yield return new WaitForSeconds(10f);
            *//*                FindInParent("Testing", obje.transform).GetComponent<MeshFilter>().mesh = Assets[0].GetComponent<MeshFilter>().mesh;
                            FindInParent("Testing", obje.transform).GetComponent<Renderer>().material = Assets[0].GetComponent<Renderer>().material;*//*
            GameObject obje = BalloonUtils.CopyBalloon(BalloonUtils.CopyBalloonPath);
            obje.transform.localPosition = Vector3.zero;
            BalloonUtils.BalloonAssets.Add(obje);
            Debug.Log(obje.transform.localScale + obje.transform.parent.name);
            BalloonUtils.FindInParent("CHOCOLATE DONUT BALLOON", obje.transform).name = "Testing";
            BalloonUtils.FindInParent("Testing", obje.transform).GetComponent<BalloonHoldable>().myIndex = GameObject.Find("Local Gorilla Player").GetComponent<BodyDockPositions>().allObjects.Length;
            BalloonUtils.FindInParent("Testing", obje.transform).GetComponent<BalloonHoldable>().currentState = TransferrableObject.PositionState.OnRightShoulder;
            BalloonUtils.FindInParent("Testing", obje.transform).GetComponent<BalloonHoldable>().storedZone = BodyDockPositions.DropPositions.RightBack;
            Debug.Log(GameObject.Find("Local Gorilla Player").GetComponent<BodyDockPositions>().allObjects.Length);
            GameObject.Find("Local Gorilla Player").GetComponent<BodyDockPositions>().allObjects.AddItem<TransferrableObject>(BalloonUtils.FindInParent("Testing", obje.transform).GetComponent<BalloonHoldable>());
            Debug.Log(GameObject.Find("Local Gorilla Player").GetComponent<BodyDockPositions>().allObjects.Length);
            GameObject.Find("Local Gorilla Player").GetComponent<VRRigReliableState>().activeTransferrableObjectIndex[2] = BalloonUtils.FindInParent("Testing", obje.transform).GetComponent<BalloonHoldable>().myIndex;
            BalloonUtils.FindInParent("Testing", obje.transform).gameObject.SetActive(true);
            BalloonUtils.FindInParent("Testing", obje.transform).GetComponent<MeshFilter>().mesh = null;

            *//*BalloonUtils.FindInParent("Testing", obje.transform).GetComponent<Renderer>().material = Assets[0].GetComponent<Renderer>().material;*/

            /*                Vector3 currentScale = BalloonUtils.FindInParent("Testing", obje.transform).transform.lossyScale;
                            Vector3 scaleFactor = new Vector3(Assets[0].transform.localScale.x / currentScale.x, Assets[0].transform.localScale.y / currentScale.y, Assets[0].transform.localScale.z / currentScale.z);
                            BalloonUtils.FindInParent("Testing", obje.transform).transform.localScale = scaleFactor;*/
            /*                FindInParent("Testing", obje.transform).GetComponent<BalloonHoldable>().beginScale = 0.03f;*//*
            Plugin.ChildBalloonObj = BalloonUtils.FindInParent("Testing", obje.transform).gameObject;
            GameObject.Instantiate(BalloonUtils.Assets[0], Plugin.ChildBalloonObj.transform);*//*
            yield return null;
        }*/

        void Update()
        {
            if (DisplayText != null && ObjectsInParent.Count != 0)
            {
                if (CurrentSelectedObject != ObjectsInParent[CurrentBalloonTab])
                {
                    for (int i = 0; i < ObjectsInParent.Count; i++)
                    {
                        if (i == CurrentBalloonTab)
                        {
                            ObjectsInParent[i].gameObject.SetActive(true);
                            CurrentSelectedObject = ObjectsInParent[i];
                            DisplayText.text = BalloonNames[i];
                        }
                        else
                        {
                            ObjectsInParent[i].gameObject.SetActive(false);
                        }
                    }
                }
            }

                if (ControllerInputPoller.instance.leftControllerPrimaryButton && Enabled)
                {
                ActiveBalloonsPad.SetActive(true);
                }
                else
                {
                ActiveBalloonsPad.SetActive(false);
                }
        }

        [ModdedGamemodeJoin]
        public void OnJoin(string gamemode)
        {
            inRoom = true;
        }

        public static void UpdateTab(bool Up)
        {
            if (Up)
            {
                if (CurrentBalloonTab < ObjectsInParent.Count - 1)
                {
                    CurrentBalloonTab++;
                }
                else
                {
                    CurrentBalloonTab = 0;
                }
            }
            else
            {
                if (CurrentBalloonTab != 0)
                {
                    CurrentBalloonTab--;
                }
                else
                {
                    CurrentBalloonTab = ObjectsInParent.Count - 1;
                }
            }
        }

        public static void EnableCurrentBallon()
        {
            if (!BalloonUtils.BalloonAssets[CurrentBalloonTab].BalloonObject.activeSelf)
            {
                BalloonUtils.SetCustomBalloon(true, CurrentBalloonTab);
            }
            else
            {
                BalloonUtils.SetCustomBalloon(false, CurrentBalloonTab);
            }
        }

        [ModdedGamemodeLeave]
        public void OnLeave(string gamemode)
        {

            inRoom = false;
        }
    }
}
