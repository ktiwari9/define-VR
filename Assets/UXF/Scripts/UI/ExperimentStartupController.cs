using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

namespace UXF
{
    public class ExperimentStartupController : MonoBehaviour
    {

        [Header("Quick start")]
        [Tooltip("When enabled, the experiment will instantly start using the 'quick_start' as the participant id, 1 as the session, and the save folder and settings path provided")]
        public bool quickStartMode;

        [Tooltip("Save data location in quick start (i.e. directory where the participant list is located). Relative to project path.")]
        [ConditionalHide("quickStartMode", true)]
        public string saveDataLocation = "example_output";

        [Tooltip("Name of the settings file to be used in quick start (as located in StreamingAssets folder)")]

        [ConditionalHide("quickStartMode", true)]
        public string experimentSettingsName = "example_experiment_1.json";

        [Header("User interface")]

        [Tooltip("List of datapoints you want to collect per participant. These will be generated for the GUI and added as new columns in the participant list. Participant ID is added automatically.")]
        [SerializeField]
        private List<FormElementEntry> _participantDataPoints = new List<FormElementEntry>();
        /// <summary>
        /// List of datapoints you want to collect per participant. These will be generated for the GUI and added as new columns in the participant list. Participant ID is added automatically.
        /// </summary>
        public List<FormElementEntry> participantDataPoints { get { return _participantDataPoints; } }

        [Tooltip("Maximum number available to select as the session number via the UI")]
        public int maxNumSessions = 1;

        [Tooltip("Search pattern to use when scanning the StreamingAssets folder for settings files.")]
        public string settingsSearchPattern = "*.json";

        [HideInInspector]
        public string newParticipantName = "<i><color=grey>+ New participant</color></i>";

        [Header("Instance references")]
        public SettingsSelector locomotionSelector;
        public SettingsSelector scoreSelector;
        public SettingsSelector scenarioSelector;
        public SettingsSelector environmentSelector;
        public ParticipantListSelection ppListSelect;
        public FillableFormController ppInfoForm;
        public DropDownController sessionNumDropdown;
        public Toggle AutopilotToggle;
        
        public PopupController popupController;

        public GameObject startupPanel;

        [Space]
        public Session session;

        // Autopilot variables
        [HideInInspector]
        public int autopilotIndex = 0;
        [HideInInspector]
        public System.Data.DataTable autopilotTable;
        private bool autopilotLoaded = false;
        private bool loadAutopilot = false;
        private string autopilotPpID;
        private bool first = true;
        private bool autopilotTableLoaded = false;

        void Start()
        {

            if (quickStartMode)
            {
                QuickStart();
            }
            else
            {
                ppInfoForm.Generate(participantDataPoints, true);

                List<string> sessionList = new List<string>();
                for (int i = 1; i <= maxNumSessions; i++)
                {
                    sessionList.Add(i.ToString());
                }
                sessionNumDropdown.SetItems(sessionList);

                ppListSelect.Init();
            }

        }

        public void QuickStart()
        {

            string experimentName = Path.GetFileNameWithoutExtension(experimentSettingsName);

            string path = Path.IsPathRooted(saveDataLocation) ? saveDataLocation : Path.Combine(Directory.GetCurrentDirectory(), saveDataLocation);

            if (!Directory.Exists(path))
            {
                Debug.LogErrorFormat("Quick start failed: Cannot find path {0}", path);
                return;
            }

            Action<Dictionary<string, object>> finish = new Action<Dictionary<string, object>>((dict) =>
            {
                session.Begin(
                    experimentName,
                    "quick_start",
                    path,
                    1,
                    null,
                    new Settings(dict)
                );
                startupPanel.SetActive(false);
            });

            session.ReadSettingsFile(
                Path.Combine(locomotionSelector.settingsFolder, experimentSettingsName),
                finish
            );
            
        }

        public static void SetSelectableAndChildrenInteractable(GameObject stepGameObject, bool state)
        {
            try { stepGameObject.GetComponent<Selectable>().interactable = state; }
            catch (NullReferenceException) { }

            var selectables = stepGameObject.GetComponentsInChildren<Selectable>();
            foreach (var selectable in selectables)
            {
                selectable.interactable = state;
            }
        }



