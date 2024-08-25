# IGINeonixEditor
Welcome to **IGINeonixEditor**, an advanced utility for easily modifying `.res` and `.tex` texture files in *Project IGI 1* and *IGI 2*.

## Contribution
Join Project Outerloop is dedicated to reverse-engineering *Project IGI 1* and *IGI 2*, providing insights into the game's mechanics and structure.
[Join Project Outerloop on Discord](https://discord.gg/G9F3gauatb)

##IGI Research
### Data Section
The `Data` section contains all the files used for game levels, AI, 3D objects, graphs, using runtime or static analysis methods.
- **`Data/AI`**: Contains AI behavior and action scripts used in levels, such as `level/AI/503.qvm`.
- **`Data/GRAPHS`**: Includes game graph data like area information and node, vertex, and material details, with some graphs converted to [SVG](https://en.wikipedia.org/wiki/Scalable_Vector_Graphics) format.
- **`Data/MISC`**: Holds information on game variables and constants used in the IGI game engine, along with a comprehensive [Cheat Engine](https://en.wikipedia.org/wiki/Cheat_Engine) table (`PROJECT-IGI-1.CT`) that includes data for players, AI, vehicles, game states, profiles, and more.
- **`Data/GRAPH`**: Contains detailed analysis of the game’s graph structure, nodes, vertices, and signatures, including a Russian translation of the notes.
- **`Data/MEF`**: Explains the structure of MEF files.
- **`Data/QVM`**: Details the structure of QVM files.
- **`Data/QSC`**: Provides an explanation of QSC (Q-Scripts) files, which were decompiled using a Python tool.
- **`Data/NATIVES`**: 
  - `IGI-Natives.json` contains 81 native methods used in *Project IGI 1*, decompiled from the original `igi.exe` using IDA/Ghidra for educational purposes.
  - `IGI-Models.json` lists nearly 600 building and object models, including MEF data, extracted using the developers’ debug mode, which can be unlocked with [this script](https://gist.github.com/haseeb-heaven/721d82fccc8de3e6da95cfa609230cea).

The `Templates` section contains templates for [010 Editor](https://www.sweetscape.com/010editor/) for both IGI 1 and IGI 2.

## Disclaimer
Some data was decompiled from the original `igi.exe` using [IDA](https://hex-rays.com/ida-pro/) and [Ghidra](https://ghidra-sre.org/) for educational purposes only, without intent to harm game files or infringe on intellectual property rights.

## Credits
- **neoxaero** - Neonix Team CEO
- **Soon**	- Coding IGINeonixEditor
- **Cynergy** - Project Outerloop CEO
- **HeavenHM** - IGI 1 Tools
- **Yoejin Light** - MTP, models structure, and information
- **Dimon Krevedko** - Graphs and nodes structure and information
- **Artiom Rotari** - DConv tools for decompiling and scripting
- **ORWA S** - Compilation of information on graphs area and nodes
- **GM123** - Detailed models information
- **Dark** - Contributions to various projects and files (Resources, QVM, QSC, etc.).
- **FeritCoder** - Assistance with converting IGI 2 editor maps/models to IGI 1