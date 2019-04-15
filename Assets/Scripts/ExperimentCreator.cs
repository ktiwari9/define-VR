using System;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UXF;

/// <summary>
/// The creator of the experiments. Read settings and assigns appropriate variables for other scripts
/// </summary>
public class ExperimentCreator : MonoBehaviour {

    public GameObject goal;
    public GameObject start;
    public GameObject locomotion;
    public EnvironmentController environmentController;
    public ExperimentController experimentController;
    public UIController uicontroller;
    public PopupController popupController;
    public ExperimentStartupController startupController;
    public ParticipantListSelection ppListSelect;

    Session session;

    // Settings to be read from the settings file
    public int numPracticeTrials;
    public int numMainTrials;
    public float deadzone;
    
    public float trialTimer;
    
    public string locomotionMethod;
    public bool startBlocksTrain;
    public bool startBlocksTest;
    public bool startIconsTrain;
    public bool startIconsTest;
    public bool hideWallsInTest;
    public bool hideInactiveStarts;
    public bool oneActiveGoalInTest;
    public bool fogInTest;
    public List<string> blockForms;
    public List<float> roomSize;
    public string textureType;

    // Highscore settings

    public float timeCoeff;
    public float distCoeff;
    public float alpha_t;
    public float alpha_d;
    public float scalingFactor;
    public bool fakeBoard;
    public float scoreTimer;
    public string scoringMethod;
    public bool roundscore;

    [HideInInspector]
    public string highscoreLoadFile;
    [HideInInspector]
    public string highscoreSaveFile;
    public bool highscoreRetain;

    // Fly controller settings
    public float flyTrainingRadius;
    public float flyTestRadius;
    public float flyOffset;
    public float flyMinHeight;
    public float flyMaxHeight;
    public float flySpeed;
    public float flyOffsetStepSize;
    private bool error;


    public void Start()
    {
        try
        {
            SteamVR_Actions.vr_interaction.Activate(SteamVR_Input_Sources.Any, 0, true);
        } catch (NullReferenceException)
        {
            Debug.LogWarning("VR Inputs not set! Are the VR devices connected?");
        }
    }


