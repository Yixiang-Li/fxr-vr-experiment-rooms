# Asset provenance

## Included packages and original work

- Unity 3D URP template: Unity 6000.3.24f1, template 17.0.14.
- XR Interaction Toolkit **3.3.2 Starter Assets** and **XR Interaction Simulator**: Unity Technologies. Original metadata and [Unity Companion License notice](ThirdParty/XRInteractionToolkit-LICENSE.md) are retained.
- TextMesh Pro / LiberationSans SDF: bundled Unity resources; retained notices in `Assets/TextMesh Pro/Fonts/`.
- Experiment code, card/symbol meshes, UI, supplementary furniture, architectural details, water feature and short neutral task cues: created for this project with AI assistance.

## Free 3D models and photographic materials

Source: **Poly Haven**, [CC0 license](https://polyhaven.com/license). Included assets may be redistributed with this project. Courtesy credits are provided even though attribution is not required by CC0.

| Asset | Source |
| --- | --- |
| Metal office desk | [metal_office_desk](https://polyhaven.com/a/metal_office_desk) |
| Upholstered chair | [modern_arm_chair_01](https://polyhaven.com/a/modern_arm_chair_01) |
| Large plant | [potted_plant_01](https://polyhaven.com/a/potted_plant_01) |
| Broadleaf plant | [potted_plant_02](https://polyhaven.com/a/potted_plant_02) |
| Small plant | [potted_plant_04](https://polyhaven.com/a/potted_plant_04) |
| Clock | [wall_clock](https://polyhaven.com/a/wall_clock) |
| Desk lamp | [desk_lamp_arm_01](https://polyhaven.com/a/desk_lamp_arm_01) |
| Wood surface | [wood_table_001](https://polyhaven.com/a/wood_table_001) |
| Tiled floor | [floor_tiles_06](https://polyhaven.com/a/floor_tiles_06) |
| Plaster | [painted_plaster_wall](https://polyhaven.com/a/painted_plaster_wall) |
| Forest window | [forest_slope](https://polyhaven.com/a/forest_slope) |
| City window | [potsdamer_platz](https://polyhaven.com/a/potsdamer_platz) |

Included FBX geometry and 1K textures live in `Assets/Rooms/Memory/ThirdParty/PolyHaven/`. Texture adaptations: Unity metallic/smoothness packing, normal-map import, foliage transparency, Android ASTC compression, and mesh grouping by material without dropping source submeshes. Window backplates are perspective crops from the official tonemapped HDRI files, **not website example renders**. They are fixed distant views, not explorable outdoor environments.

Each asset has a `source.json`. Source FBX/checksum metadata and download preparation are in `tools/fetch_real_assets.py`; Unity material assignment and placement are automated by `MemorySourceAssets.cs` and `MemoryRoomPolisher.cs`. Teammates receive ready assets and do not need Blender or the download script.

## Actual recorded ambience

All four source pages identify the recording as **CC0**. Public high-quality MP3 previews linked by Freesound were downloaded and decoded; these are not the original uncompressed uploads.

| Project clip | Recording and author |
| --- | --- |
| Office | [Government office ambience, Globofonia, #563248](https://freesound.org/people/Globofonia/sounds/563248/) |
| OfficeDetail | [Computer keyboard, Breviceps, #447909](https://freesound.org/people/Breviceps/sounds/447909/) |
| Nature | [Forest birds, NightVoice, #123999](https://freesound.org/people/NightVoice/sounds/123999/) |
| NatureDetail | [Forest stream, guidofm, #509174](https://freesound.org/people/guidofm/sounds/509174/) |

Edits: mono 44.1 kHz conversion, DC removal, loop seams, irregular pauses for typing, smooth peak limiting and matched source RMS. Each base clip has RMS 0.050 and each detail clip RMS 0.018, before Unity gain and spatial attenuation. Long loops use Vorbis streaming in Unity to avoid keeping full decoded recordings in memory. This is **not a measured match of perceived headset loudness**. Office conversations are source ambience, not experiment instructions. Original MP3s stay in ignored `Builds/AssetSources/`; exact provenance and processing are in `ThirdParty/RecordedAudioSources.json`.

## Research inspiration

Orti et al. (2026), *Environmental Modulation of Perceived Cognitive Workload During a Memory Task: Evidence from Immersive Virtual Reality*, Journal of Intelligence 14(8), 158. https://doi.org/10.3390/jintelligence14080158

This prototype does not claim to reproduce the study or demonstrate a memory/workload effect. Its one-item rating, time limit and indoor environments are adaptations. Review screenshots show automatically completed test sessions; timings and ratings are fixtures, not participant results.
