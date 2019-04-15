using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The controller of VR navigation experiments
/// </summary>
public class ExperimentController : MonoBehaviour {
    public UXF.Session session; // Provided by experiment creator
    public GameObject player;
    public GameObject startupPanel;
    public UXF.NotesController notesController;

    // Helper variables
    private float startTime;        // time the trial started, used for counting time elapsed for trial for scoring
    private GameObject activeStart;
    private GameObject activeGoal;
    private float pathlength;
    private bool training = true;   // A bool to indicate if we are still in the training block
    private bool escPressed;
    private Vector3 oldpos;
    public EnvironmentController enviromentController;
    public UIController uiController;
    private bool scoreboardVisible;     // A variable to indicate if scoreboard should be visible
    private bool scoreboardPhase;  // Indicate if during the scoreboard phase. Seperate from above as in situations scoreboard is hidden, it can still indicate the time between trials.

    // Lists of goal and starts, filled by experimentcreator
    public List<GameObject> start_positions = new List<GameObject>();
    public List<GameObject> end_positions = new List<GameObject>();
    
    // Things to be read from config JSON, assigned by experimentcreator
    private float timeCoeff;
    private float distCoeff;
    private float alpha_t;
    private float alpha_d;
    private bool roundScore;
    private float deadzone;
    private float flyTestRadius;
    private float trialTime;
    private float scoreTime;
    private float scalingFactor;
    private string scoringMethod;
    private bool testWalls;
    private bool startIconTest;
    private bool startBlocksTest;
    private bool hideInactiveStarts;
    private bool testFog;
    private bool oneActiveGoalInTest;
    public Locomotion activeLocomotion;
    private bool formsAtEndOfBlock = true;
    private bool formsActive = false;
    private bool userInputForTrialStart;

    public void Update()
    {
        Vector3 newpos = player.transform.position;
        pathlength += Vector2.Distance(new Vector2(newpos.x, newpos.z), new Vector2(oldpos.x, oldpos.z));
        oldpos = newpos;

        if(formsActive && !enviromentController.getFormsBeingFilled())
        {
            deactivateForms();
        }
    }

    public void onPause()
    {
        uiController.pause.enabled = true;
    }

    public void onUnPause()
    {
        uiController.pause.enabled = false;
    }

    public void disableLocomotion()
    {
        activeLocomotion.enabled = false;
    }

    public void enableLocomotion()
    {
        activeLocomotion.enabled = true;
    }

    public void setVariables(float tcoeff, float dcoeff, float at, float ad, bool rscore, float sfactor, float dz, float ftr, float ttime, float stime, string smethod, bool twalls, bool sbTest, bool siTest, bool his, bool tfog, bool oag)
    {
        timeCoeff = tcoeff;
        distCoeff = dcoeff;
        scalingFactor = sfactor;
        deadzone = dz;
        flyTestRadius = ftr;
        trialTime = ttime;
        scoreTime = stime;
        scoringMethod = smethod;
        testWalls = twalls;
        startBlocksTest = sbTest;
        startIconTest = siTest;
        hideInactiveStarts = his;
        testFog = tfog;
        training = true;
        oneActiveGoalInTest = oag;
        alpha_t = at;
        alpha_d = ad;
        roundScore = rscore;
    }

    /// <summary>
    /// Cleans all experiment objects and brings up the startup panel for starting new experiment
    /// </summary>
    public void CleanUp()
    {
        foreach (GameObject end in end_positions)
        {
            Destroy(end);
        }
        foreach (GameObject start in start_positions)
        {
            Destroy(start);
        }
        uiController.cleanup();
        GameObject locomotion = player.transform.Find("Locomotion").gameObject;
        foreach(Locomotion loc in locomotion.GetComponents<Locomotion>())
        {
            loc.enabled = false;
        }
        locomotion.GetComponent<AlignPlayerWithHMD>().enabled = false;
        session.highScoreController.Cleanup();
        session = null;
        start_positions = new List<GameObject>();
        end_positions = new List<GameObject>();
        UXF.ParticipantSelector pps = startupPanel.transform.Find("Panel").Find("SelectParticipant").gameObject.GetComponent<UXF.ParticipantSelector>();
        pps.refresh();
        notesController.ResetNotes();
        enviromentController.resetForms();
        enviromentController.hideForms();
        scoreboardPhase = false;
        scoreboardVisible = false;

        if (!GetComponent<ExperimentCreator>().loadAutopilot())
        {
            startupPanel.SetActive(true);
            GetComponent<ExperimentCreator>().resetAutopilot();
        }
    }

