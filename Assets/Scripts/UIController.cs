using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Text score;
    public Text timer;
    public Text scoretimer;
    public Text pause;
    public Text scoreboard;
    public Text Info;
    public GameObject experimentControls;
    public int highscoreColumnWidth = 25;

    private ExperimentController experimentController;
    private float time;
    private bool runout;
    private float scoretime;
    private bool scorerunout;
    private bool scoretimefreeze;
    private string scoringMethod;

    // Start is called before the first frame update
    void Start()
    {
        experimentController = experimentControls.GetComponent<ExperimentController>();
        
    }

    // Get the scoring method from the experiment createor
    public void setScoringMethod(string method) {
        scoringMethod = method;
    }


    // Display the initial score, or nothing if the scoring method is "none"
    public void displayEmptyScore() {
        if (scoringMethod == "none")
        {
            score.text = " ";
        }
        else {
            score.text = "Latest Score:\n 0";
        }
        

    }

    // Display the score of the previous trial on the top side of VR screen
    public void displayScore(float scoreNum, bool round)
    {
        score.text = "Latest Score:\n" + (round ? System.Math.Round(scoreNum, 0).ToString() : scoreNum.ToString());
        //score.text = "Latest Score:\n" + scoreNum.ToString();
    }


    
    //Display the scoreboard
        public void displayScoreboard(System.Data.DataTable table, bool practice, string scoringMethod, bool round)
    {
        int rankpad = 10;
        string newscore;
        string partID = experimentController.session.ppid;
        int newindex = experimentController.session.highScoreController.getLastIndex();

        string scorestr = "";
        string header = "RANK".PadRight(rankpad);

        double nscore = System.Convert.ToSingle(experimentController.session.CurrentTrial.result["score"]);
        nscore = round ? System.Math.Round(nscore) : nscore;
        newscore = nscore.ToString();

        header += "PARTICIPANT ID".PadRight(highscoreColumnWidth);
        scorestr += "<b><color=#ffff00>" + header + " SCORE</color></b>\n";
        if (table == null)
        {
            scorestr += "\nNo highscores logged yet\n";
            scorestr += "\nPlease create a table first\n";
            scorestr += "\n...\n";
            string id = "Your last score";
            id = id.PadRight(highscoreColumnWidth);
            scorestr += id + " " + newscore + "  <color=red>*Practice*</color>\n";
            scoreboard.text = scorestr;
            scoreboard.enabled = true;
            scoretimer.enabled = true;
            return;
        }

        bool onlist = false;
        int rownum = 0;
        foreach (System.Data.DataRow row in table.Rows)
        {
            string rank = (rownum + 1).ToString().PadRight(rankpad);
            string id = row[0].ToString().PadRight(highscoreColumnWidth);
            string points = round ? System.Math.Round(System.Convert.ToSingle(row[1]),0).ToString() : row[1].ToString();

            // Participant made a new highscore in the last trial
            if (rownum == newindex && !practice)
            {
                onlist = true;
                id = "NEW HIGHSCORE".PadRight(highscoreColumnWidth);
                scorestr += "<b><color=red>" + rank + id + points + "</color></b>\n";
                rownum++;
                continue;
            }

            // Participant would have made it to board, but it was practice
            if(rownum == newindex && practice)
            {
            //    rank = "<b><Color=red>" + rank + "</color></b>";
            }

            // A highscore made by current participant in another trial
            else if (id == partID.PadRight(highscoreColumnWidth))
            {
                id = "** YOU **".PadRight(highscoreColumnWidth); ;
            }

            // Write the row
            scorestr += rank + id + points + "\n";
            rownum++;
        }

        // If participant didn't get to board on this trial, show the score below the table
        if (!onlist)
        {
            scorestr += "...\n";
            string id = "Your last score".PadRight(highscoreColumnWidth);
            string rank;
            if(newindex != -1)
            {
                rank = (newindex +1).ToString().PadRight(rankpad);
            }
            else
            {
                rank = "-".PadRight(rankpad);
            }
            scorestr += "<b><Color=red>" + rank + id + " " + newscore + "</color></b>";
            if (practice)
            {
                scorestr += "  <color=red>*Practice*</color>";
            }
            
            scorestr += "\n";

        }

        scoreboard.text = scorestr;
        scoreboard.enabled = true;
        scoretimer.enabled = true;
    }


    // This function is used in case the settings are not displaying the scoreboard. In that case, the time to next trial should still be displayed
    public void displayTimerOnly() {

        scoretimer.enabled = true;

    }


    // Hide the scoreboard after it has been shown
    public void hideScoreboard()
    {
        scoreboard.enabled = false;
        scoretimer.enabled = false;
    }

    
    // During fixedupdate, the timer for the trial and scoreboard are ticked down in their respective phases
    void FixedUpdate()
    {
        if (!runout)
        {
            time = time - Time.deltaTime;
            if (time < 0)
            {
                time = 0;
                runout = true;
                experimentController.Timeout();
            }
            float minutes = ((int)time / 60);
            float seconds = Mathf.Floor(time % 60);
            timer.text = string.Format("{0:0}:{1:00}", minutes, seconds);            
        }


        if (!scorerunout && !scoretimefreeze)
        {
            scoretime = scoretime - Time.deltaTime;
            if (scoretime < 0)
            {
                scoretime = 0;
                scorerunout = true;
                experimentController.Timeout();
            }
            float minutes = ((int)scoretime / 60);
            float seconds = Mathf.Floor(scoretime % 60);
            scoretimer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            scoretimer.text = "Next trial in  " + scoretimer.text;
        }




    }

    // Set the trial timer
    public void setTimer(float t)
    {
        if (t > 0)
        {
            time = t;
            runout = false;
            
        }
        else
        {
            timer.text = string.Format("0:00");
            
        }
    }

    // Set the scoreboard timer
    public void setScoreTimer(float t)
    {
        if (t > 0)
        {
            scoretime = t;
            scorerunout = false;
            scoretimefreeze = false;
            
        }
        else
        {
            scoretimer.text = string.Format("0:00");
            
        }
    }


    // This is used when the scoeboard phase has been manually skipped. In that case the timer should stop ticking down.
    public void freezeScoreTimer() {

        scoretimefreeze = true;
    }


          
    internal void cleanup()
    {
        setScoreTimer(0);
        hideScoreboard();
        setTimer(0);
        displayScore(0, false);
    }

    internal void showInfotext()
    {
        Info.enabled = true;
    }

    public void hideInfotext()
    {
        Info.enabled = false;
    }
}