        /// <summary>
        /// Called upon press of the start button in the UI. Creates the experiment session
        /// </summary>
        public void StartExperiment()
        {
            string ppid = ppListSelect.Finish();
            var infoDict = ppListSelect.GenerateDict();
            autopilotPpID = ppid;

            int sessionNum;
            Settings locomotionSettings;
            Settings environmentSettings;
            Settings scenarioSettings;
            Settings scoreSettings;

            if (first && AutopilotToggle.isOn)
            {
                string path = System.IO.Path.Combine(Application.streamingAssetsPath, "autopilot.csv");
                session.fileIOManager.ReadCSV(path, new Action<System.Data.DataTable>((dict) => { setAutopilotTable(dict); }));
                ppListSelect.GetCheckParticipantList();
                return;
            }

            else
            {
                sessionNum = int.Parse(sessionNumDropdown.GetContents().ToString());
                locomotionSettings = locomotionSelector.GetSettings();
                environmentSettings = environmentSelector.GetSettings();
                scenarioSettings = scenarioSelector.GetSettings();
                scoreSettings = scoreSelector.GetSettings();
            }

            Action finish = new Action(() =>
                {
                    session.Begin(scenarioSelector.experimentName +"_"+ locomotionSelector.experimentName,
                                                    ppid,
                                                    ppListSelect.currentFolder,
                                                    sessionNum,
                                                    infoDict,
                                                    locomotionSettings,
                                                    scoreSettings,
                                                    scenarioSettings,
                                                    environmentSettings);
                    startupPanel.SetActive(false);
                }
            );

            bool exists = Session.CheckSessionExists(scenarioSelector.experimentName + "_" + locomotionSelector.experimentName, ppid, ppListSelect.currentFolder, sessionNum);
            if (exists)
            {
                Popup existsWarning = new Popup();
                existsWarning.messageType = MessageType.Warning;
                existsWarning.message = string.Format("Warning - session \\{0}\\{1}\\{2:0000}\\ already exists. Pressing OK will overwrite all data collected for this session", session.experimentName, ppid, sessionNum);
                existsWarning.onOK = finish;
                popupController.DisplayPopup(existsWarning);
            }
            else
            {
                finish.Invoke();
            }

        }

        /***************** Autopilot functions ****************************************************************/
        public void setAutopilotTable(System.Data.DataTable table)
        {
            autopilotTable = table;
            autopilotTableLoaded = true;
        }

        public void resetAutopilot()
        {
            autopilotIndex = 0;
            first = true;
            autopilotLoaded = false;
            autopilotTableLoaded = false;
        }

        /// <summary>
        /// Sets up the autopilot settings. The next experiment session is started from update as soon as all settings have been loaded successfully.
        /// </summary>
        /// <param name="participantkey"></param>
        /// <param name="locomotionkey"></param>
        /// <param name="scenariokey"></param>
        /// <param name="environmentkey"></param>
        /// <param name="sessionnumkey"></param>
        public bool Autopilot()
        {
            string locomotion = "";
            string scoring = "";
            string scenario = "";
            string environment = "";
            string sessionNum = "";


            try
            {
                if (AutopilotToggle.isOn && autopilotIndex < autopilotTable.Rows.Count)
                {
                    System.Data.DataRow row = autopilotTable.Rows[autopilotIndex];
                    locomotion = row[0].ToString();
                    scoring = row[1].ToString();
                    scenario = row[2].ToString();
                    environment = row[3].ToString();
                    sessionNum = row[4].ToString();
                    autopilotIndex++;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }

            locomotionSelector.setFromKey(locomotion);
            scoreSelector.setFromKey(scoring);
            scenarioSelector.setFromKey(scenario);
            environmentSelector.setFromKey(environment);
            sessionNumDropdown.SetContents(sessionNum);
            loadAutopilot = true;   // indicates that we should try to load autopilot in update
            return true;
        }

        public void LateUpdate()
        {
            if(AutopilotToggle.isOn && first && autopilotTableLoaded)
            {
                first = false;
                Autopilot();
            }

            if (loadAutopilot)
            {

                // If everything has been successfully loaded, start next session with the settings
                if(locomotionSelector.loaded && scenarioSelector.loaded && environmentSelector.loaded && ppListSelect.loaded)
                {
                    ppListSelect.UpdateFormByPPID(autopilotPpID);
                    ppListSelect.participantSelector.participantDropdown.SetContents(autopilotPpID);

                    loadAutopilot = false;
                    StartExperiment();
                }
            }
        }
    }
}