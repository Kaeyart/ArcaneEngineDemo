# Elemental Reaction Mechanical Codex

Every one of the 120 multi-element signatures below has an executable resolve graph and an independently executable death graph.

## Fusion

### Thermal Shock — `0x03`
Elements: **Fire + Cold**  
Graph ID: `334e754edefac965`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.71, radius 3.16, duration 4.50, count 1, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.16, duration 0.77, count 1, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Compression` — element Cold, payload Fire|Cold, magnitude 0.78, radius 4.01, duration 3.70, count 5, delay 0.38.

**Death mechanics**

- `Burst` — element Cold, payload Fire|Cold, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Fire|Cold, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Fire|Cold, magnitude 0.71, radius 4.18, duration 3.90, count 5, delay 0.38.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.20, duration 1.02, count 1, delay 0.00.

### Plasma — `0x05`
Elements: **Fire + Lightning**  
Graph ID: `85f79f8a8888026e`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.71, radius 3.16, duration 4.50, count 1, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.60, radius 4.96, duration 0.00, count 4, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Fire|Lightning, magnitude 0.83, radius 4.35, duration 3.90, count 5, delay 0.49.

**Death mechanics**

- `Burst` — element Lightning, payload Fire|Lightning, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Fire|Lightning, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Lightning, payload Fire|Lightning, magnitude 0.68, radius 4.18, duration 3.90, count 5, delay 0.16.
- `ChainArc` — element Lightning, payload Fire|Lightning, magnitude 0.87, radius 6.00, duration 0.00, count 5, delay 0.00.

### Blastwave — `0x09`
Elements: **Fire + Physical**  
Graph ID: `f30bad19e4536768`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.71, radius 3.32, duration 4.50, count 1, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.56, radius 3.32, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.12, radius 3.32, duration 2.00, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `ShardNova` — element Physical, payload Fire|Physical, magnitude 0.73, radius 4.35, duration 3.50, count 6, delay 0.27.

**Death mechanics**

- `Burst` — element Physical, payload Fire|Physical, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Fire|Physical, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Physical, payload Fire|Physical, magnitude 0.83, radius 3.84, duration 3.70, count 4, delay 0.49.

### Searing Hemorrhage — `0x11`
Elements: **Fire + Blood**  
Graph ID: `7b273606e977578f`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.71, radius 3.16, duration 4.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.34, radius 3.16, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.64, radius 3.16, duration 5.00, count 1, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Blood, magnitude 0.73, radius 4.35, duration 3.50, count 4, delay 0.27.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Blood, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Blood, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Blood, magnitude 0.71, radius 4.01, duration 3.30, count 7, delay 0.38.

### Combustion — `0x21`
Elements: **Fire + Toxic**  
Graph ID: `61ab3d79b5daf028`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.71, radius 3.24, duration 4.50, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.38, radius 2.66, duration 5.10, count 6, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.60, radius 3.24, duration 5.50, count 1, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `ShardNova` — element Fire, payload Fire|Toxic, magnitude 0.73, radius 4.35, duration 3.50, count 4, delay 0.27.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Toxic, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Toxic, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Toxic, magnitude 0.71, radius 4.35, duration 3.50, count 5, delay 0.16.

### Blackflame — `0x41`
Elements: **Fire + Void**  
Graph ID: `a02316a479cb21b1`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.71, radius 3.40, duration 4.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.51, radius 4.10, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.54, radius 3.90, duration 0.00, count 1, delay 0.36.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Void, magnitude 0.73, radius 4.35, duration 3.50, count 4, delay 0.27.

**Death mechanics**

- `Burst` — element Void, payload Fire|Void, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Void, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Void, magnitude 0.68, radius 4.18, duration 3.90, count 5, delay 0.49.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.20, duration 0.00, count 1, delay 0.00.

### Superconduct — `0x06`
Elements: **Cold + Lightning**  
Graph ID: `65577a41de54e700`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.24, duration 0.77, count 1, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.60, radius 5.04, duration 0.00, count 4, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `PulseNova` — element Cold, payload Cold|Lightning, magnitude 0.66, radius 3.84, duration 3.30, count 6, delay 0.16.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Lightning, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Cold, payload Cold|Lightning, magnitude 0.76, radius 4.01, duration 3.70, count 7, delay 0.27.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.87, radius 6.00, duration 0.00, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.20, duration 1.02, count 1, delay 0.00.

### Shatter — `0x0A`
Elements: **Cold + Physical**  
Graph ID: `b8462fc6bc167d43`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.32, duration 0.77, count 1, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.56, radius 3.32, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.12, radius 3.32, duration 2.00, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Physical, magnitude 0.76, radius 3.84, duration 3.70, count 7, delay 0.38.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Physical, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Physical, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Cold, payload Cold|Physical, magnitude 0.71, radius 3.84, duration 3.70, count 6, delay 0.16.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.20, duration 1.02, count 1, delay 0.00.

### Crimson Frost — `0x12`
Elements: **Cold + Blood**  
Graph ID: `bf3a6425577b4ae0`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.16, duration 0.77, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.34, radius 3.16, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.64, radius 3.16, duration 5.00, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Blood, payload Cold|Blood, magnitude 0.76, radius 3.84, duration 3.70, count 5, delay 0.38.

**Death mechanics**

- `Burst` — element Blood, payload Cold|Blood, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Cold|Blood, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Blood, payload Cold|Blood, magnitude 0.81, radius 3.84, duration 3.30, count 5, delay 0.49.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.20, duration 1.02, count 1, delay 0.00.

### Cryotoxin — `0x22`
Elements: **Cold + Toxic**  
Graph ID: `15991e2a97112a74`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.24, duration 0.77, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.38, radius 2.66, duration 5.10, count 6, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.60, radius 3.24, duration 5.50, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `SplitFields` — element Toxic, payload Cold|Toxic, magnitude 0.76, radius 3.84, duration 3.70, count 5, delay 0.38.

**Death mechanics**

- `Burst` — element Toxic, payload Cold|Toxic, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Cold|Toxic, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Toxic, payload Cold|Toxic, magnitude 0.78, radius 4.18, duration 3.50, count 7, delay 0.27.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.20, duration 1.02, count 1, delay 0.00.

### Entropic Prison — `0x42`
Elements: **Cold + Void**  
Graph ID: `99346edaa68247f4`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.08, duration 0.77, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.51, radius 3.78, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.54, radius 3.58, duration 0.00, count 1, delay 0.36.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `PulseNova` — element Cold, payload Cold|Void, magnitude 0.76, radius 3.84, duration 3.70, count 5, delay 0.38.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Void, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Void, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Cold, payload Cold|Void, magnitude 0.76, radius 4.18, duration 3.90, count 7, delay 0.16.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.20, duration 1.02, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.20, duration 0.00, count 1, delay 0.00.

### Thunderclap — `0x0C`
Elements: **Lightning + Physical**  
Graph ID: `ff667a8b0846175f`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.60, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.56, radius 3.40, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.12, radius 3.40, duration 2.00, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Physical, payload Lightning|Physical, magnitude 0.81, radius 4.18, duration 3.90, count 7, delay 0.49.

**Death mechanics**

- `Burst` — element Physical, payload Lightning|Physical, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Lightning|Physical, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Physical, payload Lightning|Physical, magnitude 0.68, radius 4.35, duration 3.50, count 6, delay 0.38.
- `ChainArc` — element Lightning, payload Lightning|Physical, magnitude 0.87, radius 6.00, duration 0.00, count 5, delay 0.00.

### Neuroshock — `0x14`
Elements: **Lightning + Blood**  
Graph ID: `d6779fe4ecc19c8e`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.60, radius 5.04, duration 0.00, count 4, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.34, radius 3.24, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.64, radius 3.24, duration 5.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `OrbitingNodes` — element Blood, payload Lightning|Blood, magnitude 0.81, radius 4.18, duration 3.90, count 5, delay 0.49.

**Death mechanics**

- `Burst` — element Blood, payload Lightning|Blood, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Lightning|Blood, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Blood, payload Lightning|Blood, magnitude 0.78, radius 3.84, duration 3.30, count 5, delay 0.27.
- `ChainArc` — element Lightning, payload Lightning|Blood, magnitude 0.87, radius 6.00, duration 0.00, count 5, delay 0.00.

### Ionized Miasma — `0x24`
Elements: **Lightning + Toxic**  
Graph ID: `7caaa9c039558a71`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.60, radius 5.12, duration 0.00, count 4, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.38, radius 2.72, duration 5.10, count 6, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.60, radius 3.32, duration 5.50, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `TrailLine` — element Toxic, payload Lightning|Toxic, magnitude 0.81, radius 4.18, duration 3.90, count 5, delay 0.49.

**Death mechanics**

- `Burst` — element Toxic, payload Lightning|Toxic, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Lightning|Toxic, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Lightning|Toxic, magnitude 0.76, radius 4.01, duration 3.30, count 7, delay 0.38.
- `ChainArc` — element Lightning, payload Lightning|Toxic, magnitude 0.87, radius 6.00, duration 0.00, count 5, delay 0.00.

### Gravity Storm — `0x44`
Elements: **Lightning + Void**  
Graph ID: `8d15def722ca59dc`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.60, radius 4.88, duration 0.00, count 4, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.51, radius 3.78, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.54, radius 3.58, duration 0.00, count 1, delay 0.36.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Lightning|Void, magnitude 0.81, radius 4.18, duration 3.90, count 5, delay 0.49.

**Death mechanics**

- `Burst` — element Lightning, payload Lightning|Void, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Lightning|Void, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Lightning|Void, magnitude 0.73, radius 4.01, duration 3.70, count 7, delay 0.27.
- `ChainArc` — element Lightning, payload Lightning|Void, magnitude 0.87, radius 6.00, duration 0.00, count 5, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.20, duration 0.00, count 1, delay 0.00.

### Rupture — `0x18`
Elements: **Physical + Blood**  
Graph ID: `86ee030909e37c43`

**Resolution mechanics**

- `Push` — element Physical, payload Physical, magnitude 0.56, radius 3.40, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.12, radius 3.40, duration 2.00, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.34, radius 3.40, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.64, radius 3.40, duration 5.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Physical, payload Physical|Blood, magnitude 0.71, radius 4.18, duration 3.50, count 6, delay 0.27.

**Death mechanics**

- `Burst` — element Physical, payload Physical|Blood, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Physical, payload Physical|Blood, magnitude 0.73, radius 4.18, duration 3.90, count 4, delay 0.16.

### Aerosolization — `0x28`
Elements: **Physical + Toxic**  
Graph ID: `2a25b43597b6d473`

**Resolution mechanics**

- `Push` — element Physical, payload Physical, magnitude 0.56, radius 3.08, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.12, radius 3.08, duration 2.00, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.38, radius 2.53, duration 5.10, count 6, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.60, radius 3.08, duration 5.50, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Physical|Toxic, magnitude 0.71, radius 4.18, duration 3.50, count 6, delay 0.27.

**Death mechanics**

- `Burst` — element Toxic, payload Physical|Toxic, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Physical|Toxic, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Physical|Toxic, magnitude 0.71, radius 3.84, duration 3.30, count 6, delay 0.38.

### Implosion — `0x48`
Elements: **Physical + Void**  
Graph ID: `e9a0d2d32c9f9eca`

**Resolution mechanics**

- `Push` — element Physical, payload Physical, magnitude 0.56, radius 3.24, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.12, radius 3.24, duration 2.00, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.51, radius 3.94, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.54, radius 3.74, duration 0.00, count 1, delay 0.36.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `TrailLine` — element Void, payload Physical|Void, magnitude 0.71, radius 4.18, duration 3.50, count 6, delay 0.27.

**Death mechanics**

- `Burst` — element Void, payload Physical|Void, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Physical|Void, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Physical|Void, magnitude 0.68, radius 4.35, duration 3.50, count 6, delay 0.27.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.20, duration 0.00, count 1, delay 0.00.

### Sepsis — `0x30`
Elements: **Blood + Toxic**  
Graph ID: `ce10acf4bf7afd60`

**Resolution mechanics**

- `Execute` — element Blood, payload Blood, magnitude 0.34, radius 3.32, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.64, radius 3.32, duration 5.00, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.38, radius 2.72, duration 5.10, count 6, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.60, radius 3.32, duration 5.50, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Blood, payload Blood|Toxic, magnitude 0.71, radius 4.18, duration 3.50, count 4, delay 0.27.

**Death mechanics**

- `Burst` — element Blood, payload Blood|Toxic, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Blood|Toxic, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Blood, payload Blood|Toxic, magnitude 0.81, radius 4.01, duration 3.70, count 5, delay 0.16.

### Soul Rupture — `0x50`
Elements: **Blood + Void**  
Graph ID: `c0dae33c8cc48dfd`

**Resolution mechanics**

- `Execute` — element Blood, payload Blood, magnitude 0.34, radius 3.08, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.64, radius 3.08, duration 5.00, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.51, radius 3.78, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.54, radius 3.58, duration 0.00, count 1, delay 0.36.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Blood|Void, magnitude 0.71, radius 4.18, duration 3.50, count 4, delay 0.27.

**Death mechanics**

- `Burst` — element Void, payload Blood|Void, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Blood|Void, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Blood|Void, magnitude 0.78, radius 3.84, duration 3.30, count 5, delay 0.16.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.20, duration 0.00, count 1, delay 0.00.

### Decay Well — `0x60`
Elements: **Toxic + Void**  
Graph ID: `3f86fbae1f49ba91`

**Resolution mechanics**

- `Field` — element Toxic, payload Toxic, magnitude 0.38, radius 2.59, duration 5.10, count 6, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.60, radius 3.16, duration 5.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.51, radius 3.86, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.54, radius 3.66, duration 0.00, count 1, delay 0.36.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Void, payload Toxic|Void, magnitude 0.71, radius 4.18, duration 3.50, count 4, delay 0.27.

**Death mechanics**

- `Burst` — element Void, payload Toxic|Void, magnitude 1.18, radius 4.24, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Toxic|Void, magnitude 0.92, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Toxic|Void, magnitude 0.76, radius 4.18, duration 3.50, count 7, delay 0.27.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.20, duration 0.00, count 1, delay 0.00.

## Compound

### Cinder Rime Volt Tempest — `0x07`
Elements: **Fire + Cold + Lightning**  
Graph ID: `f6b752c0b8552c66`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.74, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.69, duration 4.35, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.74, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.54, duration 1.80, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.54, duration 0.00, count 5, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `SplitFields` — element Lightning, payload Fire|Cold|Lightning, magnitude 0.80, radius 4.60, duration 4.05, count 7, delay 0.27.

**Death mechanics**

- `Burst` — element Lightning, payload Fire|Cold|Lightning, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Fire|Cold|Lightning, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Fire|Cold|Lightning, magnitude 0.75, radius 4.43, duration 4.25, count 6, delay 0.38.
- `SplitFields` — element Lightning, payload Fire|Cold|Lightning, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Cinder Rime Ruin Maelstrom — `0x0B`
Elements: **Fire + Cold + Physical**  
Graph ID: `aec514ab02f29098`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.50, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.52, duration 4.35, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.50, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.30, duration 1.80, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.50, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.50, duration 2.25, count 1, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `Rebound` — element Cold, payload Fire|Cold|Physical, magnitude 0.90, radius 4.60, duration 4.45, count 8, delay 0.49.

**Death mechanics**

- `Burst` — element Cold, payload Fire|Cold|Physical, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Fire|Cold|Physical, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Cold, payload Fire|Cold|Physical, magnitude 0.90, radius 4.77, duration 4.05, count 5, delay 0.27.
- `SplitFields` — element Cold, payload Fire|Cold|Physical, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Cinder Rime Crimson Cascade — `0x13`
Elements: **Fire + Cold + Blood**  
Graph ID: `d196996cdff3aa95`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.74, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.69, duration 4.35, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.74, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.54, duration 1.80, count 8, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.74, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.74, duration 5.00, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Fire, payload Fire|Cold|Blood, magnitude 0.90, radius 4.60, duration 4.45, count 6, delay 0.49.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Cold|Blood, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Cold|Blood, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Fire, payload Fire|Cold|Blood, magnitude 0.78, radius 4.26, duration 3.85, count 8, delay 0.16.
- `SplitFields` — element Fire, payload Fire|Cold|Blood, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Cinder Rime Miasma Confluence — `0x23`
Elements: **Fire + Cold + Toxic**  
Graph ID: `cfa40b492a995335`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.42, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.46, duration 4.35, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.42, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.22, duration 1.80, count 8, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 2.80, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.42, duration 5.50, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Rebound` — element Fire, payload Fire|Cold|Toxic, magnitude 0.90, radius 4.60, duration 4.45, count 6, delay 0.49.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Cold|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Cold|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Fire, payload Fire|Cold|Toxic, magnitude 0.78, radius 4.60, duration 4.05, count 6, delay 0.38.
- `SplitFields` — element Fire, payload Fire|Cold|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Cinder Rime Umbral Tempest — `0x43`
Elements: **Fire + Cold + Void**  
Graph ID: `024899ab7905b92f`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.58, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.58, duration 4.35, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.58, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.38, duration 1.80, count 8, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.28, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.08, duration 0.00, count 2, delay 0.40.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `SplitFields` — element Fire, payload Fire|Cold|Void, magnitude 0.90, radius 4.60, duration 4.45, count 6, delay 0.49.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Cold|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Cold|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Fire, payload Fire|Cold|Void, magnitude 0.75, radius 4.43, duration 4.25, count 6, delay 0.27.
- `SplitFields` — element Fire, payload Fire|Cold|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Cinder Volt Ruin Maelstrom — `0x0D`
Elements: **Fire + Lightning + Physical**  
Graph ID: `6c131b3cb499f7ec`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.50, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.52, duration 4.35, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.30, duration 0.00, count 5, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.50, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.50, duration 2.25, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Lightning|Physical, magnitude 0.75, radius 4.26, duration 3.85, count 5, delay 0.16.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Lightning|Physical, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Lightning|Physical, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Fire, payload Fire|Lightning|Physical, magnitude 0.88, radius 4.60, duration 4.05, count 5, delay 0.49.
- `SplitFields` — element Fire, payload Fire|Lightning|Physical, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Physical, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.

