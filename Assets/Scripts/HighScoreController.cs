using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;



namespace UXF
{
    public class HighScoreController : MonoBehaviour
    {
        public Session session { get; private set; }

        [Tooltip("The name of the file to save scores to.")]
        public string savefile = "highscores.csv";
        [Tooltip("The name of the file to load scores to.")]
        public string loadfile = "highscores.csv";
        [Tooltip("Should we retain the scores between sessions (assuming the program is not closed in between) or cleanup after every session")]
        public bool retain;
        [Tooltip("If using a fake scoreboard")]
        public bool fakeBoard;

        System.Data.DataTable scores;
        FileIOManager fileIOManager;
        string basepath;
        private int newestScoreIndex = -1;
        [HideInInspector]
        public bool practice;

        public void SetReferences(Session s, FileIOManager iomanager, string p)
        {
            session = s;
            fileIOManager = iomanager;
            basepath = p;
            
        }

        /// <summary>
        /// Setups and loads the scoreboard from file.
        /// If loadfile is empty string, the scores aren't loaded from any file, but a new board is created.
        /// If savefile is empty string, the changes to the scores are not actually saved to file, but the changes will still be visible during the session.
        /// If the retain flag is set, the system will keep the scores for as long as the program is running.
        /// </summary>
        /// <param name="savefile">Filename to save the scores to. If empty, changes to scores are not saved to file</param>
        /// <param name="load_file">The file to load scores from. If empty, will create a brand new list instead of loading from file.</param>
        /// /// <param name="doRetain">Retain the scores between sessions (when the program is kept running).</param>
        /// /// /// <param name="fake">Using a fake scoreboard.</param>
        public void Setup(string save_file, string load_file, bool doRetain, bool fake)
        {
            savefile = save_file;
            loadfile = load_file;
            retain = doRetain;
            fakeBoard = fake;
            if (load_file != "")
            {
                LoadHighScores(load_file);
            }
        }

        public void LoadHighScores()
        {
            if (scores == null && loadfile != "")
            {
                string path = Path.Combine(basepath, loadfile);
                if (System.IO.File.Exists(path))
                {
                    fileIOManager.ReadCSV(path, new System.Action<System.Data.DataTable>((dict) => { scores = dict; }));
                }
                else
                {
                    Debug.LogWarning("Specified highscore file does not exist! Proceeding without.");
                }
            }
        }

        public void LoadHighScores(string fname)
        {
            fileIOManager.ReadCSV(Path.Combine(basepath, fname), new System.Action<System.Data.DataTable>((dict) => { scores = dict; }));
        }

        public void SaveHighScores()
        {
            WriteFileInfo fileInfo = new WriteFileInfo( WriteFileType.Dictionary, basepath, savefile);
            fileIOManager.WriteCSV(scores, fileInfo);
        }

        public (System.Data.DataTable, bool) getHighScores()
        {
            return (scores, practice);
        }

        public void Cleanup()
        {
            if (!retain)
            {
                scores = null;
            }
        }

        public int getLastIndex()
        {
            return newestScoreIndex;
        }

        /// <summary>
        /// A function to compare a score against the scoreboard and update the scoreboard if the new score is higher than previous scores.
        /// Only updates the board if the current block does not have a setting "practice" set to true
        /// </summary>
        /// <param name="partID">The participant ID</param>
        /// <param name="newscore">The new score that should be compared to the scoreboard</param>
        public void CompareHighScores(string partID, float newscore)
        {
            // Check if this is practice block and ignore if it is. If such key has not been set, continue as usual
            try
            {
                if (session.CurrentBlock.settings.GetBool("practice"))
                {
                    practice = true;
                }
                else
                {
                    practice = false;
                }
            }
            catch (KeyNotFoundException) { practice = false; }

            // If there is no table, make one unless using a fake scoreboard.
            if(scores == null && !fakeBoard)
            {                
                scores = new System.Data.DataTable();
                scores.Columns.Add("Participant ID");
                scores.Columns.Add("Score");
            }

            for(int i = 0; i < 10; i++)
            {
                // If the row does not exist, make it and fill it with newscore
                if (scores.Rows.Count <= i)
                {
                    if (!practice)
                    {
                        scores.Rows.Add(partID, newscore);
                        scores.AcceptChanges();
                    }
                    newestScoreIndex = i;
                    return;
                }

                // If newscore higher than the value on this row, put newscore to this row
                if (newscore > Convert.ToSingle(scores.Rows[i][1]))
                {
                    if (!practice)
                    {
                        System.Data.DataRow toInsert = scores.NewRow();
                        toInsert[0] = partID;
                        toInsert[1] = newscore;
                        scores.Rows.InsertAt(toInsert, i);
                        Debug.Log("NEW HIGHSCORE");

                        // If there are more than 10 scorerows, remove the extra ones.
                        for (int n = scores.Rows.Count - 1; n > 9; n--)
                        {
                            scores.Rows.RemoveAt(n);
                        }

                        // Save changes to the file
                        scores.AcceptChanges();

                        // If not using a fake scoreboard, save the changes to the file
                        if (savefile != "")
                        {
                            SaveHighScores();
                        }

                        // Invoke the new highscore actions
                        session.onNewHighscore.Invoke(session.CurrentTrial);
                    }
                    newestScoreIndex = i;
                    return;
                }
            }
            newestScoreIndex = -1;
        }
    }
}