    /// <summary>
    /// generates the trials and blocks for the session
    /// </summary>
    /// <param name="experimentSession"></param>
    public void GenerateExperiment(Session experimentSession)
    {
        error = false;
        // save reference to session
        session = experimentSession;
        experimentController.session = session;

        // Read locomotion settings
        readSetting(session.locomotionSettings, "locomotion", ref locomotionMethod);
        // The rest of locomotionsettings are read later in a separate finction

        // Read scenario settings
        readSetting(session.scenarioSettings, "n_training_trials", ref numPracticeTrials);
        readSetting(session.scenarioSettings, "n_testing_trials", ref numMainTrials);

        

        readSetting(session.scenarioSettings, "use_start_blocks_in_training", ref startBlocksTrain);
        readSetting(session.scenarioSettings, "use_start_blocks_in_testing", ref startBlocksTest);
        readSetting(session.scenarioSettings, "use_start_icons_in_training", ref startIconsTrain);
        readSetting(session.scenarioSettings, "use_start_icons_in_testing", ref startIconsTest);
        readSetting(session.scenarioSettings, "trial_time", ref trialTimer);

        

        // Read scoring settings
        readSetting(session.scoreSettings, "score_time", ref scoreTimer);
        readSetting(session.scoreSettings, "keyword", ref scoringMethod);
        readSetting(session.scoreSettings, "speed_coeff", ref timeCoeff);
        readSetting(session.scoreSettings, "acc_coeff", ref distCoeff);
        readSetting(session.scoreSettings, "alpha_t", ref alpha_t);
        readSetting(session.scoreSettings, "alpha_d", ref alpha_d);
        readSetting(session.scoreSettings, "scaling_factor", ref scalingFactor);
        readSetting(session.scoreSettings, "fake_scoreboard", ref fakeBoard);
        readSetting(session.scoreSettings, "distance_deadzone", ref deadzone);
        readSetting(session.scoreSettings, "round_score", ref roundscore);



        // Read environment settings
        readSetting(session.environmentSettings, "fly_test_radius", ref flyTestRadius);
        readSetting(session.environmentSettings, "fly_training_radius", ref flyTrainingRadius);
        readSetting(session.environmentSettings, "fly_speed", ref flySpeed);
        readSetting(session.environmentSettings, "fly_offset", ref flyOffset);
        readSetting(session.environmentSettings, "fly_min_height", ref flyMinHeight);
        readSetting(session.environmentSettings, "fly_max_height", ref flyMaxHeight);
        readSetting(session.environmentSettings, "fly_offset_step_size", ref flyOffsetStepSize);
        readSetting(session.environmentSettings, "remove_walls_for_test", ref hideWallsInTest);
        readSetting(session.environmentSettings, "use_fog_in_test", ref fogInTest);
        readSetting(session.environmentSettings, "hide_inactive_starts", ref hideInactiveStarts);
        readSetting(session.environmentSettings, "one_active_goal_in_test", ref oneActiveGoalInTest);
        readSetting(session.environmentSettings, "texture_style", ref textureType);

        readSetting(session.environmentSettings, "highscore_retain", ref highscoreRetain);

        readSetting(session.environmentSettings, "block_forms", ref blockForms);
        readSetting(session.environmentSettings, "enable_mouse_for_forms", ref environmentController.forms.GetComponent<FormController>().useSeparateCamera);

        // Setup highscore controller and correct reference to the header to be logged in high scores table
        if (scoringMethod == "none") {

            highscoreSaveFile = "";
            highscoreLoadFile = "";

        }
        else if (!fakeBoard)
        {
            highscoreSaveFile = "highscores_" + scoringMethod + ".csv";
            highscoreLoadFile = "highscores_" + scoringMethod + ".csv";
        }

        else {
            highscoreSaveFile = "";
            highscoreLoadFile = "highscores_" + scoringMethod + "_fake.csv";

            string basefolder = ppListSelect.currentFolder;
            string path = System.IO.Path.Combine(basefolder, highscoreLoadFile);

            if (!System.IO.File.Exists(path)) {
                errorPopup("Fake Scoreboard Not Found!");
                error = true;
                return;

            }



        }
        
        session.headerToHighscore = "score";

        

        session.highScoreController.Setup(highscoreSaveFile, highscoreLoadFile, highscoreRetain, fakeBoard);
        uicontroller.setScoringMethod(scoringMethod);

        // Read room size, check that it contains relevant number of values and resize room accordingly
        List<float> backuproomsize = roomSize;
        readSetting(session.environmentSettings, "room_size", ref roomSize);
        if(roomSize.Count != 3)
        {
            Debug.LogWarning("Invalid roomsize ("+roomSize.ToString()+") given, using default ("+backuproomsize.ToString()+") instead.");
            roomSize = backuproomsize;
        }
        environmentController.setRoomSize(roomSize[0],roomSize[1],roomSize[2]);
        environmentController.chooseTexture(textureType);
        environmentController.showTraining();
        environmentController.hideTesting();
        RenderSettings.fog = false;
        environmentController.setForms(blockForms,2);

        // create blocks
        if (numPracticeTrials > 0)
        {
            Block practiceBlock = session.CreateBlock(numPracticeTrials);       // Block 1
            practiceBlock.settings.SetValue("practice", true);
        }
        if (numMainTrials > 0)
        {
            Block mainBlock = session.CreateBlock(numMainTrials);               // Block 2
            mainBlock.settings.SetValue("practice", false);
        }

        // Set the proper values to Experiment controller
        experimentController.setVariables(timeCoeff, distCoeff, alpha_t, alpha_d, roundscore, scalingFactor, deadzone, flyTestRadius,
            trialTimer, scoreTimer, scoringMethod,
            hideWallsInTest, startBlocksTest, startIconsTest, hideInactiveStarts, fogInTest, oneActiveGoalInTest);

        // Load and validate start positions
        LoadAndInstantiatePositions("start");

        // Load and validate end positions
        LoadAndInstantiatePositions("end");

        // Read what locomotion method to use and activate it
        activateLocomotionMethod("locomotion");

        

        if (error)
        {
            experimentController.CleanUp();
        }
    }

