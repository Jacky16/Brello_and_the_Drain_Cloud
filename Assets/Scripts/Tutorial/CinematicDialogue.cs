using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Globalization;

public class CinematicDialogue : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI dialogueText;

    [Header("Variables del dialogo")]
    [SerializeField] DialogueObjectData dialogueObject;
    private string[] dialogueInParts;
    private string displayText;

    [SerializeField] float speed = 0.05f;
    private int index;

    [SerializeField] BackgroundMusic bm;

    // Start is called before the first frame update
    public void StartDialogue()
    {
        index = 0;

        Debug.Log("He entrado");

        ReadCurrentDialogue();

        StartCoroutine(TypeDialogue());
    }
    void ReadCurrentDialogue()
    {
        //IMPORTANTE PARA ENTENDER ESTE CODIGO:
        //Al separar por tags, lo de fuera de un tag siempre estará en una posicion par en el array
        //y lo de dentro de un tag siempre estará en una posicion impar.
        //Es por esta razon por la que se verá como compruebo si el número es par o impar en varias ocasiones.

        //Ponemos a 0 el numero de caracteres visibles para cargar el texto en 2o plano.
        dialogueText.maxVisibleCharacters = 0;

        //Dividimos el texto actual separandolo cuando encuentre un tag
        dialogueInParts = dialogueObject.dialogue.conversationBlock[index].Split('<', '>');

        displayText = string.Empty;
        for (int i = 0; i < dialogueInParts.Length; i++)
        {
            //Añadimos el texto tal cual
            if (i % 2 == 0)
                displayText += dialogueInParts[i];
            //Si no es un tag hecho por nosotros, lo ponemos para que TMP haga lo suyo.
            else if (!isCustomTag(dialogueInParts[i].Replace(" ", "")))
                displayText += $"<{dialogueInParts[i]}>";
        }

        dialogueText.text = displayText;
    }
    IEnumerator TypeDialogue()
    {
        int subCounter = 0;
        int visibleCounter = 0;

        while (subCounter < dialogueInParts.Length)
        {
            if (subCounter % 2 == 1)
            {
                yield return EvaluateTag(dialogueInParts[subCounter].Replace(" ", ""));
            }
            else
            {
                while (visibleCounter < dialogueInParts[subCounter].Length)
                {
                    visibleCounter++;
                    dialogueText.maxVisibleCharacters++;
                    yield return new WaitForSeconds(speed);
                }
                visibleCounter = 0;
            }
            subCounter++;
        }
        yield return null;

        WaitForSeconds EvaluateTag(string tag)
        {
            if (tag.Length > 0)
            {
                if (tag.StartsWith("speed="))
                {
                    speed = float.Parse(tag.Split('=')[1], CultureInfo.InvariantCulture);
                }
                else if (tag.StartsWith("pause="))
                {
                    return new WaitForSeconds(float.Parse(tag.Split('=')[1], CultureInfo.InvariantCulture));
                }
                else if (tag.StartsWith("continue"))
                {
                    NextDialogue();
                }
            }
            return null;
        }
    }

    bool isCustomTag(string tag)
    {
        return tag.StartsWith("speed=")
            || tag.StartsWith("pause=")
            || tag.StartsWith("emotion=")
            || tag.StartsWith("action=")
            || tag.StartsWith("shop")
            || tag.StartsWith("end")
            || tag.StartsWith("continue");
    }

    public void NextDialogue()
    {
        Debug.Log("He entrado 2");
        index++;

        ReadCurrentDialogue();
        StartCoroutine(TypeDialogue());
    }

    public void ChangeScene()
    {
        bm.StopMusic();
        SceneManager.LoadScene("Grasslands");
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    bm.StopMusic();
        //    SceneManager.LoadScene("Grasslands");
        //}
    }
}
