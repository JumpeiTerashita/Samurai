using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KilledNum : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;

    public static int KillCounter;

    public static bool IsStarted;

    int[] DispBox = new int[5];

    [SerializeField]
    Text text;
    [SerializeField]
    Text Satsu;
    GameObject StartMessage;
    string[] DispNum = { "０", "１", "２", "３", "４", "５", "６", "７", "８", "９" };

    // Use this for initialization
    void Start()
    {
        IsStarted = false;
        Satsu.enabled = false;
        text.enabled = false;
        StartMessage = GameObject.Find("StartMessage");
        KillCounter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsStarted && !Satsu.enabled)
        {
            Destroy(StartMessage.gameObject);
            Satsu.enabled = true;
            text.enabled = true;
            return;
        }

        text.text = null;
        int KillCountCopy = KillCounter;
        if (KillCountCopy == 0)
        {
            text.text = "　　　０";
        }
        else if (KillCountCopy > 9999)
        {
            text.text = DispNum[9] + DispNum[9] + DispNum[9] + DispNum[9];

        }
        else
        {

            for (int i = 0; i < 5 && (KillCountCopy > 0); i++)
            {
                if (KillCountCopy / 10 > 0)
                {
                    DispBox[i] = KillCountCopy % 10;
                    //Debug.Log("DispBox[" + i + "] =" + DispBox[i]);
                    KillCountCopy /= 10;
                }
                else
                {
                   // Debug.Log(DispBox[i]);
                  //  Debug.Log("DispBox[" + i + "] =" + DispBox[i]);
                    DispBox[i] = KillCountCopy;
                    KillCountCopy = 0;
                }

            }



            for (int j = 4; j >= 0; j--)
            {
                if (j == 4 && DispBox[j] == 0)
                {
                    text.text += "　";
                    continue;
                }

                if (j != 4 && DispBox[j] == 0)
                {
                    if (DispBox[j + 1] == 0)
                    {
                        text.text += "　";
                        continue;
                    }

                }
                text.text += DispNum[DispBox[j]];
            }
        }


    }


}
