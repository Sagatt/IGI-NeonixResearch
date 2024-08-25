# IGINeonixEditor
Welcome to **IGINeonixEditor**, an advanced utility for modifying textures in *Project IGI 1* and *IGI 2*.

## Features
- **Texture Replacement**: Easily modify `.res` and `.tex` texture files.

## Contribution
Join Project Outerloop is dedicated to reverse-engineering *Project IGI 1* and *IGI 2*, providing insights into the game's mechanics and structure.
[Join Project Outerloop on Discord](https://discord.gg/G9F3gauatb)

## Disclaimer
Please note that modifying game assets may violate the game's terms of service or intellectual property rights. Use this tool responsibly and ethically, strictly for personal or educational purposes.

## Data Section
The `Data` section contains all the files used for game levels, AI, 3D objects, graphs, and more.

- **`Data/AI`**: Contains AI behavior and action scripts used in levels, such as `level/AI/503.qvm`.
- **`Data/Graphs`**: Includes game graph data like area information and node, vertex, and material details, with some graphs converted to [SVG](https://en.wikipedia.org/wiki/Scalable_Vector_Graphics) format.
- **`Data/Misc`**: Holds information on game variables and constants used in the IGI game engine, along with a comprehensive [Cheat Engine](https://en.wikipedia.org/wiki/Cheat_Engine) table (`PROJECT-IGI-1.CT`) that includes data for players, AI, vehicles, game states, profiles, and more.

## Research Section
The `Research` section includes all research conducted on game files and memory using runtime or static analysis methods.

- **`Research/GRAPH`**: Contains detailed analysis of the game’s graph structure, nodes, vertices, and signatures, including a Russian translation of the notes.
- **`Research/MEF`**: Explains the structure of MEF files.
- **`Research/QVM`**: Details the structure of QVM files.
- **`Research/QSC`**: Provides an explanation of QSC (Q-Scripts) files, which were decompiled using a Python tool.
- **`Research/Natives`**: 
  - `IGI-Natives.json` contains 81 native methods used in *Project IGI 1*, decompiled from the original `igi.exe` using IDA/Ghidra for educational purposes.
  - `IGI-Models.json` lists nearly 600 building and object models, including MEF data, extracted using the developers’ debug mode, which can be unlocked with [this script](https://gist.github.com/haseeb-heaven/721d82fccc8de3e6da95cfa609230cea).

## Tools Section
The `Tools` section provides custom tools developed for *Project IGI 1* and *Project IGI 2* during research.

- **`ResView`**: View IGI resource files like `.tex`, `.spr`, and `.tga` without extracting them. (Dev: Dark)
- **`ResConv`**: Pack/unpack IGI resource files, such as `.res` files. (Dev: Dark/HM)
- **`3DSMax Tools`**: Export models created in [3ds Max](https://en.wikipedia.org/wiki/Autodesk_3ds_Max) (v8, 2005) to the MEF format used in IGI. (Dev: IGI-Devs)
- **`QCompiler`**: Compile/decompile game scripts and binary files, including `QSC -> QVM` and `QVM -> QSC`. (Dev: HM)
- **`Mtp_Decoder`**: Convert `MTP` objects like `level1.dat` to `level1.mtp`, useful for adding custom objects to levels. (Dev: Unknown)
- **`EngineExt`**: Extract variables and data from the [game engine](https://en.wikipedia.org/wiki/Game_engine) for external use. (Dev: HM)
- **`QVMEditor`**: A powerful tool for viewing and editing QVM files, featuring model information, syntax highlighting, auto-complete, and more. (Dev: HM)
- **`GraphEditor`**: A powerful tool for viewing and editing graph files like `Graph4019.dat`, including nodes, links, and vertices analysis. (Dev: HM)
- **`Natives-Info`**: View game native methods (e.g., `HumanPlayerLoad()`) and their signature and hash information. (Dev: HM)
- **`TGaConv`**: A texture converter tool that converts `.tex` files (e.g., `wood_material.tex`) to `PNG` format and vice versa.
  - Convert texture to PNG: `tgaconv.exe filename.tga -ToPng`
  - Convert texture to TGA: `tgaconv.exe filename.png -ToTga`

## Templates Section
The `Templates` section contains templates for [010 Editor](https://www.sweetscape.com/010editor/) for both IGI 1 and IGI 2.

## Data Disclaimer
Some data was decompiled from the original `igi.exe` using [IDA](https://hex-rays.com/ida-pro/) and [Ghidra](https://ghidra-sre.org/) for educational purposes only, without intent to harm game files or infringe on intellectual property rights.

## Credits
- **neoxaero** - Neonix Team CEO
- **Soon**	- Coding IGINeonixEditor
- **Cynergy** - Creating Project Outerloop
- **Feritcoder** - Reverse Engineering IGI2
- **Yoejin Light** - Reverse Engineering IGI1
- **Dimon Krevedko** - [VK Profile](https://vk.com/dimonkrevedko)
- **Artiom Rotari** - [GitHub Profile](https://github.com/NEWME0)
- **ORWA S** - Graphs Area and Nodes
