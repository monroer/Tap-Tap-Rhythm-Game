# CS467-Tap-Tap-Rhythm-Capstone
2D tap-based rhythm game for Android/iOS built in Unity (C#) — CS467 capstone project

By Jared Reddy, Riley Monroe, and Philip Gadsden

A lightweight, fully client-side tap-rhythm game for Android, built in Unity over an 8-week capstone sprint. No ads, no accounts, no live-service overhead, just pick up the device and play.

<img width="455" height="1005" alt="image" src="https://github.com/user-attachments/assets/0af5f536-a233-4caf-a8bd-13666deb5bbf" /><img width="453" height="1004" alt="image" src="https://github.com/user-attachments/assets/64ab2da7-8ee3-4c23-b5f2-be958749a832" /><img width="452" height="1003" alt="image" src="https://github.com/user-attachments/assets/72256d9d-b953-455e-8d7c-615f7a60e5ce" />


What It Does

Players choose a 3-rail or 4-rail mode and one of two songs. Notes fall down color-coded rails in time with the beat, and the player taps the matching rail as each note reaches the hit zone. Timing accuracy is graded (Good / Great / Perfect!), and consecutive hits build a streak that triggers Hot Streak mode; a score multiplier with escalating visual feedback. A miss resets the streak. At the end of a song, a results screen shows final score, accuracy, hit/miss count, and longest streak.

My Contributions

This was a 3-person team project (Jared Reddy, Riley Monroe, Philip Gadsden). I owned the gameplay logic layer:

InputManager — multi-rail tap detection using Unity's Input System package (event-driven, low-latency multi-touch), firing onRailTapped events with rail index + a DSP timestamp
HitDetectionManager — compares tap timing against each note's target hit time to classify hits vs. misses, tuned to a ~24–80ms responsiveness window
ScoreManager — tracks score, hits, misses, accuracy, and multiplier with Inspector-exposed tuning values
HotStreakStateMachine — Normal → Building → Hot Streak transitions, streak tracking, miss-reset logic, and state-change events consumed by the HUD/visuals
GameManager — top-level game state machine (MainMenu → Playing → Paused → GameOver → Results)

Key technical problem solved: frame-based input polling produced inconsistent timing delay. I solved this by using AudioSettings.dspTime as a shared clock on both the input side and the note-spawning side, rather than each system tracking time independently — this also fixed timing drift across pause/resume.

Tech Stack
Engine/Language: Unity 6, C#
Systems: Unity Input System package, AudioSettings.dspTime for audio-synced timing, TrailRenderer for Hot Streak visuals, PlayerPrefs for local stats storage
UI: Unity UI (canvas-based) + TextMeshPro
Tools: Android Studio (build/APK), VS Code, GitHub with branch-protection rulesets and required PRs, GitHub Projects for task tracking
Lessons Learned
Editor testing ≠ physical device testing — an EventSystem bug made menu buttons unresponsive on-device despite working fine in-editor
A shared clock only works if every system reads from it — independently-timed components drift
Integration and cross-system wiring consistently took longer than building each feature in isolation
Try It

An installable APK is available in Releases (https://github.com/monroer/Tap-Tap-Rhythm-Game/releases/tag/v1.0.0). Requires Android 7.0+.

Future Work

Broader device testing (only tested on a Moto G Pure), iOS support, additional songs, and custom song upload.





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

- Main Menu: Toggle 4-rail mode on or off (leaving it off plays in 3-rail mode), select a song from the dropdown menu (Song A or B). The default Song choice is Song A. Tap Start Game to begin the game.
- Gameplay: Notes fall down colored rails toward the horizontal target line near the bottom of the screen. Tap or click (if running in Unity) the rail directly below a note as it crosses the target line. Score, streak, and accuracy update live in the HUD.
- Hot Streak: Build several consecutive hits in a row to trigger Hot Streak mode. The score multiplier increases, and the background and note trails change visually to indicate Hot Streak. A single miss resets the streak and multiplier back to baseline.
- Pause: Tap the Pause button in the top corner during gameplay to access Resume, Restart, and Back to Main Menu options.
- Results Screen: After a song completes, a summary screen displays the final score, accuracy, hits, misses, and longest streak. From here, choose to either Restart Song to play again or Main Menu to select a different song or rail mode.
- Local Data: All session and highscore data is shown directly after the song finishes. It is not stored at this point in time for top highscores. No account is needed, no sign-in, and no network connection at any point to use the application.

System Requirements and Notes
- Android 8.0 or newer is recommended.
- The build does target both ARMv7 and ARM64 architectures. Therefore, this covers a large majority of the Android devices currently in use today.
- No internet connection is required to install (after downloading the APK) or to play.
- No account, login, or testing credentials are needed anywhere in this application.
- The application was developed and validated primarily on a Moto G Pure. The behavior on other screen sizes was not tested due to time constraints and inability to procure additional test devices.
