# <img alt="logo" height="26" src="https://github.com/user-attachments/assets/9d9dac2f-bfa5-4510-9d88-fe28ffcc1619"/> Sub Asset Editor <!-- omit in toc -->

[![](https://img.shields.io/npm/v/com.coffee.sub-asset-editor?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.sub-asset-editor/)
[![](https://img.shields.io/github/v/release/mob-sakai/SubAssetEditor?include_prereleases)](https://github.com/mob-sakai/SubAssetEditor/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/SubAssetEditor.svg)](https://github.com/mob-sakai/SubAssetEditor/releases)  
![](https://img.shields.io/badge/Unity-5.5+-57b9d3.svg?style=flat&logo=unity)
![](https://img.shields.io/badge/Unity-6000.0+-57b9d3.svg?style=flat&logo=unity)  
[![](https://img.shields.io/github/license/mob-sakai/SubAssetEditor.svg)](https://github.com/mob-sakai/SubAssetEditor/blob/main/LICENSE.md)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)
[![](https://img.shields.io/github/watchers/mob-sakai/SubAssetEditor.svg?style=social&label=Watch)](https://github.com/mob-sakai/SubAssetEditor/subscription)
[![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai)

<< [📝 Description](#-description-) | [📌 Key Features](#-key-features) | [⚙ Installation](#-installation) | [🚀 Usage](#-usage) | [🤝 Contributing](#-contributing) >>

## 📝 Description <!-- omit in toc -->

**Sub Asset Editor** is an editor tool for adding, removing, and editing sub-assets within your project.

![](https://github.com/user-attachments/assets/cdb95619-6fc6-4dcc-8287-685c2696c5d8)  
![](https://github.com/user-attachments/assets/f8e1f88d-c821-48d1-80ae-13b364fc1adb)


- [📌 Key Features](#-key-features)
- [⚙ Installation](#-installation)
   - [Install via OpenUPM](#install-via-openupm)
   - [Install via UPM (with Package Manager UI)](#install-via-upm-with-package-manager-ui)
   - [Install via UPM (Manually)](#install-via-upm-manually)
   - [Install as Embedded Package](#install-as-embedded-package)
- [🚀 Usage](#-usage)
- [🤝 Contributing](#-contributing)
   - [Issues](#issues)
   - [Pull Requests](#pull-requests)
   - [Support](#support)
- [License](#license)
- [Author](#author)
- [See Also](#see-also)

<br><br>

## 📌 Key Features

- **Add Sub-Assets**: Add sub-assets to a main asset.
    - Drag and drop assets to add them as sub-assets.
    - Convert assets referenced by the main asset into sub-assets. References are automatically updated.
- **Remove Sub-Assets**: Remove sub-assets from a main asset.
- **Remove Missing Script Assets**: (Unity 2023.2 or later) Remove ScriptableObject sub-assets with missing scripts.
- **Rename Sub-Assets**: Change the names of sub-assets.
- **Show/Hide Sub-Assets**: Toggle the visibility of sub-assets.
    - Hidden sub-assets will not appear in the Project view.
- **Synchronize selection with Project View**: Selecting a main asset in the Project view will automatically open it in the Sub Asset Editor (default: disabled).

<br><br>

## ⚙ Installation

_This package requires **Unity 5.5 or later**._

### Install via OpenUPM

- This package is available on [OpenUPM](https://openupm.com/packages/com.coffee.sub-asset-editor/) package registry.
- This is the preferred method of installation, as you can easily receive updates as they're released.
- If you have [openupm-cli](https://github.com/openupm/openupm-cli) installed, then run the following command in your project's directory:
  ```
  openupm add com.coffee.sub-asset-editor
  ```
- To update the package, use Package Manager UI (`Window > Package Manager`) or run the following command with `@{version}`:
  ```
  openupm add com.coffee.sub-asset-editor@1.2.0
  ```

### Install via UPM (with Package Manager UI)

- Click `Window > Package Manager` to open Package Manager UI.
- Click `+ > Add package from git URL...` and input the repository URL: `https://github.com/mob-sakai/SubAssetEditor.git`  
  ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/upm-add-from-url.png)
- To update the package, change suffix `#{version}` to the target version.
    - e.g. `https://github.com/mob-sakai/SubAssetEditor.git#1.2.0`

### Install via UPM (Manually)

- Open the `Packages/manifest.json` file in your project. Then add this package somewhere in the `dependencies` block:
  ```json
  {
    "dependencies": {
      "com.coffee.sub-asset-editor": "https://github.com/mob-sakai/SubAssetEditor.git",
      ...
    }
  }
  ```

- To update the package, change suffix `#{version}` to the target version.
    - e.g. `"com.coffee.sub-asset-editor": "https://github.com/mob-sakai/SubAssetEditor.git#1.2.0",`

### Install as Embedded Package

1. Download the `Source code (zip)` file from [Releases](https://github.com/mob-sakai/SubAssetEditor/releases) and
   extract it.
2. Move the `<extracted_dir>/Packages/src` directory into your project's `Packages` directory.  
   ![](https://github.com/mob-sakai/mob-sakai/releases/download/docs/upm-add-as-embedded.png)
    - You can rename the `src` directory if needed.
    - If you intend to fix bugs or add features, installing it as an embedded package is recommended.
    - To update the package, re-download it and replace the existing contents.

<br><br>

## 🚀 Usage

![](https://github.com/user-attachments/assets/a1adf613-6b53-46a5-aaa1-94546c2a0531)

1. [Install the package](#-installation).
2. Open the Sub Asset Editor from `Assets > Edit Sub Asset`.
   - You can also access it via the context menu by right-clicking an asset and selecting `Edit Sub Asset`.
3. Select a main asset.
4. To add a sub-asset, drag and drop an asset onto <img height="16" src="https://github.com/user-attachments/assets/6f51f9cc-049f-4bed-8865-5328327a9b99"/> or click the "+" button.
5. To remove a sub-asset, click the "-" button.
6. To rename a sub-asset, edit its name directly in the text field.
7. To toggle the visibility of a sub-asset, click <img height="14" src="https://github.com/user-attachments/assets/3336fe78-3429-46d4-8076-8e68955d87a1"/> or <img height="14" src="https://github.com/user-attachments/assets/1b03a5b2-7b0a-4246-9a63-c24693feca50"/>.
8. If a sub-asset is referenced by the main asset, <img height="14" src="https://github.com/user-attachments/assets/f7f8b37b-6a6a-4d18-99e3-0c67820679fa"/> will be displayed.
9. Enjoy!

<br><br>

## 🤝 Contributing

### Issues

Issues are incredibly valuable to this project:

- Ideas provide a valuable source of contributions that others can make.
- Problems help identify areas where this project needs improvement.
- Questions indicate where contributors can enhance the user experience.

### Pull Requests

Pull requests offer a fantastic way to contribute your ideas to this repository.  
Please refer to [CONTRIBUTING.md](https://github.com/mob-sakai/SubAssetEditor/tree/main/CONTRIBUTING.md)
and use [develop branch](https://github.com/mob-sakai/SubAssetEditor/tree/develop) for development.

### Support

This is an open-source project developed during my spare time.  
If you appreciate it, consider supporting me.  
Your support allows me to dedicate more time to development. 😊

[![](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)  
[![](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/2343451?)

<br><br>

## License

* MIT

## Author

* ![](https://user-images.githubusercontent.com/12690315/96986908-434a0b80-155d-11eb-8275-85138ab90afa.png) [mob-sakai](https://github.com/mob-sakai) [![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai) ![GitHub followers](https://img.shields.io/github/followers/mob-sakai?style=social)

## See Also

* GitHub page : https://github.com/mob-sakai/SubAssetEditor
* Releases : https://github.com/mob-sakai/SubAssetEditor/releases
* Issue tracker : https://github.com/mob-sakai/SubAssetEditor/issues
* Change log : https://github.com/mob-sakai/SubAssetEditor/blob/main/CHANGELOG.md
