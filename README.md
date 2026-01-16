 <img src="https://github.com/user-attachments/assets/ae1c9820-9579-4ac5-b7e9-7f23497d4936" width="50" /> ARtist

**ARtist** is an Augmented Reality application built with Unity that turns your real-world environment into a digital canvas. Users can decorate walls with virtual graffiti stickers or express themselves by drawing freely in 3D space.

---

## 📱 Features

* **Start Screen:** Simple and intuitive main menu to launch the experience.
* **Vertical Plane Detection:** Scans the real world to automatically detect walls/vertical surfaces suitable for art.
* **Persistent Plane Visualization:**  
  Detected planes remain visually highlighted even after content placement. This is an intentional design decision that allows users to easily return to previously used walls and continue editing or adjusting their graffiti after moving to another surface.
* **Graffiti Placement:** Select from a curated library of famous paintings and street art.
* **Preview System:** Visual preview of the selected art in the UI before placing it in the world.
* **Interactive Manipulation:** Move and Scale placed graffiti using intuitive touch gestures (Pinch-to-Scale).
* **Delete Mode:** Remove specific graffiti using a toggleable trash bin tool.
* **AR Painting:** Freehand drawing directly onto detected planes.
* **Painting Tools:** Change brush colors, Undo the last stroke, or Clear the entire canvas.
* **Occlusion:** Realistic depth masking using AR Occlusion Manager (virtual objects are correctly hidden behind real-world objects or people).

---

## 🛠️ Tech Stack & Unity Components

This project was developed using **Unity** and **AR Foundation**. Key components include:

* **AR Foundation & ARCore/ARKit:** For robust cross-platform AR support.
* **AR Plane Manager:** To detect and visualize vertical surfaces (walls).
* **AR Raycast Manager:** To detect touch positions relative to the physical world geometry.
* **AR Occlusion Manager:** Enabled for Environment Depth and Human Segmentation, ensuring graffiti stays realistically behind real objects.
* **Line Renderer:** Used to generate the mesh for freehand drawing in 3D space.
* **New Input System:** For handling touch inputs and multi-touch gestures.
* **Canvas & UI System:** For the dynamic interface that switches between Menu, Graffiti, and Painting modes.

---

## 📖 User Guide

### 1. Getting Started
* Launch the application.
* Press **START** on the main menu.
* Move your phone slowly around the room to scan your environment. For easier plane recognition move also front and back not only to sides.
  Highlighted planes will appear once the camera detects vertical surfaces (walls).

### 2. Graffiti Mode ("PlaceGraffiti")
Tap the **PlaceGraffiti** button to enter this mode:
* **Select:** Tap a name in the menu (top-left) to choose an artwork.
* **Preview:** See the selected image in the preview box (top-right).
* **Place:** Tap on a detected wall to place the graffiti.
* **Edit:**
    * **Move:** Drag the graffiti with one finger to reposition it.
    * **Scale:** Use two fingers (pinch) to make the graffiti larger or smaller.
* **Delete:** Tap the **Trash Can icon** to activate Delete Mode (the icon changes color). Then, tap any graffiti to remove it. Tap the Trash Can again to exit Delete Mode.

### 3. Paint Mode ("Paint")
Tap the **Paint** button to enter drawing mode (the Graffiti UI will disappear):
* **Draw:** Drag one finger across a detected wall to draw.
* **Change Color:** Select a color from the palette squares in the top-right corner.
* **Undo:** Tap the **Undo** button to remove the last line drawn.
* **Clear All:** Tap **Clear All** to erase all drawings from the scene.

---


## 📸 Demo: Screenshots + Video

| Start Screen | Plane Detection | Graffiti Mode | Paint Mode | Occlusion |
|-------------|----------------|---------------|------------|-----------|
| <img src="https://github.com/user-attachments/assets/c01951b7-3ca3-4bd9-84a7-e613d3e45de3" width="180"/> | <img src="https://github.com/user-attachments/assets/8ede07c9-e9c5-4fc6-aeca-2a424e6d415a" width="180"/> | <img src="https://github.com/user-attachments/assets/e51b1950-0555-43d1-b946-b67716760fe8" width="180"/> | <img src="https://github.com/user-attachments/assets/6e0468e1-8b7d-4845-acbe-d6f7d81a20f3" width="180"/> | <img src="https://github.com/user-attachments/assets/5f2eb7c8-bd86-4899-8c3e-1f2b7ea48279" width="180"/> |
| Main menu | Detected vertical surfaces | Placing & editing graffiti | Freehand AR drawing | Realistic depth masking |


See the app demonstration: [https://www.youtube.com/shorts/hZBVDFfRK2s]

---

## 📦 Installation

1. Clone this repository.
2. Open the project in **Unity** (ensure Android/iOS Build Support modules are installed).
3. Go to **File > Build Settings**.
4. Add the **MenuScene** (Index 0) and the **GameScene** (Index 1) to the build.
5. Select your platform (Android or iOS).
6. Build and Run on your device.

---

## 🎨 Asset Credits & References

The graffiti images used in this project were sourced from an online graffiti and street art database: [https://graffiti-database.com]

All images are used for **educational and non-commercial purposes only**.

---

## 🎓 Academic Context

This project was developed as part of a **university-level AR/VR course** at Management Center Innsbruck

---