    /// <summary>
    /// The function that gets called by UI controller if the timer runs out
    /// End current trial and continue on to next one
    /// </summary>
    public void Timeout()
    {
        // Scoreboard timer has run out
        if (scoreboardPhase)
        {
            uiController.hideScoreboard();
            scoreboardVisible = false;
            scoreboardPhase = false;

            // Show forms, if end of block and settings say to show forms at the end of blocks
            if (session != null && session.CurrentBlock.lastTrial == session.CurrentTrial && formsAtEndOfBlock)
            {
                activateForms(2);
            }

            if (!formsActive)
            {
                if (session != null && session.CurrentTrial == session.LastTrial)
                {
                    session.End();
                }
                else if(session != null)
                {
                    session.BeginNextTrial();
                }
            }  
        }

        // Trial timer has run out
        else
        {
            tryToEnd();
        }
    }

    /// <summary>
    /// The function that gets called at the beginning of every trial
    /// </summary>
    public void onTrialStart()
    {
        // If there is no practice block, this will check and enter testing block
        if(session.CurrentTrial == session.FirstTrial && !session.CurrentBlock.settings.GetBool("practice"))
        {
            enterTestBlock();
        }
        activeStart = start_positions[UnityEngine.Random.Range(0, start_positions.Count)];
        activeStart.SetActive(true);
        if (!training && oneActiveGoalInTest)
        {
            activeGoal = end_positions[UnityEngine.Random.Range(0, end_positions.Count)];
            activeGoal.SetActive(true);
        }

        // Teleport player to the start position and orientation
        player.transform.position = activeStart.transform.position;
        player.transform.rotation = activeStart.transform.rotation;

        oldpos = player.transform.position;
        startTime = Time.time;
        uiController.setTimer(trialTime);
        pathlength = 0;
    }



    /// <summary>
    /// The function that gets called at the end of every trial
    /// </summary>
    public void onTrialEnd()
    {
        // hides the start
        if (hideInactiveStarts)
        {
            activeStart.SetActive(false);
        }

        // Hides goals if only one is needed to be active
        if (!training && oneActiveGoalInTest)
        {
            activeGoal.SetActive(false);
        }

        if(session.CurrentBlock.lastTrial == session.CurrentTrial)
        {
            // Check if entering testing block next
            if (training && session.CurrentBlock.settings.GetBool("practice"))
            {
                training = false;
                enterTestBlock();
            }
        }
    }

    /// <summary>
    /// Activates the forms. The parameter addressStage controls which list of addresses to use.
    /// </summary>
    /// <param name="addressStage"></param>
    public void activateForms(int addressStage)
    {
        enviromentController.disablePostProcessing();
        enviromentController.positionForms(player.transform);
        enviromentController.showForms(session.ppid, addressStage);
        formsActive = true;
    }

    public void deactivateForms()
    {
        enviromentController.enablePostProcessing();
        if (formsActive)
        {
            activeLocomotion.enabled = true;
            formsActive = false;
            if (session != null && session.CurrentTrial == session.LastTrial)
            {
                session.End();
            }
            else if(session != null)
            {
                uiController.showInfotext();
                userInputForTrialStart = true;
            }
        }
    }

