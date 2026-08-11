# CS467-Tap-Tap-Rhythm-Capstone
2D tap-based rhythm game for Android/iOS built in Unity (C#) — CS467 capstone project

By Jared Reddy, Riley Monroe, and Philip Gadsden

Installation Option A: Android Virtual Device
1. Install Android Studio (free) from developer.android.com/studio, if not already installed.
2. Open Android Studio and go to Tools → Device Manager.
3. Click ‘Create Virtual Device’, choose any phone profile (e.g. Pixel 10), and select a system running API 26 or higher (API 33-36 recommended for best performance).
4. Finish the wizard and click the Play button next to the new virtual device to launch the emulator.
5. Go to the repository link or open the provided source code folder. The APK will be located inside a folder labeled APKs in the top-level folder.
6. Once the emulator has fully booted to the home screen, drag and drop the downloaded .apk file directly onto the emulator window. Android will install it automatically.
7. Alternatively, install it via command line with Android SDK platform-tools installed: 
adb install path/to/Tap-Tap-Rhythm.apk
8. Locate and tap the Tap Tap Rhythm app icon (shows up as a Unity symbol) on the emulator’s home screen or app drawer to launch it.
9. Follow the Using the Application guide further below to play Tap Tap Rhythm

Installation Option B: Physical Android Device
1. Go to the repository link or open the provided source code folder. The APK will be located inside a folder labeled APKs in the top-level folder.
2. Download the TapTapRhythm.apk to your computer.
3. Transfer the APK file to an Android device. This can be done via USB cable or by emailing/uploading the file to Box or other file share services and downloading it on the device.
4. On the Android device, open the file manager app and find the downloaded APK file. Tap to begin installation. If prompted to choose an installer, choose Package Installer.
5. If prompted with “Install Blocked” or “Install Unknown Apps”, follow the instructions on screen or via the More Details tab to allow the installation of a third-party app outside of the Google Play Store. You should then be able to install the application successfully.
6. Once the installation has completed, tap to open or locate the Tap Tap Rhythm icon (Unity logo) on the device’s home screen or app drawer and launch it.
7. The application opens directly to the main menu. Refer to the “Using the Application” below to start playing.

Using the Application
Once launched, either on the Android platform or in the Unity Editor, the application can be explored as follows:

Main Menu: Toggle 4-rail mode on or off (leaving it off plays in 3-rail mode), select a song from the dropdown menu (Song A or B). The default Song choice is Song A. Tap Start Game to begin the game.
Gameplay: Notes fall down colored rails toward the horizontal target line near the bottom of the screen. Tap or click (if running in Unity) the rail directly below a note as it crosses the target line. Score, streak, and accuracy update live in the HUD.
Hot Streak: Build several consecutive hits in a row to trigger Hot Streak mode. The score multiplier increases, and the background and note trails change visually to indicate Hot Streak. A single miss resets the streak and multiplier back to baseline.
Pause: Tap the Pause button in the top corner during gameplay to access Resume, Restart, and Back to Main Menu options.
Results Screen: After a song completes, a summary screen displays the final score, accuracy, hits, misses, and longest streak. From here, choose to either Restart Song to play again or Main Menu to select a different song or rail mode.
Local Data: All session and highscore data is shown directly after the song finishes. It is not stored at this point in time for top highscores. No account is needed, no sign-in, and no network connection at any point to use the application.

System Requirements and Notes
- Android 8.0 or newer is recommended.
- The build does target both ARMv7 and ARM64 architectures. Therefore, this covers a large majority of the Android devices currently in use today.
- No internet connection is required to install (after downloading the APK) or to play.
- No account, login, or testing credentials are needed anywhere in this application.
- The application was developed and validated primarily on a Moto G Pure. The behavior on other screen sizes was not tested due to time constraints and inability to procure additional test devices.