### Cinder Volt Crimson Cascade — `0x15`
Elements: **Fire + Lightning + Blood**  
Graph ID: `bcccfc8a992ce01c`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.42, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.46, duration 4.35, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.22, duration 0.00, count 5, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.42, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.42, duration 5.00, count 2, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `TrailLine` — element Fire, payload Fire|Lightning|Blood, magnitude 0.75, radius 4.26, duration 3.85, count 7, delay 0.16.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Lightning|Blood, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Lightning|Blood, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Fire, payload Fire|Lightning|Blood, magnitude 0.75, radius 4.77, duration 4.45, count 8, delay 0.38.
- `SplitFields` — element Fire, payload Fire|Lightning|Blood, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Blood, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.

### Cinder Volt Miasma Confluence — `0x25`
Elements: **Fire + Lightning + Toxic**  
Graph ID: `56485079525b4f8c`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.50, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.52, duration 4.35, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.30, duration 0.00, count 5, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 2.87, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.50, duration 5.50, count 2, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Fire, payload Fire|Lightning|Toxic, magnitude 0.75, radius 4.26, duration 3.85, count 7, delay 0.16.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Lightning|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Lightning|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Fire, payload Fire|Lightning|Toxic, magnitude 0.75, radius 4.43, duration 3.85, count 6, delay 0.49.
- `SplitFields` — element Fire, payload Fire|Lightning|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Toxic, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.

### Cinder Volt Umbral Tempest — `0x45`
Elements: **Fire + Lightning + Void**  
Graph ID: `84815b64dec6b2fa`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.66, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.64, duration 4.35, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.46, duration 0.00, count 5, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.36, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.16, duration 0.00, count 2, delay 0.40.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Void, payload Fire|Lightning|Void, magnitude 0.75, radius 4.26, duration 3.85, count 7, delay 0.16.

**Death mechanics**

- `Burst` — element Void, payload Fire|Lightning|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Lightning|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Void, payload Fire|Lightning|Void, magnitude 0.93, radius 4.26, duration 4.25, count 5, delay 0.38.
- `SplitFields` — element Void, payload Fire|Lightning|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Void, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Cinder Ruin Crimson Confluence — `0x19`
Elements: **Fire + Physical + Blood**  
Graph ID: `699c207dec61672c`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.50, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.52, duration 4.35, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.50, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.50, duration 2.25, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.50, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.50, duration 5.00, count 2, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Physical, payload Fire|Physical|Blood, magnitude 0.85, radius 4.26, duration 4.25, count 8, delay 0.38.

**Death mechanics**

- `Burst` — element Physical, payload Fire|Physical|Blood, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Fire|Physical|Blood, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Physical, payload Fire|Physical|Blood, magnitude 0.90, radius 4.60, duration 4.45, count 7, delay 0.27.
- `SplitFields` — element Physical, payload Fire|Physical|Blood, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.

### Cinder Ruin Miasma Cascade — `0x29`
Elements: **Fire + Physical + Toxic**  
Graph ID: `e9233617dd0cf8a2`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.58, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.58, duration 4.35, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.58, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.58, duration 2.25, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 2.94, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.58, duration 5.50, count 2, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Fire|Physical|Toxic, magnitude 0.85, radius 4.26, duration 4.25, count 8, delay 0.38.

**Death mechanics**

- `Burst` — element Toxic, payload Fire|Physical|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Fire|Physical|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Toxic, payload Fire|Physical|Toxic, magnitude 0.90, radius 4.77, duration 4.45, count 5, delay 0.49.
- `SplitFields` — element Toxic, payload Fire|Physical|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.

### Cinder Ruin Umbral Maelstrom — `0x49`
Elements: **Fire + Physical + Void**  
Graph ID: `9ea9e3f04d220513`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.74, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.69, duration 4.35, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.74, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.74, duration 2.25, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.44, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.24, duration 0.00, count 2, delay 0.40.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Physical, payload Fire|Physical|Void, magnitude 0.85, radius 4.26, duration 4.25, count 8, delay 0.38.

**Death mechanics**

- `Burst` — element Physical, payload Fire|Physical|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Fire|Physical|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Physical, payload Fire|Physical|Void, magnitude 0.88, radius 4.77, duration 4.05, count 5, delay 0.38.
- `SplitFields` — element Physical, payload Fire|Physical|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Cinder Crimson Miasma Bloom — `0x31`
Elements: **Fire + Blood + Toxic**  
Graph ID: `5f4d381b110f883b`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.42, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.46, duration 4.35, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.42, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.42, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 2.80, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.42, duration 5.50, count 2, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Blood, payload Fire|Blood|Toxic, magnitude 0.85, radius 4.26, duration 4.25, count 6, delay 0.38.

**Death mechanics**

- `Burst` — element Blood, payload Fire|Blood|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Fire|Blood|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Blood, payload Fire|Blood|Toxic, magnitude 0.78, radius 4.26, duration 4.25, count 8, delay 0.38.
- `SplitFields` — element Blood, payload Fire|Blood|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.

### Cinder Crimson Umbral Cascade — `0x51`
Elements: **Fire + Blood + Void**  
Graph ID: `c22d6ec118e4f81d`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.58, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.58, duration 4.35, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.58, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.58, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.28, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.08, duration 0.00, count 2, delay 0.40.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Fire, payload Fire|Blood|Void, magnitude 0.85, radius 4.26, duration 4.25, count 6, delay 0.38.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Blood|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Blood|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Fire, payload Fire|Blood|Void, magnitude 0.75, radius 4.26, duration 3.85, count 8, delay 0.27.
- `SplitFields` — element Fire, payload Fire|Blood|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Cinder Miasma Umbral Confluence — `0x61`
Elements: **Fire + Toxic + Void**  
Graph ID: `3a23902849038b60`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.79, radius 3.74, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.40, radius 2.69, duration 4.35, count 6, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 3.07, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.74, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.44, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.24, duration 0.00, count 2, delay 0.40.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Toxic|Void, magnitude 0.85, radius 4.26, duration 4.25, count 6, delay 0.38.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Toxic|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Toxic|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Fire, payload Fire|Toxic|Void, magnitude 0.75, radius 4.43, duration 3.85, count 6, delay 0.38.
- `SplitFields` — element Fire, payload Fire|Toxic|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Rime Volt Ruin Cascade — `0x0E`
Elements: **Cold + Lightning + Physical**  
Graph ID: `f8e58a5e346b92d1`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.58, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.38, duration 1.80, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.38, duration 0.00, count 5, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.58, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.58, duration 2.25, count 1, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Physical, magnitude 0.78, radius 4.43, duration 3.85, count 5, delay 0.16.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Lightning|Physical, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Physical, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Cold, payload Cold|Lightning|Physical, magnitude 0.75, radius 4.60, duration 4.05, count 7, delay 0.16.
- `SplitFields` — element Cold, payload Cold|Lightning|Physical, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Physical, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Rime Volt Crimson Bloom — `0x16`
Elements: **Cold + Lightning + Blood**  
Graph ID: `efff130668cdadaa`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.42, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.22, duration 1.80, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.22, duration 0.00, count 5, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.42, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.42, duration 5.00, count 2, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `ThermalCycle` — element Cold, payload Cold|Lightning|Blood, magnitude 0.78, radius 4.43, duration 3.85, count 7, delay 0.16.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Lightning|Blood, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Blood, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Cold, payload Cold|Lightning|Blood, magnitude 0.85, radius 4.77, duration 4.45, count 6, delay 0.49.
- `SplitFields` — element Cold, payload Cold|Lightning|Blood, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Blood, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Rime Volt Miasma Crucible — `0x26`
Elements: **Cold + Lightning + Toxic**  
Graph ID: `26c16de076998718`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.50, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.30, duration 1.80, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.30, duration 0.00, count 5, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 2.87, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.50, duration 5.50, count 2, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Cold|Lightning|Toxic, magnitude 0.78, radius 4.43, duration 3.85, count 7, delay 0.16.

**Death mechanics**

- `Burst` — element Toxic, payload Cold|Lightning|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Cold|Lightning|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Toxic, payload Cold|Lightning|Toxic, magnitude 0.82, radius 4.43, duration 3.85, count 8, delay 0.16.
- `SplitFields` — element Toxic, payload Cold|Lightning|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Toxic, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Rime Volt Umbral Confluence — `0x46`
Elements: **Cold + Lightning + Void**  
Graph ID: `322d1dc863926eb6`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.66, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.46, duration 1.80, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.46, duration 0.00, count 5, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.36, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.16, duration 0.00, count 2, delay 0.40.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Lightning, payload Cold|Lightning|Void, magnitude 0.78, radius 4.43, duration 3.85, count 7, delay 0.16.

**Death mechanics**

- `Burst` — element Lightning, payload Cold|Lightning|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Cold|Lightning|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Lightning, payload Cold|Lightning|Void, magnitude 0.80, radius 4.26, duration 4.25, count 7, delay 0.49.
- `SplitFields` — element Lightning, payload Cold|Lightning|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Void, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Rime Ruin Crimson Crucible — `0x1A`
Elements: **Cold + Physical + Blood**  
Graph ID: `32910721d9bc3ebf`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.50, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.30, duration 1.80, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.50, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.50, duration 2.25, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.50, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.50, duration 5.00, count 2, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Cold|Physical|Blood, magnitude 0.88, radius 4.43, duration 4.25, count 8, delay 0.38.

**Death mechanics**

- `Burst` — element Physical, payload Cold|Physical|Blood, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Cold|Physical|Blood, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Physical, payload Cold|Physical|Blood, magnitude 0.80, radius 4.43, duration 4.25, count 5, delay 0.38.
- `SplitFields` — element Physical, payload Cold|Physical|Blood, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Rime Ruin Miasma Bloom — `0x2A`
Elements: **Cold + Physical + Toxic**  
Graph ID: `c5ca38e01869b8a7`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.66, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.46, duration 1.80, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.66, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.66, duration 2.25, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 3.00, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.66, duration 5.50, count 2, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Physical, payload Cold|Physical|Toxic, magnitude 0.88, radius 4.43, duration 4.25, count 8, delay 0.38.

**Death mechanics**

