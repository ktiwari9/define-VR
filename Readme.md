# Delayed Feedback based Immervise Navigation Environment (DeFINE) for Studying Goal-Directed Human Navigation

In order to reduce the burden of the behavioral psychologists when it comes to setting up an experiment for analyzing behavior, we developed the Delayed Feedback based Immersive Navigation Environment (DeFINE). DeFINE is based on game engines like Unity3D and hence relies heavily on C# as a programming language. All the low-level implementation is already taken care-of to minimize the workload of the end-users who can simply use the modular components to either modify the existing settings or customize them to their liking. DeFINE aims specifically to provide an easy-to-use, stimulus-response-feedback architecture based experiment environment which can be used
to study goal-directed spatial navigation behavior in humans.

![mainscene](https://gitlab.com/aalto-qut/environment/uploads/0a264fc80f190aba26fc688383463b17/practicerun.gif)

## Prerequisites

The following external software is required to run the experiments.

### Unity

This project was developed with Unity version 2019.1.0f2. Please refer to the official [Unity webpage](https://unity.com/) for download and installation instructions. Using the latest available version is recommended.


### SteamVR

To manage interactions between the VR kit and the development environment, OpenVR package is utilized in Unity. This feature requires SteamVR to be set up on your system to work properly.

SteamVR can be directly installed from the Steam platform, please see [SteamVR webpage](https://steamcommunity.com/steamvr) for detailed instructions.

### A SteamVR Compatible VR Kit

A variety of VR kits can be used with this repository, provided that they allow for SteamVR integration. Please refer to manufacturer's instructions to set up the required software for the VR kit.

### Base Framework

Unity Experiment Framework ([UXF](https://github.com/immersivecognition/unity-experiment-framework)) is used as the base framework to run the experiments and log the experiment results, and further developed to create this environment that meets the needs of navigation experiments with feedback. The [UXF Wiki](https://github.com/immersivecognition/unity-experiment-framework/wiki) can be checked for more detailed explanation of the used terminology (Trials, Sessions etc), however, as the framework is already integrated in this repository, no further knowledge or installation is required to run the experiments.


## Repository Information

This repository is presented as a ready-to-use build in the **master** branch and a standard Unity project on the **development** branch.

The **master** branch only includes the build, with the executable file that is ready to be launched. While not as extensive as **development** branch, many settings related to the experiment can be customized by modifying the settings files in the branch.

The **development** branch includes Assets, Project Settings and Packages that are used in Unity. Furthermore, an Example Results folder is included to demonstrate the logged data during a trial. The main folder also includes '.json' files that define bindings for [Vive Controllers](https://www.vive.com/eu/). In case hardware from another brand is used, similar files will be created on the first run. The experiment, environment and scenarios are fully customizable in this branch, with minimal need for coding. 

In the following sections, a brief overview of how to use the repository to run the experiments is presented. For more detailed instructions, please refer to [User Manual](https://gitlab.com/aalto-qut/environment/blob/master/user_manual.pdf).


## Running and customizing the experiments in in the development branch

As the **development** branch is structured as a Unity project, it should be launched from Unity Hub. If you wish to run the experiment on Unity without modifications, double click the MainScene object to load the scene.

![mainscene](https://gitlab.com/aalto-qut/environment/uploads/1b2cb3fb708344f18d4558a6a3bb23f7/mainscene.PNG)

Then, click the Play button and repeat run the experiment in the same way as described for **master** branch.

This branch offers many customization options with minimal need for coding. To generate any goal-directed navigation experiment, you can simply modify the start and goal prefabs, or the environment game objects as shown in the picture below. They will be launched and controlled as specified in project settings files without any need of modifying the attached scripts. 

![modifications](https://gitlab.com/aalto-qut/environment/uploads/033196bc15a022f08223fc32d3725456/modifications.PNG)


Please see the following video for a more detailed overview of some possible modifications: [Video in Youtube](https://youtu.be/smIp5n9kyAM).

## Good to know

### Dependencies

* [Unity](https://unity.com/)
* [UXF](https://github.com/immersivecognition/unity-experiment-framework) - Framework for creating human behaviour experiments in Unity
* [SteamVR](https://steamcommunity.com/steamvr)


### Contributing

Please read [CONTRIBUTING.md](https://gitlab.com/aalto-qut/environment/blob/development/Contributing.md) for details on our code of conduct, and the process for submitting pull requests to us. In order to let us know that you are interested in contributing to this project, please reach our to our dev team using the [contact form](https://docs.google.com/forms/d/e/1FAIpQLSdYFUWHLymib7pcJrPEvQqEnBj9gJ_MsG9DZJq77wTlphRyRA/viewform?usp=pp_url).

### Feedback
If you are using DeFINE for your research or your projects, we would love to know more about your experience to further enhance the quality. Please do leave us a feedback by filling in the details [here](https://docs.google.com/forms/d/e/1FAIpQLSePMRakyGq_EsM5d0OckcP4IGakRB0sAvihEgg6nckXoHhSgw/viewform?usp=pp_url&entry.260474072=5&entry.845789868=Yes).

### Developers

* **Kshitij Tiwari**  - [Personal Website](https://sites.google.com/view/kshitijtiwari/home)
* **Ville Sinkkonen** 
* **Onur Sari**



### License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

### Acknowledgments

We would like to thank Prof. Ville Kyrki  from Aalto University, Finland, Dr. Allen Cheung from Queensland Brain Institute, Australia and Prof. Naohide Yamamoto from Queensland University of Technology, Australia for their support for developing this project.





