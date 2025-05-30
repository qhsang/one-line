using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EasyMobile;

public class UIController : MonoBehaviour
{

    public static int totalLevel = LevelData.totalLevelsPerWorld * LevelData.worldNames.Length;
    public static int totalLevelInWorld = LevelData.totalLevelsPerWorld;

    public Transform packagesContent, levelsContent;

    public enum UIMODE : int
    {
        OPENPLAYSCREEN,
        OPENLEVELSCREEN,
        OPENWORLDSCREEN
    }

    public static UIMODE mode = UIMODE.OPENPLAYSCREEN;

    public GameObject playScreen;
    public GameObject playBackGround;

    public GameObject levelScreen;

    public GameObject worldScene;

    public UnlockPackageDialog unlockPackageDialog;

    public Sprite disableSprite;
    public Sprite enableSprite;

    public GameObject soundButton;
    public Sprite soundOn;
    public Sprite soundOf;

    public GameObject musicButton;
    public Sprite musicOn;
    public Sprite musicOff;

    public Image shopImage;
    public Sprite rateSprite;

    public ShopDialog shopDialog;

    // Use this for initialization
    void Start()
    {
        if (mode == UIMODE.OPENPLAYSCREEN)
        {
            EnablePlayScreen();
        }
        else if (mode == UIMODE.OPENWORLDSCREEN)
        {
            EnableWorldScreen();
        }
        else if (mode == UIMODE.OPENLEVELSCREEN)
        {
            EnableStageScreen(LevelData.worldSelected);
        }

        ChangeMusic(Music.instance.IsEnabled());
        ChangeSound(Sound.instance.IsEnabled());

        CUtils.ChangeGameMusic();

        CUtils.CloseBannerAd();

        // if (!Purchaser.instance.isEnabled)
        // {
        //     shopImage.sprite = rateSprite;
        // }
    }

    public void PlayButtonSound()
    {
        Sound.instance.PlayButton();
    }

    public void EnablePlayScreen()
    {
        playScreen.SetActive(true);
        playBackGround.SetActive(true);

        levelScreen.SetActive(false);
        worldScene.SetActive(false);
    }

    public void EnableWorldScreen()
    {
        PrepareWorldScreen();
        playScreen.SetActive(false);
        playBackGround.SetActive(false);

        levelScreen.SetActive(false);
        worldScene.SetActive(true);
    }

    public void EnableStageScreen()
    {
        playScreen.SetActive(false);
        playBackGround.SetActive(false);

        levelScreen.SetActive(true);
        worldScene.SetActive(false);
    }

    // data for worlds
    private void PrepareWorldScreen()
    {
        int cCount = packagesContent.childCount;
        UpdateWorldTitle(worldScene.transform.Find("Title"));

        for (int i = 0; i < (cCount - 1); i++)
        {
            UpdateWorld(packagesContent.GetChild(i), i + 1);
        }
    }

    private void UpdateWorldTitle(Transform title)
    {
        Text levels = title.GetComponentInChildren<Text>();
        levels.text = "" + PlayerData.instance.GetTotalLevelCrossed() + " / " + totalLevel;
    }

    private void UpdateWorld(Transform world, int index)
    {
        int isUnlocked = PlayerData.instance.LEVELUNLOCKED[index];

        var starObj = world.Find("Button/Star").gameObject;
        var progressTextObj = world.Find("Button/ProgressText").gameObject;
        var lockedTextObj = world.Find("Button/Locked").gameObject;
        var packageName = world.Find("PackageName").GetComponent<Text>();
        packageName.text = LevelData.worldNames[index - 1];

        if (index > 1 && isUnlocked == 0)
        {
            int prvLevelCrossed = PlayerData.instance.LevelCrossedForOneWorld(index - 1);

            if (prvLevelCrossed < LevelData.prvLevelToCrossToUnLock)
            {
                starObj.SetActive(false);
                progressTextObj.SetActive(false);
                lockedTextObj.SetActive(true);
                return;
            }
        }

        starObj.SetActive(true);
        progressTextObj.SetActive(true);
        lockedTextObj.SetActive(false);

        int levelCrossed = PlayerData.instance.LevelCrossedForOneWorld(index);

        Text levels = world.GetComponentInChildren<Text>();
        levels.text = "" + levelCrossed + " / " + totalLevelInWorld;
    }