- `Burst` — element Physical, payload Cold|Physical|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Cold|Physical|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Physical, payload Cold|Physical|Toxic, magnitude 0.78, radius 4.77, duration 4.45, count 7, delay 0.16.
- `SplitFields` — element Physical, payload Cold|Physical|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Rime Ruin Umbral Cascade — `0x4A`
Elements: **Cold + Physical + Void**  
Graph ID: `706d3d32b52be2b5`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.42, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.22, duration 1.80, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.42, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.42, duration 2.25, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.12, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 3.92, duration 0.00, count 2, delay 0.40.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DetonateBuildup` — element Physical, payload Cold|Physical|Void, magnitude 0.88, radius 4.43, duration 4.25, count 8, delay 0.38.

**Death mechanics**

- `Burst` — element Physical, payload Cold|Physical|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Cold|Physical|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Physical, payload Cold|Physical|Void, magnitude 0.75, radius 4.60, duration 4.05, count 7, delay 0.49.
- `SplitFields` — element Physical, payload Cold|Physical|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Rime Crimson Miasma Triune — `0x32`
Elements: **Cold + Blood + Toxic**  
Graph ID: `61ab4b4d71b30534`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.50, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.30, duration 1.80, count 8, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.50, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.50, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 2.87, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.50, duration 5.50, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Blood|Toxic, magnitude 0.88, radius 4.43, duration 4.25, count 6, delay 0.38.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Blood|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Blood|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Cold, payload Cold|Blood|Toxic, magnitude 0.88, radius 4.26, duration 4.25, count 6, delay 0.38.
- `SplitFields` — element Cold, payload Cold|Blood|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.

### Rime Crimson Umbral Bloom — `0x52`
Elements: **Cold + Blood + Void**  
Graph ID: `8d1550f3cb40e1c4`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.66, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.46, duration 1.80, count 8, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.66, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.66, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.36, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.16, duration 0.00, count 2, delay 0.40.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Blood, payload Cold|Blood|Void, magnitude 0.88, radius 4.43, duration 4.25, count 6, delay 0.38.

**Death mechanics**

- `Burst` — element Blood, payload Cold|Blood|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Cold|Blood|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Blood, payload Cold|Blood|Void, magnitude 0.85, radius 4.77, duration 4.45, count 6, delay 0.27.
- `SplitFields` — element Blood, payload Cold|Blood|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Rime Miasma Umbral Crucible — `0x62`
Elements: **Cold + Toxic + Void**  
Graph ID: `47cc76d5df65cb87`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.74, duration 0.93, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.60, radius 4.54, duration 1.80, count 8, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 3.07, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.74, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.44, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.24, duration 0.00, count 2, delay 0.40.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Toxic|Void, magnitude 0.88, radius 4.43, duration 4.25, count 6, delay 0.38.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Toxic|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Toxic|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `Compression` — element Cold, payload Cold|Toxic|Void, magnitude 0.82, radius 4.43, duration 3.85, count 8, delay 0.49.
- `SplitFields` — element Cold, payload Cold|Toxic|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.55, duration 1.18, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Volt Ruin Crimson Crucible — `0x1C`
Elements: **Lightning + Physical + Blood**  
Graph ID: `e3c74a7466590ab9`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.38, duration 0.00, count 5, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.58, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.58, duration 2.25, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.58, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.58, duration 5.00, count 2, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Lightning, payload Lightning|Physical|Blood, magnitude 0.93, radius 4.77, duration 4.45, count 8, delay 0.49.

**Death mechanics**

- `Burst` — element Lightning, payload Lightning|Physical|Blood, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Lightning|Physical|Blood, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Lightning, payload Lightning|Physical|Blood, magnitude 0.78, radius 4.43, duration 4.25, count 5, delay 0.16.
- `SplitFields` — element Lightning, payload Lightning|Physical|Blood, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Physical|Blood, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.

### Volt Ruin Miasma Bloom — `0x2C`
Elements: **Lightning + Physical + Toxic**  
Graph ID: `5b6fb902132b0f9c`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.46, duration 0.00, count 5, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.66, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.66, duration 2.25, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 3.00, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.66, duration 5.50, count 2, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Lightning, payload Lightning|Physical|Toxic, magnitude 0.93, radius 4.77, duration 4.45, count 8, delay 0.49.

**Death mechanics**

- `Burst` — element Lightning, payload Lightning|Physical|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Lightning|Physical|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Lightning, payload Lightning|Physical|Toxic, magnitude 0.75, radius 4.60, duration 4.45, count 7, delay 0.27.
- `SplitFields` — element Lightning, payload Lightning|Physical|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Physical|Toxic, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.

### Volt Ruin Umbral Cascade — `0x4C`
Elements: **Lightning + Physical + Void**  
Graph ID: `6dec1468f242194d`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.30, duration 0.00, count 5, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.50, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.50, duration 2.25, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.00, duration 0.00, count 2, delay 0.40.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DelayedEcho` — element Lightning, payload Lightning|Physical|Void, magnitude 0.93, radius 4.77, duration 4.45, count 8, delay 0.49.

**Death mechanics**

- `Burst` — element Lightning, payload Lightning|Physical|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Lightning|Physical|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Lightning, payload Lightning|Physical|Void, magnitude 0.93, radius 4.60, duration 4.05, count 7, delay 0.16.
- `SplitFields` — element Lightning, payload Lightning|Physical|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Physical|Void, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Volt Crimson Miasma Triune — `0x34`
Elements: **Lightning + Blood + Toxic**  
Graph ID: `825c24fd23713909`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.30, duration 0.00, count 5, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.50, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.50, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 2.87, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.50, duration 5.50, count 2, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Lightning, payload Lightning|Blood|Toxic, magnitude 0.93, radius 4.77, duration 4.45, count 6, delay 0.49.

**Death mechanics**

- `Burst` — element Lightning, payload Lightning|Blood|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Lightning|Blood|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Lightning, payload Lightning|Blood|Toxic, magnitude 0.85, radius 4.77, duration 4.05, count 6, delay 0.16.
- `SplitFields` — element Lightning, payload Lightning|Blood|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Blood|Toxic, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.

### Volt Crimson Umbral Bloom — `0x54`
Elements: **Lightning + Blood + Void**  
Graph ID: `6b2328a727c7ed67`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.54, duration 0.00, count 5, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.74, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.74, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.44, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.24, duration 0.00, count 2, delay 0.40.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Lightning, payload Lightning|Blood|Void, magnitude 0.93, radius 4.77, duration 4.45, count 6, delay 0.49.

**Death mechanics**

- `Burst` — element Lightning, payload Lightning|Blood|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Lightning|Blood|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Lightning, payload Lightning|Blood|Void, magnitude 0.82, radius 4.60, duration 4.45, count 5, delay 0.49.
- `SplitFields` — element Lightning, payload Lightning|Blood|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Blood|Void, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Volt Miasma Umbral Crucible — `0x64`
Elements: **Lightning + Toxic + Void**  
Graph ID: `b08871ad93b69896`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.66, radius 5.22, duration 0.00, count 5, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 2.80, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.42, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.12, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 3.92, duration 0.00, count 2, delay 0.40.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Lightning|Toxic|Void, magnitude 0.93, radius 4.77, duration 4.45, count 6, delay 0.49.

**Death mechanics**

- `Burst` — element Void, payload Lightning|Toxic|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Lightning|Toxic|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Void, payload Lightning|Toxic|Void, magnitude 0.80, radius 4.26, duration 3.85, count 7, delay 0.27.
- `SplitFields` — element Void, payload Lightning|Toxic|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Toxic|Void, magnitude 0.93, radius 6.50, duration 0.00, count 6, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Ruin Crimson Miasma Paradox — `0x38`
Elements: **Physical + Blood + Toxic**  
Graph ID: `af9f2d340ec06666`

**Resolution mechanics**

- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.66, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.66, duration 2.25, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.66, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.66, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 3.00, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.66, duration 5.50, count 2, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Blood, payload Physical|Blood|Toxic, magnitude 0.82, radius 4.77, duration 4.05, count 7, delay 0.27.

**Death mechanics**

- `Burst` — element Blood, payload Physical|Blood|Toxic, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Physical|Blood|Toxic, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Physical|Blood|Toxic, magnitude 0.80, radius 4.43, duration 3.85, count 5, delay 0.49.
- `SplitFields` — element Blood, payload Physical|Blood|Toxic, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.

### Ruin Crimson Umbral Crucible — `0x58`
Elements: **Physical + Blood + Void**  
Graph ID: `d63ca474720085d6`

**Resolution mechanics**

- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.42, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.42, duration 2.25, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.42, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.42, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.12, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 3.92, duration 0.00, count 2, delay 0.40.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Blood, payload Physical|Blood|Void, magnitude 0.82, radius 4.77, duration 4.05, count 7, delay 0.27.

**Death mechanics**

- `Burst` — element Blood, payload Physical|Blood|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Physical|Blood|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Physical|Blood|Void, magnitude 0.78, radius 4.43, duration 4.25, count 5, delay 0.38.
- `SplitFields` — element Blood, payload Physical|Blood|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Ruin Miasma Umbral Bloom — `0x68`
Elements: **Physical + Toxic + Void**  
Graph ID: `127228dd8881d9ab`

**Resolution mechanics**

- `Push` — element Physical, payload Physical, magnitude 0.63, radius 3.50, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.13, radius 3.50, duration 2.25, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 2.87, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.50, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.00, duration 0.00, count 2, delay 0.40.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Physical|Toxic|Void, magnitude 0.82, radius 4.77, duration 4.05, count 7, delay 0.27.

**Death mechanics**

- `Burst` — element Toxic, payload Physical|Toxic|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Physical|Toxic|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Toxic, payload Physical|Toxic|Void, magnitude 0.75, radius 4.77, duration 4.45, count 7, delay 0.16.
- `SplitFields` — element Toxic, payload Physical|Toxic|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

### Crimson Miasma Umbral Triune — `0x70`
Elements: **Blood + Toxic + Void**  
Graph ID: `bcfbce6e50a24e21`

**Resolution mechanics**

- `Execute` — element Blood, payload Blood, magnitude 0.39, radius 3.74, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.71, radius 3.74, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.42, radius 3.07, duration 5.55, count 7, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.67, radius 3.74, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.59, radius 4.44, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.60, radius 4.24, duration 0.00, count 2, delay 0.40.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Toxic, payload Blood|Toxic|Void, magnitude 0.82, radius 4.77, duration 4.05, count 5, delay 0.27.

**Death mechanics**

- `Burst` — element Toxic, payload Blood|Toxic|Void, magnitude 1.32, radius 4.76, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Blood|Toxic|Void, magnitude 1.02, radius 4.90, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Toxic, payload Blood|Toxic|Void, magnitude 0.85, radius 4.77, duration 4.05, count 6, delay 0.49.
- `SplitFields` — element Toxic, payload Blood|Toxic|Void, magnitude 0.65, radius 4.50, duration 6.10, count 5, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 5.70, duration 0.00, count 1, delay 0.00.

## Catastrophe

