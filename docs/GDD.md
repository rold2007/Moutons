# Moutons - Game Design Document

* **Project Name:** Moutons ("Sheeps")
* **Repository:** https://github.com/rold2007/Moutons
* **Target Audience:** Casual / Strategy
* **Target Platforms:** PC (desktop)

## 1. Game Overview

*Moutons* is a strategy game about managing and protecting a flock of sheep. The player guides the flock through changing conditions, finds pasture, and keeps sheep safe from predators and environmental hazards.

* A 2D, console-based, strategy game in which the player issues indirect commands to the shepherd/sheeps/sheepdog such as `Flock`, `Flee`, `Protect`, and `Search Pasture`.

### 1.1 Core Pillars

1. **Flock management:** Sheep react to one another and to player direction or commands.
2. **Indirect, accessible strategy:** Simple actions or commands lead to emergent tactical decisions.
3. **Protection and survival:** Keep the flock together, find resources, and respond to threats and changing conditions.
4. **Escalating challenge:** Predators, weather, terrain, and time or resource pressure create increasingly difficult decisions.

### 1.2 Core Loop

1. Observe the map, flock, available pasture, and current hazards.
2. Direct the sheep with movement and abilities or issue flock commands.
3. Protect the flock, manage its condition, and locate grazing areas.
4. Reach the day's or level's objective, then carry the flock's state into the next stage.

## 2. Game Format and Technical Direction

A console application using Spectre.Console, with an ASCII-art or text-grid map. The target scope is approximately one month of development.

## 3. Gameplay

### 3.1 Player Actions

* **Flock:** Gather sheep together for protection or efficiency.
* **Flee:** Evacuate the flock from a threat zone.
* **Protect:** Take a defensive action against a threat such as a wolf or fox.
* **Search Pasture:** Locate grazing areas or resources.

### 3.2 Sheep Behavior and Flock Management

* **Cohesion:** Stay near the main group.
* **Separation:** Maintain personal space from other sheep.
* **Idle / Grazing:** Wander slowly around grass or pasture.
* **Alert:** Stop grazing when a player or predator is nearby.
* **Fleeing:** Run away from a threat.
* **Penned:** Stay safe inside the fold and no longer flee.

Sheep have **Health** and **Fullness** stats, which are hidden from the player.

### 3.3 Environment and Threats

* **Pasture / grass:** Grazing areas that restore sheep fullness.
* **Obstacles:** Fences and rocks block movement.
* **Hazardous terrain:** Rivers can drown sheep, and cliffs can cause sheep to be lost if driven over the edge.
* **Predators:** Wolves and other predators, including foxes, may threaten sheep, particularly isolated animals.
* **Weather and conditions:** Snow and other changing conditions can increase difficulty.
* **Panic:** One fleeing sheep may trigger a chain reaction in nearby sheep.

The details of how `Protect` and `Flee` respond to enemy or weather events, and whether health or fatigue changes between days or only during gameplay, remain open questions.

## 4. Progression and Objectives

### 4.1 Day / Level Structure

1. Launch directly into gameplay, without an intro or selection menu initially.
2. Show an intro after the first day; make a menu accessible through a keyboard shortcut.
3. Complete the day's gameplay.
4. Automatically save at the end of the day. On restart, load the latest save automatically.
5. Carry the flock into the next day, preserving its stats.

### 4.2 Objectives and Outcomes

* Survive as long as possible; lose when no sheep remain by the end of a day.

### 4.3 Difficulty

Difficulty can increase through wolves, snow, terrain obstacles, and time constraints. These challenges should call for planning and spatial awareness.

## 5. Presentation

* **Console:** ASCII art or a text grid, with a header showing status such as sheep count and time, a central map view, and a footer listing commands.

Suggested audio for the 2D presentation includes calm and panicked sheep bleats, whistles, ambient nature sounds such as wind, water, and birds, and light acoustic folk music that builds tension when predators approach or time runs low.

## 6. Development Milestones

1. **Prototype the core loop:** Create a basic field, player movement, sheep with flee behavior, and a pen or area-boundary win condition.
2. **Add mechanics and AI:** Implement full flocking behavior, wolf AI, and terrain hazards.
3. **Build levels and polish:** Add a level-design pipeline, UI menus, audio, and scoring.
