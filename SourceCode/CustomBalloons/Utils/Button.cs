using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace CustomBalloons.Utils
{
    public class Button : MonoBehaviour
    {
        public string ButtonFunction = "";
        public bool Up;
        public bool OnCooldown;

        public void OnTriggerEnter(Collider other)
        {
            if (other.name == "RightHandTriggerCollider")
            {
                if (!OnCooldown)
                {
                    if (ButtonFunction == "")
                    {
                        Plugin.UpdateTab(Up);
                    }
                    else
                    {
                        Plugin.EnableCurrentBallon();
                    }
                }
                RedButton(true);

                GorillaTagger.Instance.offlineVRRig.PlayHandTapLocal(67, false, 0.09f);
                StartCoroutine(StartCooldown());
            }


        }

        public void OnTriggerExit(Collider other)
        {
            if (other.name == "RightHandTriggerCollider")
            {
                RedButton(false);
            }
        }

        public void RedButton(bool On)
        {
            switch (On)
            {
                case true:
                    gameObject.GetComponent<Renderer>().material = Plugin.RedMat;
                    break;
                case false:
                    gameObject.GetComponent<Renderer>().material = Plugin.GreenMat;
                    break;
            }
        }

        public IEnumerator StartCooldown()
        {
            OnCooldown = true;
            yield return new WaitForSeconds(0.2f);
            OnCooldown = false;
        }
    }
}
