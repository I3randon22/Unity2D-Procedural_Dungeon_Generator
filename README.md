# Unity2D-Procedural_Dungeon_Generator
Procedural dungeon generator designed for Unity. The generator is intended for use in top-down 2D roguelike games, creating dynamic and randomized dungeon layouts. You have multiple algorithms to choose from.

## Features

Absolutely, let's craft a detailed and professional description for your procedural dungeon generation system, tilemap functionality, and modular design.

## Dungeon Generation System

The procedural dungeon generation system in this Unity project offers a versatile and customizable framework for creating dynamic and engaging 2D dungeons. The core algorithm begins with a grid-based approach, allowing the creation of multiple interconnected grids. Each grid represents a distinct section of the dungeon, and the system intelligently places them, ensuring non-overlapping layouts. The initial implementation employs a standard algorithm that generates box-sized rooms, forming the foundation of the dungeon layout.

### Grid Expansion and Algorithm Integration

One of the standout features is the ability to seamlessly integrate different algorithms into the dungeon generation process. Users can choose specific grids to replace with alternative algorithms, resulting in diverse and unique room shapes beyond the traditional box layout. This modular approach empowers creators to craft dungeons with a blend of room styles, enhancing gameplay variety and visual aesthetics.

### Connection and Pathfinding

The system utilizes A* pathfinding to intelligently connect rooms, ensuring coherent and navigable dungeons. This dynamic connectivity enhances player exploration and provides a sense of continuity throughout the generated environment. The algorithm adapts to the chosen room shapes, creating paths that fit seamlessly within the layout.

## Tilemap Functionality

The tilemap functionality is a crucial component of the project, responsible for visually translating the generated dungeon layouts. The system utilizes binary representations to efficiently paint different tiles, including walls, floors, corners, and other elements. This binary approach simplifies the painting process, making it both robust and performance-friendly.

### Binary Painting and Customization

The binary system allows users to precisely define which tiles should appear in the dungeon, providing granular control over the visual elements. This level of customization extends to the choice of tiles for walls, floors, and other environmental features, enabling developers to tailor the dungeon's aesthetic to suit their game's theme and style.

## Modular Design

The project's modular design stands out as a key strength, offering flexibility and ease of customization. Creators can adjust various parameters, including room sizes, quantity, and difficulty levels. This modularity extends to object placement, with a random generator facilitating the spawning of diverse in-game elements. The modular design further enables the definition of specific tiles for painting, ensuring a cohesive and visually appealing dungeon atmosphere.

### Dynamic Difficulty and Object Placement

The modular design allows for dynamic difficulty adjustments, influencing the distribution of enemies, chests, and other interactive elements. Higher difficulty levels correlate with increased enemy presence, while later rooms have higher chances of spawning valuable chests. The inclusion of stairs in the end room provides a seamless transition to the next floor, creating a multi-layered and immersive dungeon experience.

In summary, this Unity project's procedural dungeon generation system, coupled with advanced tilemap functionality and a modular design approach, empowers game developers with unparalleled flexibility. From diverse room shapes and interconnected layouts to customized visuals and dynamic difficulty scaling, this system offers a robust foundation for creating engaging and endlessly replayable 2D roguelike dungeons.

### Unity Technologies Used
- C# scripting for Unity game engine.
- Unity Tilemap system for efficient 2D level design.

## How to Use

1. **Installation**: Clone or download the repository and open it in Unity.
2. **Integration**: Integrate the `Unity2D-ProceduralDungeonGenerator` into your Unity project.
3. **Configuration**: Adjust the parameters in the scripts to tailor the dungeon generation to your specific game requirements.
4. **Run**: Execute the dungeon generation script to create a new randomized dungeon layout.
5. **Visualize**: Use Unity's Scene view to visualize the generated dungeon.


## Author

Brandon Skinner

This project is currently in a developmental phase, and while it strives for excellence, it acknowledges the ongoing refinement process. Certain aspects may undergo further adjustments to enhance performance and functionality. Your inquiries and potential collaboration proposals are welcomed. Feel free to reach out for additional information or to explore collaboration opportunities.

