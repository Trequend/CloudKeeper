# CloudKeeper
A simple 2D game in Unity

![Demo](https://github.com/Trequend/CloudKeeper/blob/media/Demo.gif)
## Builds
### Android
[Download APK](https://github.com/Trequend/CloudKeeper/releases/download/v1.0/CloudKeeper.apk)
### WebGL
[![MainMenu](https://github.com/Trequend/CloudKeeper/blob/media/MainMenu.gif)](https://trequend.github.io/cloud-keeper)
## Assets
- 2D Art
  - [Luiz Melo](https://luizmelo.itch.io/wizard-pack)
  - [unTied Games](https://untiedgames.itch.io/floating-skull-enemy)
  - [vnitti](https://vnitti.itch.io/grassy-mountains-parallax-background)
  - [LateNightCoffee](https://latenightcoffe.itch.io/2d-pixel-art-semi-realistic-clouds)
  - [Willibab](https://willibab.itch.io/willibabs-simple-weapon-icons)
- UI Art
  - [Wenrexa](https://wenrexa.itch.io/uimobile-free)
  - [GDev](https://assetstore.unity.com/packages/2d/gui/icons/139-vector-icons-69968)
- Music
  - [GWriterStudio](https://assetstore.unity.com/packages/audio/music/8bit-music-album-051321-196147)
- Sounds
  - [Dustyroom](https://assetstore.unity.com/packages/audio/sound-fx/free-casual-game-sfx-pack-54116)
## Under the hood
### Figures
- Vertical line
- Horizontal line
- Phi
- Caret
- Epsilon
- Vi
### Figures recognition
Convolutional Neural Network (CNN) powered by:
- Python 3.7.0 (required version for TensorFlow)
- TensorFlow 2.3.0 (required version for keras2onnx)
- keras2onnx 1.7.0
- Unity Barracuda
### Dataset
- Training (about 38k samples)
- Validation (about 7k samples)
- Test (about 4k samples)
### Dataset generator
![Dataset generator](https://github.com/Trequend/CloudKeeper/blob/media/DatasetGenerator.gif)