    public void showScoreboard()
    {
        if(scoreTime > 0)
        {
            (System.Data.DataTable, bool) scores = session.highScoreController.getHighScores();
            if (scoringMethod != "none")
            {

                uiController.displayScoreboard(scores.Item1, scores.Item2, scoringMethod, roundScore);
                scoreboardVisible = true;
            }

            else {

                uiController.displayTimerOnly();
            }
            
            uiController.setScoreTimer(scoreTime);
        }
        
        scoreboardPhase = true;
    }

    
    public void EndAndPrepare()
    {
        // Log results of trial
        float time = Time.time - startTime;
        double distance = minDistance();
        double score = (timeCoeff * Math.Exp(-alpha_t*time) + distCoeff * Math.Exp(-alpha_d*distance)) * scalingFactor;
        if(score < 0)
        {
            score = 0;
        }
        
        session.CurrentTrial.result["time"] = time;
        session.CurrentTrial.result["distance"] = distance;
        session.CurrentTrial.result["score"] = score;
        session.CurrentTrial.result["scoring_method"] = scoringMethod;
        session.CurrentTrial.result["path_length"] = pathlength;

        if (scoringMethod == "none")
        {

            uiController.displayEmptyScore();
        }

        else {
            if (roundScore)
            {
                score = System.Math.Round(score, 0);
            }
            uiController.displayScore((float)score, roundScore);
        }

        // End current trial and begin next, if available
        Debug.Log("Ending trial");
        session.CurrentTrial.End();
        showScoreboard();
        
    }

    /// <summary>
    /// Calculates and returns the distance to the closest goal position
    /// </summary>
    float minDistance()
    {
        float distance = 999999;
        Vector2 pos = new Vector2(player.transform.position.x, player.transform.position.z);
        for (int i = 0; i < end_positions.Count; i++)
        {
            if(!training && oneActiveGoalInTest && end_positions[i] != activeGoal)
            {
                continue;
            }
            float dist = (pos - new Vector2(end_positions[i].transform.position[0],end_positions[i].transform.position[2])).magnitude;
            if(dist < distance)
            {
                distance = dist;
            }
        }

        if (distance < deadzone)
        {
            distance = 0;
        }
        return distance;
    }

    /// <summary>
    /// This function is called from locomotion scripts when the user tries to end a trial. Performs a check to see if ending trial is possible and ends if it is
    /// </summary>
    public void tryToEnd()
    {

        //If we were in after forms phase

        if (userInputForTrialStart)
        {
            userInputForTrialStart = false;
            uiController.hideInfotext();
            
            session.BeginNextTrial();
            return;
        }

        //If we were in scoreboard phase

        if (scoreboardPhase) {

            uiController.freezeScoreTimer();
            Timeout();
            return;


        }


        // If the trials were running, end the trial
        if (session != null && session.currentTrialNum > 0 && !session.CurrentTrial.paused && !scoreboardVisible && !formsActive)
        {
            EndAndPrepare();
        }
    }

    /// <summary>
    /// A function to alter the test environment between training and testing blocks
    /// Called at the start of the first trial of the testing block
    /// </summary>
    public void enterTestBlock()
    {
        training = false;
        // Alter start landmarks
        for(int i = 0; i < start_positions.Count; i++)
        {
            GameObject start = start_positions[i];
            start.transform.Find("Icon").gameObject.SetActive(startIconTest);
            start.transform.Find("Box").gameObject.SetActive(startBlocksTest);
        }

        // Alter the fly settings
        for(int i = 0; i < end_positions.Count; i++)
        {
            GameObject end = end_positions[i];
            FlyController flycontroller = end.GetComponentInChildren<FlyController>();
            flycontroller.setRadius(flyTestRadius);
            if (oneActiveGoalInTest)
            {
                end.SetActive(false);
            }
        }
        if (testWalls)
        {
            enviromentController.hideTraining();
            enviromentController.showTesting();
        }
        if (testFog)
        {
            RenderSettings.fog = true;
        }
    }
}