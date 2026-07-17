# Known Issues — 2.1 Source Candidate

- Unity compilation and Play Mode tests were not available in the packaging environment.
- Generated audio clips are functional placeholders and do not satisfy the final authored-audio requirement.
- Legacy `DemoUI` remains involved in some normal-player flows, so the complete UI architecture replacement is unfinished.
- The Workshop preview is deterministic and mechanic-aware, but it is not yet the exact gameplay spell executor rendered into a preview scene.
- Enemy status presentation does not yet include duration radials for every timed effect.
- Health-bar crowd overlap uses hiding/off-screen handling but not a complete label-layout solver.
- Physical-controller behavior, glyph coverage and controller rebinding require hardware verification and further completion.
- The automated suite is an initial safety suite, not full proof of every acceptance item in the 2.1 contract.
- Performance budgets are not accepted until Unity Profiler captures are recorded on stated hardware.

