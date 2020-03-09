# Delayed Feedback based Immervise Navigation Environment (DeFINE) for Studying Goal-Directed Human Navigation

In order to reduce the burden of the behavioral psychologists when it comes to setting up an experiment for analyzing behavior, we developed the Delayed Feedback based Immersive Navigation Environment (DeFINE). DeFINE is based on game engines like Unity3D and hence relies heavily on C# as a programming language. All the low-level implementation is already taken care-of to minimize the workload of the end-users who can simply use the modular components to either modify the existing settings or customize them to their liking. DeFINE aims specifically to provide an easy-to-use, stimulus-response-feedback architecture based experiment environment which can be used
to study goal-directed spatial navigation behavior in humans.

![practicerun](https://gitlab.com/aalto-qut/environment/uploads/0a264fc80f190aba26fc688383463b17/practicerun.gif)


## Prerequisites

The following external software and hardware is required to run the experiments.

### SteamVR

To manage interactions between the VR kit and the development environment, OpenVR package is utilized in Unity. This feature requires SteamVR to be set up on your system to work properly.

SteamVR can be directly installed from the Steam platform, please see [SteamVR webpage](https://steamcommunity.com/steamvr) for detailed instructions.

### A SteamVR Compatible VR Kit

A variety of VR kits can be used with this repository, provided that they allow for SteamVR integration. Please refer to manufacturer's instructions to set up the required software for the VR kit.


## Repository Information

This repository is presented as a ready-to-use build in the **master** branch and a standard Unity project on the **development** branch.

The **master** branch only includes the build, with the executable file that is ready to be launched. While not as extensive as **development** branch, many settings related to the experiment can be customized by modifying the settings files in the branch.

The **development** branch includes Assets, Project Settings and Packages that are used in Unity. Furthermore, an Example Results folder is included to demonstrate the logged data during a trial. The main folder also includes '.json' files that define bindings for [Vive Controllers](https://www.vive.com/eu/). In case hardware from another brand is used, similar files will be created on the first run. The experiment, environment and scenarios are fully customizable in this branch, with minimal need for coding. 

In the following sections, a brief overview of how to use the repository to run the experiments is presented. For more detailed instructions, please refer to [User Manual](https://gitlab.com/aalto-qut/environment/blob/master/user_manual.pdf).


## Running and customizing the experiments in the master branch


The **master** branch includes the built version of the project. The executable can simply be launched to start the experiments. Upon launch,  a menu with dropdown items, where you can select the experiment and participant settings will be displayed. You can either choose a previously created participant list, or create one with specifying the directory. Please note that the directory of the participant list will also be used as the place to store the experiment results, as described in "Experiment Results" section later on.

![menu_ui](https://gitlab.com/aalto-qut/environment/uploads/7fc408b5b746a8da41fcfa121bd53a3e/menu_ui.PNG)

Once the experiment and participant settings are ready, press the start button to begin the first trial. The participant should navigate to the goal position indicated by the firefly and end the trial.

![try2](https://gitlab.com/aalto-qut/environment/uploads/2c09636cd2bd1778e70ab5b280c30401/try2.png)

At the end of each trial, a scoreboard will be displayed that shows the top 10 performances of previous participants, and the achieved score and position of from the last trial (or, prospective rank and score if the trial was a practice one).

![scoreboard](https://gitlab.com/aalto-qut/environment/uploads/577448822b648e6438a89a4336699fc6/scoreboard.PNG) 


For more information on running and modifying the experiment on **master** branch, please see the following [introduction video](https://youtu.be/OVYiSHygye0).



## Experiment Results

A tracker that automatically logs the movement of the player, together with settings used in each trial and resulting score is included in the repository. After the experiment is run, a results directory will be available, categorized according to locomotion method and scenario selected. These results can be used for further analysis as desired.

An example directory of results of a session performed with "Keyboard" locomotion and "Scenario 1" can be found in the repository [here](https://gitlab.com/aalto-qut/environment/tree/development/Example%20Results). 

![results](https://gitlab.com/aalto-qut/environment/uploads/4f28b6daa8c434f91d70ab4a5605df37/results1.PNG)


The player movement files include the path taken by the player in each trial, and "trial_results" includes an overview and scoring of each trial.

![results2](https://gitlab.com/aalto-qut/environment/uploads/1c967ad9eb9cecb386b31a534548df67/results2.PNG)




## Good to know

### Dependencies
* [SteamVR](https://steamcommunity.com/steamvr)

### Feedback
If you are using DeFINE for your research or your projects, we would love to know more about your experience to further enhance the quality. Please do leave us a feedback by filling in the details [here](https://docs.google.com/forms/d/e/1FAIpQLSePMRakyGq_EsM5d0OckcP4IGakRB0sAvihEgg6nckXoHhSgw/viewform?usp=pp_url&entry.260474072=5&entry.845789868=Yes).

### Developers

* [**Kshitij Tiwari**](https://sites.google.com/view/kshitijtiwari/home)
* **Ville Sinkkonen** 
* **Onur Sari**



### License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details. If you use DeFINE for academic research, please cite as:
>  Tiwari, K., Kyrki, V., Cheung, A., & Yamamoto, N. (2020). DeFINE: Delayed Feedback based Immersive Navigation Environment for Studying Goal-directed Human Navigation.  arXiv preprint arXiv:2003.03133 (2020). http://arxiv.org/abs/2003.03133

### Acknowledgments

We would like to thank Prof. Ville Kyrki  from Aalto University, Finland, Dr. Allen Cheung from Queensland Brain Institute, Australia and Dr. Naohide Yamamoto from Queensland University of Technology, Australia for their support for developing this project.





