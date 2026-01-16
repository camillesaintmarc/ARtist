# <img src="https://github.com/user-attachments/assets/ae1c9820-9579-4ac5-b7e9-7f23497d4936" width="50" /> ARtist

**ARtist** is an Augmented Reality application built with Unity that turns your real-world environment into a digital canvas. Users can decorate walls with virtual graffiti stickers or express themselves by drawing freely in 3D space.

## 📱 Features

* **Start Screen:** Simple and intuitive main menu to launch the experience.
* **Vertical Plane Detection:** Scans the real world to automatically detect walls/vertical surfaces suitable for art.
* **Graffiti Placement:** Select from a curated library of famous paintings and street art.
* **Preview System:** Visual preview of the selected art in the UI before placing it in the world.
* **Interactive Manipulation:** Move and Scale placed graffiti using intuitive touch gestures (Pinch-to-Scale).
* **Delete Mode:** Remove specific graffiti using a toggleable trash bin tool.
* **AR Painting:** Freehand drawing directly onto detected planes.
* **Painting Tools:** Change brush colors, Undo the last stroke, or Clear the entire canvas.
* **Occlusion:** Realistic depth masking using AR Occlusion Manager (virtual objects are correctly hidden behind real-world objects or people).

## 🛠️ Tech Stack & Unity Components

This project was developed using **Unity** and **AR Foundation**. Key components include:

* **AR Foundation & ARCore/ARKit:** For robust cross-platform AR support.
* **AR Plane Manager:** To detect and visualize vertical surfaces (walls).
* **AR Raycast Manager:** To detect touch positions relative to the physical world geometry.
* **AR Occlusion Manager:** Enabled for Environment Depth and Human Segmentation, ensuring graffiti stays realistically behind real objects.
* **Line Renderer:** Used to generate the mesh for freehand drawing in 3D space.
* **New Input System:** For handling touch inputs and multi-touch gestures.
* **Canvas & UI System:** For the dynamic interface that switches between Menu, Graffiti, and Painting modes.

## 📖 User Guide

### 1. Getting Started
* Launch the application.
* Press **START** on the main menu.
* Move your phone slowly around the room to scan your environment. Dots and planes will appear once the camera detects vertical surfaces (walls).

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
Tap the **Paint** button to enter drawing mode (The Graffiti UI will disappear):
* **Draw:** Drag one finger across a detected wall to draw.
* **Change Color:** Select a color from the palette squares in the top-right corner.
* **Undo:** Tap the **Undo** button to remove the last line drawn.
* **Clear All:** Tap **Clear All** to erase all drawings from the scene.

## 📸 Screenshots

## 📦 Installation

1.  Clone this repository.
2.  Open the project in **Unity** (Ensure you have the Android/iOS Build Support modules installed).
3.  Go to **File > Build Settings**.
4.  Add the **MenuScene** (Index 0) and the **GameScene** (Index 1) to the build.
5.  Select your platform (Android or iOS).
6.  Build and Run on your device.

---

**Developed with Unity AR Foundation.**
