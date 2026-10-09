"""Generate original, mono placeholder sound loops; no third-party recordings."""
from pathlib import Path
import math
import random
import struct
import wave

RATE, SECONDS = 16000, 12
DEST = Path(__file__).resolve().parents[1] / 'Assets/Rooms/Memory/Audio'
random.seed(84)
for name in ('Office', 'Nature'):
    samples, smooth = [], 0.0
    for i in range(RATE * SECONDS):
        t = i / RATE
        noise = random.uniform(-1, 1)
        smooth = 0.985 * smooth + 0.015 * noise
        if name == 'Office':
            v = 0.10 * math.sin(2 * math.pi * 110 * t) + 0.02 * noise
            # Small keyboard-like taps and a distant two-tone ring.
            beat = t % 0.43
            if beat < 0.015:
                v += 0.14 * noise * (1 - beat / 0.015)
            if 6.0 < t < 7.4:
                v += 0.09 * (math.sin(2 * math.pi * 440 * t) + math.sin(2 * math.pi * 480 * t)) * math.sin(math.pi * (t - 6) / 1.4)
        else:
            v = smooth * 0.8 + 0.012 * noise
            bird = t % 3.0
            if 0.15 < bird < 0.55:
                p = bird - 0.15
                envelope = math.sin(math.pi * p / 0.4) ** 2
                v += 0.16 * envelope * math.sin(2 * math.pi * (1600 * p + 600 * p * p))
        fade = min(1, t / 0.05, (SECONDS - t) / 0.05)
        samples.append(max(-1, min(1, v * fade)))
    DEST.mkdir(parents=True, exist_ok=True)
    with wave.open(str(DEST / (name + '.wav')), 'wb') as out:
        out.setnchannels(1); out.setsampwidth(2); out.setframerate(RATE)
        out.writeframes(struct.pack('<' + 'h' * len(samples), *(int(v * 22000) for v in samples)))
print('Generated Office.wav and Nature.wav (original placeholder ambience).')
