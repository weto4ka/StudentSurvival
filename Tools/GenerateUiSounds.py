"""Generate the original, short UI cues used by Student Survival (2026-11-21).
Standard library only. Does not play audio or run Unity.
"""
from pathlib import Path
import math, struct, wave
RATE = 44100
ROOT = Path(__file__).resolve().parents[1] / "Assets/Resources/Audio/UI"
CUES = {
    "hover": (0.09, [(880, 0, 0.09, 0.12)]),
    "click": (0.11, [(460, 0, 0.06, 0.18), (920, 0.025, 0.08, 0.10)]),
    "pause": (0.24, [(660, 0, 0.14, 0.12), (495, 0.07, 0.16, 0.10)]),
    "resume": (0.24, [(495, 0, 0.14, 0.12), (660, 0.07, 0.16, 0.10)]),
}
PEAKS = {"hover": 0.42, "click": 0.55, "pause": 0.45, "resume": 0.45}
def main():
    ROOT.mkdir(parents=True, exist_ok=True)
    for name, (duration, notes) in CUES.items():
        samples = []
        for i in range(round(duration * RATE)):
            t = i / RATE
            sample = 0.0
            for frequency, start, length, gain in notes:
                age = t - start
                if 0 <= age < length:
                    envelope = min(1.0, age / 0.006) * (1 - age / length) ** 2
                    sample += gain * envelope * (math.sin(2*math.pi*frequency*age) + 0.16*math.sin(4*math.pi*frequency*age))
            samples.append(sample)
        # Normalize the finished cue, preserving headroom for hover/click overlap.
        # The original low-gain oscillators became nearly inaudible after UI/master gain.
        gain = PEAKS[name] / max(abs(sample) for sample in samples)
        frames = b"".join(struct.pack("<h", round(sample * gain * 32767)) for sample in samples)
        with wave.open(str(ROOT / (name + ".wav")), "wb") as output:
            output.setnchannels(1); output.setsampwidth(2); output.setframerate(RATE)
            output.writeframes(frames)
        print(name + ".wav", "peak", PEAKS[name], "duration", duration)
if __name__ == "__main__":
    main()
