using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_CutSceneDirector : MonoBehaviour
{
    public Camera Camera1;
    public UI_DialogueBubble Bubble1_Rogue;
    public float DELAY1 = 2f;
    public float DELAY2 = 0.25f;
    public UI_DialogueBubble Bubble2_Pally;
    public float DELAY3 = 3.25f;
    public UI_DialogueBubble Bubble3_Priest;
    public float DELAY4 = 1.25f;
    public UI_DialogueBubble Bubble4_Pally;
    public Sprite RogueTalking;
    public Sprite RogueSilent;

    public SpriteRenderer RogueBody;

    
    public Camera Camera2;
    public Camera Camera3;

public float DELAY4B = 0.5f;
    public float DELAY5 = 3f;
    public float DELAY6 = 3f;
    public SpriteRenderer WarlockBody;
    public Sprite WarlockIdle;
    public Sprite WarlockEyeRoll;

    public float DELAY7 = 3f;
    public GameObject Priest;

public float DELAY8 = 3f;
    public SpriteRenderer PaladinBody;
public float DELAY9 = 3f;
    public Sprite PallyNormal;
    public Sprite PallyAngry;
public float DELAY10 = 3f;
public UI_FTBAndSwap Fader;
GameController_DDOL ddol;
    void Start()
    {
        ddol = FindFirstObjectByType<GameController_DDOL>();

        if (!ddol.IsMusicPlaying()) {
            ddol.PlayMusic("Intro1");
        }
        Doit();
    }

    [ContextMenu("Doit")]
    void Doit() {
        StartCoroutine(DoCutScene());
    }

    IEnumerator DoCutScene() {
        Camera1.enabled = true;
        Camera2.enabled = false;
        Camera3.enabled = false;

        yield return new WaitForSeconds(0.5f);
        RogueBody.sprite = RogueTalking;
        Bubble1_Rogue.Say("Hey Pally, want to go slay dragons with us? They have shiny scales!");

        yield return new WaitForSeconds(DELAY1);

        RogueBody.sprite = RogueSilent;
        Bubble1_Rogue.Hide();

        yield return new WaitForSeconds(DELAY2);
Camera.SetupCurrent(Camera1);
        Bubble2_Pally.Say("I don't fight for riches or glory, only for the good of the realm.");

        yield return new WaitForSeconds(DELAY3);

        Camera1.enabled = false;
        Camera2.enabled = true;
        Camera3.enabled = false;

        yield return new WaitForSeconds(DELAY4);

        Bubble2_Pally.Hide();
        WarlockBody.sprite = WarlockEyeRoll;
        Priest.SetActive(true);

        
        yield return new WaitForSeconds(DELAY4B);

        WarlockBody.sprite = WarlockIdle;

        yield return new WaitForSeconds(DELAY5);

        Camera1.enabled = true;
        Camera2.enabled = false;
        Camera3.enabled = false;

        yield return new WaitForSeconds(DELAY6);

        Bubble3_Priest.Say("I think they kidnapped some children.");

        yield return new WaitForSeconds(DELAY7);
        ddol.PlayMusic("Intro2");
        Bubble3_Priest.Hide();
        Camera1.enabled = false;
        Camera2.enabled = false;
        Camera3.enabled = true;
        
        yield return new WaitForSeconds(DELAY8);
        ddol.PlayMusic("Intro3");
        PaladinBody.sprite = PallyAngry;

        yield return new WaitForSeconds(DELAY9);

        Bubble4_Pally.Say("I will bathe in the dragon's blood!");

        yield return new WaitForSeconds(DELAY10);

        while (Camera3.transform.position.z < -7) {
            Camera3.transform.position = new Vector3(Camera3.transform.position.x, Camera3.transform.position.y, Camera3.transform.position.z + 0.014f);
            yield return new WaitForSeconds(0.01f);
        }

        Fader.Go();
    }
}