### Cinder–Rime–Volt–Ruin Event — `0x0F`
Elements: **Fire + Cold + Lightning + Physical**  
Graph ID: `285943af8df1c6ac`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 4.08, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.94, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.08, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.88, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.88, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 4.08, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 4.08, duration 2.50, count 1, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Lightning, payload Fire|Cold|Lightning|Physical, magnitude 0.92, radius 5.19, duration 4.60, count 6, delay 0.27.
- `Contagion` — element Lightning, payload Fire|Cold|Lightning|Physical, magnitude 1.01, radius 4.68, duration 4.80, count 9, delay 0.16.
- `PulseNova` — element Lightning, payload Fire|Cold|Lightning|Physical, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Lightning, payload Fire|Cold|Lightning|Physical, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Lightning, payload Fire|Cold|Lightning|Physical, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Fire|Cold|Lightning|Physical, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Lightning, payload Fire|Cold|Lightning|Physical, magnitude 0.94, radius 5.02, duration 4.60, count 6, delay 0.27.
- `SplitFields` — element Lightning, payload Fire|Cold|Lightning|Physical, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Physical, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Cinder–Rime–Volt–Crimson Event — `0x17`
Elements: **Fire + Cold + Lightning + Blood**  
Graph ID: `0af1c55080b4328d`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.92, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.82, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.92, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.72, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.72, duration 0.00, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.92, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.92, duration 5.00, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `DetonateBuildup` — element Blood, payload Fire|Cold|Lightning|Blood, magnitude 0.92, radius 5.19, duration 4.60, count 8, delay 0.27.
- `TrailLine` — element Blood, payload Fire|Cold|Lightning|Blood, magnitude 0.92, radius 4.85, duration 4.80, count 7, delay 0.27.
- `PulseNova` — element Blood, payload Fire|Cold|Lightning|Blood, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Blood, payload Fire|Cold|Lightning|Blood, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Blood, payload Fire|Cold|Lightning|Blood, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Fire|Cold|Lightning|Blood, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Blood, payload Fire|Cold|Lightning|Blood, magnitude 1.01, radius 5.19, duration 5.00, count 8, delay 0.16.
- `SplitFields` — element Blood, payload Fire|Cold|Lightning|Blood, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Blood, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Cinder–Rime–Volt–Miasma Event — `0x27`
Elements: **Fire + Cold + Lightning + Toxic**  
Graph ID: `b2709eda901dd467`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 4.00, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.88, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.00, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.80, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.80, duration 0.00, count 6, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.28, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 4.00, duration 5.50, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `ShardNova` — element Fire, payload Fire|Cold|Lightning|Toxic, magnitude 0.92, radius 5.19, duration 4.60, count 8, delay 0.27.
- `Contagion` — element Fire, payload Fire|Cold|Lightning|Toxic, magnitude 0.92, radius 5.19, duration 5.00, count 7, delay 0.49.
- `PulseNova` — element Fire, payload Fire|Cold|Lightning|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Fire, payload Fire|Cold|Lightning|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Cold|Lightning|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Cold|Lightning|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Cold|Lightning|Toxic, magnitude 1.01, radius 4.68, duration 4.40, count 6, delay 0.27.
- `SplitFields` — element Fire, payload Fire|Cold|Lightning|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Toxic, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Cinder–Rime–Volt–Umbral Event — `0x47`
Elements: **Fire + Cold + Lightning + Void**  
Graph ID: `8eeadc957ac620a8`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.84, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.76, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.84, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.64, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.64, duration 0.00, count 6, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.54, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.34, duration 0.00, count 2, delay 0.44.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Cold|Lightning|Void, magnitude 0.92, radius 5.19, duration 4.60, count 8, delay 0.27.
- `TrailLine` — element Void, payload Fire|Cold|Lightning|Void, magnitude 0.94, radius 5.19, duration 4.60, count 8, delay 0.49.
- `PulseNova` — element Void, payload Fire|Cold|Lightning|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Void, payload Fire|Cold|Lightning|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Fire|Cold|Lightning|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Cold|Lightning|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Fire|Cold|Lightning|Void, magnitude 0.99, radius 4.68, duration 4.80, count 6, delay 0.16.
- `SplitFields` — element Void, payload Fire|Cold|Lightning|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Cinder–Rime–Ruin–Crimson Event — `0x1B`
Elements: **Fire + Cold + Physical + Blood**  
Graph ID: `30cfdd75410501f1`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 4.08, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.94, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.08, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.88, duration 1.80, count 9, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 4.08, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 4.08, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 4.08, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 4.08, duration 5.00, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Fire|Cold|Physical|Blood, magnitude 1.01, radius 5.19, duration 5.00, count 9, delay 0.49.
- `Contagion` — element Blood, payload Fire|Cold|Physical|Blood, magnitude 0.86, radius 4.85, duration 4.80, count 6, delay 0.27.
- `PulseNova` — element Blood, payload Fire|Cold|Physical|Blood, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Blood, payload Fire|Cold|Physical|Blood, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Blood, payload Fire|Cold|Physical|Blood, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Fire|Cold|Physical|Blood, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Blood, payload Fire|Cold|Physical|Blood, magnitude 0.96, radius 4.85, duration 4.80, count 8, delay 0.49.
- `SplitFields` — element Blood, payload Fire|Cold|Physical|Blood, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Cinder–Rime–Ruin–Miasma Event — `0x2B`
Elements: **Fire + Cold + Physical + Toxic**  
Graph ID: `c38383cae1430b81`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.76, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.71, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.76, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.56, duration 1.80, count 9, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.76, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.76, duration 2.50, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.08, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.76, duration 5.50, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Cold, payload Fire|Cold|Physical|Toxic, magnitude 1.01, radius 5.19, duration 5.00, count 9, delay 0.49.
- `OrbitingNodes` — element Cold, payload Fire|Cold|Physical|Toxic, magnitude 0.86, radius 5.19, duration 5.00, count 6, delay 0.49.
- `PulseNova` — element Cold, payload Fire|Cold|Physical|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Cold, payload Fire|Cold|Physical|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Fire|Cold|Physical|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Fire|Cold|Physical|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Cold, payload Fire|Cold|Physical|Toxic, magnitude 0.96, radius 5.19, duration 5.00, count 6, delay 0.27.
- `SplitFields` — element Cold, payload Fire|Cold|Physical|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Cinder–Rime–Ruin–Umbral Event — `0x4B`
Elements: **Fire + Cold + Physical + Void**  
Graph ID: `7c4f30b226752e51`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.92, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.82, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.92, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.72, duration 1.80, count 9, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.92, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.92, duration 2.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.62, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.42, duration 0.00, count 2, delay 0.44.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `ShardNova` — element Fire, payload Fire|Cold|Physical|Void, magnitude 1.01, radius 5.19, duration 5.00, count 9, delay 0.49.
- `Contagion` — element Fire, payload Fire|Cold|Physical|Void, magnitude 0.89, radius 5.19, duration 4.60, count 7, delay 0.49.
- `PulseNova` — element Fire, payload Fire|Cold|Physical|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Fire, payload Fire|Cold|Physical|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Cold|Physical|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Cold|Physical|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Cold|Physical|Void, magnitude 0.94, radius 5.02, duration 4.60, count 6, delay 0.16.
- `SplitFields` — element Fire, payload Fire|Cold|Physical|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Cinder–Rime–Crimson–Miasma Event — `0x33`
Elements: **Fire + Cold + Blood + Toxic**  
Graph ID: `81c5a23f40983a83`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 4.00, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.88, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.00, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.80, duration 1.80, count 9, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 4.00, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 4.00, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.28, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 4.00, duration 5.50, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Fire|Cold|Blood|Toxic, magnitude 1.01, radius 5.19, duration 5.00, count 7, delay 0.49.
- `Contagion` — element Blood, payload Fire|Cold|Blood|Toxic, magnitude 0.99, radius 4.68, duration 4.40, count 9, delay 0.16.
- `PulseNova` — element Blood, payload Fire|Cold|Blood|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Blood, payload Fire|Cold|Blood|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Blood, payload Fire|Cold|Blood|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Fire|Cold|Blood|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Blood, payload Fire|Cold|Blood|Toxic, magnitude 0.84, radius 5.19, duration 4.60, count 9, delay 0.49.
- `SplitFields` — element Blood, payload Fire|Cold|Blood|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Cinder–Rime–Crimson–Umbral Event — `0x53`
Elements: **Fire + Cold + Blood + Void**  
Graph ID: `d568d295d7c06efb`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.76, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.71, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.76, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.56, duration 1.80, count 9, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.76, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.76, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.46, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.26, duration 0.00, count 2, delay 0.44.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Fire|Cold|Blood|Void, magnitude 1.01, radius 5.19, duration 5.00, count 7, delay 0.49.
- `TrailLine` — element Cold, payload Fire|Cold|Blood|Void, magnitude 1.01, radius 4.68, duration 4.80, count 9, delay 0.16.
- `PulseNova` — element Cold, payload Fire|Cold|Blood|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Cold, payload Fire|Cold|Blood|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Fire|Cold|Blood|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Fire|Cold|Blood|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Cold, payload Fire|Cold|Blood|Void, magnitude 1.01, radius 5.19, duration 5.00, count 9, delay 0.38.
- `SplitFields` — element Cold, payload Fire|Cold|Blood|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Cinder–Rime–Miasma–Umbral Event — `0x63`
Elements: **Fire + Cold + Toxic + Void**  
Graph ID: `d813eacd88fe0968`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.84, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.76, duration 4.80, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.84, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.64, duration 1.80, count 9, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.15, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.84, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.54, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.34, duration 0.00, count 2, delay 0.44.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Toxic, payload Fire|Cold|Toxic|Void, magnitude 1.01, radius 5.19, duration 5.00, count 7, delay 0.49.
- `Contagion` — element Toxic, payload Fire|Cold|Toxic|Void, magnitude 1.01, radius 5.02, duration 5.00, count 9, delay 0.38.
- `PulseNova` — element Toxic, payload Fire|Cold|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Toxic, payload Fire|Cold|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Fire|Cold|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Fire|Cold|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Toxic, payload Fire|Cold|Toxic|Void, magnitude 1.01, radius 4.85, duration 4.40, count 7, delay 0.16.
- `SplitFields` — element Toxic, payload Fire|Cold|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Cinder–Volt–Ruin–Crimson Event — `0x1D`
Elements: **Fire + Lightning + Physical + Blood**  
Graph ID: `115e6e22efbe9809`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.76, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.71, duration 4.80, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.56, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.76, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.76, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.76, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.76, duration 5.00, count 2, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Lightning, payload Fire|Lightning|Physical|Blood, magnitude 0.86, radius 4.85, duration 4.40, count 6, delay 0.16.
- `PulseNova` — element Lightning, payload Fire|Lightning|Physical|Blood, magnitude 0.94, radius 5.02, duration 4.60, count 8, delay 0.38.
- `PulseNova` — element Lightning, payload Fire|Lightning|Physical|Blood, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Lightning, payload Fire|Lightning|Physical|Blood, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Lightning, payload Fire|Lightning|Physical|Blood, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Fire|Lightning|Physical|Blood, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Lightning, payload Fire|Lightning|Physical|Blood, magnitude 0.94, radius 4.68, duration 4.80, count 8, delay 0.27.
- `SplitFields` — element Lightning, payload Fire|Lightning|Physical|Blood, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Physical|Blood, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.

### Cinder–Volt–Ruin–Miasma Event — `0x2D`
Elements: **Fire + Lightning + Physical + Toxic**  
Graph ID: `d224b4ffe84134cf`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.84, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.76, duration 4.80, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.64, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.84, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.84, duration 2.50, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.15, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.84, duration 5.50, count 2, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Toxic, payload Fire|Lightning|Physical|Toxic, magnitude 0.86, radius 4.85, duration 4.40, count 6, delay 0.16.
- `SplitFields` — element Toxic, payload Fire|Lightning|Physical|Toxic, magnitude 0.96, radius 4.68, duration 4.80, count 8, delay 0.16.
- `PulseNova` — element Toxic, payload Fire|Lightning|Physical|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Toxic, payload Fire|Lightning|Physical|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Fire|Lightning|Physical|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Fire|Lightning|Physical|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Toxic, payload Fire|Lightning|Physical|Toxic, magnitude 0.94, radius 5.02, duration 5.00, count 6, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Lightning|Physical|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Physical|Toxic, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.

### Cinder–Volt–Ruin–Umbral Event — `0x4D`
Elements: **Fire + Lightning + Physical + Void**  
Graph ID: `561ec368a4281bc4`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 4.00, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.88, duration 4.80, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.80, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 4.00, duration 2.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.70, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.50, duration 0.00, count 2, delay 0.44.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `FieldSurge` — element Physical, payload Fire|Lightning|Physical|Void, magnitude 0.86, radius 4.85, duration 4.40, count 6, delay 0.16.
- `PulseNova` — element Physical, payload Fire|Lightning|Physical|Void, magnitude 0.99, radius 4.68, duration 4.40, count 9, delay 0.16.
- `PulseNova` — element Physical, payload Fire|Lightning|Physical|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Physical, payload Fire|Lightning|Physical|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Physical, payload Fire|Lightning|Physical|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Fire|Lightning|Physical|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Physical, payload Fire|Lightning|Physical|Void, magnitude 0.89, radius 4.85, duration 4.40, count 9, delay 0.27.
- `SplitFields` — element Physical, payload Fire|Lightning|Physical|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Physical|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Cinder–Volt–Crimson–Miasma Event — `0x35`
Elements: **Fire + Lightning + Blood + Toxic**  
Graph ID: `7eb8d84905a96253`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 4.08, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.94, duration 4.80, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.88, duration 0.00, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 4.08, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 4.08, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.35, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 4.08, duration 5.50, count 2, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Lightning|Blood|Toxic, magnitude 0.86, radius 4.85, duration 4.40, count 8, delay 0.16.
- `PulseNova` — element Fire, payload Fire|Lightning|Blood|Toxic, magnitude 0.86, radius 4.85, duration 4.80, count 6, delay 0.27.
- `PulseNova` — element Fire, payload Fire|Lightning|Blood|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Fire, payload Fire|Lightning|Blood|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Lightning|Blood|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Lightning|Blood|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Lightning|Blood|Toxic, magnitude 1.01, radius 5.19, duration 4.60, count 9, delay 0.27.
- `SplitFields` — element Fire, payload Fire|Lightning|Blood|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Blood|Toxic, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.

