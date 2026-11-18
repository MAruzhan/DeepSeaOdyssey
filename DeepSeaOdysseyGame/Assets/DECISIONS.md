# AI Decision Log - Deep Sea Odyssey

## Week 10: Three Enemy Types
* **2026-11-10:** Asked AI for a 2D Patroller FSM. Initially, the AI put `GameObject.FindWithTag("Player")` inside `Update()`, which hurts mobile performance. I changed it to cache the player reference inside `Start()`.
* **2026-11-11:** Charger enemy was unfair because it dashed immediately upon detecting the player. Added a 0.7s `WindUp` state with a yellow sprite color flash so the player can react.
* **2026-11-12:** Playtest results: Tester 1 took 5 tries, Tester 2 took 6 tries to beat the level. Increased Shooter `cooldownTime` from 1.0s to 1.6s and `windUpTime` from 0.4s to 0.6s. Now new players beat the level in 3 tries.