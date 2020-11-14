# VDungeon
An immersive way to play RPGs online with your friends 

### Test Gyroscope on mobile Android device

1. Change the scene to ***MobileView***
2. Select ***Build Setting***, switch the platform to Android (make sure Android Build Support has been installed)
3. Connect your Andriod device to your computer using a USB cable, make sure your phone has change to develop mode
4. In ***Build Setting > Android***, choose your device on the drop-done menu of ***Run Device***
5. Click ***Build And Run*** to test this scene on your phone. This will also save an apk on your computer.

You could also click ***Build*** to directly save an apk on your computer, and send it to your mobile to install and run. Here is an [apk](https://drive.google.com/file/d/165wnzIi1u_5Lrr9eO59EoHXFHgNPnhS8/view?usp=sharing) that have been generated and could be directly run.

For more information, please check [Android environment setup](https://docs.unity3d.com/Manual/android-sdksetup.html).

### Sychrosize the mobile view on Unity Editor to test
Check [Unity Remote](https://docs.unity3d.com/Manual/UnityRemote5.html)

### Face tracking environment setup
1. Make sure you have the Python 3 set up in your local environment
2. Use pip to inastall the required libraries: 
   - OpenCV: https://pypi.org/project/opencv-python/
   - cmake: https://pypi.org/project/cmake/
   - dlib: https://pypi.org/project/dlib/
3. Clone the repository to your local environment and open the project in Unity
4. Create am empty Game Object and attach the script called "SocketClient" to the Game Object
5. Press the Play button to run the project
6. Terminate the python script by pressing ESC, press the play button again will not terminate the python server [TEMPORARY]
