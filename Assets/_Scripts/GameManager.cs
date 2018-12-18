using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using Facebook.Unity;

public class GameManager : MonoBehaviour {

    public static GameManager instance;

    public Camera mainCam;
    public Light directionalLight;
    [Space(10)]
    [Header("UiPart")]
    public GameObject mainMenu;
    public GameObject retryMenu;
    public GameObject showCaseImage;
    public GameObject levelCompletedScreen;
    public Image retryCountDown;
    public GameObject noThanksButton;
    public Text fromLevel, toLevel;
    public Image waterLevelIndicator, progressBar;
    public GameObject fuelIndicator;
    public GameObject scrollArea;
    public Text completionText;
    public string[] levelCOmpletionTextcontents;
    public GameObject muteAudioIcon, enableAudioIcon;

    [Space]
    [Header("Level Controller")]
    public GameObject gun;
    public GameObject rayCastCollider;
    public List<LevelPrefabSets> levelPrefabSets;
    public GameObject[] levelPrefabs;
    public int[] levelwater;
    public float waterLimit, maxLimit;
    GameObject currentLevel;
   
    [HideInInspector] public  int levelnumber,nextRandomIndex; public bool gameOver;
    [HideInInspector] public bool levelLoaded, gameStartted;

    [Space]
    [Header("ParticalSystem")]
    public GameObject waterPartical;
    public GameObject explositionPartical;
    public GameObject firePartical;

    [Space]
    [Header("3D Models")]
    public GameObject[] animalModels;

    [Space]
    [Header("Color Manager")]
    public Color showCaseScrollObjectsBg;
    public GameObject[] scrollingScreenImages;


    [Space]
    [Header("Audio Management")]
    [Header("Audio Clips")]
    public AudioClip waveAudio;
    public AudioClip[] fallinginwaterSound;
    public AudioClip bombAudio;
    public AudioClip[] showCaseStrikeAudio;
    public AudioClip shootingAudioClip;

    [Header("Audio Source")]
    public AudioSource waveaudioSource;
    public AudioSource fallinginWaterAudioSource;
    public AudioSource showCaseAudioSource;
    public AudioSource bombAudioSource;
    public AudioSource shootingAudioSource;

    [Space]
    [Header("Mass Manager")]
    public List<float> massManagerList;

    public Text testLevelIndicator;

    int deathLevel;
    public enum PickUpEnum
    {
        Water,
        Bomb
    }

    public enum MassTypes
    {
        VeryLarge,
        Large,
        Medium,
        Small,
        VerySmall
    }

    private void Awake()
    {
        Facebook.Unity.FB.Init();
        Application.targetFrameRate = 60;
        instance = this;
    }