    public bool loadAutopilot()
    {
        return startupController.Autopilot();
    }

    public void resetAutopilot()
    {
        startupController.resetAutopilot();
    }


    // A function to quit, in case of critical errors etc
    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void errorPopup(string text)
    {
        Popup epopup = new Popup();
        epopup.messageType = MessageType.Error;
        epopup.message = string.Format(text);
        epopup.onOK = new System.Action(() => {/*Do nothing*/});
        popupController.DisplayPopup(epopup);
        session.End(true);
        return;
    }

    
    void LoadAndInstantiatePositions(String se)
    {
        for (int i = 1; true; i++)
        {
            // TODO: Load all as list of lists or similar structure?
            // If the next start / end position is not found, stop looping
            if (!session.scenarioSettings.baseDict.ContainsKey(se+"_" + i.ToString()))
            {
                // Need at least first start / end. Complain if such was not found
                if (i == 1)
                {
                    errorPopup("Mandatory configuration \""+se+"_1\" not found in settings file!");
                    error = true;
                    return;
                }
                break;
            }

            // Get list of floats to be used as coordinates and rotation
            List<float> item = session.scenarioSettings.GetFloatList(se+"_" + i.ToString());
            if (item.Count != 4)
            {
                // Three numbers given, assuming X, Z, and Rotation
                if (item.Count == 3)
                {
                    item.Insert(1, 0);
                }

                // Two numbers given, assuming X and Z, presuming others to be zero
                else if (item.Count == 2)
                {
                    item.Insert(1, 0);
                    item.Insert(3, 0);
                }

                // If weird number of coordinates (not 3 or 4), complain
                else
                {
                    Debug.LogError("\"" + se + "_" + i.ToString() + "\" needs three or four coordinates, but" + item.Count.ToString() + " was given.");
                    break;
                }
            }

            if (se.Equals("start"))
            {
                GameObject pos = Instantiate(start, new Vector3(item[0], item[1], item[2]), Quaternion.identity);
                pos.transform.Rotate(0, item[3] ,0);
                pos.transform.Find("Box").gameObject.SetActive(startBlocksTrain);
                GameObject icon = pos.transform.Find("Icon").gameObject;
                Sprite sprt = Resources.Load<Sprite>("icons/"+se+"_"+i.ToString()+"_icon");
                icon.GetComponent<SpriteRenderer>().sprite = sprt;
                icon.SetActive(startIconsTrain);
                
                experimentController.start_positions.Add(pos);
                pos.SetActive(false);
            }
            if (se.Equals("end"))
            {
                GameObject pos = Instantiate(goal, new Vector3(item[0], item[1], item[2]), Quaternion.identity);
                pos.transform.Rotate(0, item[3], 0);
                FlyController flyController = pos.GetComponentInChildren<FlyController>();
                flyController.setConfigurations(flySpeed,flyTrainingRadius,flyOffset,flyMinHeight,flyMaxHeight,flyOffsetStepSize);
                flyController.setRadius(flyTrainingRadius);
                experimentController.end_positions.Add(pos);
            }
        }
    }

    
    // Enables one of the locomotion methods and reads relevant variables from json
    void activateLocomotionMethod(String key)
    {
        Locomotion method = null;   // Assigned in the switch case below
        bool alignHMD = true;
        switch (locomotionMethod)
        {
            case "keyboard":
                method = locomotion.GetComponent<KeyboardLocomotion>();
                readSetting(session.locomotionSettings, "rotation_speed", ref ((KeyboardLocomotion)method).rotateSpeed);
                break;

            case "controller":
                method = locomotion.GetComponent<ControllerLocomotion>();
                readSetting(session.locomotionSettings, "use_for_rotation", ref ((ControllerLocomotion)method).useForRotation);
                if (((ControllerLocomotion)method).useForRotation)
                {
                    readSetting(session.locomotionSettings, "rotation_speed", ref ((ControllerLocomotion)method).rotateSpeed);
                }
                break;

            case "armswing":
                method = locomotion.GetComponent<ArmswingLocomotion>();
                readSetting(session.locomotionSettings, "require_moving_both", ref ((ArmswingLocomotion)method).movingBothRequired);
                readSetting(session.locomotionSettings, "min_movement_threshold", ref ((ArmswingLocomotion)method).minMoveThreshold);
                readSetting(session.locomotionSettings, "max_movement_threshold", ref ((ArmswingLocomotion)method).maxMoveThreshold);
                readSetting(session.locomotionSettings, "max_swing_amount", ref ((ArmswingLocomotion)method).maximumSwing);
                break;

            case "teleport":
                method = locomotion.GetComponent<TeleportLocomotion>();
                break;

            case "headbob":
                method = locomotion.GetComponent<HeadbobLocomotion>();
                readSetting(session.locomotionSettings, "deadzone", ref ((HeadbobLocomotion)method).bobDeadzone);
                readSetting(session.locomotionSettings, "threshold", ref ((HeadbobLocomotion)method).bobThreshold);
                readSetting(session.locomotionSettings, "bob_hold", ref ((HeadbobLocomotion)method).holdBob);
                readSetting(session.locomotionSettings, "rotation_threshold", ref ((HeadbobLocomotion)method).rotThreshold);
                break;

            case "walking":
                method = locomotion.GetComponent<PhysicalWalkingLocomotion>();
                alignHMD = false;
                break;

            default:
                Debug.LogWarning("Invalid locomotion method specified! Using default: keyboard");
                method = locomotion.GetComponent<KeyboardLocomotion>();
                return;
        }
        if (alignHMD)
        {
            locomotion.GetComponent<AlignPlayerWithHMD>().enabled = true;
            locomotion.GetComponent<AlignPlayerWithHMD>().init();
        }
        method.enabled = true;
        experimentController.activeLocomotion = method;
        readSetting(session.locomotionSettings, "speed", ref method.velocityMultiplier);
        readSetting(session.environmentSettings, "max_forward_speed", ref method.maxForwardSpeed);
    }