### Cinder–Volt–Crimson–Umbral Event — `0x55`
Elements: **Fire + Lightning + Blood + Void**  
Graph ID: `8137524740ff98b8`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.84, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.76, duration 4.80, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.64, duration 0.00, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.84, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.84, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.54, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.34, duration 0.00, count 2, delay 0.44.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Fire|Lightning|Blood|Void, magnitude 0.86, radius 4.85, duration 4.40, count 8, delay 0.16.
- `Rebound` — element Void, payload Fire|Lightning|Blood|Void, magnitude 0.89, radius 4.85, duration 4.40, count 7, delay 0.27.
- `PulseNova` — element Void, payload Fire|Lightning|Blood|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Void, payload Fire|Lightning|Blood|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Fire|Lightning|Blood|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Lightning|Blood|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Void, payload Fire|Lightning|Blood|Void, magnitude 0.99, radius 5.02, duration 5.00, count 8, delay 0.16.
- `SplitFields` — element Void, payload Fire|Lightning|Blood|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Blood|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Cinder–Volt–Miasma–Umbral Event — `0x65`
Elements: **Fire + Lightning + Toxic + Void**  
Graph ID: `0e8c3dafa4fd38e8`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.92, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.82, duration 4.80, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.72, duration 0.00, count 6, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.21, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.92, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.62, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.42, duration 0.00, count 2, delay 0.44.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Lightning|Toxic|Void, magnitude 0.86, radius 4.85, duration 4.40, count 8, delay 0.16.
- `PulseNova` — element Fire, payload Fire|Lightning|Toxic|Void, magnitude 0.89, radius 5.19, duration 4.60, count 7, delay 0.49.
- `PulseNova` — element Fire, payload Fire|Lightning|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Fire, payload Fire|Lightning|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Lightning|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Lightning|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Lightning|Toxic|Void, magnitude 0.99, radius 4.68, duration 4.40, count 6, delay 0.38.
- `SplitFields` — element Fire, payload Fire|Lightning|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Toxic|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Cinder–Ruin–Crimson–Miasma Event — `0x39`
Elements: **Fire + Physical + Blood + Toxic**  
Graph ID: `6918c289184f99a6`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.76, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.71, duration 4.80, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.76, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.76, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.76, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.76, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.08, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.76, duration 5.50, count 2, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Compression` — element Fire, payload Fire|Physical|Blood|Toxic, magnitude 0.96, radius 4.85, duration 4.80, count 9, delay 0.38.
- `SplitFields` — element Fire, payload Fire|Physical|Blood|Toxic, magnitude 1.01, radius 4.85, duration 4.80, count 9, delay 0.27.
- `PulseNova` — element Fire, payload Fire|Physical|Blood|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Fire, payload Fire|Physical|Blood|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Physical|Blood|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Physical|Blood|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Physical|Blood|Toxic, magnitude 0.96, radius 4.85, duration 4.40, count 8, delay 0.16.
- `SplitFields` — element Fire, payload Fire|Physical|Blood|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.

### Cinder–Ruin–Crimson–Umbral Event — `0x59`
Elements: **Fire + Physical + Blood + Void**  
Graph ID: `d472b712456fe481`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.92, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.82, duration 4.80, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.92, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.92, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.92, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.92, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.62, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.42, duration 0.00, count 2, delay 0.44.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Physical|Blood|Void, magnitude 0.96, radius 4.85, duration 4.80, count 9, delay 0.38.
- `PulseNova` — element Void, payload Fire|Physical|Blood|Void, magnitude 0.84, radius 4.85, duration 4.40, count 6, delay 0.27.
- `PulseNova` — element Void, payload Fire|Physical|Blood|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Void, payload Fire|Physical|Blood|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Fire|Physical|Blood|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Physical|Blood|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Void, payload Fire|Physical|Blood|Void, magnitude 0.94, radius 4.85, duration 4.80, count 8, delay 0.16.
- `SplitFields` — element Void, payload Fire|Physical|Blood|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Cinder–Ruin–Miasma–Umbral Event — `0x69`
Elements: **Fire + Physical + Toxic + Void**  
Graph ID: `034f8f3dd3e1f1b2`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 4.08, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.94, duration 4.80, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 4.08, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 4.08, duration 2.50, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.35, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 4.08, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.78, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.58, duration 0.00, count 2, delay 0.44.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `Compression` — element Physical, payload Fire|Physical|Toxic|Void, magnitude 0.96, radius 4.85, duration 4.80, count 9, delay 0.38.
- `SplitFields` — element Physical, payload Fire|Physical|Toxic|Void, magnitude 0.84, radius 5.19, duration 4.60, count 6, delay 0.49.
- `PulseNova` — element Physical, payload Fire|Physical|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Physical, payload Fire|Physical|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Physical, payload Fire|Physical|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Fire|Physical|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Physical, payload Fire|Physical|Toxic|Void, magnitude 0.94, radius 5.02, duration 5.00, count 6, delay 0.27.
- `SplitFields` — element Physical, payload Fire|Physical|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Cinder–Crimson–Miasma–Umbral Event — `0x71`
Elements: **Fire + Blood + Toxic + Void**  
Graph ID: `aa89dca7ea86694a`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.87, radius 3.92, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.44, radius 2.82, duration 4.80, count 7, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.92, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.92, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.21, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.92, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.62, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.42, duration 0.00, count 2, delay 0.44.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Blood|Toxic|Void, magnitude 0.96, radius 4.85, duration 4.80, count 7, delay 0.38.
- `PulseNova` — element Toxic, payload Fire|Blood|Toxic|Void, magnitude 0.96, radius 4.68, duration 4.80, count 8, delay 0.16.
- `PulseNova` — element Toxic, payload Fire|Blood|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Toxic, payload Fire|Blood|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Fire|Blood|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Fire|Blood|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Toxic, payload Fire|Blood|Toxic|Void, magnitude 1.01, radius 5.19, duration 4.60, count 9, delay 0.16.
- `SplitFields` — element Toxic, payload Fire|Blood|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Rime–Volt–Ruin–Crimson Collapse — `0x1E`
Elements: **Cold + Lightning + Physical + Blood**  
Graph ID: `4ab16a84a24444aa`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.76, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.56, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.56, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.76, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.76, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.76, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.76, duration 5.00, count 2, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Cold, payload Cold|Lightning|Physical|Blood, magnitude 0.89, radius 5.02, duration 4.60, count 6, delay 0.27.
- `DelayedEcho` — element Cold, payload Cold|Lightning|Physical|Blood, magnitude 0.89, radius 4.68, duration 4.40, count 7, delay 0.16.
- `PulseNova` — element Cold, payload Cold|Lightning|Physical|Blood, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Cold, payload Cold|Lightning|Physical|Blood, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Lightning|Physical|Blood, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Physical|Blood, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Cold, payload Cold|Lightning|Physical|Blood, magnitude 0.84, radius 4.68, duration 4.80, count 6, delay 0.38.
- `SplitFields` — element Cold, payload Cold|Lightning|Physical|Blood, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Physical|Blood, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Rime–Volt–Ruin–Miasma Collapse — `0x2E`
Elements: **Cold + Lightning + Physical + Toxic**  
Graph ID: `82769ca3ec76190d`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.84, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.64, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.64, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.84, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.84, duration 2.50, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.15, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.84, duration 5.50, count 2, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Physical, payload Cold|Lightning|Physical|Toxic, magnitude 0.89, radius 5.02, duration 4.60, count 6, delay 0.27.
- `FieldSurge` — element Physical, payload Cold|Lightning|Physical|Toxic, magnitude 0.89, radius 5.02, duration 4.60, count 7, delay 0.38.
- `PulseNova` — element Physical, payload Cold|Lightning|Physical|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Physical, payload Cold|Lightning|Physical|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Physical, payload Cold|Lightning|Physical|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Cold|Lightning|Physical|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Physical, payload Cold|Lightning|Physical|Toxic, magnitude 1.01, radius 4.85, duration 4.80, count 7, delay 0.49.
- `SplitFields` — element Physical, payload Cold|Lightning|Physical|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Physical|Toxic, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Rime–Volt–Ruin–Umbral Collapse — `0x4E`
Elements: **Cold + Lightning + Physical + Void**  
Graph ID: `60304bd68422eeb0`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.00, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.80, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.80, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 4.00, duration 2.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.70, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.50, duration 0.00, count 2, delay 0.44.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `TrailLine` — element Lightning, payload Cold|Lightning|Physical|Void, magnitude 0.89, radius 5.02, duration 4.60, count 6, delay 0.27.
- `DelayedEcho` — element Lightning, payload Cold|Lightning|Physical|Void, magnitude 0.92, radius 5.02, duration 5.00, count 7, delay 0.38.
- `PulseNova` — element Lightning, payload Cold|Lightning|Physical|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Lightning, payload Cold|Lightning|Physical|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Lightning, payload Cold|Lightning|Physical|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Cold|Lightning|Physical|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Lightning, payload Cold|Lightning|Physical|Void, magnitude 0.99, radius 4.85, duration 4.40, count 7, delay 0.38.
- `SplitFields` — element Lightning, payload Cold|Lightning|Physical|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Physical|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Rime–Volt–Crimson–Miasma Collapse — `0x36`
Elements: **Cold + Lightning + Blood + Toxic**  
Graph ID: `b68bb2be03d5ffac`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.08, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.88, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.88, duration 0.00, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 4.08, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 4.08, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.35, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 4.08, duration 5.50, count 2, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Toxic, payload Cold|Lightning|Blood|Toxic, magnitude 0.89, radius 5.02, duration 4.60, count 8, delay 0.27.
- `DelayedEcho` — element Toxic, payload Cold|Lightning|Blood|Toxic, magnitude 0.99, radius 5.19, duration 4.60, count 9, delay 0.49.
- `PulseNova` — element Toxic, payload Cold|Lightning|Blood|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Toxic, payload Cold|Lightning|Blood|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Cold|Lightning|Blood|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Cold|Lightning|Blood|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Cold|Lightning|Blood|Toxic, magnitude 0.92, radius 5.02, duration 4.60, count 6, delay 0.38.
- `SplitFields` — element Toxic, payload Cold|Lightning|Blood|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Blood|Toxic, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Rime–Volt–Crimson–Umbral Collapse — `0x56`
Elements: **Cold + Lightning + Blood + Void**  
Graph ID: `e69edffd549aafef`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.84, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.64, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.64, duration 0.00, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.84, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.84, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.54, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.34, duration 0.00, count 2, delay 0.44.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Blood, payload Cold|Lightning|Blood|Void, magnitude 0.89, radius 5.02, duration 4.60, count 8, delay 0.27.
- `Compression` — element Blood, payload Cold|Lightning|Blood|Void, magnitude 1.01, radius 5.19, duration 5.00, count 9, delay 0.49.
- `PulseNova` — element Blood, payload Cold|Lightning|Blood|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Blood, payload Cold|Lightning|Blood|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Blood, payload Cold|Lightning|Blood|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Cold|Lightning|Blood|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Blood, payload Cold|Lightning|Blood|Void, magnitude 0.89, radius 5.02, duration 5.00, count 6, delay 0.27.
- `SplitFields` — element Blood, payload Cold|Lightning|Blood|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Blood|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Rime–Volt–Miasma–Umbral Collapse — `0x66`
Elements: **Cold + Lightning + Toxic + Void**  
Graph ID: `448fa18aa25bd319`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.92, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.72, duration 1.80, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.72, duration 0.00, count 6, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.21, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.92, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.62, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.42, duration 0.00, count 2, delay 0.44.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Cold, payload Cold|Lightning|Toxic|Void, magnitude 0.89, radius 5.02, duration 4.60, count 8, delay 0.27.
- `DelayedEcho` — element Cold, payload Cold|Lightning|Toxic|Void, magnitude 0.84, radius 4.85, duration 4.40, count 6, delay 0.27.
- `PulseNova` — element Cold, payload Cold|Lightning|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Cold, payload Cold|Lightning|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Lightning|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Cold, payload Cold|Lightning|Toxic|Void, magnitude 0.86, radius 5.19, duration 5.00, count 8, delay 0.49.
- `SplitFields` — element Cold, payload Cold|Lightning|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Toxic|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Rime–Ruin–Crimson–Miasma Collapse — `0x3A`
Elements: **Cold + Physical + Blood + Toxic**  
Graph ID: `b0bc5c0532e8b744`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.84, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.64, duration 1.80, count 9, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.84, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.84, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.84, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.84, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.15, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.84, duration 5.50, count 2, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Cold|Physical|Blood|Toxic, magnitude 0.99, radius 5.02, duration 5.00, count 9, delay 0.49.
- `FieldSurge` — element Toxic, payload Cold|Physical|Blood|Toxic, magnitude 0.94, radius 5.19, duration 4.60, count 8, delay 0.49.
- `PulseNova` — element Toxic, payload Cold|Physical|Blood|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Toxic, payload Cold|Physical|Blood|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Cold|Physical|Blood|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Cold|Physical|Blood|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Cold|Physical|Blood|Toxic, magnitude 0.86, radius 4.85, duration 4.40, count 6, delay 0.27.
- `SplitFields` — element Toxic, payload Cold|Physical|Blood|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.

### Rime–Ruin–Crimson–Umbral Collapse — `0x5A`
Elements: **Cold + Physical + Blood + Void**  
Graph ID: `ffd2a18f52d77225`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.00, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.80, duration 1.80, count 9, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 4.00, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 4.00, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 4.00, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.70, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.50, duration 0.00, count 2, delay 0.44.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Blood, payload Cold|Physical|Blood|Void, magnitude 0.99, radius 5.02, duration 5.00, count 9, delay 0.49.
- `DelayedEcho` — element Blood, payload Cold|Physical|Blood|Void, magnitude 0.96, radius 5.19, duration 5.00, count 8, delay 0.49.
- `PulseNova` — element Blood, payload Cold|Physical|Blood|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Blood, payload Cold|Physical|Blood|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Blood, payload Cold|Physical|Blood|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Cold|Physical|Blood|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Blood, payload Cold|Physical|Blood|Void, magnitude 0.84, radius 4.68, duration 4.80, count 6, delay 0.16.
- `SplitFields` — element Blood, payload Cold|Physical|Blood|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Rime–Ruin–Miasma–Umbral Collapse — `0x6A`
Elements: **Cold + Physical + Toxic + Void**  
Graph ID: `f7d813b849686ae5`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.08, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.88, duration 1.80, count 9, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 4.08, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 4.08, duration 2.50, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.35, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 4.08, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.78, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.58, duration 0.00, count 2, delay 0.44.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Cold, payload Cold|Physical|Toxic|Void, magnitude 0.99, radius 5.02, duration 5.00, count 9, delay 0.49.
- `FieldSurge` — element Cold, payload Cold|Physical|Toxic|Void, magnitude 0.99, radius 4.85, duration 4.40, count 9, delay 0.27.
- `PulseNova` — element Cold, payload Cold|Physical|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Cold, payload Cold|Physical|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Physical|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Physical|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Cold, payload Cold|Physical|Toxic|Void, magnitude 1.01, radius 5.02, duration 5.00, count 8, delay 0.38.
- `SplitFields` — element Cold, payload Cold|Physical|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Rime–Crimson–Miasma–Umbral Collapse — `0x72`
Elements: **Cold + Blood + Toxic + Void**  
Graph ID: `1388a2052b6b7cae`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 3.92, duration 1.09, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.65, radius 4.72, duration 1.80, count 9, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.92, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.92, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.21, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.92, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.62, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.42, duration 0.00, count 2, delay 0.44.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Blood, payload Cold|Blood|Toxic|Void, magnitude 0.99, radius 5.02, duration 5.00, count 7, delay 0.49.
- `DelayedEcho` — element Blood, payload Cold|Blood|Toxic|Void, magnitude 0.89, radius 5.02, duration 4.60, count 7, delay 0.38.
- `PulseNova` — element Blood, payload Cold|Blood|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Blood, payload Cold|Blood|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Blood, payload Cold|Blood|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Cold|Blood|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Blood, payload Cold|Blood|Toxic|Void, magnitude 0.92, radius 5.19, duration 4.60, count 7, delay 0.27.
- `SplitFields` — element Blood, payload Cold|Blood|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.90, duration 1.34, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Volt–Ruin–Crimson–Miasma Collapse — `0x3C`
Elements: **Lightning + Physical + Blood + Toxic**  
Graph ID: `e6a6492e2ffc8775`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.64, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.84, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.84, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 3.84, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 3.84, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.15, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.84, duration 5.50, count 2, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Physical, payload Lightning|Physical|Blood|Toxic, magnitude 0.84, radius 4.68, duration 4.40, count 6, delay 0.16.
- `ThermalCycle` — element Physical, payload Lightning|Physical|Blood|Toxic, magnitude 0.84, radius 4.68, duration 4.40, count 6, delay 0.16.
- `PulseNova` — element Physical, payload Lightning|Physical|Blood|Toxic, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Physical, payload Lightning|Physical|Blood|Toxic, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Physical, payload Lightning|Physical|Blood|Toxic, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Lightning|Physical|Blood|Toxic, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Physical, payload Lightning|Physical|Blood|Toxic, magnitude 0.84, radius 4.68, duration 4.40, count 6, delay 0.49.
- `SplitFields` — element Physical, payload Lightning|Physical|Blood|Toxic, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Physical|Blood|Toxic, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.

### Volt–Ruin–Crimson–Umbral Collapse — `0x5C`
Elements: **Lightning + Physical + Blood + Void**  
Graph ID: `fd8081763b0c433f`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.88, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 4.08, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 4.08, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 4.08, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 4.08, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.78, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.58, duration 0.00, count 2, delay 0.44.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Physical, payload Lightning|Physical|Blood|Void, magnitude 0.84, radius 4.68, duration 4.40, count 6, delay 0.16.
- `ShardNova` — element Physical, payload Lightning|Physical|Blood|Void, magnitude 0.86, radius 4.68, duration 4.80, count 6, delay 0.16.
- `PulseNova` — element Physical, payload Lightning|Physical|Blood|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Physical, payload Lightning|Physical|Blood|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Physical, payload Lightning|Physical|Blood|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Lightning|Physical|Blood|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Physical, payload Lightning|Physical|Blood|Void, magnitude 0.99, radius 5.19, duration 4.60, count 9, delay 0.38.
- `SplitFields` — element Physical, payload Lightning|Physical|Blood|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Physical|Blood|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Volt–Ruin–Miasma–Umbral Collapse — `0x6C`
Elements: **Lightning + Physical + Toxic + Void**  
Graph ID: `4d998aef9a5178d7`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.56, duration 0.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.70, radius 3.76, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 3.76, duration 2.50, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.08, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 3.76, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.46, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.26, duration 0.00, count 2, delay 0.44.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Toxic, payload Lightning|Physical|Toxic|Void, magnitude 0.84, radius 4.68, duration 4.40, count 6, delay 0.16.
- `ThermalCycle` — element Toxic, payload Lightning|Physical|Toxic|Void, magnitude 0.86, radius 5.02, duration 5.00, count 6, delay 0.38.
- `PulseNova` — element Toxic, payload Lightning|Physical|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Toxic, payload Lightning|Physical|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Lightning|Physical|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Lightning|Physical|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Lightning|Physical|Toxic|Void, magnitude 0.99, radius 4.85, duration 4.80, count 7, delay 0.16.
- `SplitFields` — element Toxic, payload Lightning|Physical|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Physical|Toxic|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Volt–Crimson–Miasma–Umbral Collapse — `0x74`
Elements: **Lightning + Blood + Toxic + Void**  
Graph ID: `2df723a22644422f`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.72, radius 5.80, duration 0.00, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 4.00, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 4.00, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.28, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 4.00, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.70, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.50, duration 0.00, count 2, delay 0.44.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Void, payload Lightning|Blood|Toxic|Void, magnitude 0.84, radius 4.68, duration 4.40, count 8, delay 0.16.
- `ShardNova` — element Void, payload Lightning|Blood|Toxic|Void, magnitude 0.96, radius 5.19, duration 5.00, count 8, delay 0.49.
- `PulseNova` — element Void, payload Lightning|Blood|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Void, payload Lightning|Blood|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Lightning|Blood|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Lightning|Blood|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Void, payload Lightning|Blood|Toxic|Void, magnitude 0.89, radius 5.02, duration 4.60, count 6, delay 0.38.
- `SplitFields` — element Void, payload Lightning|Blood|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Blood|Toxic|Void, magnitude 0.99, radius 7.00, duration 0.00, count 7, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

### Ruin–Crimson–Miasma–Umbral Collapse — `0x78`
Elements: **Physical + Blood + Toxic + Void**  
Graph ID: `ca287e3cb80d9379`

**Resolution mechanics**

- `Push` — element Physical, payload Physical, magnitude 0.70, radius 4.08, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.15, radius 4.08, duration 2.50, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.44, radius 4.08, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.78, radius 4.08, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.46, radius 3.35, duration 6.00, count 8, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.74, radius 4.08, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.67, radius 4.78, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.66, radius 4.58, duration 0.00, count 2, delay 0.44.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Void, payload Physical|Blood|Toxic|Void, magnitude 0.94, radius 4.68, duration 4.80, count 9, delay 0.38.
- `ThermalCycle` — element Void, payload Physical|Blood|Toxic|Void, magnitude 0.92, radius 5.19, duration 5.00, count 7, delay 0.49.
- `PulseNova` — element Void, payload Physical|Blood|Toxic|Void, magnitude 0.94, radius 5.20, duration 4.40, count 6, delay 0.08.
- `Field` — element Void, payload Physical|Blood|Toxic|Void, magnitude 0.62, radius 4.40, duration 7.30, count 8, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Physical|Blood|Toxic|Void, magnitude 1.46, radius 5.28, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Physical|Blood|Toxic|Void, magnitude 1.12, radius 5.40, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Void, payload Physical|Blood|Toxic|Void, magnitude 0.84, radius 4.68, duration 4.40, count 6, delay 0.38.
- `SplitFields` — element Void, payload Physical|Blood|Toxic|Void, magnitude 0.72, radius 5.00, duration 6.80, count 6, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.20, duration 0.00, count 1, delay 0.00.

## Convergence

### Cinder Rime Convergence of Lightning–Physical–Blood — `0x1F`
Elements: **Fire + Cold + Lightning + Physical + Blood**  
Graph ID: `f7eec2bbe06f4d72`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.26, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.07, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.26, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 5.06, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 6.06, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.26, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.26, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.26, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.26, duration 5.00, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 1.03, radius 5.10, duration 5.35, count 8, delay 0.38.
- `FieldSurge` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 0.98, radius 5.44, duration 5.15, count 8, delay 0.38.
- `PulseNova` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 1.10, radius 5.27, duration 4.95, count 10, delay 0.38.
- `PulseNova` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 1.00, radius 5.61, duration 5.15, count 8, delay 0.49.
- `SplitFields` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Physical|Blood, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `FieldSurge` — element Cold, payload Fire|Cold|Lightning|Physical|Blood, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Rime Aetherstorm of Lightning–Physical–Toxic — `0x2F`
Elements: **Fire + Cold + Lightning + Physical + Toxic**  
Graph ID: `64f72aafef57818a`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.34, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.12, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.34, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 5.14, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 6.14, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.34, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.34, duration 2.75, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.56, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.34, duration 5.50, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 1.03, radius 5.10, duration 5.35, count 8, delay 0.38.
- `Compression` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 1.00, radius 5.10, duration 5.35, count 8, delay 0.16.
- `SplitFields` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 1.08, radius 5.27, duration 5.35, count 9, delay 0.49.
- `PulseNova` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 0.98, radius 5.27, duration 5.35, count 10, delay 0.16.
- `SplitFields` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `FieldSurge` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Rime Devouring Crown of Lightning–Physical–Void — `0x4F`
Elements: **Fire + Cold + Lightning + Physical + Void**  
Graph ID: `096779271b4b41ee`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.18, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.01, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.18, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 4.98, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 5.98, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.18, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.18, duration 2.75, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.88, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.68, duration 0.00, count 2, delay 0.48.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 1.03, radius 5.10, duration 5.35, count 8, delay 0.38.
- `FieldSurge` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 1.03, radius 5.10, duration 4.95, count 9, delay 0.16.
- `PulseNova` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 1.00, radius 5.44, duration 5.55, count 10, delay 0.16.
- `PulseNova` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 0.95, radius 5.10, duration 4.95, count 10, delay 0.49.
- `SplitFields` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Physical|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Cold|Lightning|Physical|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Rime Ascendance of Lightning–Blood–Toxic — `0x37`
Elements: **Fire + Cold + Lightning + Blood + Toxic**  
Graph ID: `c755e4d7f6711034`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.26, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.07, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.26, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 5.06, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 6.06, duration 0.00, count 7, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.26, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.26, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.49, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.26, duration 5.50, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 1.03, radius 5.10, duration 5.35, count 10, delay 0.38.
- `FieldSurge` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 1.10, radius 5.27, duration 5.35, count 10, delay 0.27.
- `PulseNova` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 0.95, radius 5.61, duration 5.15, count 8, delay 0.49.
- `PulseNova` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 1.08, radius 5.44, duration 5.15, count 9, delay 0.49.
- `SplitFields` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Rime Convergence of Lightning–Blood–Void — `0x57`
Elements: **Fire + Cold + Lightning + Blood + Void**  
Graph ID: `b10474056bbba0cc`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.42, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.18, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.42, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 5.22, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 6.22, duration 0.00, count 7, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.42, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.42, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 5.12, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.92, duration 0.00, count 2, delay 0.48.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `TrailLine` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 1.03, radius 5.10, duration 5.35, count 10, delay 0.38.
- `DelayedEcho` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 0.93, radius 5.27, duration 4.95, count 7, delay 0.27.
- `Rebound` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 1.10, radius 5.10, duration 5.35, count 10, delay 0.16.
- `PulseNova` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 1.05, radius 5.27, duration 5.35, count 9, delay 0.38.
- `SplitFields` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Blood|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Cold, payload Fire|Cold|Lightning|Blood|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Rime Aetherstorm of Lightning–Toxic–Void — `0x67`
Elements: **Fire + Cold + Lightning + Toxic + Void**  
Graph ID: `d511e287bf2de0c8`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.10, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 2.95, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.10, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 4.90, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 5.90, duration 0.00, count 7, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.36, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.10, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.80, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.60, duration 0.00, count 2, delay 0.48.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 1.03, radius 5.10, duration 5.35, count 10, delay 0.38.
- `FieldSurge` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 0.93, radius 5.61, duration 5.15, count 7, delay 0.49.
- `PulseNova` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 1.08, radius 5.10, duration 4.95, count 9, delay 0.16.
- `PulseNova` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 1.05, radius 5.61, duration 5.55, count 7, delay 0.16.
- `SplitFields` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Toxic|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Cold|Lightning|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Rime Convergence of Physical–Blood–Toxic — `0x3B`
Elements: **Fire + Cold + Physical + Blood + Toxic**  
Graph ID: `5bdd1155e588248b`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.34, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.12, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.34, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 5.14, duration 1.80, count 10, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.34, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.34, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.34, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.34, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.56, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.34, duration 5.50, count 2, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 0.93, radius 5.10, duration 4.95, count 7, delay 0.16.
- `Compression` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 1.05, radius 5.44, duration 5.55, count 9, delay 0.38.
- `SplitFields` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 1.00, radius 5.44, duration 5.55, count 10, delay 0.27.
- `PulseNova` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 1.03, radius 5.10, duration 4.95, count 9, delay 0.38.
- `SplitFields` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `FieldSurge` — element Physical, payload Fire|Cold|Physical|Blood|Toxic, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Rime Grand Crucible of Physical–Blood–Void — `0x5B`
Elements: **Fire + Cold + Physical + Blood + Void**  
Graph ID: `99adc7d56e8ad824`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.10, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 2.95, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.10, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 4.90, duration 1.80, count 10, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.10, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.10, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.10, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.10, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.80, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.60, duration 0.00, count 2, delay 0.48.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 0.93, radius 5.10, duration 4.95, count 7, delay 0.16.
- `FieldSurge` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 1.08, radius 5.44, duration 5.15, count 10, delay 0.38.
- `PulseNova` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 0.93, radius 5.61, duration 5.55, count 7, delay 0.38.
- `PulseNova` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 1.00, radius 5.10, duration 5.35, count 9, delay 0.27.
- `SplitFields` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Cold|Physical|Blood|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Rime Devouring Crown of Physical–Toxic–Void — `0x6B`
Elements: **Fire + Cold + Physical + Toxic + Void**  
Graph ID: `b556303171878299`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.18, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.01, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.18, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 4.98, duration 1.80, count 10, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.18, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.18, duration 2.75, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.43, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.18, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.88, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.68, duration 0.00, count 2, delay 0.48.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 0.93, radius 5.10, duration 4.95, count 7, delay 0.16.
- `Compression` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 1.10, radius 5.10, duration 5.35, count 10, delay 0.16.
- `SplitFields` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 1.10, radius 5.61, duration 5.15, count 10, delay 0.49.
- `PulseNova` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 0.98, radius 5.27, duration 5.35, count 10, delay 0.49.
- `SplitFields` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Physical|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Rime Convergence of Blood–Toxic–Void — `0x73`
Elements: **Fire + Cold + Blood + Toxic + Void**  
Graph ID: `244656ea68f38bf3`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.42, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.18, duration 5.25, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.42, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 5.22, duration 1.80, count 10, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.42, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.42, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.62, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.42, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 5.12, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.92, duration 0.00, count 2, delay 0.48.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 0.93, radius 5.10, duration 4.95, count 9, delay 0.16.
- `FieldSurge` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 1.00, radius 5.27, duration 5.35, count 8, delay 0.27.
- `PulseNova` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 1.00, radius 5.27, duration 4.95, count 10, delay 0.49.
- `PulseNova` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 1.08, radius 5.44, duration 5.15, count 9, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Blood|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Volt Aetherstorm of Physical–Blood–Toxic — `0x3D`
Elements: **Fire + Lightning + Physical + Blood + Toxic**  
Graph ID: `f1c0ed4d3e714fce`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.42, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.18, duration 5.25, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 6.22, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.42, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.42, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.42, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.42, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.62, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.42, duration 5.50, count 2, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 0.98, radius 5.44, duration 5.15, count 7, delay 0.27.
- `DetonateBuildup` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 0.93, radius 5.44, duration 5.15, count 7, delay 0.38.
- `TrailLine` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 0.93, radius 5.10, duration 5.35, count 7, delay 0.16.
- `PulseNova` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 1.00, radius 5.10, duration 4.95, count 9, delay 0.16.
- `SplitFields` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `FieldSurge` — element Physical, payload Fire|Lightning|Physical|Blood|Toxic, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Volt Devouring Crown of Physical–Blood–Void — `0x5D`
Elements: **Fire + Lightning + Physical + Blood + Void**  
Graph ID: `4145d8b536876399`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.18, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.01, duration 5.25, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 5.98, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.18, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.18, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.18, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.18, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.88, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.68, duration 0.00, count 2, delay 0.48.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 0.98, radius 5.44, duration 5.15, count 7, delay 0.27.
- `ThermalCycle` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 0.95, radius 5.44, duration 5.55, count 7, delay 0.38.
- `OrbitingNodes` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 1.05, radius 5.10, duration 5.35, count 8, delay 0.27.
- `PulseNova` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 0.98, radius 5.61, duration 5.15, count 8, delay 0.49.
- `SplitFields` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Physical|Blood|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Lightning|Physical|Blood|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Volt Worldstorm of Physical–Toxic–Void — `0x6D`
Elements: **Fire + Lightning + Physical + Toxic + Void**  
Graph ID: `7267be8d72619389`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.26, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.07, duration 5.25, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 6.06, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.26, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.26, duration 2.75, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.49, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.26, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.96, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.76, duration 0.00, count 2, delay 0.48.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 0.98, radius 5.44, duration 5.15, count 7, delay 0.27.
- `DetonateBuildup` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 0.98, radius 5.10, duration 4.95, count 8, delay 0.16.
- `TrailLine` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 1.03, radius 5.27, duration 4.95, count 7, delay 0.38.
- `PulseNova` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 0.95, radius 5.27, duration 5.35, count 10, delay 0.27.
- `SplitFields` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Physical|Toxic|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Lightning|Physical|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Volt Aetherstorm of Blood–Toxic–Void — `0x75`
Elements: **Fire + Lightning + Blood + Toxic + Void**  
Graph ID: `64da5227a8bfe6a2`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.10, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 2.95, duration 5.25, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 5.90, duration 0.00, count 7, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.10, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.10, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.36, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.10, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.80, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.60, duration 0.00, count 2, delay 0.48.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 0.98, radius 5.44, duration 5.15, count 9, delay 0.27.
- `ThermalCycle` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 1.08, radius 5.27, duration 4.95, count 10, delay 0.27.
- `OrbitingNodes` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 1.10, radius 5.61, duration 5.55, count 10, delay 0.38.
- `PulseNova` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 1.05, radius 5.27, duration 4.95, count 9, delay 0.49.
- `SplitFields` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Blood|Toxic|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Blood, payload Fire|Lightning|Blood|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Cinder Ruin Devouring Crown of Blood–Toxic–Void — `0x79`
Elements: **Fire + Physical + Blood + Toxic + Void**  
Graph ID: `3130d69630f760bc`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 0.95, radius 4.26, duration 4.50, count 2, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.48, radius 3.07, duration 5.25, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.26, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.26, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.26, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.26, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.49, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.26, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.96, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.76, duration 0.00, count 2, delay 0.48.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 1.08, radius 5.44, duration 5.55, count 10, delay 0.49.
- `DetonateBuildup` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 1.03, radius 5.27, duration 4.95, count 9, delay 0.27.
- `TrailLine` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 0.95, radius 5.44, duration 5.15, count 8, delay 0.16.
- `PulseNova` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `Rebound` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 1.00, radius 5.10, duration 4.95, count 9, delay 0.49.
- `SplitFields` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Physical|Blood|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Rime Volt Ascendance of Physical–Blood–Toxic — `0x3E`
Elements: **Cold + Lightning + Physical + Blood + Toxic**  
Graph ID: `6fa57c32edeaa2d0`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.42, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 5.22, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 6.22, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.42, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.42, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.42, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.42, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.62, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.42, duration 5.50, count 2, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.00, radius 5.61, duration 5.15, count 7, delay 0.27.
- `PulseNova` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.08, radius 5.10, duration 4.95, count 10, delay 0.16.
- `ThermalCycle` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.08, radius 5.61, duration 5.15, count 9, delay 0.38.
- `PulseNova` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.08, radius 5.61, duration 5.55, count 10, delay 0.27.
- `SplitFields` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `FieldSurge` — element Cold, payload Cold|Lightning|Physical|Blood|Toxic, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Rime Volt Convergence of Physical–Blood–Void — `0x5E`
Elements: **Cold + Lightning + Physical + Blood + Void**  
Graph ID: `d098e28c50bedec1`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.18, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 4.98, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 5.98, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.18, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.18, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.18, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.18, duration 5.00, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.88, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.68, duration 0.00, count 2, delay 0.48.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.00, radius 5.61, duration 5.15, count 7, delay 0.27.
- `Rebound` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.10, radius 5.10, duration 5.35, count 10, delay 0.16.
- `ShardNova` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.03, radius 5.61, duration 5.15, count 7, delay 0.49.
- `PulseNova` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.05, radius 5.61, duration 5.15, count 10, delay 0.16.
- `SplitFields` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Cold, payload Cold|Lightning|Physical|Blood|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Rime Volt Aetherstorm of Physical–Toxic–Void — `0x6E`
Elements: **Cold + Lightning + Physical + Toxic + Void**  
Graph ID: `9fea072caffe722e`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.34, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 5.14, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 6.14, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.34, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.34, duration 2.75, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.56, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.34, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 5.04, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.84, duration 0.00, count 2, delay 0.48.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 1.00, radius 5.61, duration 5.15, count 7, delay 0.27.
- `PulseNova` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 1.10, radius 5.44, duration 5.55, count 10, delay 0.38.
- `ThermalCycle` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 0.98, radius 5.61, duration 5.55, count 9, delay 0.49.
- `PulseNova` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 1.05, radius 5.10, duration 5.35, count 8, delay 0.27.
- `SplitFields` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Physical|Toxic|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Cold|Lightning|Physical|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Rime Volt Ascendance of Blood–Toxic–Void — `0x76`
Elements: **Cold + Lightning + Blood + Toxic + Void**  
Graph ID: `97d1a968afe8dfde`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.18, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 4.98, duration 1.80, count 10, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 5.98, duration 0.00, count 7, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.18, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.18, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.43, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.18, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.88, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.68, duration 0.00, count 2, delay 0.48.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 1.00, radius 5.61, duration 5.15, count 9, delay 0.27.
- `Rebound` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 1.00, radius 5.61, duration 5.55, count 8, delay 0.49.
- `ShardNova` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 1.08, radius 5.27, duration 5.35, count 9, delay 0.49.
- `PulseNova` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 0.95, radius 5.27, duration 4.95, count 7, delay 0.16.
- `SplitFields` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Blood|Toxic|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Cold, payload Cold|Lightning|Blood|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Rime Ruin Convergence of Blood–Toxic–Void — `0x7A`
Elements: **Cold + Physical + Blood + Toxic + Void**  
Graph ID: `a5c52134f325ee0a`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.26, duration 1.25, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.70, radius 5.06, duration 1.80, count 10, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.26, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.26, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.26, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.26, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.49, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.26, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 4.96, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.76, duration 0.00, count 2, delay 0.48.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 1.10, radius 5.61, duration 5.55, count 10, delay 0.49.
- `PulseNova` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 0.95, radius 5.61, duration 5.55, count 7, delay 0.49.
- `ThermalCycle` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 0.93, radius 5.10, duration 4.95, count 7, delay 0.27.
- `PulseNova` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 1.10, radius 5.10, duration 4.95, count 7, delay 0.16.
- `SplitFields` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.25, duration 1.50, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Void, payload Cold|Physical|Blood|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

### Volt Ruin Aetherstorm of Blood–Toxic–Void — `0x7C`
Elements: **Lightning + Physical + Blood + Toxic + Void**  
Graph ID: `e3b8b757b3b3609f`

**Resolution mechanics**

- `ChainArc` — element Lightning, payload Lightning, magnitude 0.78, radius 6.14, duration 0.00, count 7, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.77, radius 4.34, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.17, radius 4.34, duration 2.75, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.49, radius 4.34, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.85, radius 4.34, duration 5.00, count 2, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.50, radius 3.56, duration 6.45, count 9, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.81, radius 4.34, duration 5.50, count 2, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.75, radius 5.04, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.72, radius 4.84, duration 0.00, count 2, delay 0.48.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `ThermalCycle` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 0.95, radius 5.27, duration 4.95, count 7, delay 0.16.
- `OrbitingNodes` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 1.05, radius 5.10, duration 5.35, count 9, delay 0.16.
- `Compression` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 1.03, radius 5.44, duration 5.55, count 7, delay 0.27.
- `PulseNova` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 1.02, radius 5.65, duration 4.90, count 7, delay 0.08.
- `Field` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 0.68, radius 4.80, duration 8.00, count 9, delay 0.00.
- `FieldSurge` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 1.00, radius 7.00, duration 2.00, count 7, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 1.60, radius 5.80, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 1.22, radius 5.90, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 1.05, radius 5.61, duration 5.55, count 10, delay 0.27.
- `SplitFields` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 0.79, radius 5.50, duration 7.50, count 7, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning|Physical|Blood|Toxic|Void, magnitude 1.05, radius 7.50, duration 0.00, count 8, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 6.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Lightning|Physical|Blood|Toxic|Void, magnitude 1.00, radius 8.00, duration 3.00, count 7, delay 0.00.

## Calamity

### Prismatic Detonation — `0x3F`
Elements: **Fire + Cold + Lightning + Physical + Blood + Toxic**  
Graph ID: `1fa6370cba369095`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 1.03, radius 4.60, duration 4.50, count 3, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.52, radius 3.31, duration 5.70, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.60, duration 1.41, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.75, radius 5.40, duration 1.80, count 11, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.84, radius 6.40, duration 0.00, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.84, radius 4.60, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.19, radius 4.60, duration 3.00, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.54, radius 4.60, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.92, radius 4.60, duration 5.00, count 3, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.54, radius 3.77, duration 6.90, count 10, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.88, radius 4.60, duration 5.50, count 3, delay 0.00.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Compression` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.15, radius 5.69, duration 5.90, count 9, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.17, radius 5.86, duration 5.70, count 11, delay 0.38.
- `DetonateBuildup` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.15, radius 6.03, duration 5.70, count 9, delay 0.38.
- `TrailLine` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.15, radius 5.86, duration 5.70, count 11, delay 0.27.
- `PulseNova` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.10, radius 6.10, duration 5.40, count 8, delay 0.08.
- `Field` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 0.74, radius 5.20, duration 8.70, count 10, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.06, radius 7.40, duration 2.00, count 8, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 0.85, radius 5.50, duration 6.00, count 11, delay 0.00.
- `ShardNova` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 0.90, radius 6.20, duration 2.80, count 24, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.74, radius 6.32, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.32, radius 6.40, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.07, radius 6.03, duration 6.10, count 9, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 0.86, radius 6.00, duration 8.20, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.11, radius 8.00, duration 0.00, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.60, duration 1.66, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic, magnitude 1.00, radius 8.30, duration 3.00, count 8, delay 0.00.