    public void CheatClear()
    {
      
        LevelSetUp.instance.NextLevel();
    }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
        CheckShowCaseClearedAnimals();
        //UpdateShowCase(11);
        Invoke("CreateWaveAudioSound", Random.Range(4, 10));
        mainCam.enabled = true;
        fromLevel.text = "1";
        toLevel.text = "2";
        //levelnumber = 0;
        levelnumber = PlayerPrefs.GetInt("LevelCompleted", 0) * 5;
        deathLevel = levelnumber + Random.Range(6, 9);
        CreateLevel();
        InitializeSdk();
        CheckAudioSaved();
        //print(levelnumber);
    }


    public void CheckAudioSaved()
    {
        int audioCheck = PlayerPrefs.GetInt("Audio", 1);
        if(audioCheck == 0)
        {
            muteAudioIcon.SetActive(true);
            enableAudioIcon.SetActive(false);
            AudioListener.pause = true;
        } else 
        {
            muteAudioIcon.SetActive(false);
            enableAudioIcon.SetActive(true);
            AudioListener.pause = false;
        }
    }

    public void InitializeSdk()
    {
        AppsFlyer.setAppsFlyerKey("2Pqe3Frcy5XeNKzunyDoBL");
        /* For detailed logging */
        /* AppsFlyer.setIsDebug (true); */
        #if UNITY_IOS
                /* Mandatory - set your apple app ID
                   NOTE: You should enter the number only and not the "ID" prefix */
                AppsFlyer.setAppID("1439940175");
                AppsFlyer.trackAppLaunch();
        #elif UNITY_ANDROID
           /* Mandatory - set your Android package name */
           AppsFlyer.setAppID ("YOUR_ANDROID_PACKAGE_NAME_HERE");
           /* For getting the conversion data in Android, you need to add the "AppsFlyerTrackerCallbacks" listener.*/
           AppsFlyer.init ("YOUR_APPSFLYER_DEV_KEY","AppsFlyerTrackerCallbacks");
        #endif
    }

    public void MuteAudio()
    {
        AudioListener.pause = !AudioListener.pause;
        if(AudioListener.pause == false)
        {
            PlayerPrefs.SetInt("Audio", 1);
        } else 
        {
            PlayerPrefs.SetInt("Audio", 0);
        }
        //CheckAudioSaved();
    }

    public void PlayButtonPressed()
    {
        rayCastCollider.SetActive(true);
        mainMenu.SetActive(false);
        FireWater.instance.CreateLevel();
        gameOver = false;
        gameStartted = true;
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }

    void CreateWaveAudioSound()
    {
        waveaudioSource.clip = waveAudio;
        waveaudioSource.Play();
        Invoke("CreateWaveAudioSound", Random.Range(4, 10));
    }

    public void CreateLevelFromSavedData()
    {
        
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            retryCountDown.fillAmount = 1;
            noThanksButton.transform.localScale = Vector3.zero;
            noThanksButton.transform.DOScale(1, 1.5f).SetEase(Ease.InOutExpo);
            gameOver = true;
        }
    }

    public void LogLevelCompleteEvent(int numberoflevelcrossed)
    {
        var parameters = new Dictionary<string, object>();
        parameters["numberoflevelcrossed"] = numberoflevelcrossed;
        FB.LogAppEvent(
            "LevelComplete",
            0,parameters
        );
    }

    public void PlayBombSound()
    {
        bombAudioSource.clip = bombAudio;
        bombAudioSource.Play();
    }

    public void CreateLevel()
    {
        // if(levelnumber > 0)
        //rayCastCollider.SetActive(true);
        waterLimit = 0;
        maxLimit = 0;
        // print(levelnumber);
        FireWater.instance.gunPrefab.transform.rotation = Quaternion.Euler(Vector3.zero);
        if ((levelnumber) % 5 == 0 && levelnumber > 1)
        {
            fromLevel.text = (((levelnumber ) / 5) + 1).ToString();
            toLevel.text = (((levelnumber ) / 5) + 2).ToString();
            PlayerPrefs.SetInt("LevelCompleted", levelnumber / 5);
            //print(levelnumber);
            // currentLevel = Instantiate(levelPrefabSets[nextRandomIndex].level1);
        }

        //CancelInvoke();
        int startingLevel = PlayerPrefs.GetInt("LevelCompleted", 0);
        print(startingLevel);
        if (startingLevel == 0)
        {
            nextRandomIndex = 0;
        }
        else
        {
            if(levelnumber%5 ==0)
            nextRandomIndex = levelnumber / 5;
        }

        levelLoaded = false;
        if (currentLevel)
        {
            FireWater.instance.CreateLevel();
            Destroy(currentLevel);
            gameOver = false;
        }

        print(levelnumber);
        testLevelIndicator.text = (levelnumber+1).ToString();
        float levelNumber = (levelnumber ) % 5;
        if (nextRandomIndex < 14)
        {
            if (levelNumber == 1)
            {
                progressBar.fillAmount = 0.20f;
                currentLevel = Instantiate(levelPrefabSets[nextRandomIndex].level2);
            }
            else if (levelNumber == 2)
            {
                progressBar.fillAmount = 0.40f;
                currentLevel = Instantiate(levelPrefabSets[nextRandomIndex].level3);
            }
            else if (levelNumber == 3)
            {
                progressBar.fillAmount = 0.60f;
                currentLevel = Instantiate(levelPrefabSets[nextRandomIndex].level4);
            }
            else if (levelNumber == 4)
            {
                progressBar.fillAmount = 0.80f;
                currentLevel = Instantiate(levelPrefabSets[nextRandomIndex].level5);
            }
            else
            {
                progressBar.fillAmount = 0.0f;
                currentLevel = Instantiate(levelPrefabSets[nextRandomIndex].level1);
            }
        } else 
        {
            if (levelNumber == 1)
            {
                progressBar.fillAmount = 0.20f;

            }
            else if (levelNumber == 2)
            {
                progressBar.fillAmount = 0.40f;

            }
            else if (levelNumber == 3)
            {
                progressBar.fillAmount = 0.60f;

            }
            else if (levelNumber == 4)
            {
                progressBar.fillAmount = 0.80f;

            }
            else
            {
                progressBar.fillAmount = 0.0f;

            }
            currentLevel = Instantiate(levelPrefabs[Random.Range(0, levelPrefabs.Length)]);
        }
       

        //maxLimit = levelwater[levelnumber];
        //waterLimit = levelwater[levelnumber];
        if (deathLevel != levelnumber)
        {
            foreach (Rigidbody item in currentLevel.GetComponent<LevelSetUp>().defaultRigidBody)
            {
                if (item.gameObject.GetComponent<MassManager>())
                {
                    if (item.gameObject.GetComponent<MassManager>())
                    {
                        if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.VeryLarge)
                        {
                            maxLimit += 0.5f;
                            waterLimit += 0.5f;
                            item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[0];
                        }
                        else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.Large)
                        {
                            maxLimit += 0.4f;
                            waterLimit += 0.4f;
                            item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[1];
                        }
                        else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.Medium)
                        {
                            maxLimit += 0.2f;
                            waterLimit += 0.2f;
                            item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[2];
                        }
                        else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.Small)
                        {
                            maxLimit += 0.1f;
                            waterLimit += 0.1f;
                            item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[3];
                        }
                        else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.VerySmall)
                        {
                            maxLimit += 0.05f;
                            waterLimit += 0.05f;
                            item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[4];
                        }

                    }
                }
            }
        }
        else 
        {
            //print("asddddddasdasdasdasdasdasdasdasdasdasdasdasdasdas" + levelnumber);
            foreach (Rigidbody item in currentLevel.GetComponent<LevelSetUp>().defaultRigidBody)
            {
                if (item.gameObject.GetComponent<MassManager>())
                {
                    
                    if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.VeryLarge)
                    {
                        maxLimit += 0.4f;
                        waterLimit += 0.4f;
                        item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[0]* 2;
                    }
                    else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.Large)
                    {
                        maxLimit += 0.3f;
                        waterLimit += 0.3f;
                        item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[1]*2;
                    }
                    else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.Medium)
                    {
                        maxLimit += 0.152f;
                        waterLimit += 0.152f;
                        item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[2]* 2;
                    }
                    else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.Small)
                    {
                        maxLimit += 0.071f;
                        waterLimit += 0.071f;
                        item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[3]* 2;
                    }
                    else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.VerySmall)
                    {
                        maxLimit += 0.03f;
                        waterLimit += 0.03f;
                        item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[4] * 2;
                    }





                    //if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.VeryLarge)
                    //{
                    //    maxLimit += 0.3f;
                    //    waterLimit += 0.3f;
                    //   // item.mass = item.mass * 10f;
                    //    item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[0] * 10;
                    //}
                    //else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.Large)
                    //{
                    //    maxLimit += 0.2f;
                    //    waterLimit += 0.2f;
                    //    //item.gameObject.GetComponent<Rigidbody>().mass = item.gameObject.GetComponent<Rigidbody>().mass * 10f
                    //    item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[1] * 5;
                    //}
                    //else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.Medium)
                    //{
                    //    maxLimit += 0.08f;
                    //    waterLimit += 0.08f;
                    //    //item.gameObject.GetComponent<Rigidbody>().mass = item.gameObject.GetComponent<Rigidbody>().mass * 10f;
                    //    item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[2] *5;
                    //}
                    //else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.Small)
                    //{
                    //    maxLimit += 0.06f;
                    //    waterLimit += 0.06f;
                    //    //item.gameObject.GetComponent<Rigidbody>().mass = item.gameObject.GetComponent<Rigidbody>().mass *10f;
                    //    item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[3] *5;
                    //}
                    //else if (item.gameObject.GetComponent<MassManager>().massTypes == MassTypes.VerySmall)
                    //{
                    //    maxLimit += 0.02f;
                    //    waterLimit += 0.02f;
                    //    //item.gameObject.GetComponent<Rigidbody>().mass = item.gameObject.GetComponent<Rigidbody>().mass * 10f;
                    //    item.GetComponent<Rigidbody>().mass = GameManager.instance.massManagerList[4] *5;
                    //}
                }
            }
            deathLevel = deathLevel + Random.Range(5,7);
        }

        FireWater.instance.waterAmount = 1;
        UpdateWaterIndicator();
    }

    void GetIndex()
    {
        nextRandomIndex = 1;
        int randomIndex =  levelPrefabSets.IndexOf(levelPrefabSets[Random.Range(1, levelPrefabSets.Count)]);
        if(PlayerPrefs.GetInt("levelPrefabSet"+nextRandomIndex,0)==0)
        {
            nextRandomIndex = randomIndex;
        } else 
        {
            GetIndex();
        }
    }

    public void IncrementScrollBar(float scrollValue)
    {
        progressBar.fillAmount = progressBar.fillAmount + ((1 * 0.20f) / scrollValue);
    }

    public void WaterPickUp(Vector3 position)
    {
        waterLimit = 12.5f;
        maxLimit = 20;
        UpdateWaterIndicator();
    }

    public void ContinueCurrentLevel()
    {
        retryMenu.SetActive(false);
        DOTween.KillAll();
        gameOver = false;
        waterLimit += 15;
    }

    private void CheckShowCaseClearedAnimals()
    {
        for (int i = 0; i < scrollingScreenImages.Length; i++)
        {
            GameObject currentImage = scrollingScreenImages[i];
            string crossedDetails = PlayerPrefs.GetString("Animal" + i, "Pending");
            if (crossedDetails == "Cleared")
            {
                currentImage.transform.GetChild(1).gameObject.SetActive(true);
                currentImage.transform.GetChild(2).gameObject.SetActive(false);
                currentImage.transform.GetChild(3).gameObject.SetActive(true);
                currentImage.transform.GetChild(3).GetComponent<Image>().DOFade(1, 0.2f);
                currentImage.transform.GetChild(4).gameObject.SetActive(false);
            }
        }
    }

    public void CreateDropSound()
    {
        if(!fallinginWaterAudioSource.isPlaying)
        {
            fallinginWaterAudioSource.clip = fallinginwaterSound[Random.Range(0, fallinginwaterSound.Length)];
            fallinginWaterAudioSource.Play();
        }
    }

    public void UpdateShowCase(int animalNumber)
    {
        //animalNumber -= 1;
        PlayerPrefs.SetInt("levelPrefabSet" + nextRandomIndex, 1);
        if(animalNumber <12)
        {
            scrollArea.GetComponent<ScrollRect>().verticalNormalizedPosition = 1f;
        }  else 
        {
            scrollArea.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
        }
        showCaseImage.SetActive(true);
        Color objColor = scrollingScreenImages[animalNumber].transform.GetChild(3).GetComponent<Image>().color;
        objColor.a = 0;
        scrollingScreenImages[animalNumber].transform.GetChild(3).GetComponent<Image>().color = objColor;
        scrollingScreenImages[animalNumber].transform.GetChild(3).transform.localScale = Vector3.one * 10;
        scrollingScreenImages[animalNumber].transform.GetChild(3).gameObject.SetActive(true);
        scrollingScreenImages[animalNumber].transform.GetChild(3).transform.DOScale(1, 0.5f).SetEase(Ease.Linear);
        showCaseAudioSource.clip = showCaseStrikeAudio[Random.Range(0, showCaseStrikeAudio.Length)];
        scrollingScreenImages[animalNumber].transform.GetChild(3).GetComponent<Image>().DOFade(1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            
            scrollingScreenImages[animalNumber].transform.GetChild(4).GetComponent<Image>().fillAmount = 0;
            scrollingScreenImages[animalNumber].transform.GetChild(4).gameObject.SetActive(false);
            scrollingScreenImages[animalNumber].transform.GetChild(4).GetComponent<Image>().DOFillAmount(1, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                scrollingScreenImages[animalNumber].transform.GetChild(1).transform.localScale = Vector3.one;
                scrollingScreenImages[animalNumber].transform.GetChild(1).gameObject.SetActive(true);
                scrollingScreenImages[animalNumber].transform.GetChild(1).transform.DOScale(1, 0.5f).OnComplete(() => {
                    scrollingScreenImages[animalNumber].transform.GetChild(1).transform.DOScale(1, 0.5f).OnComplete(() =>
                    {
                        PlayerPrefs.SetString("Animal" + animalNumber, "Cleared");
                        LevelSetUp.instance.NextLevel();
                        showCaseImage.SetActive(false);
                        CheckShowCaseClearedAnimals();
                    });
                });
            });
        });
    }

    public void UpdateWaterIndicator()
    {
        float percent = waterLimit / maxLimit;
        float difference = 1 - percent;
        float crossedAngle = difference * 180;
        float remainingAngle =  90 - crossedAngle;
        fuelIndicator.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, -remainingAngle));

        //print(waterLimit);
        if (waterLimit <= 0 && !gameOver)
        {
            gameOver = true;
            StopCoroutine(RestartMenu());
            StartCoroutine(RestartMenu());
        }
    }

    public IEnumerator RestartMenu()
    {
        yield return new WaitForSeconds(6f);
        gameOver = true;
        retryMenu.SetActive(true);
        rayCastCollider.SetActive(false);
    }


}