    // Functions to read different datatypes from the settings

    private void settingNotFound(String key, String defaultValue)
    {
        Debug.LogWarning("Key \"" + key + "\" not found in settings file, using default value (" + defaultValue + ") instead.");
    }
        // INT
    private void readSetting(UXF.Settings settings, String key, ref int readTo)
    {
        try
        {
            readTo = settings.GetInt(key);
        }
        catch (KeyNotFoundException)
        {
            settingNotFound(key, readTo.ToString());
        }
    }

        // FLOAT
    private void readSetting(UXF.Settings settings, String key, ref float readTo)
    {
        try
        {
            readTo = settings.GetFloat(key);
        }
        catch (KeyNotFoundException)
        {
            settingNotFound(key, readTo.ToString());
        }
    }


        // STRING
    private void readSetting(UXF.Settings settings, String key, ref string readTo)
    {
        try
        {
            readTo = settings.GetString(key);
        }
        catch (KeyNotFoundException)
        {
            settingNotFound(key, readTo.ToString());
        }
    }


        // BOOL
    private void readSetting(UXF.Settings settings, String key, ref bool readTo)
    {
        try
        {
            readTo = settings.GetBool(key);
        }
        catch (KeyNotFoundException)
        {
            settingNotFound(key, readTo.ToString());
        }
    }

    // float list
    private void readSetting(UXF.Settings settings, String key, ref List<float> readTo)
    {
        try
        {
            readTo = settings.GetFloatList(key);
        }
        catch (KeyNotFoundException)
        {
            settingNotFound(key, readTo.ToString());
        }
    }

    // string list
    private void readSetting(UXF.Settings settings, String key, ref List<string> readTo)
    {
        try
        {
            readTo = settings.GetStringList(key);
        }
        catch (KeyNotFoundException)
        {
            settingNotFound(key, readTo.ToString());
        }
    }
}