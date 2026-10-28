# GitHub Copilot Instructions for AnimalHarvestingSpot (Continued)

## Mod Overview and Purpose

AnimalHarvestingSpot (Continued) is a RimWorld mod that extends the functionality of animal harvesting by adding four new specialized spots: Milking, Shearing, Slaughtering, and Training. These spots allow pawns to perform specific actions on animals, making the process more organized and efficient. The mod also includes new icons, integrates with zones, and provides an alternate texture mod. Additionally, a German translation has been added, thanks to Dimos.

## Key Features and Systems

1. **New Harvesting Spots**:
   - **Milking**: Allows pawns to milk animals at designated spots.
   - **Shearing**: Allows pawns to shear animals for wool at designated spots.
   - **Slaughtering**: Allows pawns to slaughter animals at designated spots.
   - **Training**: Allows pawns to train animals at designated spots.

2. **Placement in Zones**: The new spots can be placed within zones, providing more control over where harvesting activities take place.

3. **Alternate Textures**: An alternate texture mod by Si-Cafe is available, enhancing the visual appeal of the mod.

4. **German Translation**: The mod now supports German, thanks to the contribution of Dimos.

## Coding Patterns and Conventions

### C# Files

- **JobDriver_Shear_OnSpot.cs**:
  - Handles the shearing job.
  - Contains methods like `GetComp`, `TryMakePreToilReservations`, and `WorkTotal`.

- **AnimalHarvestingSpot.cs**:
  - Manages the harvesting spot.
  - Contains methods like `CanTarget` and `MakeNewToils`.

- **WorkGiver_Milk_OnSpot.cs**:
  - Manages the milk job.
  - Contains methods like `findSpot` and `JobOnThing`.

- **JobDriver_Milk_OnSpot.cs**:
  - Handles the milk job.
  - Contains methods like `GetComp`, `TryMakePreToilReservations`, and `WorkTotal`.

- **JobDriver_Slaughter_OnSpot.cs**:
  - Handles the slaughter job.
  - Contains methods like `CanTarget`, `MakeNewToils`, and `TryMakePreToilReservations`.

### XML Files

- **Buildings_Misc_Harvest.xml**:
  - Defines the new harvesting spots.
  - Contains `ThingDef` definitions for the Milking, Shearing, Slaughtering, and Training spots.

- **JobDefs.xml**:
  - Defines the job definitions for the new spots.
  - Contains `JobDef` definitions for the Milking, Shearing, Slaughtering, and Training jobs.

- **Jobs_Work_Override.xml**:
  - Overrides existing job definitions to include the new spots.

- **TranslationTemplate.xml**:
  - Contains translation templates for the new spots.

## XML Integration

To integrate the new harvesting spots into the game, the mod modifies and adds XML definitions. These files are located in the `Defs` and `Overwrite-Defs` directories. The `ThingDefs_Buildings` and `JobDefs` files are specifically modified to include the new spot definitions.

## Harmony Patching

The mod uses Harmony to patch existing RimWorld code to integrate the new spots seamlessly. Harmony patches are located in the `Patches` directory. These patches modify methods in the RimWorld codebase to include the new spots in the game logic.

## Suggestions for Copilot

1. **C# Code Completion**: Use Copilot to generate methods and classes for new job drivers and work givers, ensuring that they follow the coding patterns and conventions established in the existing codebase.

2. **XML Definitions**: Use Copilot to generate new XML definitions for additional harvesting spots or job overrides. Ensure that the generated XML follows the structure and naming conventions used in the existing files.

3. **Harmony Patches**: Use Copilot to generate Harmony patches for modifying existing RimWorld code. Ensure that the generated patches are correctly integrated into the mod's patching system.

4. **Documentation**: Use Copilot to generate documentation for new classes and methods, providing a clear description of their purpose and usage.

5. **Localization**: Use Copilot to generate translation templates for new strings and phrases, ensuring that the mod supports multiple languages.

By following these guidelines, you can effectively use GitHub Copilot to enhance and maintain the AnimalHarvestingSpot (Continued) mod.

## Project Solution Guidelines
- Relevant mod XML files are included as Solution Items under the solution folder named XML, these can be read and modified from within the solution.
- Use these in-solution XML files as the primary files for reference and modification.
- The `.github/copilot-instructions.md` file is included in the solution under the `.github` solution folder, so it should be read/modified from within the solution instead of using paths outside the solution. Update this file once only, as it and the parent-path solution reference point to the same file in this workspace.
- When making functional changes in this mod, ensure the documented features stay in sync with implementation; use the in-solution `.github` copy as the primary file.
- In the solution is also a project called Assembly-CSharp, containing a read-only version of the decompiled game source, for reference and debugging purposes.
- For any new documentation, update this copilot-instructions.md file rather than creating separate documentation files.


## Hard rules (must follow)
- Do NOT run commands that modify the repo (no git commit, git apply, dotnet format) unless explicitly asked.
- Prefer minimal reads: read only the smallest code region needed (around the suspicious lines).
- When mentioning SonarQube issues, automatically use the SonarQube MCP service to fetch and address issues instead of making inferred fixes without querying SonarQube first.
- When mentioning the rimworld log, automatically use the Rimworld MCP service to fetch the log.