    //data for level
    public void EnableStageScreen(int indexWorld)
    {
        if (indexWorld == 1)
        {
            LevelSetup(indexWorld);
            EnableStageScreen();
        }
        else
        {
            LevelData.pressedWorld = indexWorld;
            int isUnLockedByInApp = PlayerData.instance.LEVELUNLOCKED[indexWorld];

            if (isUnLockedByInApp == 0)
            {
                int prvLevelCrossed = PlayerData.instance.LevelCrossedForOneWorld(indexWorld - 1);

                if (prvLevelCrossed >= LevelData.prvLevelToCrossToUnLock)
                {
                    // play level
                    LevelSetup(indexWorld);
                    EnableStageScreen();
                }
                else
                {
                    unlockPackageDialog.Show(LevelData.worldNames, indexWorld);
                }
            }
            else
            {
                //play level
                LevelSetup(indexWorld);
                EnableStageScreen();
            }
        }
    }

    private void LevelSetup(int indexWorld)
    {
        LevelData.worldSelected = indexWorld;
        LevelScreenReader(indexWorld);
    }

    void LevelScreenReader(int indexWorld)
    {
        Transform top = levelScreen.transform.GetChild(1);
        top.Find("title").GetComponent<Text>().text = LevelData.worldNames[indexWorld - 1];
        top.Find("Score").GetComponent<Text>().text = PlayerData.instance.LevelCrossedForOneWorld(indexWorld) + "/" + totalLevelInWorld;

        // list of all levelssssss
        int largetLevel = PlayerData.instance.GetLargestLevel(indexWorld);
        for (int i = 0; i < LevelData.totalLevelsPerWorld; i++)
        {
            bool isShownHint = true;
            Transform child = levelsContent.GetChild(i);
            child.GetComponentInChildren<Text>().text = "" + (i + 1);
            Transform locked = child.Find("Locked");
            Transform unlocked = child.Find("Unlocked");

            if (i < largetLevel + 3)
            {
                locked.gameObject.SetActive(false);
                unlocked.gameObject.SetActive(true);

                if (PlayerData.instance.IsLevelCrossed(indexWorld, i + 1))
                {
                    isShownHint = false;
                    unlocked.Find("Star1").gameObject.SetActive(true);
                    unlocked.Find("Star2").gameObject.SetActive(false);
                }
                else
                {
                    unlocked.Find("Star1").gameObject.SetActive(false);
                    unlocked.Find("Star2").gameObject.SetActive(true);
                }

                child.GetComponent<Button>().interactable = true;
            }
            else
            {
                locked.gameObject.SetActive(true);
                unlocked.gameObject.SetActive(false);
                child.GetComponent<Button>().interactable = false;
            }
            if (isShownHint && LevelData.isLevelIsHintLevel(indexWorld, i + 1))
            {
                child.Find("Hint").gameObject.SetActive(true);
            }
            else
            {
                child.Find("Hint").gameObject.SetActive(false);
            }
        }
    }

    public void OnPackageUnlocked()
    {
        PrepareWorldScreen();
        unlockPackageDialog.gameObject.SetActive(false);
    }

    public void Share()
    {
#if UNITY_EDITOR
       Toast.instance.ShowMessage("This feature only works in Android and iOS");
#elif (UNITY_ANDROID || UNITY_IPHONE)
        StartCoroutine(DoShare());
#endif
    }

    private IEnumerator DoShare()
    {
        yield return new WaitForEndOfFrame();
        Sharing.ShareScreenshot("screenshot", "");
    }

    public void LoadLevel(int levelSelected)
    {
        PlayButtonSound();
        LevelData.levelSelected = levelSelected;
        SceneManager.LoadScene(1);
    }

    public void ToggleMusic()
    {
        bool isEnabled = !Music.instance.IsEnabled();
        Music.instance.SetEnabled(isEnabled, true);
        ChangeMusic(isEnabled);
    }

    void ChangeMusic(bool isEnabled)
    {
        if (isEnabled)
        {
            musicButton.GetComponent<Image>().sprite = musicOn;
        }
        else
        {
            musicButton.GetComponent<Image>().sprite = musicOff;
        }
    }

    public void ToggleSound()
    {
        bool isEnabled = !Sound.instance.IsEnabled();
        Sound.instance.SetEnabled(isEnabled);
        ChangeSound(isEnabled);
    }

    void ChangeSound(bool isEnabled)
    {
        if (isEnabled)
        {
            soundButton.GetComponent<Image>().sprite = soundOn;
        }
        else
        {
            soundButton.GetComponent<Image>().sprite = soundOf;
        }
    }

    public void OnShopClick()
    {
        // if (Purchaser.instance.isEnabled)
        // {
        //     shopDialog.Show();
        // }
        // else
        // {
        //     CUtils.OpenStore();
        // }
        shopDialog.Show();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (playScreen.activeSelf)
            {
#if UNITY_ANDROID
                Application.Quit();
#endif
            }
            else if (worldScene.activeSelf && !unlockPackageDialog.gameObject.activeSelf)
            {
                EnablePlayScreen();
            }
            else if (levelScreen.activeSelf)
            {
                EnableWorldScreen();
            }
        }
    }
}
