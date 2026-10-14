# My Memory Room — Mini PRD

**Delivery:** Desktop keyboard/mouse, Unity 2022.3.62f3. Updated 14 October 2026. This document covers only my room.

## Concept and research goal

Participants perform the same matching task in an office setting and a nature-inspired setting. The question is whether the setting changes **memory performance or reported mental effort** in a desktop 3D environment.

Nature may feel less demanding; objective performance differences remain an open question. The prototype and a small usability trial cannot establish a psychological effect. Desktop findings should not be presented as headset-VR findings.

## Paper inspiration

Orti et al. (2026), [Environmental Modulation of Perceived Cognitive Workload During a Memory Task: Evidence from Immersive Virtual Reality](https://doi.org/10.3390/jintelligence14080158), uses a card-matching task to compare environmental settings in immersive VR.

I adapt the environment/task comparison, performance logging and subjective effort question. My delivery uses a monitor and mouse, and a single exploratory 1–5 question; it is not a replication of the paper's immersive setup or measurement instrument.

## Participant flow

1. Preview Office/Nature, choose the round order and sound, and optionally practise dragging the orange cube.
2. Start using the panel or Space. The camera returns to the same task view in both rounds.
3. Left-click two cards to reveal symbols. Find six pairs among 12 cards within 180 seconds.
4. After completion/timeout, rate mental effort 1–5. Continue to the other condition with a reshuffled arrangement.
5. After both ratings, see both results. CSV is saved automatically; CSV FOLDER opens its location.

F returns to the task view. Outside the measured task, WASD/right drag explores and G switches glide/fly (Q/E moves vertically). During rounds, navigation and practice dragging are locked. Esc pauses; window focus loss and Reset confirmation also stop timing. Interrupted rounds are marked and should be repeated for comparison.

## Conditions and control

| Setting | Included assets and ambience |
| --- | --- |
| Office | CC0 desks/chairs/lamp/clock, photographic surfaces/city view, office/keyboard recordings, cool background accent |
| Nature | CC0 plants, greenery/forest view, forest/stream recordings, softer green/warm background accent |

Card count, rules, task light, card faces, task camera, layout and time limit remain the same. Use the same computer, headphones and system volume. Alternate the starting condition across participants; reshuffling reduces memorizing the previous layout but does not remove every practice effect.

## Implementation and data

- Existing MemoryDev scene remains my editable source; the room prefab is integrated in MainScene without recreating room geometry.
- Canvas and 3D TextMeshPro; clear previews, timer/progress, ratings, Help, Pause, Reset confirmation and New Session confirmation.
- Ray-based mouse card selection and a separate draggable physics cube; matched cards fall into a collection tray.
- Props, accent lighting and spatial detail audio change at runtime. Task cues are neutral.
- CSV: condition, completion, elapsed time, pairs, flips, attempts, mistakes, effort, input mode, build version, sound, interruption. No participant names are requested.

## Plan and remaining work

- **14–16 October:** finish desktop controls/UI, automated checks and native Mac review; obtain one Windows check.
- **17–19 October:** 1–2 independent usability trials and any resulting fixes; group integrates the other rooms.
- **20–21 October:** record the complete demonstration, finish contributions/README and freeze the project.
- **22 October before midnight:** final shared repository/video and email delivery, following the brief.

**Room done:** a new user can complete both rounds, give both ratings, retrieve results and reset without developer help. The full group submission also needs the two teammates' rooms and connected MainScene. Keyboard input versus the brief's literal touch-pad wording remains a classroom clarification, not an independently confirmed exemption.
