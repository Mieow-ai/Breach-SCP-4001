# SCP-4001 Breach

A video game developed with Unity.

---

## How to Play

### 1. Download
Grab the latest release from the [Releases page](https://github.com/REPLACE_WITH_YOUR_USERNAME/Breach/releases/latest).
Download the `.zip` archive for your platform and extract it on your computer.

---

### 2. Launching the Game (macOS)

Because the app is not signed with an Apple developer certificate, macOS Gatekeeper may show a warning like **"The application is damaged and can't be opened"** or **"Unidentified developer"**.

Here is how to open it:

#### Option A (Quickest)
1. **Right-click** (or `Control` + click) on `Breach.app`.
2. Select **Open**.
3. In the warning prompt, click **Open**.

#### Option B (Via System Settings)
1. Double-click the app (it will be blocked).
2. Go to **System Settings** > **Privacy & Security**.
3. Scroll down to the **Security** section.
4. Click **Open Anyway** next to the notification for Breach.

#### Option C (If it says "The application is damaged")
Open the **Terminal** and enter the following command:
```bash
xattr -cr /path/to/Breach.app