### Astral Tempest — `0x5F`
Elements: **Fire + Cold + Lightning + Physical + Blood + Void**  
Graph ID: `9e2922fe5203f69d`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 1.03, radius 4.76, duration 4.50, count 3, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.52, radius 3.43, duration 5.70, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.76, duration 1.41, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.75, radius 5.56, duration 1.80, count 11, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.84, radius 6.56, duration 0.00, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.84, radius 4.76, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.19, radius 4.76, duration 3.00, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.54, radius 4.76, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.92, radius 4.76, duration 5.00, count 3, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.83, radius 5.46, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.78, radius 5.26, duration 0.00, count 3, delay 0.52.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.15, radius 5.69, duration 5.90, count 9, delay 0.38.
- `PulseNova` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.20, radius 5.86, duration 6.10, count 11, delay 0.38.
- `ThermalCycle` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.07, radius 6.03, duration 5.70, count 10, delay 0.49.
- `OrbitingNodes` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.12, radius 5.69, duration 5.90, count 8, delay 0.27.
- `PulseNova` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.10, radius 6.10, duration 5.40, count 8, delay 0.08.
- `Field` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 0.74, radius 5.20, duration 8.70, count 10, delay 0.00.
- `FieldSurge` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.06, radius 7.40, duration 2.00, count 8, delay 0.00.
- `OrbitingNodes` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 0.85, radius 5.50, duration 6.00, count 11, delay 0.00.
- `ShardNova` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 0.90, radius 6.20, duration 2.80, count 24, delay 0.00.

