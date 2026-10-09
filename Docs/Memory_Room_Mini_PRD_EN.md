# VR Memory Room — Mini PRD (English)

**Status:** Draft concept (8 October 2026)  
**Scope:** This document covers only my memory experiment room. The room switches between two environmental conditions within the same space.

## 1. Concept

Participants complete the same VR card-matching memory task in a busy office setting and a calmer, nature-inspired setting. The room records task performance and asks how mentally demanding each round felt. The aim is to explore whether changing the environment changes **performance or perceived mental workload**.

## 2. Research goal

**Research question:** When the memory task stays the same, do the office and nature-inspired conditions affect completion time, mistakes, or reported mental effort?

**Working expectation:** The nature-inspired condition may feel less demanding. Whether objective memory performance changes is an open question. This course prototype demonstrates an experimental design; a small usability trial would not establish a psychological effect.

## 3. Paper inspiration

Orti et al. (2026), [*Environmental Modulation of Perceived Cognitive Workload During a Memory Task: Evidence from Immersive Virtual Reality*](https://doi.org/10.3390/jintelligence14080158), compared a card-matching task in a VR office and an urban park. They measured moves, completion time, and perceived workload. The paper reported no significant difference in objective task performance between environments, while perceived workload was lower in the naturalistic setting.

**What I adapt:** Use the same type of memory task in two environments and look at both behavior and subjective experience. **What I simplify:** Two lightweight room atmospheres, one short mental-effort question, and a countdown and controls required for the course project.

## 4. Participant flow

1. Read brief instructions and start the first condition.
2. Use a VR controller ray to turn over 12 cards and find six matching pairs. The initial countdown limit is **180 seconds**.
3. After completion or timeout, see the result and answer: “How mentally demanding was that task?” (1–5).
4. Reset and rearrange the cards, switch to the other environment, and repeat the same task.
5. View both results; use Reset to start over.

To reduce practice effects in a trial, some participants will do the office condition first and others the nature-inspired condition first. Both rounds will keep the card count, rules, table position, controls, and time limit the same.

## 5. Environmental conditions

| Condition | Visuals and lighting | Spatial audio |
| --- | --- | --- |
| Office | Documents, screens, cooler lighting, a slightly busy atmosphere | Keyboard sounds, a phone, and soft office ambience |
| Nature-inspired | Plants, green scenery, softer lighting | Birds, running water, and soft natural ambience |

The room switches its props, lighting, and audio at runtime; the card task itself stays the same. Sound levels will remain comfortable for VR use.

## 6. Features and data

- **Interaction:** Smooth movement using the controller's 2D input, ray-based card selection, Start and Reset buttons. Matched cards can drop into a collection box to demonstrate physics.
- **Feedback:** A world-space Canvas, 3D TextMeshPro instructions, countdown, matching progress, and results panel.
- **Environment:** Runtime switching of props, lighting, and spatial audio.
- **Data:** Condition, completion or timeout, time taken, number of card flips or pairing attempts, mismatched attempts, and the 1–5 mental-effort rating. Results will first appear in the room; CSV export is optional if time allows.

## 7. Build plan and definition of done

| Target dates | Milestone |
| --- | --- |
| 8–11 October | Playable greybox: VR movement, card selection and matching, countdown, and Reset |
| 12–16 October | Two environmental conditions, lighting, spatial audio, results, and rating |
| 17–20 October | Meta Quest testing; fix text size, interaction distance, comfort, and bugs |
| 21 October | Freeze a demonstrable version and prepare this room's description and recording |

**Done means:** A participant can complete both rounds without developer help, see both results, and use Reset to repeat the experiment. Cards, timer, and ratings must return to the correct state when conditions change. A third condition is outside the initial scope.
