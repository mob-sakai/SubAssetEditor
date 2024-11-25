Sub Asset Editor
===

An editor tool for SubAsset in your project.

[![](https://img.shields.io/npm/v/com.coffee.sub-asset-editor?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.coffee.sub-asset-editor/)
[![](https://img.shields.io/github/v/release/mob-sakai/SubAssetEditor?include_prereleases)](https://github.com/mob-sakai/SubAssetEditor/releases)
[![](https://img.shields.io/github/release-date/mob-sakai/SubAssetEditor.svg)](https://github.com/mob-sakai/SubAssetEditor/releases)  [![](https://img.shields.io/github/license/mob-sakai/SubAssetEditor.svg)](https://github.com/mob-sakai/SubAssetEditor/blob/main/LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-orange.svg)](http://makeapullrequest.com)  
![](https://img.shields.io/badge/Unity%205.5+-supported-blue.svg)  

<< [Description](#Description) | [Installation](#installation) | [Usage](#usage) | [Development Note](#development-note) | [Change log](https://github.com/mob-sakai/SubAssetEditor/blob/main/CHANGELOG.md) >>



<br><br><br><br>

## Description

'SubAsset' is an asset that is stored in the same file as the main asset.

It is good to organize multiple assets, to manage many assets.

For example, you can integrate Animation assets and AnimatorController asset into one file.

![image](https://user-images.githubusercontent.com/12690315/30265492-32755304-9717-11e7-8bca-f7a472a56be8.png)

![image](https://user-images.githubusercontent.com/12690315/45538310-af67ca80-b841-11e8-8dd3-46012d018891.png)


### Features

* Add other assets to main asset
* Add referencing asset to main asset
* Rename
* Export
* Delete
* Lock main asset selection



<br><br><br><br>

## Installation

### Requirement

* Unity 5.5 or later

### (For Unity 2018.3 or later) Using OpenUPM

This package is available on [OpenUPM](https://openupm.com).  
You can install it via [openupm-cli](https://github.com/openupm/openupm-cli).
```
openupm add com.coffee.sub-asset-editor
```

### (For Unity 2018.3 or later) Using Git

Find the manifest.json file in the Packages folder of your project and add a line to `dependencies` field.

* Major version: ![](https://img.shields.io/github/v/release/mob-sakai/SubAssetEditor)  
`"com.coffee.sub-asset-editor": "https://github.com/mob-sakai/SubAssetEditor.git"`

To update the package, change suffix `#{version}` to the target version.

* e.g. `"com.coffee.sub-asset-editor": "https://github.com/mob-sakai/SubAssetEditor.git#1.0.0",`

Or, use [UpmGitExtension](https://github.com/mob-sakai/UpmGitExtension) to install and update the package.

#### For Unity 2018.2 or earlier

1. Download a source code zip file from [Releases](https://github.com/mob-sakai/SubAssetEditor/releases) page
2. Extract it
3. Import it into the following directory in your Unity project
   - `Packages` (It works as an embedded package. For Unity 2018.1 or later)
   - `Assets` (Legacy way. For Unity 2017.1 or later)



<br><br><br><br>

## Usage

1. From the menu, click `Assets > Sub Asset Editor`.
2. Select an asset in project window.
3. Enjoy!



<br><br><br><br>

## Contributing

### Issues

Issues are very valuable to this project.

- Ideas are a valuable source of contributions others can make
- Problems show where this project is lacking
- With a question you show where contributors can improve the user experience

### Pull Requests

Pull requests are, a great way to get your ideas into this repository.  
See [sandbox/README.md](https://github.com/mob-sakai/SubAssetEditor/blob/sandbox/README.md).

### Support

This is an open source project that I am developing in my spare time.  
If you like it, please support me.  
With your support, I can spend more time on development. :)

[![](https://user-images.githubusercontent.com/12690315/50731629-3b18b480-11ad-11e9-8fad-4b13f27969c1.png)](https://www.patreon.com/join/mob_sakai?)  
[![](https://user-images.githubusercontent.com/12690315/66942881-03686280-f085-11e9-9586-fc0b6011029f.png)](https://github.com/users/mob-sakai/sponsorship)



<br><br><br><br>

## License

* MIT



## Author

* ![](https://user-images.githubusercontent.com/12690315/96986908-434a0b80-155d-11eb-8275-85138ab90afa.png) [mob-sakai](https://github.com/mob-sakai) [![](https://img.shields.io/twitter/follow/mob_sakai.svg?label=Follow&style=social)](https://twitter.com/intent/follow?screen_name=mob_sakai) ![GitHub followers](https://img.shields.io/github/followers/mob-sakai?style=social)



## See Also

* GitHub page : https://github.com/mob-sakai/SubAssetEditor
* Releases : https://github.com/mob-sakai/SubAssetEditor/releases
* Issue tracker : https://github.com/mob-sakai/SubAssetEditor/issues
* Change log : https://github.com/mob-sakai/SubAssetEditor/blob/main/CHANGELOG.md