**Death mechanics**

- `Burst` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.74, radius 6.32, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.32, radius 6.40, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.04, radius 5.86, duration 5.70, count 9, delay 0.27.
- `SplitFields` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 0.86, radius 6.00, duration 8.20, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.11, radius 8.00, duration 0.00, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.60, duration 1.66, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 7.20, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Blood, payload Fire|Cold|Lightning|Physical|Blood|Void, magnitude 1.00, radius 8.30, duration 3.00, count 8, delay 0.00.

### Elemental Cataclysm — `0x6F`
Elements: **Fire + Cold + Lightning + Physical + Toxic + Void**  
Graph ID: `3ff80ebc9a1b8cdf`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 1.03, radius 4.44, duration 4.50, count 3, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.52, radius 3.20, duration 5.70, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.44, duration 1.41, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.75, radius 5.24, duration 1.80, count 11, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.84, radius 6.24, duration 0.00, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.84, radius 4.44, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.19, radius 4.44, duration 3.00, count 1, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.54, radius 3.64, duration 6.90, count 10, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.88, radius 4.44, duration 5.50, count 3, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.83, radius 5.14, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.78, radius 4.94, duration 0.00, count 3, delay 0.52.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `Compression` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.15, radius 5.69, duration 5.90, count 9, delay 0.38.
- `SplitFields` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.02, radius 5.52, duration 5.50, count 8, delay 0.16.
- `DetonateBuildup` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.04, radius 5.52, duration 5.50, count 9, delay 0.16.
- `TrailLine` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.09, radius 6.03, duration 5.70, count 11, delay 0.16.
- `PulseNova` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.10, radius 6.10, duration 5.40, count 8, delay 0.08.
- `Field` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 0.74, radius 5.20, duration 8.70, count 10, delay 0.00.
- `FieldSurge` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.06, radius 7.40, duration 2.00, count 8, delay 0.00.
- `OrbitingNodes` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 0.85, radius 5.50, duration 6.00, count 11, delay 0.00.
- `ShardNova` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 0.90, radius 6.20, duration 2.80, count 24, delay 0.00.

**Death mechanics**

- `Burst` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.74, radius 6.32, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.32, radius 6.40, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.02, radius 5.52, duration 5.90, count 11, delay 0.49.
- `SplitFields` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 0.86, radius 6.00, duration 8.20, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.11, radius 8.00, duration 0.00, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.60, duration 1.66, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 7.20, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Cold, payload Fire|Cold|Lightning|Physical|Toxic|Void, magnitude 1.00, radius 8.30, duration 3.00, count 8, delay 0.00.

### Living Apocalypse — `0x77`
Elements: **Fire + Cold + Lightning + Blood + Toxic + Void**  
Graph ID: `22424edb75e44778`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 1.03, radius 4.68, duration 4.50, count 3, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.52, radius 3.37, duration 5.70, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.68, duration 1.41, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.75, radius 5.48, duration 1.80, count 11, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.84, radius 6.48, duration 0.00, count 8, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.54, radius 4.68, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.92, radius 4.68, duration 5.00, count 3, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.54, radius 3.84, duration 6.90, count 10, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.88, radius 4.68, duration 5.50, count 3, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.83, radius 5.38, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.78, radius 5.18, duration 0.00, count 3, delay 0.52.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.15, radius 5.69, duration 5.90, count 11, delay 0.38.
- `PulseNova` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.12, radius 5.69, duration 5.50, count 10, delay 0.27.
- `ThermalCycle` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.12, radius 5.86, duration 6.10, count 8, delay 0.16.
- `OrbitingNodes` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.09, radius 5.69, duration 5.50, count 10, delay 0.49.
- `PulseNova` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.10, radius 6.10, duration 5.40, count 8, delay 0.08.
- `Field` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 0.74, radius 5.20, duration 8.70, count 10, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.06, radius 7.40, duration 2.00, count 8, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 0.85, radius 5.50, duration 6.00, count 11, delay 0.00.
- `ShardNova` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 0.90, radius 6.20, duration 2.80, count 24, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.74, radius 6.32, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.32, radius 6.40, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.12, radius 5.69, duration 5.50, count 10, delay 0.27.
- `SplitFields` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 0.86, radius 6.00, duration 8.20, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.11, radius 8.00, duration 0.00, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.60, duration 1.66, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 7.20, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Fire|Cold|Lightning|Blood|Toxic|Void, magnitude 1.00, radius 8.30, duration 3.00, count 8, delay 0.00.

### Silent Cataclysm — `0x7B`
Elements: **Fire + Cold + Physical + Blood + Toxic + Void**  
Graph ID: `595250f9804e9b64`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 1.03, radius 4.76, duration 4.50, count 3, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.52, radius 3.43, duration 5.70, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.76, duration 1.41, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.75, radius 5.56, duration 1.80, count 11, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.84, radius 4.76, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.19, radius 4.76, duration 3.00, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.54, radius 4.76, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.92, radius 4.76, duration 5.00, count 3, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.54, radius 3.90, duration 6.90, count 10, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.88, radius 4.76, duration 5.50, count 3, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.83, radius 5.46, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.78, radius 5.26, duration 0.00, count 3, delay 0.52.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `Compression` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.04, radius 5.69, duration 5.50, count 8, delay 0.16.
- `SplitFields` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.07, radius 5.86, duration 5.70, count 9, delay 0.38.
- `DetonateBuildup` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.17, radius 5.69, duration 5.50, count 10, delay 0.38.
- `TrailLine` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.17, radius 5.69, duration 5.90, count 9, delay 0.16.
- `PulseNova` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.10, radius 6.10, duration 5.40, count 8, delay 0.08.
- `Field` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 0.74, radius 5.20, duration 8.70, count 10, delay 0.00.
- `FieldSurge` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.06, radius 7.40, duration 2.00, count 8, delay 0.00.
- `OrbitingNodes` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 0.85, radius 5.50, duration 6.00, count 11, delay 0.00.
- `ShardNova` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 0.90, radius 6.20, duration 2.80, count 24, delay 0.00.

**Death mechanics**

- `Burst` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.74, radius 6.32, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.32, radius 6.40, duration 0.00, count 1, delay 0.00.
- `ShardNova` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.07, radius 6.03, duration 6.10, count 9, delay 0.27.
- `SplitFields` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 0.86, radius 6.00, duration 8.20, count 8, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.60, duration 1.66, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 7.20, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Blood, payload Fire|Cold|Physical|Blood|Toxic|Void, magnitude 1.00, radius 8.30, duration 3.00, count 8, delay 0.00.

### Worldfire Collapse — `0x7D`
Elements: **Fire + Lightning + Physical + Blood + Toxic + Void**  
Graph ID: `cf760ba3ef58ad25`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 1.03, radius 4.44, duration 4.50, count 3, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.52, radius 3.20, duration 5.70, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.84, radius 6.24, duration 0.00, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.84, radius 4.44, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.19, radius 4.44, duration 3.00, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.54, radius 4.44, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.92, radius 4.44, duration 5.00, count 3, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.54, radius 3.64, duration 6.90, count 10, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.88, radius 4.44, duration 5.50, count 3, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.83, radius 5.14, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.78, radius 4.94, duration 0.00, count 3, delay 0.52.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.09, radius 6.03, duration 5.70, count 8, delay 0.27.
- `TrailLine` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.15, radius 5.86, duration 6.10, count 10, delay 0.38.
- `DelayedEcho` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.09, radius 5.86, duration 6.10, count 11, delay 0.27.
- `Rebound` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.12, radius 5.86, duration 6.10, count 9, delay 0.49.
- `PulseNova` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.10, radius 6.10, duration 5.40, count 8, delay 0.08.
- `Field` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 0.74, radius 5.20, duration 8.70, count 10, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.06, radius 7.40, duration 2.00, count 8, delay 0.00.
- `OrbitingNodes` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 0.85, radius 5.50, duration 6.00, count 11, delay 0.00.
- `ShardNova` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 0.90, radius 6.20, duration 2.80, count 24, delay 0.00.

**Death mechanics**

- `Burst` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.74, radius 6.32, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.32, radius 6.40, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.04, radius 5.86, duration 6.10, count 9, delay 0.38.
- `SplitFields` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 0.86, radius 6.00, duration 8.20, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.11, radius 8.00, duration 0.00, count 9, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 7.20, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Void, payload Fire|Lightning|Physical|Blood|Toxic|Void, magnitude 1.00, radius 8.30, duration 3.00, count 8, delay 0.00.

### Absolute Extinction — `0x7E`
Elements: **Cold + Lightning + Physical + Blood + Toxic + Void**  
Graph ID: `62c4e1bbc3ab6329`

**Resolution mechanics**

- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 4.52, duration 1.41, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.75, radius 5.32, duration 1.80, count 11, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.84, radius 6.32, duration 0.00, count 8, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.84, radius 4.52, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.19, radius 4.52, duration 3.00, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.54, radius 4.52, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.92, radius 4.52, duration 5.00, count 3, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.54, radius 3.71, duration 6.90, count 10, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.88, radius 4.52, duration 5.50, count 3, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.83, radius 5.22, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.78, radius 5.02, duration 0.00, count 3, delay 0.52.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.12, radius 5.52, duration 5.90, count 9, delay 0.38.
- `ThermalCycle` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.09, radius 5.52, duration 5.90, count 9, delay 0.16.
- `OrbitingNodes` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.04, radius 5.69, duration 5.90, count 9, delay 0.49.
- `Compression` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.09, radius 6.03, duration 5.70, count 11, delay 0.38.
- `PulseNova` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.10, radius 6.10, duration 5.40, count 8, delay 0.08.
- `Field` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 0.74, radius 5.20, duration 8.70, count 10, delay 0.00.
- `FieldSurge` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.06, radius 7.40, duration 2.00, count 8, delay 0.00.
- `OrbitingNodes` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 0.85, radius 5.50, duration 6.00, count 11, delay 0.00.
- `ShardNova` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 0.90, radius 6.20, duration 2.80, count 24, delay 0.00.

**Death mechanics**

- `Burst` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.74, radius 6.32, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.32, radius 6.40, duration 0.00, count 1, delay 0.00.
- `PulseNova` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.12, radius 5.86, duration 6.10, count 11, delay 0.49.
- `SplitFields` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 0.86, radius 6.00, duration 8.20, count 8, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.11, radius 8.00, duration 0.00, count 9, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.60, duration 1.66, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 7.20, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Toxic, payload Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.00, radius 8.30, duration 3.00, count 8, delay 0.00.

## Apex

### Worldbreak — `0x7F`
Elements: **Fire + Cold + Lightning + Physical + Blood + Toxic + Void**  
Graph ID: `dc0aff5d45b93a64`

**Resolution mechanics**

- `BuildupWave` — element Fire, payload Fire, magnitude 1.11, radius 5.02, duration 4.50, count 3, delay 0.00.
- `Field` — element Fire, payload Fire, magnitude 0.56, radius 3.61, duration 6.15, count 10, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.02, duration 1.57, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold, magnitude 0.80, radius 5.82, duration 1.80, count 12, delay 0.00.
- `ChainArc` — element Lightning, payload Lightning, magnitude 0.90, radius 6.82, duration 0.00, count 9, delay 0.00.
- `Push` — element Physical, payload Physical, magnitude 0.91, radius 5.02, duration 0.00, count 1, delay 0.00.
- `Vulnerability` — element Physical, payload Physical, magnitude 0.21, radius 5.02, duration 3.25, count 1, delay 0.00.
- `Execute` — element Blood, payload Blood, magnitude 0.59, radius 5.02, duration 0.00, count 1, delay 0.00.
- `BuildupWave` — element Blood, payload Blood, magnitude 0.99, radius 5.02, duration 5.00, count 3, delay 0.00.
- `Field` — element Toxic, payload Toxic, magnitude 0.58, radius 4.12, duration 7.35, count 11, delay 0.00.
- `BuildupWave` — element Toxic, payload Toxic, magnitude 0.95, radius 5.02, duration 5.50, count 3, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.91, radius 5.72, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Void, magnitude 0.84, radius 5.52, duration 0.00, count 3, delay 0.56.
- `ThermalCycle` — element Fire, payload Fire|Cold, magnitude 0.92, radius 3.20, duration 3.80, count 5, delay 0.12.
- `Stagger` — element Physical, payload Fire|Cold, magnitude 0.35, radius 3.00, duration 1.20, count 1, delay 0.00.
- `OrbitingNodes` — element Lightning, payload Fire|Lightning, magnitude 0.78, radius 4.20, duration 4.50, count 3, delay 0.00.
- `ChainArc` — element Fire, payload Fire|Lightning, magnitude 0.72, radius 5.20, duration 0.00, count 4, delay 0.00.
- `Rebound` — element Physical, payload Fire|Physical, magnitude 0.85, radius 4.00, duration 1.00, count 2, delay 0.22.
- `Field` — element Fire, payload Fire|Physical, magnitude 0.38, radius 2.50, duration 3.20, count 4, delay 0.00.
- `Execute` — element Blood, payload Fire|Blood, magnitude 0.62, radius 3.30, duration 0.00, count 1, delay 0.00.
- `Contagion` — element Fire, payload Fire|Blood, magnitude 0.58, radius 4.00, duration 4.50, count 3, delay 0.00.
- `DelayedEcho` — element Fire, payload Fire|Toxic, magnitude 1.05, radius 4.20, duration 0.00, count 2, delay 0.38.
- `SplitFields` — element Toxic, payload Fire|Toxic, magnitude 0.48, radius 3.60, duration 4.50, count 4, delay 0.00.
- `Compression` — element Void, payload Fire|Void, magnitude 0.72, radius 4.50, duration 1.40, count 2, delay 0.24.
- `Field` — element Fire, payload Fire|Void, magnitude 0.44, radius 3.00, duration 4.80, count 5, delay 0.00.
- `ChainArc` — element Lightning, payload Cold|Lightning, magnitude 0.82, radius 6.00, duration 0.00, count 6, delay 0.00.
- `Freeze` — element Cold, payload Cold|Lightning, magnitude 0.00, radius 3.40, duration 1.10, count 1, delay 0.00.
- `ShardNova` — element Cold, payload Cold|Physical, magnitude 0.85, radius 5.00, duration 2.00, count 10, delay 0.00.
- `Stagger` — element Physical, payload Cold|Physical, magnitude 0.52, radius 4.00, duration 1.60, count 2, delay 0.00.
- `ShardNova` — element Blood, payload Cold|Blood, magnitude 0.68, radius 4.40, duration 2.40, count 8, delay 0.00.
- `Execute` — element Blood, payload Cold|Blood, magnitude 0.52, radius 3.80, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Cold, payload Cold|Toxic, magnitude 0.42, radius 3.60, duration 5.20, count 5, delay 0.00.
- `DelayedEcho` — element Toxic, payload Cold|Toxic, magnitude 0.72, radius 4.10, duration 0.00, count 3, delay 0.52.
- `Compression` — element Void, payload Cold|Void, magnitude 0.64, radius 4.80, duration 1.80, count 3, delay 0.20.
- `Freeze` — element Cold, payload Cold|Void, magnitude 0.00, radius 4.00, duration 1.60, count 1, delay 0.00.
- `PulseNova` — element Lightning, payload Lightning|Physical, magnitude 0.80, radius 4.80, duration 2.40, count 4, delay 0.05.
- `Push` — element Physical, payload Lightning|Physical, magnitude 0.90, radius 4.40, duration 0.00, count 1, delay 0.00.
- `ChainArc` — element Blood, payload Lightning|Blood, magnitude 0.72, radius 5.50, duration 0.00, count 5, delay 0.00.
- `Contagion` — element Lightning, payload Lightning|Blood, magnitude 0.48, radius 4.20, duration 4.00, count 4, delay 0.00.
- `Field` — element Lightning, payload Lightning|Toxic, magnitude 0.52, radius 3.80, duration 5.50, count 7, delay 0.00.
- `ChainArc` — element Toxic, payload Lightning|Toxic, magnitude 0.66, radius 5.80, duration 0.00, count 5, delay 0.00.
- `OrbitingNodes` — element Void, payload Lightning|Void, magnitude 0.75, radius 4.80, duration 5.00, count 4, delay 0.00.
- `Pull` — element Void, payload Lightning|Void, magnitude 0.62, radius 5.20, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Physical, payload Physical|Blood, magnitude 0.80, radius 4.00, duration 0.00, count 1, delay 0.00.
- `Push` — element Blood, payload Physical|Blood, magnitude 0.76, radius 4.20, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Physical|Toxic, magnitude 0.55, radius 4.20, duration 5.00, count 6, delay 0.00.
- `Push` — element Physical, payload Physical|Toxic, magnitude 0.58, radius 4.80, duration 0.00, count 1, delay 0.00.
- `Compression` — element Void, payload Physical|Void, magnitude 0.90, radius 5.00, duration 1.20, count 3, delay 0.18.
- `DelayedEcho` — element Physical, payload Physical|Void, magnitude 0.82, radius 4.40, duration 0.00, count 2, delay 0.44.
- `Contagion` — element Toxic, payload Blood|Toxic, magnitude 0.82, radius 5.00, duration 5.50, count 6, delay 0.00.
- `Execute` — element Blood, payload Blood|Toxic, magnitude 0.48, radius 4.00, duration 0.00, count 1, delay 0.00.
- `DelayedEcho` — element Void, payload Blood|Void, magnitude 0.92, radius 4.60, duration 0.00, count 3, delay 0.46.
- `Execute` — element Blood, payload Blood|Void, magnitude 0.55, radius 4.20, duration 0.00, count 1, delay 0.00.
- `Field` — element Void, payload Toxic|Void, magnitude 0.60, radius 4.50, duration 6.00, count 8, delay 0.00.
- `Pull` — element Void, payload Toxic|Void, magnitude 0.58, radius 5.00, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.26, radius 6.28, duration 6.65, count 10, delay 0.49.
- `DetonateBuildup` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.18, radius 6.28, duration 6.65, count 10, delay 0.38.
- `TrailLine` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.11, radius 6.11, duration 6.45, count 9, delay 0.49.
- `DelayedEcho` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.16, radius 5.94, duration 6.05, count 10, delay 0.38.
- `Rebound` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.11, radius 6.28, duration 6.65, count 10, delay 0.16.
- `PulseNova` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.18, radius 6.55, duration 5.90, count 9, delay 0.08.
- `Field` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 0.80, radius 5.60, duration 9.40, count 11, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.12, radius 7.80, duration 2.00, count 9, delay 0.00.
- `OrbitingNodes` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 0.85, radius 5.50, duration 6.00, count 12, delay 0.00.
- `ShardNova` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 0.90, radius 6.20, duration 2.80, count 26, delay 0.00.
- `Compression` — element Void, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.10, radius 7.50, duration 2.00, count 4, delay 0.12.
- `ThermalCycle` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.00, radius 7.00, duration 5.00, count 8, delay 0.05.
- `DetonateBuildup` — element Physical, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.20, radius 7.00, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Toxic, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 0.90, radius 6.00, duration 8.00, count 10, delay 0.00.

**Death mechanics**

- `Burst` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.88, radius 6.84, duration 0.00, count 1, delay 0.00.
- `DetonateBuildup` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.42, radius 6.90, duration 0.00, count 1, delay 0.00.
- `SplitFields` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.11, radius 6.28, duration 6.65, count 10, delay 0.16.
- `SplitFields` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 0.93, radius 6.50, duration 8.90, count 9, delay 0.00.
- `ChainArc` — element Lightning, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.17, radius 8.50, duration 0.00, count 10, delay 0.00.
- `Freeze` — element Cold, payload Cold, magnitude 0.00, radius 5.95, duration 1.82, count 1, delay 0.00.
- `Pull` — element Void, payload Void, magnitude 0.72, radius 7.70, duration 0.00, count 1, delay 0.00.
- `FieldSurge` — element Fire, payload Fire|Cold|Lightning|Physical|Blood|Toxic|Void, magnitude 1.00, radius 8.60, duration 3.00, count 9, delay 0.00.
